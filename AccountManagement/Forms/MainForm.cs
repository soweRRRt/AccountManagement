using AccountManagement.Controls;
using AccountManagement.Models;
using AccountManagement.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AccountManagement.Forms;

public partial class MainForm : Form
{
    private DatabaseService _dbService;
    private List<Account> _currentAccounts;
    private string _currentFilter = "All";

    private Panel topPanel;
    private TextBox searchBox;
    private Button addButton;

    private Panel sidebarPanel;
    private Button allButton;
    private Button favoritesButton;
    private Button manageCategoriesButton;
    private Label categoriesLabel;
    private FlowLayoutPanel categoryPanel;

    private FlowLayoutPanel accountsFlowPanel;
    private Panel detailsPanel;

    public MainForm()
    {
        _dbService = new DatabaseService();
        InitializeComponent();
        LoadAccounts();
    }

    private void InitializeComponent()
    {
        this.Text = "Password Manager";
        this.Size = new Size(1200, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(245, 245, 250);
        this.Font = new Font("Segoe UI", 9);

        topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Color.White,
            Padding = new Padding(20, 15, 20, 15)
        };

        Label titleLabel = new Label
        {
            Text = "🔐 Password Manager",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 33, 33),
            AutoSize = true,
            Location = new Point(20, 20)
        };

        searchBox = new TextBox
        {
            Size = new Size(300, 35),
            Location = new Point(300, 20),
            Font = new Font("Segoe UI", 11)
        };
        searchBox.TextChanged += SearchBox_TextChanged;

        addButton = new Button
        {
            Text = "+ Добавить",
            Size = new Size(140, 40),
            Location = new Point(1020, 15),
            BackColor = Color.FromArgb(100, 100, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        addButton.FlatAppearance.BorderSize = 0;
        addButton.Click += AddButton_Click;

        topPanel.Controls.AddRange(new Control[] { titleLabel, searchBox, addButton });

        sidebarPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 220,
            BackColor = Color.White,
            Padding = new Padding(15)
        };

        allButton = CreateSidebarButton("📁 Все аккаунты", 15);
        allButton.Click += (s, e) => FilterAccounts("All");

        favoritesButton = CreateSidebarButton("⭐ Избранное", 65);
        favoritesButton.Click += (s, e) => FilterAccounts("Favorites");

        manageCategoriesButton = CreateSidebarButton("⚙️ Управление", 115);
        manageCategoriesButton.Click += (s, e) =>
        {
            var form = new CategoriesListForm();
            form.ShowDialog();
            LoadCategories();
        };

        categoriesLabel = new Label
        {
            Text = "КАТЕГОРИИ",
            Location = new Point(15, 175),
            AutoSize = true,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(150, 150, 150)
        };

        categoryPanel = new FlowLayoutPanel
        {
            Location = new Point(15, 200),
            Size = new Size(190, 400),
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };

        sidebarPanel.Controls.AddRange(new Control[]
        {
            allButton,
            favoritesButton,
            manageCategoriesButton,
            categoriesLabel,
            categoryPanel
        });

        accountsFlowPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(20),
            BackColor = Color.FromArgb(245, 245, 250)
        };

        detailsPanel = new Panel
        {
            Dock = DockStyle.Right,
            Width = 500,
            BackColor = Color.White,
            Visible = false,
            Padding = new Padding(20)
        };

        this.Controls.Add(accountsFlowPanel);
        this.Controls.Add(detailsPanel);
        this.Controls.Add(sidebarPanel);
        this.Controls.Add(topPanel);

        LoadCategories();
    }

    private Button CreateSidebarButton(string text, int y)
    {
        var button = new Button
        {
            Text = text,
            Location = new Point(15, y),
            Size = new Size(190, 40),
            TextAlign = ContentAlignment.MiddleLeft,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            ForeColor = Color.FromArgb(80, 80, 80),
            Font = new Font("Segoe UI", 10),
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 250);

        return button;
    }

    private void LoadCategories()
    {
        categoryPanel.Controls.Clear();
        var categories = _dbService.GetAllCategories();

        foreach (var category in categories)
        {
            var categoryButton = new Button
            {
                Text = $"  {category}",
                Size = new Size(190, 35),
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(80, 80, 80),
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand,
                Tag = category
            };
            categoryButton.FlatAppearance.BorderSize = 0;
            categoryButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 250);
            categoryButton.Click += (s, e) => FilterAccounts("Category", category);

            categoryPanel.Controls.Add(categoryButton);
        }
    }

    private void LoadAccounts()
    {
        accountsFlowPanel.Controls.Clear();
        _currentAccounts = _dbService.GetAllAccounts();
        DisplayAccounts(_currentAccounts);
    }

    private void DisplayAccounts(List<Account> accounts)
    {
        accountsFlowPanel.Controls.Clear();

        if (accounts == null || accounts.Count == 0)
        {
            Label noDataLabel = new Label
            {
                Text = "Нет аккаунтов",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = true
            };
            accountsFlowPanel.Controls.Add(noDataLabel);
            return;
        }

        foreach (var account in accounts)
        {
            var card = new AccountCard(account);
            card.CardClicked += (s, e) => ShowAccountDetails(account);
            card.FavoriteClicked += (s, e) =>
            {
                _dbService.UpdateAccount(account);
                if (_currentFilter == "Favorites")
                {
                    FilterAccounts("Favorites");
                }
            };

            accountsFlowPanel.Controls.Add(card);
        }
    }

    private void FilterAccounts(string filter, string categoryName = null)
    {
        _currentFilter = filter;

        List<Account> filteredAccounts = filter switch
        {
            "All" => _dbService.GetAllAccounts(),
            "Favorites" => _dbService.GetFavoriteAccounts(),
            "Category" => _dbService.GetAccountsByCategory(categoryName),
            _ => _dbService.GetAllAccounts()
        };

        DisplayAccounts(filteredAccounts);
    }

    private void SearchBox_TextChanged(object sender, EventArgs e)
    {
        string searchTerm = searchBox.Text.Trim();

        if (string.IsNullOrEmpty(searchTerm))
        {
            LoadAccounts();
        }
        else
        {
            var results = _dbService.SearchAccounts(searchTerm);
            DisplayAccounts(results);
        }
    }

    private void AddButton_Click(object sender, EventArgs e)
    {
        var addEditForm = new AddEditAccountForm();
        if (addEditForm.ShowDialog() == DialogResult.OK)
        {
            LoadAccounts();
            LoadCategories();
        }
    }

    private void ShowAccountDetails(Account account)
    {
        detailsPanel.Controls.Clear();
        detailsPanel.Visible = true;

        var detailsForm = new AccountDetailsControl(account, _dbService);
        detailsForm.OnAccountUpdated += () =>
        {
            LoadAccounts();
            LoadCategories();
        };
        detailsForm.OnAccountDeleted += () =>
        {
            detailsPanel.Visible = false;
            LoadAccounts();
            LoadCategories();
        };
        detailsForm.OnClose += () =>
        {
            detailsPanel.Visible = false;
        };

        detailsForm.Dock = DockStyle.Fill;
        detailsPanel.Controls.Add(detailsForm);
    }
}