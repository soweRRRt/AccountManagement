using AccountManagement.Models;
using AccountManagement.Services;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AccountManagement.Forms;

public class CategoriesListForm : Form
{
    private DatabaseService _dbService;
    private FlowLayoutPanel categoriesPanel;
    private Button addButton;
    private MainForm _mainForm;

    public CategoriesListForm()
    {
        _dbService = new DatabaseService();

        foreach (Form form in Application.OpenForms)
        {
            if (form is MainForm mainForm)
            {
                _mainForm = mainForm;
                break;
            }
        }

        InitializeComponent();
        LoadCategories();
    }

    private void InitializeComponent()
    {
        this.Text = "Управление категориями";
        this.Size = new Size(600, 550);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.FromArgb(245, 245, 250);
        this.Font = new Font("Segoe UI", 9);

        Label titleLabel = new Label
        {
            Text = "📁 Управление категориями",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 33, 33),
            Location = new Point(25, 20),
            AutoSize = true
        };
        this.Controls.Add(titleLabel);

        addButton = new Button
        {
            Text = "+ Добавить категорию",
            Location = new Point(380, 15),
            Size = new Size(190, 45),
            BackColor = Color.FromArgb(100, 100, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        addButton.FlatAppearance.BorderSize = 0;
        addButton.Click += AddButton_Click;
        this.Controls.Add(addButton);

        categoriesPanel = new FlowLayoutPanel
        {
            Location = new Point(25, 80),
            Size = new Size(540, 430),
            AutoScroll = true,
            BackColor = Color.White,
            Padding = new Padding(15),
            BorderStyle = BorderStyle.FixedSingle
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
                Text = "Нет категорий. Добавьте первую категорию!",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = true,
                Margin = new Padding(20)
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
            Size = new Size(490, 60),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(5),
            Cursor = Cursors.Hand
        };

        card.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(Color.FromArgb(230, 230, 230), 2))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            }
        };

        Label nameLabel = new Label
        {
            Text = category.Name,
            Location = new Point(20, 10),
            Size = new Size(280, 28),
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 33, 33),
            AutoEllipsis = true
        };
        card.Controls.Add(nameLabel);

        Label dateLabel = new Label
        {
            Text = $"Создано: {category.CreatedAt:dd.MM.yyyy}",
            Location = new Point(20, 36),
            AutoSize = true,
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(120, 120, 120)
        };
        card.Controls.Add(dateLabel);

        Button deleteButton = new Button
        {
            Text = "🗑️ Удалить",
            Location = new Point(370, 12),
            Size = new Size(100, 36),
            BackColor = Color.FromArgb(255, 100, 100),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10),
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
            _mainForm?.LoadCategories();
        }
    }

    private void DeleteCategory(Category category)
    {
        var result = MessageBox.Show(
            $"Вы уверены, что хотите удалить категорию '{category.Name}'?\n\n" +
            "Категория будет удалена у всех аккаунтов с этой категорией.",
            "Подтверждение удаления",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        );

        if (result == DialogResult.Yes)
        {
            try
            {
                _dbService.DeleteCategory(category.Id);
                MessageBox.Show("Категория успешно удалена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCategories();
                _mainForm?.LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}