using AccountManagement.Models;
using AccountManagement.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AccountManagement.Forms;

public class CategoryManagementForm : Form
{
    private DatabaseService _dbService;
    private Category _category;
    private bool _isEditMode;

    private TextBox nameTextBox;
    private Button saveButton;
    private Button cancelButton;

    public CategoryManagementForm(Category category = null)
    {
        _dbService = new DatabaseService();
        _category = category;
        _isEditMode = category != null;

        InitializeComponent();

        if (_isEditMode)
        {
            LoadCategoryData();
        }
    }

    private void InitializeComponent()
    {
        this.Text = _isEditMode ? "Редактировать категорию" : "Добавить категорию";
        this.Size = new Size(450, 220);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = Color.FromArgb(250, 250, 255);
        this.Font = new Font("Segoe UI", 9);

        int yPos = 30;
        int leftMargin = 40;

        Label formTitle = new Label
        {
            Text = _isEditMode ? "Редактирование категории" : "Новая категория",
            Location = new Point(leftMargin, yPos),
            AutoSize = true,
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 33, 33)
        };
        this.Controls.Add(formTitle);

        yPos += 50;

        Label nameLabel = new Label
        {
            Text = "Название категории *",
            Location = new Point(leftMargin, yPos),
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(80, 80, 80)
        };
        this.Controls.Add(nameLabel);

        yPos += 30;

        nameTextBox = new TextBox
        {
            Location = new Point(leftMargin, yPos),
            Size = new Size(350, 35),
            Font = new Font("Segoe UI", 11),
            BorderStyle = BorderStyle.FixedSingle,
            MaxLength = 50
        };
        this.Controls.Add(nameTextBox);

        yPos += 50;

        saveButton = new Button
        {
            Text = _isEditMode ? "💾 Сохранить" : "➕ Добавить",
            Location = new Point(leftMargin, yPos),
            Size = new Size(170, 45),
            BackColor = Color.FromArgb(100, 100, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        saveButton.FlatAppearance.BorderSize = 0;
        saveButton.Click += SaveButton_Click;
        this.Controls.Add(saveButton);

        cancelButton = new Button
        {
            Text = "❌ Отмена",
            Location = new Point(leftMargin + 180, yPos),
            Size = new Size(170, 45),
            BackColor = Color.FromArgb(240, 240, 240),
            ForeColor = Color.FromArgb(80, 80, 80),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11),
            Cursor = Cursors.Hand
        };
        cancelButton.FlatAppearance.BorderSize = 0;
        cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
        this.Controls.Add(cancelButton);
    }

    private void LoadCategoryData()
    {
        nameTextBox.Text = _category.Name;
        nameTextBox.ReadOnly = true;
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameTextBox.Text))
        {
            MessageBox.Show("Введите название категории", "Ошибка валидации",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            nameTextBox.Focus();
            return;
        }

        try
        {
            if (_isEditMode)
            {
                _dbService.UpdateCategory(_category);
                MessageBox.Show("Категория успешно обновлена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var newCategory = new Category
                {
                    Name = nameTextBox.Text.Trim()
                };

                _dbService.AddCategory(newCategory);
                MessageBox.Show("Категория успешно добавлена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!_isEditMode)
        {
            nameTextBox.Focus();
        }
    }
}