using AccountManagement.Models;
using AccountManagement.Services;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AccountManagement.Forms;

public class CategoryManagementForm : Form
{
    private DatabaseService _dbService;
    private Category _category;
    private bool _isEditMode;

    private TextBox nameTextBox;
    private PictureBox iconPictureBox;
    private Button selectIconButton;
    private Button removeIconButton;
    private Button saveButton;
    private Button cancelButton;

    private string _selectedIconPath;

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
        this.Size = new Size(450, 350);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = Color.White;
        this.Font = new Font("Segoe UI", 9);

        int yPos = 30;
        int leftMargin = 30;

        Label nameLabel = new Label
        {
            Text = "Название категории *",
            Location = new Point(leftMargin, yPos),
            AutoSize = true,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 100, 100)
        };
        this.Controls.Add(nameLabel);

        yPos += 25;

        nameTextBox = new TextBox
        {
            Location = new Point(leftMargin, yPos),
            Size = new Size(370, 30),
            Font = new Font("Segoe UI", 10),
            BorderStyle = BorderStyle.FixedSingle
        };
        this.Controls.Add(nameTextBox);

        yPos += 50;

        Label iconLabel = new Label
        {
            Text = "Иконка категории",
            Location = new Point(leftMargin, yPos),
            AutoSize = true,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 100, 100)
        };
        this.Controls.Add(iconLabel);

        yPos += 25;

        iconPictureBox = new PictureBox
        {
            Location = new Point(leftMargin, yPos),
            Size = new Size(100, 100),
            BorderStyle = BorderStyle.FixedSingle,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.FromArgb(240, 240, 245)
        };
        this.Controls.Add(iconPictureBox);

        selectIconButton = new Button
        {
            Text = "Выбрать иконку",
            Location = new Point(leftMargin + 120, yPos),
            Size = new Size(130, 40),
            BackColor = Color.FromArgb(100, 100, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        selectIconButton.FlatAppearance.BorderSize = 0;
        selectIconButton.Click += SelectIconButton_Click;
        this.Controls.Add(selectIconButton);

        removeIconButton = new Button
        {
            Text = "Удалить иконку",
            Location = new Point(leftMargin + 260, yPos),
            Size = new Size(130, 40),
            BackColor = Color.FromArgb(220, 220, 220),
            ForeColor = Color.FromArgb(80, 80, 80),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9),
            Cursor = Cursors.Hand
        };
        removeIconButton.FlatAppearance.BorderSize = 0;
        removeIconButton.Click += RemoveIconButton_Click;
        this.Controls.Add(removeIconButton);

        yPos += 120;

        saveButton = new Button
        {
            Text = _isEditMode ? "Сохранить" : "Добавить",
            Location = new Point(leftMargin, yPos),
            Size = new Size(180, 45),
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
            Text = "Отмена",
            Location = new Point(leftMargin + 190, yPos),
            Size = new Size(180, 45),
            BackColor = Color.FromArgb(220, 220, 220),
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
        _selectedIconPath = _category.IconPath;

        if (!string.IsNullOrEmpty(_category.IconPath) && File.Exists(_category.IconPath))
        {
            iconPictureBox.Image = Image.FromFile(_category.IconPath);
        }
    }

    private void SelectIconButton_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Все файлы|*.*";
            openFileDialog.Title = "Выберите иконку";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string appIconsPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "PasswordManager",
                        "Icons"
                    );

                    if (!Directory.Exists(appIconsPath))
                        Directory.CreateDirectory(appIconsPath);

                    string fileName = Path.GetFileName(openFileDialog.FileName);
                    string newPath = Path.Combine(appIconsPath, Guid.NewGuid().ToString() + Path.GetExtension(fileName));

                    File.Copy(openFileDialog.FileName, newPath, true);

                    _selectedIconPath = newPath;
                    iconPictureBox.Image = Image.FromFile(newPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при выборе иконки: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    private void RemoveIconButton_Click(object sender, EventArgs e)
    {
        _selectedIconPath = null;
        iconPictureBox.Image = null;
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameTextBox.Text))
        {
            MessageBox.Show("Введите название категории", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            if (_isEditMode)
            {
                _category.IconPath = _selectedIconPath;
                _dbService.UpdateCategory(_category);
            }
            else
            {
                var newCategory = new Category
                {
                    Name = nameTextBox.Text.Trim(),
                    IconPath = _selectedIconPath
                };

                _dbService.AddCategory(newCategory);
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