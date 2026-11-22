using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolBorrowingSystem.UI
{
    /// <summary>
    /// All colors, fonts and "make a styled control" helpers in one place.
    /// If we want to change the look of the app, we only need to edit this file.
    /// </summary>
    public static class Theme
    {
        // ---------------- Colors ----------------
        public static readonly Color Primary = Color.FromArgb(37, 99, 235);       // blue
        public static readonly Color Success = Color.FromArgb(22, 163, 74);       // green
        public static readonly Color Danger = Color.FromArgb(220, 38, 38);        // red
        public static readonly Color Warning = Color.FromArgb(217, 119, 6);       // orange
        public static readonly Color Neutral = Color.FromArgb(100, 116, 139);     // gray

        public static readonly Color PageBackground = Color.FromArgb(241, 245, 249);
        public static readonly Color CardBackground = Color.White;
        public static readonly Color Border = Color.FromArgb(226, 232, 240);
        public static readonly Color TextDark = Color.FromArgb(15, 23, 42);
        public static readonly Color TextMuted = Color.FromArgb(100, 116, 139);

        public static readonly Color Sidebar = Color.FromArgb(15, 23, 42);
        public static readonly Color SidebarHover = Color.FromArgb(30, 41, 59);
        public static readonly Color SidebarSelected = Color.FromArgb(37, 99, 235);
        public static readonly Color SidebarText = Color.FromArgb(203, 213, 225);

        // ---------------- Fonts ----------------
        public const string FontName = "Segoe UI";
        public static readonly Font NormalFont = new Font(FontName, 10F);
        public static readonly Font SmallFont = new Font(FontName, 9F);
        public static readonly Font BoldFont = new Font(FontName, 10F, FontStyle.Bold);
        public static readonly Font SectionFont = new Font(FontName, 12F, FontStyle.Bold);
        public static readonly Font TitleFont = new Font(FontName, 18F, FontStyle.Bold);
        public static readonly Font BigNumberFont = new Font(FontName, 26F, FontStyle.Bold);
        public static readonly Font MonoFont = new Font("Consolas", 10F);

        // ---------------- Helpers ----------------

        /// <summary>Makes a color a little darker (used for hover effects).</summary>
        public static Color Darken(Color color, double amount)
        {
            int r = (int)(color.R * (1 - amount));
            int g = (int)(color.G * (1 - amount));
            int b = (int)(color.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }

        /// <summary>Mixes a color with white. Used for the soft badge backgrounds.</summary>
        public static Color Lighten(Color color, double amount)
        {
            int r = (int)(color.R + (255 - color.R) * amount);
            int g = (int)(color.G + (255 - color.G) * amount);
            int b = (int)(color.B + (255 - color.B) * amount);
            return Color.FromArgb(r, g, b);
        }

        /// <summary>A filled, flat button. Use for the main action on a screen.</summary>
        public static Button MakeButton(string text, Color color)
        {
            Button button = new Button();
            button.Text = text;
            button.Font = BoldFont;
            button.ForeColor = Color.White;
            button.BackColor = color;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Darken(color, 0.12);
            button.FlatAppearance.MouseDownBackColor = Darken(color, 0.22);
            button.Cursor = Cursors.Hand;
            button.AutoSize = true;
            button.MinimumSize = new Size(110, 38);
            button.Padding = new Padding(12, 0, 12, 0);
            button.Margin = new Padding(0, 0, 8, 0);
            button.UseVisualStyleBackColor = false;
            button.EnabledChanged += Button_EnabledChanged;
            return button;
        }

        /// <summary>
        /// Flat buttons keep their color when disabled, which makes them look clickable.
        /// So we remember the real color in Tag and show gray while disabled.
        /// </summary>
        private static void Button_EnabledChanged(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (!button.Enabled)
            {
                button.Tag = button.BackColor;
                button.BackColor = Border;
            }
            else if (button.Tag is Color)
            {
                button.BackColor = (Color)button.Tag;
            }
        }

        /// <summary>A white button with a border. Use for less important actions.</summary>
        public static Button MakeSecondaryButton(string text)
        {
            Button button = MakeButton(text, Color.White);
            button.ForeColor = TextDark;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Border;
            button.FlatAppearance.MouseOverBackColor = PageBackground;
            button.FlatAppearance.MouseDownBackColor = Border;
            return button;
        }

        public static Label MakeLabel(string text, Font font, Color color)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = font;
            label.ForeColor = color;
            label.AutoSize = true;
            label.BackColor = Color.Transparent;
            return label;
        }

        public static TextBox MakeSearchBox(string placeholder)
        {
            TextBox box = new TextBox();
            box.Font = NormalFont;
            box.Width = 260;
            box.PlaceholderText = placeholder;
            box.Margin = new Padding(0, 6, 8, 0);
            return box;
        }

        public static ComboBox MakeDropDown(int width)
        {
            ComboBox combo = new ComboBox();
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Font = NormalFont;
            combo.Width = width;
            combo.Margin = new Padding(0, 6, 8, 0);
            return combo;
        }

        /// <summary>Gives every table the same clean look.</summary>
        public static void StyleGrid(DataGridView grid)
        {
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ScrollBars = ScrollBars.Vertical;
            grid.BackgroundColor = CardBackground;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Border;
            grid.Font = NormalFont;
            grid.RowTemplate.Height = 36;

            // Header row
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 40;
            grid.ColumnHeadersDefaultCellStyle.BackColor = PageBackground;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextMuted;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font(FontName, 9F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = PageBackground;

            // Normal rows
            grid.DefaultCellStyle.BackColor = CardBackground;
            grid.DefaultCellStyle.ForeColor = TextDark;
            grid.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            grid.DefaultCellStyle.SelectionForeColor = TextDark;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

        /// <summary>
        /// Adds a column to a grid. weight = how wide it is compared to the other columns.
        /// </summary>
        public static void AddColumn(DataGridView grid, string name, string headerText, int weight)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.HeaderText = headerText;
            column.FillWeight = weight;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            grid.Columns.Add(column);
        }

        /// <summary>
        /// Colors a status cell like a little badge: soft background + strong text.
        /// </summary>
        public static void PaintBadge(DataGridViewCell cell, string status)
        {
            Color color = GetStatusColor(status);
            cell.Value = status;
            cell.Style.ForeColor = color;
            cell.Style.BackColor = Lighten(color, 0.88);
            cell.Style.SelectionForeColor = color;
            cell.Style.SelectionBackColor = Lighten(color, 0.75);
            cell.Style.Font = new Font(FontName, 9F, FontStyle.Bold);
        }

        /// <summary>Which color goes with which status word.</summary>
        public static Color GetStatusColor(string status)
        {
            if (status == "Available" || status == "Clear" || status == "On time" || status.StartsWith("Due in"))
            {
                return Success;
            }
            if (status == "On Loan")
            {
                return Primary;
            }
            if (status == "Under Repair" || status == "Missing Parts" || status == "Due soon" || status == "Late")
            {
                return Warning;
            }
            if (status == "On Hold" || status.StartsWith("Overdue"))
            {
                return Danger;
            }
            return Neutral;
        }

        public static string FormatMoney(double amount)
        {
            return "PHP " + amount.ToString("N2");
        }

        /// <summary>"Today, 3:45 PM" / "Tomorrow, 9:00 AM" / "Nov 30, 1:00 PM"</summary>
        public static string FormatFriendlyDate(DateTime date)
        {
            string time = date.ToString("h:mm tt");
            if (date.Date == DateTime.Today)
            {
                return "Today, " + time;
            }
            if (date.Date == DateTime.Today.AddDays(1))
            {
                return "Tomorrow, " + time;
            }
            if (date.Date == DateTime.Today.AddDays(-1))
            {
                return "Yesterday, " + time;
            }
            return date.ToString("MMM d") + ", " + time;
        }
    }

    /// <summary>
    /// A white panel with a thin gray border. We use it as a "card" to group things.
    /// </summary>
    public class CardPanel : Panel
    {
        public CardPanel()
        {
            BackColor = Theme.CardBackground;
            Padding = new Padding(20);
            Margin = new Padding(0, 0, 16, 16);
            DoubleBuffered = true;
            ResizeRedraw = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen pen = new Pen(Theme.Border))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
        }
    }
}
