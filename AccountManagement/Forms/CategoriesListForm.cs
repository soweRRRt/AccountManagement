using AccountManagement.Models;
using AccountManagement.Services;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AccountManagement.Forms;

public class CategoriesListForm : Form
{
    private DatabaseService _dbService;
    private FlowLayoutPanel categoriesPanel;
    private Button addButton;

    public CategoriesListForm()
    {
        _dbService = new DatabaseService();
        InitializeComponent();
        LoadCategories();
    }

    private void InitializeComponent()
    {
        this.Text = "Управление категориями";
        this.Size = new Size(600, 500);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.FromArgb(245, 245, 250);
        this.Font = new Font("Segoe UI", 9);

        Label titleLabel = new Label
        {
            Text = "📁 Категории",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 33, 33),
            Location = new Point(20, 20),
            AutoSize = true
        };
        this.Controls.Add(titleLabel);

        addButton = new Button
        {
            Text = "+ Добавить категорию",
            Location = new Point(400, 15),
            Size = new Size(170, 40),
            BackColor = Color.FromArgb(100, 100, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        addButton.FlatAppearance.BorderSize = 0;
        addButton.Click += AddButton_Click;
        this.Controls.Add(addButton);

        categoriesPanel = new FlowLayoutPanel
        {
            Location = new Point(20, 70),
            Size = new Size(550, 380),
            AutoScroll = true,
            BackColor = Color.White,
            Padding = new Padding(10)
        };
        this.Controls.Add(categoriesPanel);
    }

    private void LoadCategories()
    {
        categoriesPanel.Controls.Clear();
        var categories = _dbService.GetAllCategoriesWithIcons();

        if (categories.Count == 0)
        {
            Label noDataLabel = new Label
            {
                Text = "Нет категорий. Добавьте первую!",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = true,
                Margin = new Padding(10)
            };
            categoriesPanel.Controls.Add(noDataLabel);
            return;
        }

        foreach (var category in categories)
        {
            var categoryCard = CreateCategoryCard(category);
            categoriesPanel.Controls.Add(categoryCard);
        }
    }

    private Panel CreateCategoryCard(Category category)
    {
        Panel card = new Panel
        {
            Size = new Size(510, 70),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(5)
        };

        PictureBox iconBox = new PictureBox
        {
            Location = new Point(10, 10),
            Size = new Size(50, 50),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.FromArgb(240, 240, 245)
        };

        if (!string.IsNullOrEmpty(category.IconPath) && File.Exists(category.IconPath))
        {
            iconBox.Image = Image.FromFile(category.IconPath);
        }
        else
        {
            // Первая буква на синем кружке
            iconBox.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(100, 100, 255)))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 50, 50);
                }

                string initial = category.Name.Length > 0 ? category.Name[0].ToString().ToUpper() : "?";
                using (var font = new Font("Segoe UI", 20, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(initial, font, brush, new RectangleF(0, 0, 50, 50), sf);
                }
            };
        }

        card.Controls.Add(iconBox);

        Label nameLabel = new Label
        {
            Text = category.Name,
            Location = new Point(70, 15),
            Size = new Size(250, 25),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 33, 33)
        };
        card.Controls.Add(nameLabel);

        Label dateLabel = new Label
        {
            Text = $"Создано: {category.CreatedAt:dd.MM.yyyy}",
            Location = new Point(70, 40),
            AutoSize = true,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(120, 120, 120)
        };
        card.Controls.Add(dateLabel);

        Button editButton = new Button
        {
            Text = "✏️",
            Location = new Point(400, 15),
            Size = new Size(40, 40),
            BackColor = Color.FromArgb(100, 100, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12),
            Cursor = Cursors.Hand
        };
        editButton.FlatAppearance.BorderSize = 0;
        editButton.Click += (s, e) => EditCategory(category);
        card.Controls.Add(editButton);

        Button deleteButton = new Button
        {
            Text = "🗑️",
            Location = new Point(450, 15),
            Size = new Size(40, 40),
            BackColor = Color.FromArgb(255, 100, 100),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12),
            Cursor = Cursors.Hand
        };
        deleteButton.FlatAppearance.BorderSize = 0;
        deleteButton.Click += (s, e) => DeleteCategory(category);
        card.Controls.Add(deleteButton);

        return card;
    }

    private void AddButton_Click(object sender, EventArgs e)
    {
        var form = new CategoryManagementForm();
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadCategories();
        }
    }

    private void EditCategory(Category category)
    {
        var form = new CategoryManagementForm(category);
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadCategories();
        }
    }

    private void DeleteCategory(Category category)
    {
        var result = MessageBox.Show(
            $"Вы уверены, что хотите удалить категорию '{category.Name}'?\n\n" +
            "Категория будет удалена у всех аккаунтов.",
            "Подтверждение удаления",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        );

        if (result == DialogResult.Yes)
        {
            try
            {
                _dbService.DeleteCategory(category.Id);
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}