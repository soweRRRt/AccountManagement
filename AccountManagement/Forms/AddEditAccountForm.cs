using AccountManagement.Models;
using AccountManagement.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AccountManagement.Forms;

public partial class AddEditAccountForm : Form
{
    private DatabaseService _dbService;
    private Account _account;
    private bool _isEditMode;

    private TextBox titleTextBox;
    private TextBox usernameTextBox;
    private TextBox passwordTextBox;
    private Button generatePasswordButton;
    private Button showPasswordButton;
    private TextBox emailTextBox;
    private TextBox websiteTextBox;
    private ComboBox categoryComboBox;
    private TextBox notesTextBox;
    private CheckBox favoriteCheckBox;

    private Button saveButton;
    private Button cancelButton;

    public AddEditAccountForm(Account account = null)
    {
        _dbService = new DatabaseService();
        _account = account;
        _isEditMode = account != null;

        InitializeComponent();

        if (_isEditMode)
        {
            LoadAccountData();
        }
    }

    private void InitializeComponent()
    {
        this.Text = _isEditMode ? "Редактировать аккаунт" : "Добавить аккаунт";
        this.Size = new Size(500, 650);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = Color.White;
        this.Font = new Font("Segoe UI", 9);

        int yPos = 20;
        int leftMargin = 30;
        int controlWidth = 420;

        AddLabel("Название *", leftMargin, yPos);
        yPos += 25;
        titleTextBox = AddTextBox(leftMargin, yPos, controlWidth);
        yPos += 50;

        AddLabel("Логин *", leftMargin, yPos);
        yPos += 25;
        usernameTextBox = AddTextBox(leftMargin, yPos, controlWidth);
        yPos += 50;

        AddLabel("Пароль *", leftMargin, yPos);
        yPos += 25;

        passwordTextBox = AddTextBox(leftMargin, yPos, 310);
        passwordTextBox.UseSystemPasswordChar = true;

        showPasswordButton = new Button
        {
            Text = "👁",
            Location = new Point(leftMargin + 315, yPos),
            Size = new Size(35, 30),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(240, 240, 245),
            Cursor = Cursors.Hand
        };
        showPasswordButton.FlatAppearance.BorderSize = 0;
        showPasswordButton.Click += ShowPasswordButton_Click;

        generatePasswordButton = new Button
        {
            Text = "🎲",
            Location = new Point(leftMargin + 355, yPos),
            Size = new Size(35, 30),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(240, 240, 245),
            Cursor = Cursors.Hand
        };
        generatePasswordButton.FlatAppearance.BorderSize = 0;
        generatePasswordButton.Click += GeneratePasswordButton_Click;

        Label passwordStrengthLabel = new Label
        {
            Location = new Point(leftMargin + 395, yPos + 5),
            AutoSize = true,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(150, 150, 150)
        };

        passwordTextBox.TextChanged += (s, e) =>
        {
            int strength = CalculatePasswordStrength(passwordTextBox.Text);
            passwordStrengthLabel.Text = strength switch
            {
                >= 80 => "💪",
                >= 60 => "👍",
                >= 40 => "⚠️",
                _ => "❌"
            };
            passwordStrengthLabel.ForeColor = strength switch
            {
                >= 80 => Color.Green,
                >= 60 => Color.Orange,
                >= 40 => Color.OrangeRed,
                _ => Color.Red
            };
        };

        yPos += 50;

        AddLabel("Email", leftMargin, yPos);
        yPos += 25;
        emailTextBox = AddTextBox(leftMargin, yPos, controlWidth);
        yPos += 50;

        AddLabel("Веб-сайт", leftMargin, yPos);
        yPos += 25;
        websiteTextBox = AddTextBox(leftMargin, yPos, controlWidth);
        yPos += 50;

        AddLabel("Категория", leftMargin, yPos);
        yPos += 25;

        categoryComboBox = new ComboBox
        {
            Location = new Point(leftMargin, yPos),
            Size = new Size(controlWidth, 30),
            Font = new Font("Segoe UI", 10),
            DropDownStyle = ComboBoxStyle.DropDown
        };

        var categories = _dbService.GetAllCategories();
        categoryComboBox.Items.AddRange(categories.ToArray());

        yPos += 50;

        AddLabel("Заметки", leftMargin, yPos);
        yPos += 25;

        notesTextBox = new TextBox
        {
            Location = new Point(leftMargin, yPos),
            Size = new Size(controlWidth, 80),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Segoe UI", 10),
            BorderStyle = BorderStyle.FixedSingle
        };

        yPos += 100;

        favoriteCheckBox = new CheckBox
        {
            Text = "⭐ Добавить в избранное",
            Location = new Point(leftMargin, yPos),
            AutoSize = true,
            Font = new Font("Segoe UI", 10)
        };

        yPos += 40;

        saveButton = new Button
        {
            Text = _isEditMode ? "Сохранить" : "Добавить",
            Location = new Point(leftMargin, yPos),
            Size = new Size(200, 45),
            BackColor = Color.FromArgb(100, 100, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        saveButton.FlatAppearance.BorderSize = 0;
        saveButton.Click += SaveButton_Click;

        cancelButton = new Button
        {
            Text = "Отмена",
            Location = new Point(leftMargin + 220, yPos),
            Size = new Size(200, 45),
            BackColor = Color.FromArgb(220, 220, 220),
            ForeColor = Color.FromArgb(80, 80, 80),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11),
            Cursor = Cursors.Hand
        };
        cancelButton.FlatAppearance.BorderSize = 0;
        cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

        this.Controls.AddRange(new Control[]
        {
                titleTextBox, usernameTextBox, passwordTextBox, showPasswordButton,
                generatePasswordButton, passwordStrengthLabel, emailTextBox,
                websiteTextBox, categoryComboBox, notesTextBox, favoriteCheckBox,
                saveButton, cancelButton
        });
    }

    private Label AddLabel(string text, int x, int y)
    {
        var label = new Label
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 100, 100)
        };
        this.Controls.Add(label);
        return label;
    }

    private TextBox AddTextBox(int x, int y, int width)
    {
        var textBox = new TextBox
        {
            Location = new Point(x, y),
            Size = new Size(width, 30),
            Font = new Font("Segoe UI", 10),
            BorderStyle = BorderStyle.FixedSingle
        };
        return textBox;
    }

    private void LoadAccountData()
    {
        titleTextBox.Text = _account.Title;
        usernameTextBox.Text = _account.Username;
        passwordTextBox.Text = EncryptionService.Decrypt(_account.PasswordHash);
        emailTextBox.Text = _account.Email;
        websiteTextBox.Text = _account.Website;
        categoryComboBox.Text = _account.Category;
        notesTextBox.Text = _account.Notes;
        favoriteCheckBox.Checked = _account.IsFavorite;
    }

    private void ShowPasswordButton_Click(object sender, EventArgs e)
    {
        passwordTextBox.UseSystemPasswordChar = !passwordTextBox.UseSystemPasswordChar;
        showPasswordButton.Text = passwordTextBox.UseSystemPasswordChar ? "👁" : "🙈";
    }

    private void GeneratePasswordButton_Click(object sender, EventArgs e)
    {
        var generatedPassword = EncryptionService.GeneratePassword(16, true);
        passwordTextBox.Text = generatedPassword;
        passwordTextBox.UseSystemPasswordChar = false;
        showPasswordButton.Text = "🙈";

        MessageBox.Show(
            "Новый пароль сгенерирован!\n\nНе забудьте сохранить его.",
            "Генерация пароля",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }

    private int CalculatePasswordStrength(string password)
    {
        if (string.IsNullOrEmpty(password))
            return 0;

        int strength = 0;

        if (password.Length >= 8) strength += 20;
        if (password.Length >= 12) strength += 20;
        if (password.Length >= 16) strength += 10;

        if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]")) strength += 15;
        if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]")) strength += 15;
        if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[0-9]")) strength += 10;
        if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[^a-zA-Z0-9]")) strength += 10;

        return Math.Min(strength, 100);
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(titleTextBox.Text))
        {
            MessageBox.Show("Введите название аккаунта", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
        {
            MessageBox.Show("Введите логин", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(passwordTextBox.Text))
        {
            MessageBox.Show("Введите пароль", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            if (_isEditMode)
            {
                _account.Title = titleTextBox.Text.Trim();
                _account.Username = usernameTextBox.Text.Trim();
                _account.PasswordHash = EncryptionService.Encrypt(passwordTextBox.Text);
                _account.Email = emailTextBox.Text.Trim();
                _account.Website = websiteTextBox.Text.Trim();
                _account.Category = categoryComboBox.Text.Trim();
                _account.Notes = notesTextBox.Text.Trim();
                _account.IsFavorite = favoriteCheckBox.Checked;

                _dbService.UpdateAccount(_account);
            }
            else
            {
                var newAccount = new Account
                {
                    Title = titleTextBox.Text.Trim(),
                    Username = usernameTextBox.Text.Trim(),
                    PasswordHash = EncryptionService.Encrypt(passwordTextBox.Text),
                    Email = emailTextBox.Text.Trim(),
                    Website = websiteTextBox.Text.Trim(),
                    Category = categoryComboBox.Text.Trim(),
                    Notes = notesTextBox.Text.Trim(),
                    IsFavorite = favoriteCheckBox.Checked
                };

                _dbService.AddAccount(newAccount);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
