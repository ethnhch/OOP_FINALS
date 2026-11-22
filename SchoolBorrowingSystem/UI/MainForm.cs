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
    /// </summary>
    public class MainForm : Form
    {
        // The order here is the order of the buttons in the sidebar
        private static readonly string[] PageNames = { "Dashboard", "Inventory", "Borrow", "Return", "Borrowers", "History", "Tests" };

        private readonly Dictionary<string, BasePage> _pages = new Dictionary<string, BasePage>();
        private readonly Dictionary<string, Button> _navButtons = new Dictionary<string, Button>();

        private Panel _pageHost;
        private Label _statusLabel;
        private Panel _statusBar;
        private Button _soundButton;
        private Timer _statusTimer;
        private string _currentPage;

        private static readonly Font NavFont = new Font(Theme.FontName, 10.5F);
        private static readonly Font NavFontSelected = new Font(Theme.FontName, 10.5F, FontStyle.Bold);

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
            Size = new Size(1280, 800);
            MinimumSize = new Size(1100, 700);
            KeyPreview = true; // so Ctrl+1..7 works no matter which control has focus

            // Hide status messages again after a few seconds
            _statusTimer = new Timer();
            _statusTimer.Interval = 7000;
            _statusTimer.Tick += StatusTimer_Tick;

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
            Panel sidebar = new Panel();
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 230;
            sidebar.BackColor = Theme.Sidebar;
            sidebar.Padding = new Padding(14, 22, 14, 16);

            Label appName = Theme.MakeLabel("Equipment Office", new Font(Theme.FontName, 14F, FontStyle.Bold), Color.White);
            appName.Dock = DockStyle.Top;
            appName.Padding = new Padding(8, 0, 0, 0);

            Label appTagline = Theme.MakeLabel("School Borrowing System", Theme.SmallFont, Theme.SidebarText);
            appTagline.Dock = DockStyle.Top;
            appTagline.Padding = new Padding(10, 2, 0, 26);

            FlowLayoutPanel nav = new FlowLayoutPanel();
            nav.Dock = DockStyle.Fill;
            nav.FlowDirection = FlowDirection.TopDown;
            nav.WrapContents = false;

            for (int i = 0; i < PageNames.Length; i++)
            {
                Button button = MakeNavButton(PageNames[i], i + 1);
                _navButtons[PageNames[i]] = button;
                nav.Controls.Add(button);
            }

            _soundButton = new Button();
            _soundButton.Dock = DockStyle.Bottom;
            _soundButton.Height = 36;
            _soundButton.FlatStyle = FlatStyle.Flat;
            _soundButton.FlatAppearance.BorderColor = Theme.SidebarHover;
            _soundButton.FlatAppearance.MouseOverBackColor = Theme.SidebarHover;
            _soundButton.ForeColor = Theme.SidebarText;
            _soundButton.Font = Theme.SmallFont;
            _soundButton.Cursor = Cursors.Hand;
            _soundButton.Click += SoundButton_Click;
            UpdateSoundButtonText();

            Label shortcutHint = Theme.MakeLabel("Tip: Ctrl+1 to Ctrl+7 switch pages", Theme.SmallFont, Theme.TextMuted);
            shortcutHint.Dock = DockStyle.Bottom;
            shortcutHint.Padding = new Padding(4, 0, 0, 10);

            sidebar.Controls.Add(nav);
            sidebar.Controls.Add(shortcutHint);
            sidebar.Controls.Add(_soundButton);
            sidebar.Controls.Add(appTagline);
            sidebar.Controls.Add(appName);

            // ----- Status bar -----
            _statusBar = new Panel();
            _statusBar.Dock = DockStyle.Bottom;
            _statusBar.Height = 36;
            _statusBar.BackColor = Theme.CardBackground;
            _statusBar.Padding = new Padding(16, 0, 16, 0);

            _statusLabel = Theme.MakeLabel("Ready.", Theme.NormalFont, Theme.TextMuted);
            _statusLabel.AutoSize = false;
            _statusLabel.Dock = DockStyle.Fill;
            _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            _statusBar.Controls.Add(_statusLabel);

            // ----- Area where the current page is shown -----
            _pageHost = new Panel();
            _pageHost.Dock = DockStyle.Fill;

            Controls.Add(_pageHost);
            Controls.Add(_statusBar);
            Controls.Add(sidebar);
        }

        private Button MakeNavButton(string pageName, int shortcutNumber)
        {
            Button button = new Button();
            button.Text = "   " + pageName;
            button.Tag = pageName;
            button.Width = 200;
            button.Height = 42;
            button.Margin = new Padding(0, 0, 0, 4);
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Theme.SidebarHover;
            button.BackColor = Theme.Sidebar;
            button.ForeColor = Theme.SidebarText;
            button.Font = NavFont;
            button.Cursor = Cursors.Hand;
            button.Click += NavButton_Click;
            return button;
        }

        private void NavButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            ShowPage((string)button.Tag);
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

            // Highlight the selected button, un-highlight the others
            foreach (string name in _navButtons.Keys)
            {
                Button button = _navButtons[name];
                bool isSelected = name == pageName;

                button.BackColor = isSelected ? Theme.SidebarSelected : Theme.Sidebar;
                button.ForeColor = isSelected ? Color.White : Theme.SidebarText;
                button.FlatAppearance.MouseOverBackColor = isSelected ? Theme.SidebarSelected : Theme.SidebarHover;
                button.Font = isSelected ? NavFontSelected : NavFont;
            }
        }

        /// <summary>
        /// Shows a message at the bottom. Warnings are orange/red, normal messages are green.
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
            _soundButton.Text = AudioHelper.IsMuted ? "Sound: Off" : "Sound: On";
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
