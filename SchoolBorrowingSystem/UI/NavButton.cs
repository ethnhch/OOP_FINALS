using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolBorrowingSystem.UI
{
    /// <summary>
    /// One button in the sidebar. It draws an icon on the left and the page name next to it.
    /// When the sidebar is collapsed (narrow window) it only draws the icon.
    ///
    /// A normal Button can only use one font, but the icon needs the icon font and the
    /// text needs Segoe UI, so we draw both ourselves in OnPaint.
    /// </summary>
    public class NavButton : Control
    {
        private bool _isSelected;
        private bool _isCollapsed;
        private bool _isHovered;

        // Created once and shared by all nav buttons (fonts use Windows resources)
        private static readonly Font NormalNavFont = new Font(Theme.FontName, 10.5F);
        private static readonly Font SelectedNavFont = new Font(Theme.FontName, 10.5F, FontStyle.Bold);

        public string PageName { get; private set; }
        public string IconText { get; private set; }

        /// <summary>Shown instead of the icon if the icon font is not installed.</summary>
        public string FallbackText { get; private set; }

        public NavButton(string pageName, string iconText)
        {
            PageName = pageName;
            IconText = iconText;
            FallbackText = pageName.Substring(0, 1);
            Text = pageName;

            Height = Theme.Scale(42);
            Margin = Theme.ScalePadding(0, 0, 0, 4);
            Cursor = Cursors.Hand;
            Font = NormalNavFont;

            // Tell WinForms we paint everything ourselves (no flicker)
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                Font = value ? SelectedNavFont : NormalNavFont;
                Invalidate(); // ask for a repaint
            }
        }

        public bool IsCollapsed
        {
            get { return _isCollapsed; }
            set
            {
                _isCollapsed = value;
                Invalidate();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 1. Background
            Color background = Theme.Sidebar;
            if (_isSelected)
            {
                background = Theme.SidebarSelected;
            }
            else if (_isHovered)
            {
                background = Theme.SidebarHover;
            }
            using (SolidBrush brush = new SolidBrush(background))
            {
                g.FillRectangle(brush, ClientRectangle);
            }

            Color textColor = _isSelected ? Color.White : Theme.SidebarText;
            int iconAreaWidth = Theme.Scale(40);

            // 2. Icon (centered in its own square on the left)
            Rectangle iconArea = new Rectangle(0, 0, _isCollapsed ? Width : iconAreaWidth + Theme.Scale(8), Height);
            string icon = Theme.HasIconFont ? IconText : FallbackText;
            Font iconFont = Theme.HasIconFont ? Theme.IconFont : Theme.BoldFont;
            TextRenderer.DrawText(g, icon, iconFont, iconArea, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            // 3. Page name (only when there is room for it)
            if (!_isCollapsed)
            {
                Rectangle textArea = new Rectangle(iconArea.Right, 0, Width - iconArea.Right, Height);
                TextRenderer.DrawText(g, Text, Font, textArea, textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }
    }
}
