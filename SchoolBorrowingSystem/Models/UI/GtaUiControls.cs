using System;
using System.Collections.Generic;
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
        public static readonly Color CanvasDark = Color.FromArgb(14, 13, 24);
        public static readonly Color SurfaceCard = Color.FromArgb(22, 20, 38);
        public static readonly Color SurfaceTile = Color.FromArgb(28, 25, 46);
        public static readonly Color SurfaceAlt = Color.FromArgb(26, 24, 44);
        public static readonly Color SurfaceInput = Color.FromArgb(34, 30, 56);
        public static readonly Color BorderSubtle = Color.FromArgb(56, 50, 88);
        public static readonly Color ViceMagenta = Color.FromArgb(236, 46, 158);
        public static readonly Color LeonidaCoral = Color.FromArgb(255, 114, 107);
        public static readonly Color SunsetPeach = Color.FromArgb(255, 196, 134);
        public static readonly Color ViceTeal = Color.FromArgb(72, 232, 214);
        public static readonly Color StatusClear = Color.FromArgb(56, 238, 170);
        public static readonly Color StatusWanted = Color.FromArgb(255, 68, 96);
        public static readonly Color TextPrimary = Color.FromArgb(246, 245, 255);
        public static readonly Color TextSecondary = Color.FromArgb(164, 158, 196);

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

        public static GraphicsPath CreateRoundedRect(RectangleF rect, float radius)
        {
            var path = new GraphicsPath();
            float maxR = Math.Min(rect.Width, rect.Height) / 2f;
            float r = Math.Max(1f, Math.Min(radius, maxR));
            float d = r * 2f;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    // 1. BORDERLESS GTA TAB CONTROL
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

    // 2. MODERN GTA 6 HEADER BAR (Ambient sunset glow + subtle skyline silhouettes)
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
            Height = 82;
            BackColor = Color.FromArgb(14, 12, 24);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width < 20 || Height < 20) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Rich midnight-to-vice-plum horizontal backdrop
            using (var bg = new LinearGradientBrush(
                ClientRectangle,
                Color.FromArgb(13, 12, 23),
                Color.FromArgb(34, 18, 52),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(bg, ClientRectangle);
            }

            // Ambient radial sunset bloom on the right side
            Rectangle bloomRect = new Rectangle(Width - 460, -60, 420, 210);
            using (var bloomPath = new GraphicsPath())
            {
                bloomPath.AddEllipse(bloomRect);
                using (var pgb = new PathGradientBrush(bloomPath))
                {
                    pgb.CenterColor = Color.FromArgb(52, 236, 46, 158);
                    pgb.SurroundColors = new[] { Color.FromArgb(0, 236, 46, 158) };
                    g.FillEllipse(pgb, bloomRect);
                }
            }

            // Subtle distant Vice skyline silhouette along bottom-right
            int[] bHeights = { 18, 34, 24, 44, 28, 38, 22, 48, 32, 20, 36, 26 };
            int startX = Width - 520;
            using (var skySil = new SolidBrush(Color.FromArgb(32, 10, 8, 20)))
            {
                for (int i = 0; i < bHeights.Length; i++)
                {
                    int bx = startX + (i * 26);
                    g.FillRectangle(skySil, bx, Height - 3 - bHeights[i], 22, bHeights[i]);
                }
            }

            // Slanted Leonida light streaks
            Point[] slash1 = { new Point(Width - 380, 0), new Point(Width - 348, 0), new Point(Width - 404, Height), new Point(Width - 436, Height) };
            Point[] slash2 = { new Point(Width - 336, 0), new Point(Width - 324, 0), new Point(Width - 380, Height), new Point(Width - 392, Height) };
            using (var s1 = new SolidBrush(Color.FromArgb(24, 236, 46, 158))) g.FillPolygon(s1, slash1);
            using (var s2 = new SolidBrush(Color.FromArgb(24, 72, 232, 214))) g.FillPolygon(s2, slash2);

            // Stylized "VI" rounded neon monogram emblem on the far left
            Rectangle badgeRect = new Rectangle(18, 17, 46, 46);
            using (var badgePath = GtaPalette.CreateRoundedRect(badgeRect, 10))
            {
                using (var badgeBg = new LinearGradientBrush(badgeRect, Color.FromArgb(44, 22, 68), Color.FromArgb(24, 16, 42), 45f))
                    g.FillPath(badgeBg, badgePath);

                using (var rimBrush = new LinearGradientBrush(badgeRect, GtaPalette.ViceMagenta, GtaPalette.SunsetPeach, 45f))
                using (var rimPen = new Pen(rimBrush, 2f))
                    g.DrawPath(rimPen, badgePath);

                using (var viFont = new Font("Impact", 18f, FontStyle.Italic))
                using (var viBrush = new LinearGradientBrush(badgeRect, Color.White, GtaPalette.SunsetPeach, 90f))
                {
                    SizeF sz = g.MeasureString("VI", viFont);
                    g.DrawString("VI", viFont, viBrush, badgeRect.X + (badgeRect.Width - sz.Width) / 2f - 1, badgeRect.Y + (badgeRect.Height - sz.Height) / 2f + 1);
                }
            }

            // Main Title & Subtitle
            using (var titleFont = new Font("Bahnschrift SemiBold", 16.5f, FontStyle.Bold))
            using (var subFont = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(GtaPalette.TextPrimary))
            using (var subBrush = new SolidBrush(GtaPalette.ViceTeal))
            {
                g.DrawString(_titleText, titleFont, titleBrush, 76, 16);
                g.DrawString(_subtitleText, subFont, subBrush, 77, 47);
            }

            // Glowing Leonida Sunset bottom horizon line
            Rectangle strip = new Rectangle(0, Height - 3, Width, 3);
            using (var grad = new LinearGradientBrush(strip, GtaPalette.ViceMagenta, GtaPalette.ViceTeal, LinearGradientMode.Horizontal))
            {
                grad.InterpolationColors = new ColorBlend
                {
                    Colors = new[] { GtaPalette.ViceMagenta, GtaPalette.LeonidaCoral, GtaPalette.SunsetPeach, GtaPalette.ViceTeal },
                    Positions = new[] { 0.0f, 0.35f, 0.68f, 1.0f }
                };
                g.FillRectangle(grad, strip);
            }
        }
    }

    // 3. GTA 6 PAUSE-MENU NAVIGATION TAB BUTTON
    public class GtaNavTabButton : Button
    {
        private bool _isActive = false;
        private bool _isHovered = false;

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; Invalidate(); }
        }

        public GtaNavTabButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.FromArgb(18, 16, 30);
            ForeColor = GtaPalette.TextSecondary;
            Font = new Font("Bahnschrift SemiBold", 9.75f, FontStyle.Bold);
            Cursor = Cursors.Hand;
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
            if (Width < 6 || Height < 6) return;
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Base nav bar background
            using (var baseBg = new SolidBrush(Color.FromArgb(18, 16, 30)))
                g.FillRectangle(baseBg, ClientRectangle);

            // Subtle pill highlight on active or hover
            if (_isActive || _isHovered)
            {
                RectangleF pill = new RectangleF(6, 5, Width - 12, Height - 10);
                using (var path = GtaPalette.CreateRoundedRect(pill, 7))
                {
                    if (_isActive)
                    {
                        using (var actBg = new LinearGradientBrush(pill, Color.FromArgb(46, 26, 66), Color.FromArgb(30, 24, 52), LinearGradientMode.Vertical))
                            g.FillPath(actBg, path);
                        using (var actBorder = new Pen(Color.FromArgb(90, 236, 46, 158), 1f))
                            g.DrawPath(actBorder, path);
                    }
                    else
                    {
                        using (var hovBg = new SolidBrush(Color.FromArgb(30, 27, 48)))
                            g.FillPath(hovBg, path);
                    }
                }
            }

            // Text
            Color textColor = _isActive ? GtaPalette.TextPrimary : (_isHovered ? Color.FromArgb(220, 216, 245) : GtaPalette.TextSecondary);
            TextRenderer.DrawText(
                g,
                Text.ToUpperInvariant(),
                Font,
                ClientRectangle,
                textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            // Glowing sunset bottom indicator on active tab
            if (_isActive)
            {
                RectangleF bar = new RectangleF(16, Height - 3, Width - 32, 3);
                using (var grad = new LinearGradientBrush(bar, GtaPalette.ViceMagenta, GtaPalette.SunsetPeach, LinearGradientMode.Horizontal))
                    g.FillRectangle(grad, bar);
            }
        }
    }

    // 4. MODERN GTA 6 HUD TELEMETRY TILE (Rounded glass card + ambient glow + neon bar)
    public class GtaHudStatTile : Panel
    {
        private string _label = "STAT LABEL";
        private string _valueText = "0";
        private string _caption = "LIVE TELEMETRY";
        private Color _accentColor = GtaPalette.ViceTeal;
        private int _fillPercent = 65;

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

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int FillPercent
        {
            get => _fillPercent;
            set { _fillPercent = Math.Max(0, Math.Min(100, value)); Invalidate(); }
        }

        public GtaHudStatTile()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            BackColor = GtaPalette.CanvasDark;
            Margin = new Padding(6, 4, 6, 4);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width < 30 || Height < 30) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            RectangleF cardRect = new RectangleF(1, 1, Width - 3, Height - 3);
            using (var path = GtaPalette.CreateRoundedRect(cardRect, 10))
            {
                using (var bg = new LinearGradientBrush(cardRect, Color.FromArgb(30, 27, 50), Color.FromArgb(20, 18, 34), 35f))
                    g.FillPath(bg, path);

                // Ambient glow behind numeric value on right
                g.SetClip(path);
                Rectangle glowRect = new Rectangle(Width - 115, -20, 140, Height + 40);
                using (var gPath = new GraphicsPath())
                {
                    gPath.AddEllipse(glowRect);
                    using (var pgb = new PathGradientBrush(gPath))
                    {
                        pgb.CenterColor = Color.FromArgb(46, _accentColor);
                        pgb.SurroundColors = new[] { Color.FromArgb(0, _accentColor) };
                        g.FillEllipse(pgb, glowRect);
                    }
                }
                g.ResetClip();

                using (var border = new Pen(Color.FromArgb(65, _accentColor.R, _accentColor.G, _accentColor.B), 1.2f))
                    g.DrawPath(border, path);
            }

            // Neon status dot + Label + Caption
            using (var dotBrush = new SolidBrush(_accentColor))
                g.FillEllipse(dotBrush, 14, 13, 8, 8);

            using (var lblFont = new Font("Bahnschrift SemiBold", 8.75f, FontStyle.Bold))
            using (var capFont = new Font("Segoe UI", 8f, FontStyle.Regular))
            using (var valFont = new Font("Impact", 22f, FontStyle.Regular))
            using (var lblBrush = new SolidBrush(GtaPalette.TextPrimary))
            using (var capBrush = new SolidBrush(GtaPalette.TextSecondary))
            using (var valBrush = new SolidBrush(_accentColor))
            {
                g.DrawString(_label.ToUpperInvariant(), lblFont, lblBrush, 28, 9);
                g.DrawString(_caption, capFont, capBrush, 14, 28);

                SizeF valSize = g.MeasureString(_valueText, valFont);
                g.DrawString(_valueText, valFont, valBrush, Width - valSize.Width - 16, 10);
            }

            // Sleek proportional progress bar along bottom of card
            RectangleF track = new RectangleF(14, Height - 13, Width - 28, 4);
            using (var trackPath = GtaPalette.CreateRoundedRect(track, 2))
            using (var trackBrush = new SolidBrush(Color.FromArgb(44, 40, 68)))
                g.FillPath(trackBrush, trackPath);

            float fillW = (Width - 28) * (_fillPercent / 100f);
            if (fillW > 3)
            {
                RectangleF fill = new RectangleF(14, Height - 13, fillW, 4);
                using (var fillPath = GtaPalette.CreateRoundedRect(fill, 2))
                using (var fillBrush = new LinearGradientBrush(track, _accentColor, Color.White, LinearGradientMode.Horizontal))
                    g.FillPath(fillBrush, fillPath);
            }
        }
    }

    // 5. MODERN GTA 6 ROUNDED NEON TILE BUTTON
    public class GtaTileButton : Button
    {
        private Color _accentColor = GtaPalette.ViceTeal;
        private bool _isHovered = false;
        private bool _isPressed = false;

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        public GtaTileButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = GtaPalette.CanvasDark;
            ForeColor = GtaPalette.TextPrimary;
            Font = new Font("Bahnschrift SemiBold", 9.25f, FontStyle.Bold);
            Cursor = Cursors.Hand;
            Size = new Size(165, 36);
            UseMnemonic = false;
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; _isPressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs mevent) { _isPressed = true; Invalidate(); base.OnMouseDown(mevent); }
        protected override void OnMouseUp(MouseEventArgs mevent) { _isPressed = false; Invalidate(); base.OnMouseUp(mevent); }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (Width < 8 || Height < 8) return;
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Paint parent color in corners so rounded edges blend seamlessly
            Color parentBg = Parent != null ? Parent.BackColor : GtaPalette.CanvasDark;
            using (var clearBrush = new SolidBrush(parentBg))
                g.FillRectangle(clearBrush, ClientRectangle);

            RectangleF rect = new RectangleF(1, 1, Width - 3, Height - 3);
            using (var path = GtaPalette.CreateRoundedRect(rect, 7))
            {
                if (_isHovered)
                {
                    Color c1 = _isPressed ? ControlPaint.Dark(_accentColor, 0.12f) : _accentColor;
                    Color c2 = ControlPaint.Dark(_accentColor, 0.2f);
                    using (var fill = new LinearGradientBrush(rect, c1, c2, LinearGradientMode.Vertical))
                        g.FillPath(fill, path);
                }
                else
                {
                    using (var fill = new LinearGradientBrush(rect, Color.FromArgb(36, 32, 58), Color.FromArgb(26, 23, 44), LinearGradientMode.Vertical))
                        g.FillPath(fill, path);

                    // Left neon pill accent inside idle button
                    g.SetClip(path);
                    using (var leftBar = new SolidBrush(_accentColor))
                        g.FillRectangle(leftBar, 1, 1, 5, Height - 2);
                    g.ResetClip();
                }

                Color borderColor = _isHovered ? Color.White : Color.FromArgb(120, _accentColor);
                using (var border = new Pen(borderColor, _isHovered ? 1.5f : 1.1f))
                    g.DrawPath(border, path);
            }

            Color textCol = _isHovered ? Color.FromArgb(14, 12, 24) : GtaPalette.TextPrimary;
            TextRenderer.DrawText(
                g,
                Text.ToUpperInvariant(),
                Font,
                ClientRectangle,
                textCol,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }
    }

    // 6. MODERN GTA 6 GLASSMORPHIC CARD PANEL
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
            BackColor = GtaPalette.SurfaceCard;
            Padding = new Padding(14, 46, 14, 14);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width < 16 || Height < 16) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Color parentBg = Parent != null ? Parent.BackColor : GtaPalette.CanvasDark;
            using (var clearBrush = new SolidBrush(parentBg))
                g.FillRectangle(clearBrush, ClientRectangle);

            RectangleF cardRect = new RectangleF(1, 1, Width - 3, Height - 3);
            using (var path = GtaPalette.CreateRoundedRect(cardRect, 10))
            {
                using (var bg = new LinearGradientBrush(cardRect, Color.FromArgb(26, 23, 44), Color.FromArgb(19, 17, 33), LinearGradientMode.Vertical))
                    g.FillPath(bg, path);

                // Header band tint
                g.SetClip(path);
                using (var headerFill = new SolidBrush(Color.FromArgb(30, 26, 52)))
                    g.FillRectangle(headerFill, 0, 0, Width, 38);

                using (var topBar = new LinearGradientBrush(new Rectangle(0, 0, Width, 3), _accentColor, Color.FromArgb(60, _accentColor), LinearGradientMode.Horizontal))
                    g.FillRectangle(topBar, 0, 0, Width, 3);
                g.ResetClip();

                using (var sep = new Pen(Color.FromArgb(46, 42, 72), 1f))
                    g.DrawLine(sep, 2, 38, Width - 3, 38);

                using (var border = new Pen(GtaPalette.BorderSubtle, 1.1f))
                    g.DrawPath(border, path);
            }

            // Neon indicator pill next to card title
            using (var accentBrush = new SolidBrush(_accentColor))
                g.FillRectangle(accentBrush, 14, 14, 4, 13);

            using (var titleFont = new Font("Bahnschrift SemiBold", 9.75f, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(GtaPalette.TextPrimary))
                g.DrawString(_cardTitle.ToUpperInvariant(), titleFont, titleBrush, 24, 12);
        }
    }

    // 7. MODERN GTA 6 SEARCH BOX (Rounded frame + search glyph + glowing focus border)
    public class GtaSearchBox : UserControl
    {
        private readonly TextBox _innerTextBox;
        private bool _isFocused = false;
        private Color _accentColor = GtaPalette.ViceTeal;

        public event EventHandler SearchTextChanged;

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SearchText
        {
            get => _innerTextBox.Text;
            set => _innerTextBox.Text = value ?? "";
        }

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Placeholder
        {
            get => _innerTextBox.PlaceholderText;
            set => _innerTextBox.PlaceholderText = value ?? "";
        }

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        public GtaSearchBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            Size = new Size(250, 34);
            BackColor = GtaPalette.CanvasDark;

            _innerTextBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(30, 27, 50),
                ForeColor = GtaPalette.TextPrimary,
                Font = new Font("Segoe UI", 9.5f),
                PlaceholderText = "Search..."
            };
            _innerTextBox.Enter += (s, e) => { _isFocused = true; Invalidate(); };
            _innerTextBox.Leave += (s, e) => { _isFocused = false; Invalidate(); };
            _innerTextBox.TextChanged += (s, e) => SearchTextChanged?.Invoke(this, EventArgs.Empty);

            Controls.Add(_innerTextBox);
            PositionInnerBox();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PositionInnerBox();
        }

        private void PositionInnerBox()
        {
            if (_innerTextBox == null) return;
            _innerTextBox.Location = new Point(32, Math.Max(4, (Height - _innerTextBox.Height) / 2));
            _innerTextBox.Width = Math.Max(40, Width - 42);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width < 16 || Height < 16) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color parentBg = Parent != null ? Parent.BackColor : GtaPalette.CanvasDark;
            using (var clearBrush = new SolidBrush(parentBg))
                g.FillRectangle(clearBrush, ClientRectangle);

            RectangleF rect = new RectangleF(1, 1, Width - 3, Height - 3);
            using (var path = GtaPalette.CreateRoundedRect(rect, 8))
            {
                using (var bg = new SolidBrush(Color.FromArgb(30, 27, 50)))
                    g.FillPath(bg, path);

                using (var border = new Pen(_isFocused ? _accentColor : GtaPalette.BorderSubtle, _isFocused ? 1.5f : 1f))
                    g.DrawPath(border, path);
            }

            // Minimal magnifying glass vector icon
            int cy = Height / 2;
            using (var iconPen = new Pen(_isFocused ? _accentColor : GtaPalette.TextSecondary, 1.8f))
            {
                g.DrawEllipse(iconPen, 11, cy - 6, 10, 10);
                g.DrawLine(iconPen, 20, cy + 3, 24, cy + 7);
            }
        }
    }

    // 8. MODERN GTA 6 TOGGLE SWITCH (Replaces plain white Windows CheckBoxes)
    public class GtaToggleSwitch : Control
    {
        private bool _checked = false;
        private Color _onColor = GtaPalette.ViceTeal;

        public event EventHandler CheckedChanged;

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked == value) return;
                _checked = value;
                Invalidate();
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color OnColor
        {
            get => _onColor;
            set { _onColor = value; Invalidate(); }
        }

        public GtaToggleSwitch()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            Size = new Size(165, 28);
            Font = new Font("Bahnschrift SemiBold", 9f, FontStyle.Bold);
            ForeColor = GtaPalette.TextPrimary;
            Cursor = Cursors.Hand;
        }

        protected override void OnClick(EventArgs e)
        {
            Checked = !Checked;
            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Color parentBg = Parent != null ? Parent.BackColor : GtaPalette.CanvasDark;
            using (var clearBrush = new SolidBrush(parentBg))
                g.FillRectangle(clearBrush, ClientRectangle);

            float pillY = (Height - 20) / 2f;
            RectangleF pillRect = new RectangleF(2, pillY, 38, 20);
            using (var path = GtaPalette.CreateRoundedRect(pillRect, 10))
            {
                using (var bg = new SolidBrush(_checked ? Color.FromArgb(65, _onColor) : Color.FromArgb(38, 34, 62)))
                    g.FillPath(bg, path);
                using (var border = new Pen(_checked ? _onColor : GtaPalette.BorderSubtle, 1.3f))
                    g.DrawPath(border, path);
            }

            float knobX = _checked ? 22 : 5;
            using (var knobBrush = new SolidBrush(_checked ? _onColor : GtaPalette.TextSecondary))
                g.FillEllipse(knobBrush, knobX, pillY + 3, 14, 14);

            Rectangle textRect = new Rectangle(48, 0, Width - 50, Height);
            TextRenderer.DrawText(
                g,
                Text,
                Font,
                textRect,
                _checked ? GtaPalette.TextPrimary : GtaPalette.TextSecondary,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }
    }

    // 9. MODERN GTA 6 DATA GRID (Neon pill badges + magenta left selection bar)
    public class GtaDataGridView : DataGridView
    {
        public GtaDataGridView()
        {
            DoubleBuffered = true;
            BackgroundColor = GtaPalette.SurfaceCard;
            BorderStyle = BorderStyle.None;
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            GridColor = Color.FromArgb(40, 36, 66);
            EnableHeadersVisualStyles = false;
            RowHeadersVisible = false;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            AllowUserToResizeRows = false;
            RowTemplate.Height = 38;

            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            ColumnHeadersHeight = 40;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(18, 16, 30),
                ForeColor = GtaPalette.ViceTeal,
                Font = new Font("Bahnschrift SemiBold", 9.25f, FontStyle.Bold),
                SelectionBackColor = Color.FromArgb(18, 16, 30),
                SelectionForeColor = GtaPalette.ViceTeal,
                Padding = new Padding(10, 0, 10, 0)
            };

            DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = GtaPalette.SurfaceCard,
                ForeColor = GtaPalette.TextPrimary,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                SelectionBackColor = Color.FromArgb(56, 26, 62),
                SelectionForeColor = Color.White,
                Padding = new Padding(10, 0, 10, 0)
            };

            AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = GtaPalette.SurfaceAlt,
                ForeColor = GtaPalette.TextPrimary,
                SelectionBackColor = Color.FromArgb(56, 26, 62),
                SelectionForeColor = Color.White
            };
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                base.OnCellPainting(e);
                return;
            }

            string colName = Columns[e.ColumnIndex].Name;
            bool isSelected = (e.State & DataGridViewElementStates.Selected) != 0;

            // Render custom rounded neon status pills for Status and Hold columns
            if ((colName == "Status" || colName == "Hold") && e.Value != null)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);

                string text = e.Value.ToString().ToUpperInvariant();
                Color pillAccent = GtaPalette.ViceTeal;
                if (text.Contains("HOLD") || text.Contains("UNAVAILABLE") || text.Contains("OVERDUE") || text.Contains("REPAIR"))
                    pillAccent = GtaPalette.StatusWanted;
                else if (text.Contains("AVAILABLE") || text.Contains("CLEAR") || text.Contains("✓") || text == "ACTIVE")
                    pillAccent = GtaPalette.StatusClear;

                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                int pillH = 22;
                int pillW = Math.Min(e.CellBounds.Width - 16, Math.Max(88, text.Length * 8 + 20));
                RectangleF pillRect = new RectangleF(
                    e.CellBounds.X + 8,
                    e.CellBounds.Y + (e.CellBounds.Height - pillH) / 2f,
                    pillW,
                    pillH);

                using (var path = GtaPalette.CreateRoundedRect(pillRect, 11))
                {
                    using (var fill = new SolidBrush(Color.FromArgb(36, pillAccent)))
                        g.FillPath(fill, path);
                    using (var border = new Pen(Color.FromArgb(190, pillAccent), 1.2f))
                        g.DrawPath(border, path);
                }

                using (var pillFont = new Font("Bahnschrift SemiBold", 8.5f, FontStyle.Bold))
                {
                    TextRenderer.DrawText(
                        g,
                        text,
                        pillFont,
                        Rectangle.Round(pillRect),
                        pillAccent,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }

                e.Handled = true;
                return;
            }

            base.OnCellPainting(e);

            // GTA Pause-Menu bright Vice Magenta left bar on the selected row
            if (isSelected && e.ColumnIndex == 0)
            {
                using (var bar = new LinearGradientBrush(
                    new Rectangle(e.CellBounds.X, e.CellBounds.Y, 4, e.CellBounds.Height),
                    GtaPalette.ViceMagenta,
                    GtaPalette.SunsetPeach,
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(bar, e.CellBounds.X, e.CellBounds.Y, 4, e.CellBounds.Height);
                }
            }
        }
    }

    // 10. LIVE GTA 6 ASSET DOSSIER INSPECTOR (Fills the Inventory right-hand side with rich visual data)
    public class GtaAssetDossierPanel : Panel
    {
        private string _tag = "AV-101";
        private string _itemName = "Epson Full-HD Laser Projector";
        private string _category = "AV EQUIPMENT";
        private string _condition = "Good";
        private bool _isAvailable = true;
        private bool _isAv = true;
        private bool _hasSelection = true;
        private readonly List<string> _checklist = new List<string>
        {
            "Main Unit & Power Adapter",
            "Remote Controller (with batteries)",
            "High-Speed HDMI Cable"
        };

        public GtaAssetDossierPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            BackColor = GtaPalette.SurfaceCard;
        }

        public void SetAsset(string tag, string itemName, string category, string condition, bool isAvailable, bool isAv, IEnumerable<string> checklist)
        {
            _hasSelection = !string.IsNullOrEmpty(tag);
            _tag = tag ?? "";
            _itemName = itemName ?? "";
            _category = (category ?? "").ToUpperInvariant();
            _condition = condition ?? "Good";
            _isAvailable = isAvailable;
            _isAv = isAv;

            _checklist.Clear();
            if (checklist != null)
            {
                foreach (var c in checklist) _checklist.Add(c);
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width < 40 || Height < 60) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            if (!_hasSelection)
            {
                using (var f = new Font("Bahnschrift SemiBold", 10f, FontStyle.Bold))
                using (var b = new SolidBrush(GtaPalette.TextSecondary))
                    g.DrawString("SELECT AN EQUIPMENT ROW\r\nTO INSPECT ASSET DOSSIER", f, b, 16, 40);
                return;
            }

            Color accent = _isAvailable ? (_isAv ? GtaPalette.ViceTeal : GtaPalette.ViceMagenta) : GtaPalette.StatusWanted;

            // 1. Mini Vice City Sunset Hero Banner
            RectangleF banner = new RectangleF(0, 0, Width - 1, 108);
            using (var bPath = GtaPalette.CreateRoundedRect(banner, 9))
            {
                using (var grad = new LinearGradientBrush(banner,
                    Color.FromArgb(46, 20, 74),
                    _isAv ? Color.FromArgb(24, 62, 86) : Color.FromArgb(118, 28, 92),
                    35f))
                {
                    g.FillPath(grad, bPath);
                }

                // Stylized Vector Hardware Graphic (Projector/AV vs Lab Oscilloscope/Microscope)
                DrawHardwareGlyph(g, Width - 88, 22, _isAv, accent);

                // Property Tag Pill
                RectangleF tagPill = new RectangleF(12, 12, 86, 22);
                using (var tp = GtaPalette.CreateRoundedRect(tagPill, 6))
                {
                    using (var tb = new SolidBrush(Color.FromArgb(190, 14, 13, 24))) g.FillPath(tb, tp);
                    using (var tpen = new Pen(accent, 1.2f)) g.DrawPath(tpen, tp);
                }
                using (var tagFont = new Font("Consolas", 9.5f, FontStyle.Bold))
                using (var tagBrush = new SolidBrush(accent))
                    g.DrawString(_tag, tagFont, tagBrush, 19, 15);

                // Status Pill top-right
                string stText = _isAvailable ? "AVAILABLE" : "UNAVAILABLE";
                Color stCol = _isAvailable ? GtaPalette.StatusClear : GtaPalette.StatusWanted;
                RectangleF stPill = new RectangleF(104, 12, 92, 22);
                using (var sp = GtaPalette.CreateRoundedRect(stPill, 11))
                {
                    using (var sb = new SolidBrush(Color.FromArgb(42, stCol))) g.FillPath(sb, sp);
                    using (var spen = new Pen(stCol, 1.1f)) g.DrawPath(spen, sp);
                }
                using (var sf = new Font("Bahnschrift SemiBold", 8f, FontStyle.Bold))
                    TextRenderer.DrawText(g, stText, sf, Rectangle.Round(stPill), stCol, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                // Equipment Name + Category inside banner
                using (var nameFont = new Font("Bahnschrift SemiBold", 11.5f, FontStyle.Bold))
                using (var catFont = new Font("Segoe UI", 8.25f, FontStyle.Bold))
                using (var nb = new SolidBrush(Color.White))
                using (var cb = new SolidBrush(GtaPalette.SunsetPeach))
                {
                    g.DrawString(_category, catFont, cb, 12, 46);
                    g.DrawString(_itemName, nameFont, nb, new RectangleF(12, 64, Width - 102, 40));
                }

                using (var bp = new Pen(Color.FromArgb(110, accent), 1.2f))
                    g.DrawPath(bp, bPath);
            }

            // 2. Hardware Health / Condition Bar
            int y = 122;
            int healthPct = _condition.Equals("Good", StringComparison.OrdinalIgnoreCase) ? 100
                          : _condition.Equals("Fair", StringComparison.OrdinalIgnoreCase) ? 70 : 25;
            Color healthCol = healthPct >= 90 ? GtaPalette.StatusClear : (healthPct >= 60 ? GtaPalette.SunsetPeach : GtaPalette.StatusWanted);

            using (var lblFont = new Font("Bahnschrift SemiBold", 8.5f, FontStyle.Bold))
            using (var mutedBrush = new SolidBrush(GtaPalette.TextSecondary))
            using (var valBrush = new SolidBrush(healthCol))
            {
                g.DrawString("HARDWARE INTEGRITY", lblFont, mutedBrush, 2, y);
                string condText = _condition.ToUpperInvariant() + " (" + healthPct + "%)";
                SizeF cs = g.MeasureString(condText, lblFont);
                g.DrawString(condText, lblFont, valBrush, Width - cs.Width - 2, y);
            }

            y += 20;
            RectangleF hTrack = new RectangleF(2, y, Width - 6, 7);
            using (var tp = GtaPalette.CreateRoundedRect(hTrack, 3))
            using (var tb = new SolidBrush(Color.FromArgb(38, 34, 62)))
                g.FillPath(tb, tp);

            RectangleF hFill = new RectangleF(2, y, (Width - 6) * (healthPct / 100f), 7);
            using (var fp = GtaPalette.CreateRoundedRect(hFill, 3))
            using (var fb = new LinearGradientBrush(hTrack, healthCol, Color.White, LinearGradientMode.Horizontal))
                g.FillPath(fb, fp);

            // 3. Polymorphic Return Checklist Box
            y += 22;
            using (var secFont = new Font("Bahnschrift SemiBold", 8.75f, FontStyle.Bold))
            using (var secBrush = new SolidBrush(GtaPalette.ViceTeal))
                g.DrawString("POLYMORPHIC RETURN CHECKLIST", secFont, secBrush, 2, y);

            y += 22;
            RectangleF listBox = new RectangleF(0, y, Width - 2, Math.Max(60, Height - y - 4));
            using (var lp = GtaPalette.CreateRoundedRect(listBox, 8))
            {
                using (var lb = new SolidBrush(Color.FromArgb(16, 14, 28))) g.FillPath(lb, lp);
                using (var lpen = new Pen(GtaPalette.BorderSubtle, 1f)) g.DrawPath(lpen, lp);
            }

            int iy = y + 10;
            using (var itemFont = new Font("Segoe UI", 8.75f))
            using (var itemBrush = new SolidBrush(GtaPalette.TextPrimary))
            using (var bulletBrush = new SolidBrush(accent))
            {
                foreach (var item in _checklist)
                {
                    if (iy > Height - 22) break;
                    g.FillEllipse(bulletBrush, 12, iy + 5, 5, 5);
                    g.DrawString(item, itemFont, itemBrush, new RectangleF(24, iy, Width - 34, 20));
                    iy += 22;
                }
            }
        }

        private static void DrawHardwareGlyph(Graphics g, int x, int y, bool isAv, Color accent)
        {
            using (var pen = new Pen(Color.FromArgb(165, accent), 2f))
            using (var fill = new SolidBrush(Color.FromArgb(34, accent)))
            {
                if (isAv)
                {
                    // Stylized Projector / AV Deck Icon
                    Rectangle body = new Rectangle(x, y + 18, 66, 30);
                    g.FillRectangle(fill, body);
                    g.DrawRectangle(pen, body);
                    g.DrawEllipse(pen, x + 38, y + 23, 20, 20);
                    g.DrawEllipse(pen, x + 44, y + 29, 8, 8);
                    g.DrawLine(pen, x + 8, y + 26, x + 28, y + 26);
                    g.DrawLine(pen, x + 8, y + 33, x + 28, y + 33);
                    g.DrawLine(pen, x + 8, y + 40, x + 22, y + 40);
                }
                else
                {
                    // Stylized Lab Oscilloscope / Waveform Monitor
                    Rectangle screen = new Rectangle(x, y + 12, 66, 42);
                    g.FillRectangle(fill, screen);
                    g.DrawRectangle(pen, screen);
                    Point[] wave =
                    {
                        new Point(x + 6, y + 34),
                        new Point(x + 18, y + 34),
                        new Point(x + 25, y + 19),
                        new Point(x + 34, y + 46),
                        new Point(x + 43, y + 24),
                        new Point(x + 50, y + 34),
                        new Point(x + 60, y + 34)
                    };
                    g.DrawLines(pen, wave);
                }
            }
        }
    }

    // 11. LIVE GTA 6 BORROWER CLEARANCE / WANTED-STAR PROFILE PANEL
    public class GtaBorrowerProfilePanel : Panel
    {
        private bool _hasSelection = true;
        private string _id = "2024-10001";
        private string _fullName = "Emma Watson";
        private string _role = "Student";
        private string _dept = "Computer Science";
        private bool _hasHold = false;
        private int _activeSlips = 0;

        public GtaBorrowerProfilePanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            BackColor = GtaPalette.SurfaceCard;
        }

        public void SetBorrower(string id, string fullName, string role, string dept, bool hasHold, int activeSlips)
        {
            _hasSelection = !string.IsNullOrEmpty(id);
            _id = id ?? "";
            _fullName = fullName ?? "";
            _role = role ?? "";
            _dept = dept ?? "";
            _hasHold = hasHold;
            _activeSlips = activeSlips;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width < 40 || Height < 60) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            if (!_hasSelection)
            {
                using (var f = new Font("Bahnschrift SemiBold", 10f, FontStyle.Bold))
                using (var b = new SolidBrush(GtaPalette.TextSecondary))
                    g.DrawString("SELECT A BORROWER ROW\r\nTO VIEW CLEARANCE PROFILE", f, b, 16, 40);
                return;
            }

            Color accent = _hasHold ? GtaPalette.StatusWanted : GtaPalette.StatusClear;

            // Profile Banner
            RectangleF banner = new RectangleF(0, 0, Width - 1, 118);
            using (var bp = GtaPalette.CreateRoundedRect(banner, 9))
            {
                using (var bg = new LinearGradientBrush(banner,
                    _hasHold ? Color.FromArgb(68, 18, 34) : Color.FromArgb(18, 46, 52),
                    Color.FromArgb(24, 20, 42), 45f))
                {
                    g.FillPath(bg, bp);
                }

                // Avatar Monogram Circle
                string initials = GetInitials(_fullName);
                Rectangle avRect = new Rectangle(14, 18, 52, 52);
                using (var avFill = new SolidBrush(Color.FromArgb(42, accent))) g.FillEllipse(avFill, avRect);
                using (var avPen = new Pen(accent, 2f)) g.DrawEllipse(avPen, avRect);
                using (var avFont = new Font("Impact", 17f))
                    TextRenderer.DrawText(g, initials, avFont, avRect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                using (var nameFont = new Font("Bahnschrift SemiBold", 12.5f, FontStyle.Bold))
                using (var idFont = new Font("Consolas", 9.5f, FontStyle.Bold))
                using (var roleFont = new Font("Segoe UI", 8.75f))
                using (var nb = new SolidBrush(Color.White))
                using (var ib = new SolidBrush(accent))
                using (var rb = new SolidBrush(GtaPalette.TextSecondary))
                {
                    g.DrawString(_fullName, nameFont, nb, 78, 18);
                    g.DrawString(_id, idFont, ib, 78, 42);
                    g.DrawString(_role.ToUpperInvariant() + "  •  " + _dept.ToUpperInvariant(), roleFont, rb, 78, 62);
                }

                // GTA 5-Star Wanted / Clearance Level Bar inside bottom of banner
                int starY = 96;
                int starStartX = 24;
                for (int i = 0; i < 5; i++)
                {
                    bool lit = _hasHold && i < 3; // 3 Wanted Stars for Clearance Hold!
                    using (var starPath = CreateStar(starStartX + (i * 28), starY, 9f, 4f))
                    {
                        using (var sb = new SolidBrush(lit ? GtaPalette.StatusWanted : Color.FromArgb(52, 255, 255, 255)))
                            g.FillPath(sb, starPath);
                    }
                }

                string statusLabel = _hasHold ? "HOLD LOCKED" : "CLEAR STANDING";
                using (var sf = new Font("Bahnschrift SemiBold", 9f, FontStyle.Bold))
                using (var sb = new SolidBrush(accent))
                {
                    SizeF sz = g.MeasureString(statusLabel, sf);
                    g.DrawString(statusLabel, sf, sb, Width - sz.Width - 14, 88);
                }

                using (var border = new Pen(Color.FromArgb(120, accent), 1.2f))
                    g.DrawPath(border, bp);
            }

            // Telemetry Info Rows
            int y = 134;
            DrawStatRow(g, y, "BORROWING PRIVILEGES", _hasHold ? "SUSPENDED (HOLD)" : "AUTHORIZED", accent);
            y += 42;
            DrawStatRow(g, y, "ACTIVE CHECKED-OUT SLIPS", _activeSlips.ToString(), _activeSlips > 0 ? GtaPalette.SunsetPeach : GtaPalette.ViceTeal);
            y += 42;
            DrawStatRow(g, y, "DEPARTMENT", _dept.ToUpperInvariant(), GtaPalette.TextPrimary);
        }

        private void DrawStatRow(Graphics g, int y, string label, string value, Color valueColor)
        {
            RectangleF box = new RectangleF(0, y, Width - 2, 34);
            using (var path = GtaPalette.CreateRoundedRect(box, 6))
            {
                using (var bg = new SolidBrush(Color.FromArgb(18, 16, 30))) g.FillPath(bg, path);
                using (var pen = new Pen(GtaPalette.BorderSubtle, 1f)) g.DrawPath(pen, path);
            }

            using (var lf = new Font("Bahnschrift SemiBold", 8.25f, FontStyle.Bold))
            using (var vf = new Font("Consolas", 9f, FontStyle.Bold))
            using (var lb = new SolidBrush(GtaPalette.TextSecondary))
            using (var vb = new SolidBrush(valueColor))
            {
                g.DrawString(label, lf, lb, 10, y + 10);
                SizeF sz = g.MeasureString(value, vf);
                g.DrawString(value, vf, vb, Width - sz.Width - 12, y + 9);
            }
        }

        private static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "??";
            var parts = name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant();
            return (parts[0][0].ToString() + parts[parts.Length - 1][0]).ToUpperInvariant();
        }

        private static GraphicsPath CreateStar(float cx, float cy, float outerR, float innerR)
        {
            var path = new GraphicsPath();
            PointF[] pts = new PointF[10];
            double angle = -Math.PI / 2.0;
            double step = Math.PI / 5.0;
            for (int i = 0; i < 10; i++)
            {
                float r = (i % 2 == 0) ? outerR : innerR;
                pts[i] = new PointF(cx + (float)(Math.Cos(angle) * r), cy + (float)(Math.Sin(angle) * r));
                angle += step;
            }
            path.AddPolygon(pts);
            return path;
        }
    }

    // 12. GTA 6 QA DIAGNOSTIC METER PANEL
    public class GtaQaMeterPanel : Panel
    {
        private int _percent = 0;
        private string _statusText = "QA DIAGNOSTIC DECK  //  STANDBY (0 / 44)";
        private Color _accentColor = GtaPalette.ViceMagenta;

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Percent
        {
            get => _percent;
            set { _percent = Math.Max(0, Math.Min(100, value)); Invalidate(); }
        }

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value ?? ""; Invalidate(); }
        }

        [Category("GTA 6 UI")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        public GtaQaMeterPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            BackColor = GtaPalette.CanvasDark;
            Height = 36;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width < 30 || Height < 20) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (var f = new Font("Bahnschrift SemiBold", 9.5f, FontStyle.Bold))
            using (var b = new SolidBrush(_accentColor))
                g.DrawString(_statusText, f, b, 2, 2);

            RectangleF track = new RectangleF(2, Height - 10, Width - 6, 7);
            using (var tp = GtaPalette.CreateRoundedRect(track, 3))
            using (var tb = new SolidBrush(Color.FromArgb(34, 30, 56)))
                g.FillPath(tb, tp);

            if (_percent > 0)
            {
                RectangleF fill = new RectangleF(2, Height - 10, (Width - 6) * (_percent / 100f), 7);
                using (var fp = GtaPalette.CreateRoundedRect(fill, 3))
                using (var fb = new LinearGradientBrush(track, GtaPalette.ViceMagenta, _accentColor, LinearGradientMode.Horizontal))
                    g.FillPath(fb, fp);
            }
        }
    }

    // 13. GTA 6 "MISSION PASSED / HOLD" TOAST BANNER
    public class GtaToastBanner : Panel
    {
        private string _headline = "SYSTEM ONLINE";
        private string _detail = "Welcome to Vice Campus Equipment Command.";
        private Color _accentColor = GtaPalette.StatusClear;
        private readonly System.Windows.Forms.Timer _hideTimer;

        public GtaToastBanner()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            Size = new Size(420, 66);
            BackColor = GtaPalette.CanvasDark;
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
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            RectangleF rect = new RectangleF(1, 1, Width - 3, Height - 3);
            using (var path = GtaPalette.CreateRoundedRect(rect, 9))
            {
                using (var bg = new LinearGradientBrush(rect, Color.FromArgb(30, 24, 52), Color.FromArgb(16, 14, 28), LinearGradientMode.Horizontal))
                    g.FillPath(bg, path);

                g.SetClip(path);
                using (var leftBar = new SolidBrush(_accentColor))
                    g.FillRectangle(leftBar, 1, 1, 6, Height - 2);
                g.ResetClip();

                using (var border = new Pen(_accentColor, 1.5f))
                    g.DrawPath(border, path);
            }

            using (var headFont = new Font("Impact", 13.5f, FontStyle.Regular))
            using (var detFont = new Font("Segoe UI", 8.75f, FontStyle.Regular))
            using (var headBrush = new SolidBrush(_accentColor))
            using (var detBrush = new SolidBrush(GtaPalette.TextPrimary))
            {
                g.DrawString(_headline.ToUpperInvariant(), headFont, headBrush, 18, 9);
                g.DrawString(_detail, detFont, detBrush, 18, 37);
            }
        }
    }
}