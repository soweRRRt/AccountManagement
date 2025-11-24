using AccountManagement.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AccountManagement.Services;

public class DatabaseService
{
    private readonly string _dbPath;
    private readonly string _connectionString;

    public DatabaseService()
    {
        string appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PasswordManager"
        );

        if (!Directory.Exists(appDataPath))
            Directory.CreateDirectory(appDataPath);

        _dbPath = Path.Combine(appDataPath, "passwords.db");
        _connectionString = $"Filename={_dbPath};Password=SecureDBPassword123";
    }

    // === РАБОТА С АККАУНТАМИ ===

    public List<Account> GetAllAccounts()
    {
        using (var db = new LiteDatabase(_connectionString))
        {
            var accounts = db.GetCollection<Account>("accounts");
            return accounts.FindAll().ToList();
        }
    }

    public List<Account> GetAccountsByCategory(string category)
    {
        using (var db = new LiteDatabase(_connectionString))
        {
            var accounts = db.GetCollection<Account>("accounts");
            return accounts.Find(a => a.Category == category).ToList();
        }
    }

    public List<Account> GetFavoriteAccounts()
    {
        using (var db = new LiteDatabase(_connectionString))
        {
            var accounts = db.GetCollection<Account>("accounts");
            return accounts.Find(a => a.IsFavorite).ToList();
        }
    }

    public List<Account> SearchAccounts(string searchTerm)
    {
        using (var db = new LiteDatabase(_connectionString))
        {
            var accounts = db.GetCollection<Account>("accounts");
            searchTerm = searchTerm.ToLower();

            return accounts.Find(a =>
                a.Title.ToLower().Contains(searchTerm) ||
                a.Username.ToLower().Contains(searchTerm) ||
                a.Email.ToLower().Contains(searchTerm) ||
                a.Website.ToLower().Contains(searchTerm)
            ).ToList();
        }
    }

    public int AddAccount(Account account)
    {
        using (var db = new LiteDatabase(_connectionString))
        {
            var accounts = db.GetCollection<Account>("accounts");
            return accounts.Insert(account);
        }
    }

    public bool UpdateAccount(Account account)
    {
        account.ModifiedAt = DateTime.Now;

        using (var db = new LiteDatabase(_connectionString))
        {
            var accounts = db.GetCollection<Account>("accounts");
            return accounts.Update(account);
        }
    }

    public bool DeleteAccount(int id)
    {
        using (var db = new LiteDatabase(_connectionString))
        {
            var accounts = db.GetCollection<Account>("accounts");
            return accounts.Delete(id);
        }
    }

    public List<string> GetAllCategories()
    {
        using (var db = new LiteDatabase(_connectionString))
        {
            var accounts = db.GetCollection<Account>("accounts");
            return accounts.FindAll()
                .Select(a => a.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }
    }

    public Account GetAccountById(int id)
    {
        using (var db = new LiteDatabase(_connectionString))
        {
            var accounts = db.GetCollection<Account>("accounts");
            return accounts.FindById(id);
        }
    }

    public string GetDatabasePath()
    {
        return _dbPath;
    }

    // === РАБОТА С КАТЕГОРИЯМИ ===

    public List<Category> GetAllCategoriesWithIcons()
    {
        using (var db = new LiteDatabase(_connectionString))
        {
            var categories = db.GetCollection<Category>("categories");
            return categories.FindAll().OrderBy(c => c.Name).ToList();
        }
    }

    public Category GetCategoryByName(string name)
    {
        using (var db = new LiteDatabase(_connectionString))
        {
            var categories = db.GetCollection<Category>("categories");
            return categories.FindOne(c => c.Name == name);
        }
    }

    public int AddCategory(Category category)
    {
        using (var db = new LiteDatabase(_connectionString))
        {
            var categories = db.GetCollection<Category>("categories");

            // Проверка на дубликат
            var exists = categories.FindOne(c => c.Name == category.Name);
            if (exists != null)
            {
                throw new Exception("Категория с таким именем уже существует");
            }

            return categories.Insert(category);
        }
    }

    public bool UpdateCategory(Category category)
    {
        using (var db = new LiteDatabase(_connectionString))
        {
            var categories = db.GetCollection<Category>("categories");
            return categories.Update(category);
        }
    }

    public bool DeleteCategory(int id)
    {
        using (var db = new LiteDatabase(_connectionString))
        {
            var categories = db.GetCollection<Category>("categories");
            var category = categories.FindById(id);

            if (category != null)
            {
                // Удаляем категорию у всех аккаунтов
                var accounts = db.GetCollection<Account>("accounts");
                var accountsWithCategory = accounts.Find(a => a.Category == category.Name);

                foreach (var account in accountsWithCategory)
                {
                    account.Category = null;
                    accounts.Update(account);
                }

                return categories.Delete(id);
            }

            return false;
        }
    }

    public Category GetCategoryById(int id)
    {
        using (var db = new LiteDatabase(_connectionString))
        {
            var categories = db.GetCollection<Category>("categories");
            return categories.FindById(id);
        }
    }
}