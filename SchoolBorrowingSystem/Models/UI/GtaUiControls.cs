using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SchoolBorrowingSystem.UI
{
    public static class GtaPalette
    {
        public static readonly Color CanvasDark = Color.FromArgb(16, 15, 26);
        public static readonly Color SurfaceTile = Color.FromArgb(25, 23, 39);
        public static readonly Color SurfaceAlt = Color.FromArgb(31, 28, 48);
        public static readonly Color SurfaceInput = Color.FromArgb(36, 33, 56);
        public static readonly Color BorderSubtle = Color.FromArgb(58, 53, 86);
        public static readonly Color ViceMagenta = Color.FromArgb(228, 36, 153);
        public static readonly Color LeonidaCoral = Color.FromArgb(255, 114, 107);
        public static readonly Color SunsetPeach = Color.FromArgb(255, 190, 146);
        public static readonly Color ViceTeal = Color.FromArgb(95, 216, 204);
        public static readonly Color StatusClear = Color.FromArgb(62, 232, 168);
        public static readonly Color StatusWanted = Color.FromArgb(255, 68, 92);
        public static readonly Color TextPrimary = Color.FromArgb(245, 244, 252);
        public static readonly Color TextSecondary = Color.FromArgb(168, 164, 194);

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        public static void EnableDarkTitleBar(Form form)
        {
            if (form == null || !form.IsHandleCreated) return;
            try
            {
                int dark = 1;
                if (DwmSetWindowAttribute(form.Handle, 20, ref dark, sizeof(int)) != 0)
                    DwmSetWindowAttribute(form.Handle, 19, ref dark, sizeof(int));
            }
            catch { }
        }
    }

    // 1. BORDERLESS GTA TAB CONTROL (Keeps tabs clickable in Designer, removes white frame at runtime)
    public class GtaTabControl : TabControl
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x1328 && !DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                m.Result = (IntPtr)1;
                return;
            }
            base.WndProc(ref m);
        }
    }

    // 2. GTA 6 HEADER BAR
    public class GtaHeaderBar : Panel
    {
        private string _titleText = "SCHOOL EQUIPMENT BORROWING SYSTEM";
        private string _subtitleText = "LEONIDA CAMPUS COMMAND  //  AV & LABORATORY INVENTORY";

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleText
        {
            get => _titleText;
            set { _titleText = value ?? ""; Invalidate(); }
        }

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SubtitleText
        {
            get => _subtitleText;
            set { _subtitleText = value ?? ""; Invalidate(); }
        }

        public GtaHeaderBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            Height = 76;
            BackColor = Color.FromArgb(13, 12, 21);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width < 10 || Height < 10) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (var bg = new LinearGradientBrush(
                ClientRectangle,
                Color.FromArgb(13, 12, 21),
                Color.FromArgb(30, 21, 48),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(bg, ClientRectangle);
            }

            Point[] slash1 = { new Point(Width - 340, 0), new Point(Width - 305, 0), new Point(Width - 365, Height), new Point(Width - 400, Height) };
            Point[] slash2 = { new Point(Width - 292, 0), new Point(Width - 278, 0), new Point(Width - 338, Height), new Point(Width - 352, Height) };
            using (var s1 = new SolidBrush(Color.FromArgb(20, 228, 36, 153))) g.FillPolygon(s1, slash1);
            using (var s2 = new SolidBrush(Color.FromArgb(18, 95, 216, 204))) g.FillPolygon(s2, slash2);

            using (var leftBar = new LinearGradientBrush(
                new Rectangle(16, 15, 4, 44),
                GtaPalette.ViceMagenta,
                GtaPalette.SunsetPeach,
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(leftBar, 16, 15, 4, 44);
            }

            using (var titleFont = new Font("Bahnschrift SemiBold", 16f, FontStyle.Bold))
            using (var subFont = new Font("Segoe UI", 8.5f, FontStyle.Regular))
            using (var titleBrush = new SolidBrush(GtaPalette.TextPrimary))
            using (var subBrush = new SolidBrush(GtaPalette.ViceTeal))
            {
                g.DrawString(_titleText, titleFont, titleBrush, 30, 14);
                g.DrawString(_subtitleText, subFont, subBrush, 31, 44);
            }

            Rectangle strip = new Rectangle(0, Height - 3, Width, 3);
            using (var grad = new LinearGradientBrush(strip, GtaPalette.ViceMagenta, GtaPalette.ViceTeal, LinearGradientMode.Horizontal))
            {
                grad.InterpolationColors = new ColorBlend
                {
                    Colors = new[] { GtaPalette.ViceMagenta, GtaPalette.LeonidaCoral, GtaPalette.SunsetPeach, GtaPalette.ViceTeal },
                    Positions = new[] { 0.0f, 0.36f, 0.70f, 1.0f }
                };
                g.FillRectangle(grad, strip);
            }
        }
    }

    // 3. GTA 6 PAUSE-MENU NAVIGATION TAB BUTTON
    public class GtaNavTabButton : Button
    {
        private bool _isActive = false;

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                ForeColor = _isActive ? GtaPalette.TextPrimary : GtaPalette.TextSecondary;
                BackColor = _isActive ? Color.FromArgb(32, 28, 50) : Color.FromArgb(19, 18, 30);
                Invalidate();
            }
        }

        public GtaNavTabButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = Color.FromArgb(36, 32, 56);
            FlatAppearance.MouseDownBackColor = Color.FromArgb(46, 38, 72);
            BackColor = Color.FromArgb(19, 18, 30);
            ForeColor = GtaPalette.TextSecondary;
            Font = new Font("Bahnschrift SemiBold", 9.75f, FontStyle.Bold);
            Cursor = Cursors.Hand;
            TextAlign = ContentAlignment.MiddleCenter;
            UseMnemonic = false;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            if (!_isActive || Width < 6 || Height < 6) return;

            Rectangle bar = new Rectangle(0, Height - 3, Width, 3);
            using (var grad = new LinearGradientBrush(bar, GtaPalette.ViceMagenta, GtaPalette.SunsetPeach, LinearGradientMode.Horizontal))
                pevent.Graphics.FillRectangle(grad, bar);
        }
    }

    // 4. GTA 6 LIVE HUD TELEMETRY TILE
    public class GtaHudStatTile : Panel
    {
        private string _label = "STAT LABEL";
        private string _valueText = "0";
        private string _caption = "LIVE TELEMETRY";
        private Color _accentColor = GtaPalette.ViceTeal;

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string LabelText
        {
            get => _label;
            set { _label = value ?? ""; Invalidate(); }
        }

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ValueText
        {
            get => _valueText;
            set { _valueText = value ?? "0"; Invalidate(); }
        }

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string CaptionText
        {
            get => _caption;
            set { _caption = value ?? ""; Invalidate(); }
        }

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        public GtaHudStatTile()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            BackColor = GtaPalette.SurfaceTile;
            Margin = new Padding(5, 4, 5, 4);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width < 20 || Height < 20) return;

            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (var border = new Pen(GtaPalette.BorderSubtle, 1f))
                g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);

            using (var leftBar = new SolidBrush(_accentColor))
                g.FillRectangle(leftBar, 0, 0, 4, Height);

            using (var lblFont = new Font("Bahnschrift SemiBold", 8.5f, FontStyle.Bold))
            using (var valFont = new Font("Impact", 19f, FontStyle.Regular))
            using (var capFont = new Font("Segoe UI", 8f, FontStyle.Regular))
            using (var lblBrush = new SolidBrush(GtaPalette.TextSecondary))
            using (var valBrush = new SolidBrush(_accentColor))
            using (var capBrush = new SolidBrush(Color.FromArgb(135, 130, 165)))
            {
                g.DrawString(_label.ToUpperInvariant(), lblFont, lblBrush, 14, 7);
                g.DrawString(_caption, capFont, capBrush, 14, 24);

                SizeF valSize = g.MeasureString(_valueText, valFont);
                g.DrawString(_valueText, valFont, valBrush, Width - valSize.Width - 14, (Height - valSize.Height) / 2f + 1);
            }
        }
    }

    // 5. GTA 6 SHARP SQUARED TILE BUTTON
    public class GtaTileButton : Button
    {
        private Color _accentColor = GtaPalette.ViceTeal;
        private bool _isHovered = false;

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        public GtaTileButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = GtaPalette.SurfaceTile;
            ForeColor = GtaPalette.TextPrimary;
            Font = new Font("Bahnschrift SemiBold", 9.25f, FontStyle.Bold);
            Cursor = Cursors.Hand;
            Size = new Size(165, 34);
            UseMnemonic = false;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (Width < 4 || Height < 4) return;
            Graphics g = pevent.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Color fill = _isHovered ? _accentColor : GtaPalette.SurfaceTile;
            Color textCol = _isHovered ? Color.FromArgb(14, 13, 22) : GtaPalette.TextPrimary;

            using (var bg = new SolidBrush(fill))
                g.FillRectangle(bg, ClientRectangle);

            using (var border = new Pen(_isHovered ? _accentColor : GtaPalette.BorderSubtle, 1f))
                g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);

            if (!_isHovered)
            {
                using (var bar = new SolidBrush(_accentColor))
                    g.FillRectangle(bar, 0, 0, 4, Height);
            }

            TextRenderer.DrawText(
                g,
                Text.ToUpperInvariant(),
                Font,
                ClientRectangle,
                textCol,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }
    }

    // 6. GTA 6 SQUARED CARD PANEL
    public class GtaCardPanel : Panel
    {
        private string _cardTitle = "SECTION TITLE";
        private Color _accentColor = GtaPalette.ViceTeal;

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string CardTitle
        {
            get => _cardTitle;
            set { _cardTitle = value ?? ""; Invalidate(); }
        }

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        public GtaCardPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            BackColor = GtaPalette.SurfaceTile;
            Padding = new Padding(14, 44, 14, 14);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width < 10 || Height < 10) return;

            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (var border = new Pen(GtaPalette.BorderSubtle, 1f))
                g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);

            using (var topBar = new SolidBrush(_accentColor))
                g.FillRectangle(topBar, 0, 0, Width, 3);

            using (var sep = new Pen(Color.FromArgb(44, 40, 68), 1f))
                g.DrawLine(sep, 1, 36, Width - 2, 36);

            using (var titleFont = new Font("Bahnschrift SemiBold", 10f, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(_accentColor))
                g.DrawString(_cardTitle.ToUpperInvariant(), titleFont, titleBrush, 14, 11);
        }
    }

    // 7. GTA 6 DARK NEON DATA GRID
    public class GtaDataGridView : DataGridView
    {
        public GtaDataGridView()
        {
            DoubleBuffered = true;
            BackgroundColor = GtaPalette.SurfaceTile;
            BorderStyle = BorderStyle.None;
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            GridColor = Color.FromArgb(44, 40, 68);
            EnableHeadersVisualStyles = false;
            RowHeadersVisible = false;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            AllowUserToResizeRows = false;
            RowTemplate.Height = 34;

            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            ColumnHeadersHeight = 38;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(19, 18, 30),
                ForeColor = GtaPalette.ViceTeal,
                Font = new Font("Bahnschrift SemiBold", 9.5f, FontStyle.Bold),
                SelectionBackColor = Color.FromArgb(19, 18, 30),
                SelectionForeColor = GtaPalette.ViceTeal,
                Padding = new Padding(8, 0, 8, 0)
            };

            DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = GtaPalette.SurfaceTile,
                ForeColor = GtaPalette.TextPrimary,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                SelectionBackColor = Color.FromArgb(148, 26, 102),
                SelectionForeColor = Color.White,
                Padding = new Padding(8, 0, 8, 0)
            };

            AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = GtaPalette.SurfaceAlt,
                ForeColor = GtaPalette.TextPrimary,
                SelectionBackColor = Color.FromArgb(148, 26, 102),
                SelectionForeColor = Color.White
            };
        }

        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            base.OnCellFormatting(e);
            if (e.Value == null || e.ColumnIndex < 0) return;

            string colName = Columns[e.ColumnIndex].Name;
            if (colName != "Status" && colName != "Hold") return;

            string val = e.Value.ToString().ToUpperInvariant();
            if (val.Contains("HOLD") || val.Contains("UNAVAILABLE") || val.Contains("OVERDUE") || val.Contains("REPAIR"))
            {
                e.CellStyle.ForeColor = GtaPalette.StatusWanted;
                e.CellStyle.Font = new Font("Bahnschrift SemiBold", 9.5f, FontStyle.Bold);
            }
            else if (val.Contains("AVAILABLE") || val.Contains("CLEAR") || val.Contains("✓") || val == "ACTIVE")
            {
                e.CellStyle.ForeColor = GtaPalette.StatusClear;
                e.CellStyle.Font = new Font("Bahnschrift SemiBold", 9.5f, FontStyle.Bold);
            }
        }
    }

    // 8. GTA 6 "MISSION PASSED / HOLD" SLIDE-IN TOAST BANNER
    public class GtaToastBanner : Panel
    {
        private string _headline = "MISSION PASSED";
        private string _detail = "Action completed successfully.";
        private Color _accentColor = GtaPalette.StatusClear;
        private readonly System.Windows.Forms.Timer _hideTimer;

        public GtaToastBanner()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            Size = new Size(410, 62);
            BackColor = Color.FromArgb(20, 18, 34);
            Visible = false;

            _hideTimer = new System.Windows.Forms.Timer { Interval = 3400 };
            _hideTimer.Tick += (s, e) =>
            {
                _hideTimer.Stop();
                Visible = false;
            };
        }

        public void ShowBanner(string headline, string detail, Color accent)
        {
            _headline = headline ?? "";
            _detail = detail ?? "";
            _accentColor = accent;
            Visible = true;
            BringToFront();
            Invalidate();

            _hideTimer.Stop();
            _hideTimer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _hideTimer.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (var bg = new LinearGradientBrush(
                ClientRectangle,
                Color.FromArgb(24, 21, 40),
                Color.FromArgb(14, 13, 24),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(bg, ClientRectangle);
            }

            using (var border = new Pen(_accentColor, 1.5f))
                g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);

            using (var leftBar = new SolidBrush(_accentColor))
                g.FillRectangle(leftBar, 0, 0, 6, Height);

            using (var headFont = new Font("Impact", 13.5f, FontStyle.Regular))
            using (var detFont = new Font("Segoe UI", 8.75f, FontStyle.Regular))
            using (var headBrush = new SolidBrush(_accentColor))
            using (var detBrush = new SolidBrush(GtaPalette.TextPrimary))
            {
                g.DrawString(_headline.ToUpperInvariant(), headFont, headBrush, 16, 8);
                g.DrawString(_detail, detFont, detBrush, 16, 35);
            }
        }
    }
}