using AccountManagement.Models;
using AccountManagement.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AccountManagement.Forms;

public class AccountDetailsControl : UserControl
{
    private Account _account;
    private DatabaseService _dbService;
    private bool _passwordVisible = false;

    private Label titleLabel;
    private Button closeButton;
    private Button editButton;
    private Button deleteButton;

    private Label usernameLabel;
    private TextBox usernameTextBox;
    private Button copyUsernameButton;

    private Label passwordLabel;
    private TextBox passwordTextBox;
    private Button showPasswordButton;
    private Button copyPasswordButton;

    private Label emailLabel;
    private TextBox emailTextBox;

    private Label websiteLabel;
    private LinkLabel websiteLinkLabel;

    private Label notesLabel;
    private TextBox notesTextBox;

    private Label categoryLabel;
    private TextBox categoryTextBox;

    private Label datesLabel;

    public event Action OnAccountUpdated;
    public event Action OnAccountDeleted;
    public event Action OnClose;

    public AccountDetailsControl(Account account, DatabaseService dbService)
    {
        _account = account;
        _dbService = dbService;
        InitializeComponents();
        LoadAccountData();
    }

    private void InitializeComponents()
    {
        this.BackColor = Color.White;
        this.AutoScroll = true;

        int yPos = 20;

        titleLabel = new Label
        {
            Location = new Point(20, yPos),
            Size = new Size(280, 30),
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 33, 33)
        };

        closeButton = new Button
        {
            Text = "✕",
            Location = new Point(340, yPos),
            Size = new Size(30, 30),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            Font = new Font("Segoe UI", 12),
            Cursor = Cursors.Hand
        };
        closeButton.FlatAppearance.BorderSize = 0;
        closeButton.Click += (s, e) => OnClose?.Invoke();

        yPos += 50;

        editButton = CreateActionButton("✏️ Редактировать", 20, yPos);
        editButton.Click += EditButton_Click;

        deleteButton = CreateActionButton("🗑️ Удалить", 200, yPos);
        deleteButton.BackColor = Color.FromArgb(255, 100, 100);
        deleteButton.Click += DeleteButton_Click;

        yPos += 60;

        usernameLabel = CreateLabel("Логин", yPos);
        yPos += 25;

        usernameTextBox = CreateTextBox(yPos);
        usernameTextBox.UseSystemPasswordChar = true;
        usernameTextBox.ReadOnly = true;

        copyUsernameButton = CreateIconButton("📋", 310, yPos);
        copyUsernameButton.Click += (s, e) => CopyToClipboard(usernameTextBox.Text, "Логин");

        var showUsernameButton = CreateIconButton("👁", 345, yPos);
        showUsernameButton.Click += (s, e) => ToggleVisibility(usernameTextBox);

        yPos += 50;

        passwordLabel = CreateLabel("Пароль", yPos);
        yPos += 25;

        passwordTextBox = CreateTextBox(yPos);
        passwordTextBox.UseSystemPasswordChar = true;
        passwordTextBox.ReadOnly = true;

        copyPasswordButton = CreateIconButton("📋", 310, yPos);
        copyPasswordButton.Click += (s, e) => CopyToClipboard(passwordTextBox.Text, "Пароль");

        showPasswordButton = CreateIconButton("👁", 345, yPos);
        showPasswordButton.Click += ShowPasswordButton_Click;

        yPos += 50;

        emailLabel = CreateLabel("Email", yPos);
        yPos += 25;

        emailTextBox = CreateTextBox(yPos);
        emailTextBox.ReadOnly = true;
        emailTextBox.Width = 340;

        yPos += 50;

        websiteLabel = CreateLabel("Веб-сайт", yPos);
        yPos += 25;

        websiteLinkLabel = new LinkLabel
        {
            Location = new Point(20, yPos),
            Size = new Size(340, 30),
            Font = new Font("Segoe UI", 10),
            LinkColor = Color.FromArgb(100, 100, 255)
        };
        websiteLinkLabel.LinkClicked += (s, e) =>
        {
            if (!string.IsNullOrEmpty(websiteLinkLabel.Text))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = websiteLinkLabel.Text,
                    UseShellExecute = true
                });
            }
        };

        yPos += 50;

        categoryLabel = CreateLabel("Категория", yPos);
        yPos += 25;

        categoryTextBox = CreateTextBox(yPos);
        categoryTextBox.ReadOnly = true;
        categoryTextBox.Width = 340;

        yPos += 50;

        notesLabel = CreateLabel("Заметки", yPos);
        yPos += 25;

        notesTextBox = new TextBox
        {
            Location = new Point(20, yPos),
            Size = new Size(340, 100),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Segoe UI", 10),
            ReadOnly = true,
            BackColor = Color.FromArgb(250, 250, 250),
            BorderStyle = BorderStyle.FixedSingle
        };

        yPos += 120;

        datesLabel = new Label
        {
            Location = new Point(20, yPos),
            Size = new Size(340, 60),
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(150, 150, 150)
        };

        this.Controls.AddRange(new Control[]
        {
                titleLabel, closeButton, editButton, deleteButton,
                usernameLabel, usernameTextBox, copyUsernameButton, showUsernameButton,
                passwordLabel, passwordTextBox, copyPasswordButton, showPasswordButton,
                emailLabel, emailTextBox,
                websiteLabel, websiteLinkLabel,
                categoryLabel, categoryTextBox,
                notesLabel, notesTextBox,
                datesLabel
        });
    }

    private Label CreateLabel(string text, int yPos)
    {
        return new Label
        {
            Text = text,
            Location = new Point(20, yPos),
            AutoSize = true,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 100, 100)
        };
    }

    private TextBox CreateTextBox(int yPos)
    {
        return new TextBox
        {
            Location = new Point(20, yPos),
            Size = new Size(280, 30),
            Font = new Font("Segoe UI", 10),
            BackColor = Color.FromArgb(250, 250, 250),
            BorderStyle = BorderStyle.FixedSingle
        };
    }

    private Button CreateIconButton(string icon, int xPos, int yPos)
    {
        var button = new Button
        {
            Text = icon,
            Location = new Point(xPos, yPos),
            Size = new Size(30, 30),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(240, 240, 245),
            Font = new Font("Segoe UI", 10),
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private Button CreateActionButton(string text, int xPos, int yPos)
    {
        var button = new Button
        {
            Text = text,
            Location = new Point(xPos, yPos),
            Size = new Size(160, 40),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(100, 100, 255),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private void LoadAccountData()
    {
        titleLabel.Text = _account.Title;
        usernameTextBox.Text = _account.Username;
        passwordTextBox.Text = EncryptionService.Decrypt(_account.PasswordHash);
        emailTextBox.Text = _account.Email;
        websiteLinkLabel.Text = _account.Website;
        categoryTextBox.Text = _account.Category;
        notesTextBox.Text = _account.Notes;

        datesLabel.Text = $"Создано: {_account.CreatedAt:dd.MM.yyyy HH:mm}\n" +
                         $"Изменено: {_account.ModifiedAt:dd.MM.yyyy HH:mm}";
    }

    private void ShowPasswordButton_Click(object sender, EventArgs e)
    {
        _passwordVisible = !_passwordVisible;
        passwordTextBox.UseSystemPasswordChar = !_passwordVisible;
        showPasswordButton.Text = _passwordVisible ? "🙈" : "👁";
    }

    private void ToggleVisibility(TextBox textBox)
    {
        textBox.UseSystemPasswordChar = !textBox.UseSystemPasswordChar;
    }

    private void CopyToClipboard(string text, string fieldName)
    {
        if (!string.IsNullOrEmpty(text))
        {
            Clipboard.SetText(text);
            MessageBox.Show($"{fieldName} скопирован в буфер обмена", "Успех",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void EditButton_Click(object sender, EventArgs e)
    {
        var editForm = new AddEditAccountForm(_account);
        if (editForm.ShowDialog() == DialogResult.OK)
        {
            _account = _dbService.GetAccountById(_account.Id);
            LoadAccountData();
            OnAccountUpdated?.Invoke();
        }
    }

    private void DeleteButton_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show(
            $"Вы уверены, что хотите удалить аккаунт '{_account.Title}'?",
            "Подтверждение удаления",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        );

        if (result == DialogResult.Yes)
        {
            _dbService.DeleteAccount(_account.Id);
            OnAccountDeleted?.Invoke();
        }
    }
}
