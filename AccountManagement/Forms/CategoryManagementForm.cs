using AccountManagement.Models;
using AccountManagement.Services;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
    private bool _iconPaintHandlerAttached = false;

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
        else
        {
            LoadDefaultIcon();
        }
    }

    private void InitializeComponent()
    {
        this.Text = _isEditMode ? "Редактировать категорию" : "Добавить категорию";
        this.Size = new Size(550, 450);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = Color.FromArgb(250, 250, 255);
        this.Font = new Font("Segoe UI", 9);

        int yPos = 30;
        int leftMargin = 40;

        // Заголовок
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
            Size = new Size(450, 35),
            Font = new Font("Segoe UI", 11),
            BorderStyle = BorderStyle.FixedSingle,
            MaxLength = 50
        };
        nameTextBox.TextChanged += NameTextBox_TextChanged;
        this.Controls.Add(nameTextBox);

        yPos += 50;

        Label iconLabel = new Label
        {
            Text = "Иконка категории",
            Location = new Point(leftMargin, yPos),
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(80, 80, 80)
        };
        this.Controls.Add(iconLabel);

        yPos += 30;

        Panel iconPanel = new Panel
        {
            Location = new Point(leftMargin, yPos),
            Size = new Size(450, 120),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        iconPictureBox = new PictureBox
        {
            Location = new Point(10, 10),
            Size = new Size(100, 100),
            BorderStyle = BorderStyle.FixedSingle,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.FromArgb(245, 245, 250)
        };

        selectIconButton = new Button
        {
            Text = "📁 Выбрать файл",
            Location = new Point(130, 25),
            Size = new Size(150, 35),
            BackColor = Color.FromArgb(100, 100, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        selectIconButton.FlatAppearance.BorderSize = 0;
        selectIconButton.Click += SelectIconButton_Click;

        removeIconButton = new Button
        {
            Text = "🗑️ Удалить",
            Location = new Point(290, 25),
            Size = new Size(150, 35),
            BackColor = Color.FromArgb(220, 220, 220),
            ForeColor = Color.FromArgb(80, 80, 80),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10),
            Cursor = Cursors.Hand
        };
        removeIconButton.FlatAppearance.BorderSize = 0;
        removeIconButton.Click += RemoveIconButton_Click;

        Label iconHint = new Label
        {
            Text = "По умолчанию - первая буква названия",
            Location = new Point(130, 70),
            AutoSize = true,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(120, 120, 120)
        };

        iconPanel.Controls.AddRange(new Control[] { iconPictureBox, selectIconButton, removeIconButton, iconHint });
        this.Controls.Add(iconPanel);

        yPos += 140;

        saveButton = new Button
        {
            Text = _isEditMode ? "💾 Сохранить" : "➕ Добавить",
            Location = new Point(leftMargin, yPos),
            Size = new Size(215, 50),
            BackColor = Color.FromArgb(100, 100, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        saveButton.FlatAppearance.BorderSize = 0;
        saveButton.Click += SaveButton_Click;
        this.Controls.Add(saveButton);

        cancelButton = new Button
        {
            Text = "❌ Отмена",
            Location = new Point(leftMargin + 225, yPos),
            Size = new Size(215, 50),
            BackColor = Color.FromArgb(240, 240, 240),
            ForeColor = Color.FromArgb(80, 80, 80),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12),
            Cursor = Cursors.Hand
        };
        cancelButton.FlatAppearance.BorderSize = 0;
        cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
        this.Controls.Add(cancelButton);
    }

    private void NameTextBox_TextChanged(object sender, EventArgs e)
    {
        // Обновляем дефолтную иконку при изменении названия
        if (string.IsNullOrWhiteSpace(_selectedIconPath))
        {
            iconPictureBox.Invalidate();
        }
    }

    private void LoadDefaultIcon()
    {
        if (!_iconPaintHandlerAttached)
        {
            iconPictureBox.Paint += IconPictureBox_Paint;
            _iconPaintHandlerAttached = true;
        }
        iconPictureBox.Invalidate();
    }

    private void IconPictureBox_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        using (var brush = new SolidBrush(Color.FromArgb(100, 100, 255)))
        {
            e.Graphics.FillEllipse(brush, 0, 0, 100, 100);
        }

        string initial = (!string.IsNullOrWhiteSpace(nameTextBox.Text)
            ? nameTextBox.Text[0].ToString().ToUpper()
            : "?");

        using (var font = new Font("Segoe UI", 36, FontStyle.Bold))
        using (var brush = new SolidBrush(Color.White))
        {
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            e.Graphics.DrawString(initial, font, brush, new RectangleF(0, 0, 100, 100), sf);
        }
    }

    private void LoadCategoryData()
    {
        nameTextBox.Text = _category.Name;
        nameTextBox.ReadOnly = true;
        _selectedIconPath = _category.IconPath;

        if (!string.IsNullOrEmpty(_category.IconPath) && File.Exists(_category.IconPath))
        {
            try
            {
                iconPictureBox.Image = Image.FromFile(_category.IconPath);
            }
            catch
            {
                LoadDefaultIcon();
            }
        }
        else
        {
            LoadDefaultIcon();
        }
    }

    private void SelectIconButton_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Все файлы|*.*";
            openFileDialog.Title = "Выберите иконку категории";

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

                    if (_iconPaintHandlerAttached)
                    {
                        iconPictureBox.Paint -= IconPictureBox_Paint;
                        _iconPaintHandlerAttached = false;
                    }

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
        LoadDefaultIcon();
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
                _category.IconPath = _selectedIconPath;
                _dbService.UpdateCategory(_category);
                MessageBox.Show("Категория успешно обновлена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var newCategory = new Category
                {
                    Name = nameTextBox.Text.Trim(),
                    IconPath = _selectedIconPath
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