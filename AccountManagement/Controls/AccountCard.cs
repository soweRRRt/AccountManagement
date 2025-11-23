using AccountManagement.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AccountManagement.Controls;

public class AccountCard : UserControl
{
    private Panel mainPanel;
    private PictureBox iconPictureBox;
    private Label titleLabel;
    private Label usernameLabel;
    private PictureBox favoriteIcon;
    private Label categoryLabel;

    public Account Account { get; set; }
    public event EventHandler CardClicked;
    public event EventHandler FavoriteClicked;

    public AccountCard(Account account)
    {
        Account = account;
        InitializeComponents();
        LoadAccountData();
    }

    private void InitializeComponents()
    {
        this.Size = new Size(300, 90);
        this.Margin = new Padding(10);
        this.Cursor = Cursors.Hand;

        mainPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(15)
        };
        mainPanel.Paint += MainPanel_Paint;
        mainPanel.Click += Control_Click;

        iconPictureBox = new PictureBox
        {
            Size = new Size(50, 50),
            Location = new Point(15, 20),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.FromArgb(240, 240, 245)
        };
        iconPictureBox.Click += Control_Click;

        titleLabel = new Label
        {
            Location = new Point(80, 15),
            Size = new Size(180, 25),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 33, 33)
        };
        titleLabel.Click += Control_Click;

        usernameLabel = new Label
        {
            Location = new Point(80, 40),
            Size = new Size(180, 20),
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(120, 120, 120)
        };
        usernameLabel.Click += Control_Click;

        categoryLabel = new Label
        {
            Location = new Point(80, 60),
            AutoSize = true,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(100, 100, 255),
            BackColor = Color.FromArgb(240, 240, 255),
            Padding = new Padding(6, 3, 6, 3)
        };
        categoryLabel.Click += Control_Click;

        favoriteIcon = new PictureBox
        {
            Size = new Size(24, 24),
            Location = new Point(260, 15),
            SizeMode = PictureBoxSizeMode.Zoom,
            Cursor = Cursors.Hand
        };
        favoriteIcon.Click += FavoriteIcon_Click;

        mainPanel.Controls.AddRange(new Control[]
        {
                iconPictureBox,
                titleLabel,
                usernameLabel,
                categoryLabel,
                favoriteIcon
        });

        this.Controls.Add(mainPanel);
    }

    private void LoadAccountData()
    {
        titleLabel.Text = Account.Title;
        usernameLabel.Text = Account.Username;

        if (!string.IsNullOrEmpty(Account.Category))
        {
            categoryLabel.Text = Account.Category;
            categoryLabel.Visible = true;
        }
        else
        {
            categoryLabel.Visible = false;
        }

        if (!string.IsNullOrEmpty(Account.IconPath) && System.IO.File.Exists(Account.IconPath))
        {
            iconPictureBox.Image = Image.FromFile(Account.IconPath);
        }
        else
        {
            iconPictureBox.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(100, 100, 255)))
                {
                    e.Graphics.FillEllipse(brush, 5, 5, 40, 40);
                }

                string initial = Account.Title.Length > 0 ? Account.Title[0].ToString().ToUpper() : "?";
                using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(initial, font, brush, new RectangleF(5, 5, 40, 40), sf);
                }
            };
        }

        UpdateFavoriteIcon();
    }

    private void UpdateFavoriteIcon()
    {
        favoriteIcon.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using (var font = new Font("Segoe UI Symbol", 16))
            using (var brush = new SolidBrush(Account.IsFavorite ? Color.Gold : Color.FromArgb(200, 200, 200)))
            {
                string star = Account.IsFavorite ? "★" : "☆";
                e.Graphics.DrawString(star, font, brush, new PointF(0, 0));
            }
        };
    }

    private void MainPanel_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var rect = new Rectangle(2, 2, mainPanel.Width - 4, mainPanel.Height - 4);
        var radius = 10;

        using (var path = GetRoundedRect(rect, radius))
        using (var brush = new SolidBrush(mainPanel.BackColor))
        {
            e.Graphics.FillPath(brush, path);
        }

        using (var path = GetRoundedRect(rect, radius))
        using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
        {
            e.Graphics.DrawPath(pen, path);
        }
    }

    private System.Drawing.Drawing2D.GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();

        path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90);
        path.AddArc(bounds.Right - radius, bounds.Y, radius, radius, 270, 90);
        path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - radius, radius, radius, 90, 90);
        path.CloseFigure();

        return path;
    }

    private void Control_Click(object sender, EventArgs e)
    {
        CardClicked?.Invoke(this, EventArgs.Empty);
    }

    private void FavoriteIcon_Click(object sender, EventArgs e)
    {
        Account.IsFavorite = !Account.IsFavorite;
        UpdateFavoriteIcon();
        favoriteIcon.Invalidate();
        FavoriteClicked?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        mainPanel.BackColor = Color.FromArgb(248, 248, 252);
        mainPanel.Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        mainPanel.BackColor = Color.White;
        mainPanel.Invalidate();
    }
}
