using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.UI.Pages;

namespace SchoolBorrowingSystem.UI
{
    /// <summary>
    /// The main window: a dark sidebar on the left, the current page on the right,
    /// and a status bar at the bottom.
    ///
    /// Each page is its own class (see the Pages folder). MainForm only switches between them.
    ///
    /// Responsive behaviour:
    ///   - When the window gets narrow, the sidebar collapses to icons only.
    ///   - The menu button at the top of the sidebar collapses/expands it by hand.
    ///   - Each page rearranges itself when it gets narrow (see BasePage.ApplyLayout).
    /// </summary>
    public class MainForm : Form
    {
        // Below this window width the sidebar collapses on its own
        private const int CollapseSidebarBelowWidth = 1100;

        private const int SidebarWideWidth = 230;
        private const int SidebarCollapsedWidth = 68;

        // The order here is the order of the buttons in the sidebar
        private static readonly string[] PageNames = { "Dashboard", "Inventory", "Borrow", "Return", "Borrowers", "History", "Tests" };
        private static readonly string[] PageIcons = { Theme.IconDashboard, Theme.IconInventory, Theme.IconBorrow, Theme.IconReturn, Theme.IconBorrowers, Theme.IconHistory, Theme.IconTests };

        private readonly Dictionary<string, BasePage> _pages = new Dictionary<string, BasePage>();
        private readonly List<NavButton> _navButtons = new List<NavButton>();

        private Panel _sidebar;
        private Label _appName;
        private Label _appTagline;
        private Label _shortcutHint;
        private Button _menuButton;
        private Button _soundButton;
        private ToolTip _toolTip;

        private Panel _pageHost;
        private Label _statusLabel;
        private Panel _statusBar;
        private Timer _statusTimer;
        private string _currentPage;

        private bool _sidebarCollapsed;
        private bool _wasNarrowWindow;

        // Needed so the Visual Studio designer can open the class
        public MainForm()
            : this(new EquipmentController(), new BorrowingController(), new BorrowerController())
        {
        }

        public MainForm(EquipmentController equipment, BorrowingController borrowing, BorrowerController borrowers)
        {
            Text = "School Equipment Borrowing System";
            Font = Theme.NormalFont;
            BackColor = Theme.PageBackground;
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true; // so Ctrl+1..7 works no matter which control has focus

            // Start at a comfortable size, but never bigger than the screen
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            Size = new Size(Math.Min(Theme.Scale(1280), screen.Width), Math.Min(Theme.Scale(800), screen.Height));
            MinimumSize = Theme.Scale(760, 540);

            // Hide status messages again after a few seconds
            _statusTimer = new Timer();
            _statusTimer.Interval = 7000;
            _statusTimer.Tick += StatusTimer_Tick;

            _toolTip = new ToolTip();

            // Create one of each page
            AddPage(new DashboardPage(equipment, borrowing, borrowers));
            AddPage(new InventoryPage(equipment, borrowing, borrowers));
            AddPage(new BorrowPage(equipment, borrowing, borrowers));
            AddPage(new ReturnPage(equipment, borrowing, borrowers));
            AddPage(new BorrowersPage(equipment, borrowing, borrowers));
            AddPage(new HistoryPage(equipment, borrowing, borrowers));
            AddPage(new TestsPage(equipment, borrowing, borrowers));

            BuildLayout();
            ShowPage("Dashboard");

            Shown += delegate { AudioHelper.PlayStartup(); };
        }

        private void AddPage(BasePage page)
        {
            // The class name minus "Page" is the page name, e.g. "InventoryPage" -> "Inventory"
            string name = page.GetType().Name.Replace("Page", "");
            _pages[name] = page;

            page.StatusChanged += ShowStatus;
            page.NavigateRequested += ShowPage;
        }

        private void BuildLayout()
        {
            // ----- Sidebar -----
            _sidebar = new Panel();
            _sidebar.Dock = DockStyle.Left;
            _sidebar.Width = Theme.Scale(SidebarWideWidth);
            _sidebar.BackColor = Theme.Sidebar;
            _sidebar.Padding = Theme.ScalePadding(12, 12, 12, 16);

            _menuButton = MakeSidebarButton(Theme.HasIconFont ? Theme.IconMenu : "=");
            _menuButton.Font = Theme.HasIconFont ? Theme.IconFont : Theme.SectionFont;
            _menuButton.Dock = DockStyle.Top;
            _menuButton.TextAlign = ContentAlignment.MiddleLeft;
            _menuButton.Padding = Theme.ScalePadding(10, 0, 0, 0);
            _menuButton.Click += MenuButton_Click;
            _toolTip.SetToolTip(_menuButton, "Show / hide menu labels");

            _appName = Theme.MakeLabel("Equipment Office", new Font(Theme.FontName, 14F, FontStyle.Bold), Color.White);
            _appName.Dock = DockStyle.Top;
            _appName.Padding = Theme.ScalePadding(8, 8, 0, 0);

            _appTagline = Theme.MakeLabel("School Borrowing System", Theme.SmallFont, Theme.SidebarText);
            _appTagline.Dock = DockStyle.Top;
            _appTagline.Padding = Theme.ScalePadding(10, 2, 0, 20);

            FlowLayoutPanel nav = new FlowLayoutPanel();
            nav.Dock = DockStyle.Fill;
            nav.FlowDirection = FlowDirection.TopDown;
            nav.WrapContents = false;
            nav.AutoScroll = true; // if the window is very short, the menu can scroll
            nav.Padding = Theme.ScalePadding(0, 8, 0, 0);
            nav.Resize += delegate { ResizeNavButtons(nav); };

            for (int i = 0; i < PageNames.Length; i++)
            {
                NavButton button = new NavButton(PageNames[i], PageIcons[i]);
                button.Click += NavButton_Click;
                _toolTip.SetToolTip(button, PageNames[i] + "  (Ctrl+" + (i + 1) + ")");
                _navButtons.Add(button);
                nav.Controls.Add(button);
            }

            _soundButton = MakeSidebarButton("");
            _toolTip.SetToolTip(_soundButton, "Turn sound effects on / off");
            _soundButton.Dock = DockStyle.Bottom;
            _soundButton.FlatAppearance.BorderSize = 1;
            _soundButton.FlatAppearance.BorderColor = Theme.SidebarHover;
            _soundButton.Click += SoundButton_Click;

            _shortcutHint = Theme.MakeLabel("Tip: Ctrl+1 to Ctrl+7 switch pages", Theme.SmallFont, Theme.TextMuted);
            _shortcutHint.Dock = DockStyle.Bottom;
            _shortcutHint.Padding = Theme.ScalePadding(4, 0, 0, 10);

            // Fill first, then the docked items (see the docking note in BasePage)
            _sidebar.Controls.Add(nav);
            _sidebar.Controls.Add(_shortcutHint);
            _sidebar.Controls.Add(_soundButton);
            _sidebar.Controls.Add(_appTagline);
            _sidebar.Controls.Add(_appName);
            _sidebar.Controls.Add(_menuButton);

            // ----- Status bar -----
            _statusBar = new Panel();
            _statusBar.Dock = DockStyle.Bottom;
            _statusBar.Height = Theme.Scale(36);
            _statusBar.BackColor = Theme.CardBackground;
            _statusBar.Padding = Theme.ScalePadding(16, 0, 16, 0);

            _statusLabel = Theme.MakeLabel("Ready.", Theme.NormalFont, Theme.TextMuted);
            _statusLabel.AutoSize = false;
            _statusLabel.Dock = DockStyle.Fill;
            _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            _statusLabel.AutoEllipsis = true; // long messages end with "..." instead of being cut
            _statusBar.Controls.Add(_statusLabel);

            // ----- Area where the current page is shown -----
            _pageHost = new Panel();
            _pageHost.Dock = DockStyle.Fill;

            Controls.Add(_pageHost);
            Controls.Add(_statusBar);
            Controls.Add(_sidebar);

            // On a small screen the window starts narrow, so start with the sidebar collapsed
            _wasNarrowWindow = ClientSize.Width < Theme.Scale(CollapseSidebarBelowWidth);
            SetSidebarCollapsed(_wasNarrowWindow);
        }

        private Button MakeSidebarButton(string text)
        {
            Button button = new Button();
            button.Text = text;
            button.Height = Theme.Scale(38);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Theme.SidebarHover;
            button.BackColor = Theme.Sidebar;
            button.ForeColor = Theme.SidebarText;
            button.Font = Theme.SmallFont;
            button.Cursor = Cursors.Hand;
            return button;
        }

        // Nav buttons are as wide as the sidebar (minus room for a scrollbar if one shows up)
        private void ResizeNavButtons(FlowLayoutPanel nav)
        {
            int width = nav.ClientSize.Width - nav.Padding.Horizontal;
            foreach (NavButton button in _navButtons)
            {
                button.Width = Math.Max(width, Theme.Scale(40));
            }
        }

        // ---------------------------------------------------------------
        // Responsive sidebar
        // ---------------------------------------------------------------

        /// <summary>
        /// Collapsed = only icons (narrow). Expanded = icons + names.
        /// </summary>
        private void SetSidebarCollapsed(bool collapsed)
        {
            _sidebarCollapsed = collapsed;

            SuspendLayout(); // do all the changes at once so it doesn't flicker
            _sidebar.Width = Theme.Scale(collapsed ? SidebarCollapsedWidth : SidebarWideWidth);
            _sidebar.Padding = collapsed ? Theme.ScalePadding(8, 12, 8, 16) : Theme.ScalePadding(12, 12, 12, 16);
            _appName.Visible = !collapsed;
            _appTagline.Visible = !collapsed;
            _shortcutHint.Visible = !collapsed;
            _menuButton.TextAlign = collapsed ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
            _menuButton.Padding = collapsed ? new Padding(0) : Theme.ScalePadding(10, 0, 0, 0);

            foreach (NavButton button in _navButtons)
            {
                button.IsCollapsed = collapsed;
            }

            // Tooltips are only useful when the names are hidden
            _toolTip.Active = collapsed;

            UpdateSoundButtonText();
            ResumeLayout(true);
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            SetSidebarCollapsed(!_sidebarCollapsed);
        }

        /// <summary>
        /// Runs every time the window size changes.
        /// We only collapse/expand when the window CROSSES the breakpoint,
        /// so a sidebar the user toggled by hand isn't changed back on every tiny resize.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_sidebar == null || WindowState == FormWindowState.Minimized)
            {
                return;
            }

            bool isNarrow = ClientSize.Width < Theme.Scale(CollapseSidebarBelowWidth);
            if (isNarrow != _wasNarrowWindow)
            {
                _wasNarrowWindow = isNarrow;
                SetSidebarCollapsed(isNarrow);
            }
        }

        // ---------------------------------------------------------------
        // Pages
        // ---------------------------------------------------------------

        private void NavButton_Click(object sender, EventArgs e)
        {
            NavButton button = (NavButton)sender;
            ShowPage(button.PageName);
        }

        /// <summary>
        /// Swaps the page on the right side and highlights its sidebar button.
        /// </summary>
        public void ShowPage(string pageName)
        {
            if (!_pages.ContainsKey(pageName))
            {
                return;
            }

            BasePage page = _pages[pageName];
            _pageHost.Controls.Clear();
            _pageHost.Controls.Add(page);
            _currentPage = pageName;

            page.RefreshData(); // always show up-to-date information

            foreach (NavButton button in _navButtons)
            {
                button.IsSelected = button.PageName == pageName;
            }
        }

        // ---------------------------------------------------------------
        // Status bar and sound
        // ---------------------------------------------------------------

        /// <summary>
        /// Shows a message at the bottom. Warnings are red, normal messages are green.
        /// </summary>
        public void ShowStatus(string message, bool isWarning)
        {
            _statusLabel.Text = message;
            if (isWarning)
            {
                _statusLabel.ForeColor = Theme.Danger;
                _statusBar.BackColor = Theme.Lighten(Theme.Danger, 0.9);
            }
            else
            {
                _statusLabel.ForeColor = Theme.Success;
                _statusBar.BackColor = Theme.Lighten(Theme.Success, 0.9);
            }

            // Restart the timer so the message stays for the full time
            _statusTimer.Stop();
            _statusTimer.Start();
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            _statusTimer.Stop();
            _statusLabel.Text = "Ready.";
            _statusLabel.ForeColor = Theme.TextMuted;
            _statusBar.BackColor = Theme.CardBackground;
        }

        private void SoundButton_Click(object sender, EventArgs e)
        {
            AudioHelper.IsMuted = !AudioHelper.IsMuted;
            UpdateSoundButtonText();
            AudioHelper.PlaySuccess(); // does nothing when muted
        }

        private void UpdateSoundButtonText()
        {
            if (_sidebarCollapsed && Theme.HasIconFont)
            {
                // Just the speaker icon when there's no room for words
                _soundButton.Font = Theme.IconFont;
                _soundButton.Text = AudioHelper.IsMuted ? Theme.IconSoundOff : Theme.IconSoundOn;
            }
            else if (_sidebarCollapsed)
            {
                // No icon font on this PC: use a short word instead
                _soundButton.Font = Theme.SmallFont;
                _soundButton.Text = AudioHelper.IsMuted ? "Off" : "On";
            }
            else
            {
                _soundButton.Font = Theme.SmallFont;
                _soundButton.Text = AudioHelper.IsMuted ? "Sound: Off" : "Sound: On";
            }
        }

        /// <summary>
        /// Keyboard shortcuts: Ctrl+1 .. Ctrl+7 open the pages, F5 refreshes the current page.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5 && _currentPage != null)
            {
                ShowPage(_currentPage);
                return true;
            }

            for (int i = 0; i < PageNames.Length; i++)
            {
                if (keyData == (Keys.Control | (Keys.D1 + i)))
                {
                    ShowPage(PageNames[i]);
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
