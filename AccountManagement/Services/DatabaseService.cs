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

    public List<Account> GetAllAccounts()
    {
        try
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var accounts = db.GetCollection<Account>("accounts");
                return accounts.FindAll().OrderBy(a => a.Title).ToList();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetAllAccounts Error: {ex.Message}");
            return new List<Account>();
        }
    }

    public List<Account> GetAccountsByCategory(string category)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(category))
                return new List<Account>();

            using (var db = new LiteDatabase(_connectionString))
            {
                var accounts = db.GetCollection<Account>("accounts");
                return accounts.Find(a => a.Category == category).OrderBy(a => a.Title).ToList();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetAccountsByCategory Error: {ex.Message}");
            return new List<Account>();
        }
    }

    public List<Account> GetFavoriteAccounts()
    {
        try
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var accounts = db.GetCollection<Account>("accounts");
                return accounts.Find(a => a.IsFavorite).OrderBy(a => a.Title).ToList();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetFavoriteAccounts Error: {ex.Message}");
            return new List<Account>();
        }
    }

    public List<Account> SearchAccounts(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllAccounts();

            using (var db = new LiteDatabase(_connectionString))
            {
                var accounts = db.GetCollection<Account>("accounts");
                searchTerm = searchTerm.ToLower().Trim();

                return accounts.Find(a =>
                    (a.Title ?? "").ToLower().Contains(searchTerm) ||
                    (a.Username ?? "").ToLower().Contains(searchTerm) ||
                    (a.Email ?? "").ToLower().Contains(searchTerm) ||
                    (a.Website ?? "").ToLower().Contains(searchTerm)
                ).OrderBy(a => a.Title).ToList();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SearchAccounts Error: {ex.Message}");
            return new List<Account>();
        }
    }

    public int AddAccount(Account account)
    {
        try
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            using (var db = new LiteDatabase(_connectionString))
            {
                var accounts = db.GetCollection<Account>("accounts");
                return accounts.Insert(account);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AddAccount Error: {ex.Message}");
            throw;
        }
    }

    public bool UpdateAccount(Account account)
    {
        try
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            account.ModifiedAt = DateTime.Now;

            using (var db = new LiteDatabase(_connectionString))
            {
                var accounts = db.GetCollection<Account>("accounts");
                return accounts.Update(account);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UpdateAccount Error: {ex.Message}");
            return false;
        }
    }

    public bool DeleteAccount(int id)
    {
        try
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var accounts = db.GetCollection<Account>("accounts");
                return accounts.Delete(id);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DeleteAccount Error: {ex.Message}");
            return false;
        }
    }

    public List<string> GetAllCategories()
    {
        try
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var categoriesCollection = db.GetCollection<Category>("categories");
                var categoriesFromDb = categoriesCollection.FindAll()
                    .Select(c => c.Name)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Distinct()
                    .OrderBy(n => n)
                    .ToList();

                var accounts = db.GetCollection<Account>("accounts");
                var categoriesFromAccounts = accounts.FindAll()
                    .Select(a => a.Category)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .ToList();

                var allCategories = categoriesFromDb
                    .Union(categoriesFromAccounts)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                return allCategories;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetAllCategories Error: {ex.Message}");
            return new List<string>();
        }
    }

    public List<string> GetCategoriesWithAccounts()
    {
        try
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var accounts = db.GetCollection<Account>("accounts");

                var categoriesWithAccounts = accounts.FindAll()
                    .Select(a => a.Category)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                return categoriesWithAccounts;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetCategoriesWithAccounts Error: {ex.Message}");
            return new List<string>();
        }
    }

    public Account GetAccountById(int id)
    {
        try
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var accounts = db.GetCollection<Account>("accounts");
                return accounts.FindById(id);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetAccountById Error: {ex.Message}");
            return null;
        }
    }

    public string GetDatabasePath()
    {
        return _dbPath;
    }

    public List<Category> GetAllCategoriesWithIcons()
    {
        try
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var categories = db.GetCollection<Category>("categories");
                return categories.FindAll().OrderBy(c => c.Name).ToList();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetAllCategoriesWithIcons Error: {ex.Message}");
            return new List<Category>();
        }
    }

    public Category GetCategoryByName(string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            using (var db = new LiteDatabase(_connectionString))
            {
                var categories = db.GetCollection<Category>("categories");
                return categories.FindOne(c => c.Name == name);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetCategoryByName Error: {ex.Message}");
            return null;
        }
    }

    public int AddCategory(Category category)
    {
        try
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Название категории не может быть пустым");

            using (var db = new LiteDatabase(_connectionString))
            {
                var categories = db.GetCollection<Category>("categories");

                var exists = categories.FindOne(c => c.Name.ToLower() == category.Name.ToLower().Trim());
                if (exists != null)
                {
                    throw new Exception("Категория с таким именем уже существует");
                }

                return categories.Insert(category);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AddCategory Error: {ex.Message}");
            throw;
        }
    }

    public bool UpdateCategory(Category category)
    {
        try
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            using (var db = new LiteDatabase(_connectionString))
            {
                var categories = db.GetCollection<Category>("categories");
                return categories.Update(category);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UpdateCategory Error: {ex.Message}");
            return false;
        }
    }

    public bool DeleteCategory(int id)
    {
        try
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var categories = db.GetCollection<Category>("categories");
                var category = categories.FindById(id);

                if (category != null)
                {
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DeleteCategory Error: {ex.Message}");
            return false;
        }
    }

    public Category GetCategoryById(int id)
    {
        try
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var categories = db.GetCollection<Category>("categories");
                return categories.FindById(id);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetCategoryById Error: {ex.Message}");
            return null;
        }
    }
}