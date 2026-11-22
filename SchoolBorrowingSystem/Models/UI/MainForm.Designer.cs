using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolBorrowingSystem.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private GtaHeaderBar headerPanel;
        private Label _lblClock;
        private GtaTileButton _btnMute;
        private Panel pnlNavBar;
        private GtaNavTabButton btnNavInventory;
        private GtaNavTabButton btnNavBorrow;
        private GtaNavTabButton btnNavReturn;
        private GtaNavTabButton btnNavBorrowers;
        private GtaNavTabButton btnNavQa;

        private Panel pnlHudStrip;
        private TableLayoutPanel tlpHud;
        private GtaHudStatTile hudAvailable;
        private GtaHudStatTile hudActiveSlips;
        private GtaHudStatTile hudInRepair;
        private GtaHudStatTile hudHolds;

        private Label _lblStatusBanner;
        private GtaToastBanner _toastBanner;
        private System.Windows.Forms.Timer _clockTimer;
        private GtaTabControl tabs;

        // Tab 1: Inventory
        private TabPage tabInventory;
        private Panel pnlInvTop;
        private TextBox _txtSearchEquipment;
        private CheckBox _chkAvailableOnly;
        private GtaTileButton btnAddEquipment;
        private GtaTileButton btnMarkRepair;
        private GtaTileButton btnCompleteRepair;
        private GtaTileButton btnSave;
        private GtaTileButton btnLoad;
        private GtaDataGridView _dgvEquipment;

        // Tab 2: Borrow
        private TabPage tabBorrow;
        private TableLayoutPanel tlpBorrow;
        private GtaCardPanel grpRequest;
        private Label lblBorrower;
        private ComboBox _cmbBorrowers;
        private Label lblEquip;
        private ComboBox _cmbAvailableEquipment;
        private Label lblRoom;
        private TextBox _txtRoom;
        private Label lblCheck;
        private ListBox _lstChecklistPreview;
        private GtaTileButton btnIssueSlip;
        private GtaCardPanel grpReceipt;
        private TextBox _txtBorrowReceipt;

        // Tab 3: Returns
        private TabPage tabReturn;
        private Label lblActive;
        private GtaDataGridView _dgvActiveSlips;
        private Panel actionPanel;
        private CheckBox _chkChecklistIntact;
        private Label lblCond;
        private ComboBox _cmbReturnCondition;
        private GtaTileButton btnProcessReturn;
        private Label lblHist;
        private GtaDataGridView _dgvHistoricalSlips;

        // Tab 4: Borrowers
        private TabPage tabBorrowers;
        private Panel pnlBorTop;
        private TextBox _txtSearchBorrowers;
        private GtaTileButton btnApplyHold;
        private GtaTileButton btnLiftHold;
        private GtaDataGridView _dgvBorrowers;

        // Tab 5: QA Tests
        private TabPage tabQa;
        private Panel pnlQaTop;
        private GtaTileButton btnRunQa;
        private Label _lblQaBadge;
        private TextBox _txtQaOutput;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.headerPanel = new GtaHeaderBar();
            this._lblClock = new Label();
            this._btnMute = new GtaTileButton();

            this.pnlNavBar = new Panel();
            this.btnNavInventory = new GtaNavTabButton();
            this.btnNavBorrow = new GtaNavTabButton();
            this.btnNavReturn = new GtaNavTabButton();
            this.btnNavBorrowers = new GtaNavTabButton();
            this.btnNavQa = new GtaNavTabButton();

            this.pnlHudStrip = new Panel();
            this.tlpHud = new TableLayoutPanel();
            this.hudAvailable = new GtaHudStatTile();
            this.hudActiveSlips = new GtaHudStatTile();
            this.hudInRepair = new GtaHudStatTile();
            this.hudHolds = new GtaHudStatTile();

            this._lblStatusBanner = new Label();
            this._toastBanner = new GtaToastBanner();
            this._clockTimer = new System.Windows.Forms.Timer(this.components);
            this.tabs = new GtaTabControl();

            this.tabInventory = new TabPage();
            this.pnlInvTop = new Panel();
            this._txtSearchEquipment = new TextBox();
            this._chkAvailableOnly = new CheckBox();
            this.btnAddEquipment = new GtaTileButton();
            this.btnMarkRepair = new GtaTileButton();
            this.btnCompleteRepair = new GtaTileButton();
            this.btnSave = new GtaTileButton();
            this.btnLoad = new GtaTileButton();
            this._dgvEquipment = new GtaDataGridView();

            this.tabBorrow = new TabPage();
            this.tlpBorrow = new TableLayoutPanel();
            this.grpRequest = new GtaCardPanel();
            this.lblBorrower = new Label();
            this._cmbBorrowers = new ComboBox();
            this.lblEquip = new Label();
            this._cmbAvailableEquipment = new ComboBox();
            this.lblRoom = new Label();
            this._txtRoom = new TextBox();
            this.lblCheck = new Label();
            this._lstChecklistPreview = new ListBox();
            this.btnIssueSlip = new GtaTileButton();
            this.grpReceipt = new GtaCardPanel();
            this._txtBorrowReceipt = new TextBox();

            this.tabReturn = new TabPage();
            this.lblActive = new Label();
            this._dgvActiveSlips = new GtaDataGridView();
            this.actionPanel = new Panel();
            this._chkChecklistIntact = new CheckBox();
            this.lblCond = new Label();
            this._cmbReturnCondition = new ComboBox();
            this.btnProcessReturn = new GtaTileButton();
            this.lblHist = new Label();
            this._dgvHistoricalSlips = new GtaDataGridView();

            this.tabBorrowers = new TabPage();
            this.pnlBorTop = new Panel();
            this._txtSearchBorrowers = new TextBox();
            this.btnApplyHold = new GtaTileButton();
            this.btnLiftHold = new GtaTileButton();
            this._dgvBorrowers = new GtaDataGridView();

            this.tabQa = new TabPage();
            this.pnlQaTop = new Panel();
            this.btnRunQa = new GtaTileButton();
            this._lblQaBadge = new Label();
            this._txtQaOutput = new TextBox();

            this.headerPanel.SuspendLayout();
            this.pnlNavBar.SuspendLayout();
            this.pnlHudStrip.SuspendLayout();
            this.tlpHud.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabInventory.SuspendLayout();
            this.pnlInvTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvEquipment)).BeginInit();
            this.tabBorrow.SuspendLayout();
            this.tlpBorrow.SuspendLayout();
            this.grpRequest.SuspendLayout();
            this.grpReceipt.SuspendLayout();
            this.tabReturn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvActiveSlips)).BeginInit();
            this.actionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvHistoricalSlips)).BeginInit();
            this.tabBorrowers.SuspendLayout();
            this.pnlBorTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvBorrowers)).BeginInit();
            this.tabQa.SuspendLayout();
            this.pnlQaTop.SuspendLayout();
            this.SuspendLayout();

            // ==================== HEADER BAR ====================
            this.headerPanel.Controls.Add(this._lblClock);
            this.headerPanel.Controls.Add(this._btnMute);
            this.headerPanel.Dock = DockStyle.Top;
            this.headerPanel.Location = new Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new Size(1180, 76);
            this.headerPanel.SubtitleText = "LEONIDA CAMPUS COMMAND  //  AV & LABORATORY INVENTORY";
            this.headerPanel.TitleText = "SCHOOL EQUIPMENT BORROWING SYSTEM";

            this._lblClock.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this._lblClock.BackColor = Color.Transparent;
            this._lblClock.Font = new Font("Bahnschrift SemiBold", 10F, FontStyle.Bold);
            this._lblClock.ForeColor = Color.FromArgb(255, 190, 146);
            this._lblClock.Location = new Point(845, 25);
            this._lblClock.Name = "_lblClock";
            this._lblClock.Size = new Size(175, 24);
            this._lblClock.Text = "VICE TIME  //  10:45 PM";
            this._lblClock.TextAlign = ContentAlignment.MiddleRight;

            this._btnMute.AccentColor = Color.FromArgb(62, 232, 168);
            this._btnMute.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this._btnMute.Location = new Point(1030, 19);
            this._btnMute.Name = "_btnMute";
            this._btnMute.Size = new Size(132, 36);
            this._btnMute.Text = "AUDIO: ON";
            this._btnMute.Click += new EventHandler(this.BtnMute_Click);

            // ==================== GTA 6 PAUSE-MENU NAV BAR ====================
            this.pnlNavBar.BackColor = Color.FromArgb(19, 18, 30);
            this.pnlNavBar.Controls.Add(this.btnNavQa);
            this.pnlNavBar.Controls.Add(this.btnNavBorrowers);
            this.pnlNavBar.Controls.Add(this.btnNavReturn);
            this.pnlNavBar.Controls.Add(this.btnNavBorrow);
            this.pnlNavBar.Controls.Add(this.btnNavInventory);
            this.pnlNavBar.Dock = DockStyle.Top;
            this.pnlNavBar.Height = 42;
            this.pnlNavBar.Name = "pnlNavBar";

            this.btnNavInventory.Dock = DockStyle.Left;
            this.btnNavInventory.IsActive = true;
            this.btnNavInventory.Name = "btnNavInventory";
            this.btnNavInventory.Size = new Size(215, 42);
            this.btnNavInventory.Text = "EQUIPMENT INVENTORY";
            this.btnNavInventory.Click += new EventHandler(this.BtnNav_Click);

            this.btnNavBorrow.Dock = DockStyle.Left;
            this.btnNavBorrow.IsActive = false;
            this.btnNavBorrow.Name = "btnNavBorrow";
            this.btnNavBorrow.Size = new Size(195, 42);
            this.btnNavBorrow.Text = "BORROW EQUIPMENT";
            this.btnNavBorrow.Click += new EventHandler(this.BtnNav_Click);

            this.btnNavReturn.Dock = DockStyle.Left;
            this.btnNavReturn.IsActive = false;
            this.btnNavReturn.Name = "btnNavReturn";
            this.btnNavReturn.Size = new Size(220, 42);
            this.btnNavReturn.Text = "RETURNS & ACTIVE SLIPS";
            this.btnNavReturn.Click += new EventHandler(this.BtnNav_Click);

            this.btnNavBorrowers.Dock = DockStyle.Left;
            this.btnNavBorrowers.IsActive = false;
            this.btnNavBorrowers.Name = "btnNavBorrowers";
            this.btnNavBorrowers.Size = new Size(205, 42);
            this.btnNavBorrowers.Text = "BORROWERS & HOLDS";
            this.btnNavBorrowers.Click += new EventHandler(this.BtnNav_Click);

            this.btnNavQa.Dock = DockStyle.Left;
            this.btnNavQa.IsActive = false;
            this.btnNavQa.Name = "btnNavQa";
            this.btnNavQa.Size = new Size(205, 42);
            this.btnNavQa.Text = "AUTOMATED QA TESTS";
            this.btnNavQa.Click += new EventHandler(this.BtnNav_Click);

            // ==================== LIVE HUD TELEMETRY STRIP ====================
            this.pnlHudStrip.BackColor = Color.FromArgb(16, 15, 26);
            this.pnlHudStrip.Controls.Add(this.tlpHud);
            this.pnlHudStrip.Dock = DockStyle.Top;
            this.pnlHudStrip.Height = 64;
            this.pnlHudStrip.Name = "pnlHudStrip";
            this.pnlHudStrip.Padding = new Padding(8, 6, 8, 2);

            this.tlpHud.ColumnCount = 4;
            this.tlpHud.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            this.tlpHud.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            this.tlpHud.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            this.tlpHud.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            this.tlpHud.Controls.Add(this.hudAvailable, 0, 0);
            this.tlpHud.Controls.Add(this.hudActiveSlips, 1, 0);
            this.tlpHud.Controls.Add(this.hudInRepair, 2, 0);
            this.tlpHud.Controls.Add(this.hudHolds, 3, 0);
            this.tlpHud.Dock = DockStyle.Fill;
            this.tlpHud.RowCount = 1;
            this.tlpHud.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            this.hudAvailable.AccentColor = Color.FromArgb(62, 232, 168);
            this.hudAvailable.CaptionText = "READY FOR DISPATCH";
            this.hudAvailable.Dock = DockStyle.Fill;
            this.hudAvailable.LabelText = "AVAILABLE GEAR";
            this.hudAvailable.ValueText = "4";

            this.hudActiveSlips.AccentColor = Color.FromArgb(95, 216, 204);
            this.hudActiveSlips.CaptionText = "CURRENTLY CHECKED OUT";
            this.hudActiveSlips.Dock = DockStyle.Fill;
            this.hudActiveSlips.LabelText = "ACTIVE SLIPS";
            this.hudActiveSlips.ValueText = "0";

            this.hudInRepair.AccentColor = Color.FromArgb(255, 190, 146);
            this.hudInRepair.CaptionText = "OFFLINE / MAINTENANCE";
            this.hudInRepair.Dock = DockStyle.Fill;
            this.hudInRepair.LabelText = "UNAVAILABLE / REPAIR";
            this.hudInRepair.ValueText = "0";

            this.hudHolds.AccentColor = Color.FromArgb(255, 68, 92);
            this.hudHolds.CaptionText = "WANTED CLEARANCE LOCK";
            this.hudHolds.Dock = DockStyle.Fill;
            this.hudHolds.LabelText = "CLEARANCE HOLDS";
            this.hudHolds.ValueText = "1";

            // ==================== STATUS BAR & TOAST BANNER ====================
            this._lblStatusBanner.BackColor = Color.FromArgb(12, 11, 20);
            this._lblStatusBanner.Dock = DockStyle.Bottom;
            this._lblStatusBanner.Font = new Font("Bahnschrift SemiBold", 9.25F, FontStyle.Bold);
            this._lblStatusBanner.ForeColor = Color.FromArgb(95, 216, 204);
            this._lblStatusBanner.Name = "_lblStatusBanner";
            this._lblStatusBanner.Padding = new Padding(14, 0, 14, 0);
            this._lblStatusBanner.Size = new Size(1180, 30);
            this._lblStatusBanner.Text = "READY  //  PRESS CTRL+1..5 TO SWITCH SECTIONS";
            this._lblStatusBanner.TextAlign = ContentAlignment.MiddleLeft;

            this._toastBanner.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this._toastBanner.Location = new Point(750, 86);
            this._toastBanner.Name = "_toastBanner";
            this._toastBanner.Visible = false;

            this._clockTimer.Interval = 1000;
            this._clockTimer.Tick += new EventHandler(this.ClockTimer_Tick);

            // ==================== TABS ====================
            this.tabs.Controls.Add(this.tabInventory);
            this.tabs.Controls.Add(this.tabBorrow);
            this.tabs.Controls.Add(this.tabReturn);
            this.tabs.Controls.Add(this.tabBorrowers);
            this.tabs.Controls.Add(this.tabQa);
            this.tabs.Dock = DockStyle.Fill;
            this.tabs.Margin = new Padding(0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.SelectedIndexChanged += new EventHandler(this.Tabs_SelectedIndexChanged);

            // ==================== TAB 1: EQUIPMENT INVENTORY ====================
            this.tabInventory.BackColor = Color.FromArgb(16, 15, 26);
            this.tabInventory.Controls.Add(this._dgvEquipment);
            this.tabInventory.Controls.Add(this.pnlInvTop);
            this.tabInventory.Name = "tabInventory";
            this.tabInventory.Padding = new Padding(12);
            this.tabInventory.Text = "Equipment Inventory";

            this.pnlInvTop.BackColor = Color.FromArgb(16, 15, 26);
            this.pnlInvTop.Controls.Add(this._txtSearchEquipment);
            this.pnlInvTop.Controls.Add(this._chkAvailableOnly);
            this.pnlInvTop.Controls.Add(this.btnAddEquipment);
            this.pnlInvTop.Controls.Add(this.btnMarkRepair);
            this.pnlInvTop.Controls.Add(this.btnCompleteRepair);
            this.pnlInvTop.Controls.Add(this.btnSave);
            this.pnlInvTop.Controls.Add(this.btnLoad);
            this.pnlInvTop.Dock = DockStyle.Top;
            this.pnlInvTop.Height = 48;

            this._txtSearchEquipment.BackColor = Color.FromArgb(36, 33, 56);
            this._txtSearchEquipment.BorderStyle = BorderStyle.FixedSingle;
            this._txtSearchEquipment.Font = new Font("Segoe UI", 9.5F);
            this._txtSearchEquipment.ForeColor = Color.FromArgb(245, 244, 252);
            this._txtSearchEquipment.Location = new Point(0, 10);
            this._txtSearchEquipment.Name = "_txtSearchEquipment";
            this._txtSearchEquipment.PlaceholderText = "Search tag, name, category...";
            this._txtSearchEquipment.Size = new Size(215, 24);
            this._txtSearchEquipment.TextChanged += new EventHandler(this.TxtSearchEquipment_TextChanged);

            this._chkAvailableOnly.AutoSize = true;
            this._chkAvailableOnly.ForeColor = Color.FromArgb(245, 244, 252);
            this._chkAvailableOnly.Location = new Point(226, 12);
            this._chkAvailableOnly.Text = "Available Only";
            this._chkAvailableOnly.CheckedChanged += new EventHandler(this.ChkAvailableOnly_CheckedChanged);

            this.btnAddEquipment.AccentColor = Color.FromArgb(95, 216, 204);
            this.btnAddEquipment.Location = new Point(352, 6);
            this.btnAddEquipment.Size = new Size(135, 34);
            this.btnAddEquipment.Text = "+ ADD GEAR";
            this.btnAddEquipment.Click += new EventHandler(this.BtnAddEquipment_Click);

            this.btnMarkRepair.AccentColor = Color.FromArgb(255, 68, 92);
            this.btnMarkRepair.Location = new Point(495, 6);
            this.btnMarkRepair.Size = new Size(175, 34);
            this.btnMarkRepair.Text = "MARK FOR REPAIR";
            this.btnMarkRepair.Click += new EventHandler(this.BtnMarkRepair_Click);

            this.btnCompleteRepair.AccentColor = Color.FromArgb(62, 232, 168);
            this.btnCompleteRepair.Location = new Point(678, 6);
            this.btnCompleteRepair.Size = new Size(180, 34);
            this.btnCompleteRepair.Text = "COMPLETE REPAIR";
            this.btnCompleteRepair.Click += new EventHandler(this.BtnCompleteRepair_Click);

            this.btnSave.AccentColor = Color.FromArgb(255, 114, 107);
            this.btnSave.Location = new Point(866, 6);
            this.btnSave.Size = new Size(135, 34);
            this.btnSave.Text = "SAVE FILE";
            this.btnSave.Click += new EventHandler(this.BtnSave_Click);

            this.btnLoad.AccentColor = Color.FromArgb(255, 190, 146);
            this.btnLoad.Location = new Point(1009, 6);
            this.btnLoad.Size = new Size(125, 34);
            this.btnLoad.Text = "LOAD FILE";
            this.btnLoad.Click += new EventHandler(this.BtnLoad_Click);

            this._dgvEquipment.AllowUserToAddRows = false;
            this._dgvEquipment.AllowUserToDeleteRows = false;
            this._dgvEquipment.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._dgvEquipment.Dock = DockStyle.Fill;
            this._dgvEquipment.MultiSelect = false;
            this._dgvEquipment.ReadOnly = true;
            this._dgvEquipment.Columns.Add("Tag", "PROPERTY TAG");
            this._dgvEquipment.Columns.Add("Type", "CATEGORY");
            this._dgvEquipment.Columns.Add("Name", "EQUIPMENT NAME");
            this._dgvEquipment.Columns.Add("Condition", "CONDITION");
            this._dgvEquipment.Columns.Add("Status", "AVAILABILITY");
            this._dgvEquipment.Columns.Add("Details", "SPECIFICATIONS");

            // ==================== TAB 2: BORROW EQUIPMENT ====================
            this.tabBorrow.BackColor = Color.FromArgb(16, 15, 26);
            this.tabBorrow.Controls.Add(this.tlpBorrow);
            this.tabBorrow.Name = "tabBorrow";
            this.tabBorrow.Padding = new Padding(12);
            this.tabBorrow.Text = "Borrow Equipment";

            this.tlpBorrow.ColumnCount = 2;
            this.tlpBorrow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
            this.tlpBorrow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
            this.tlpBorrow.Controls.Add(this.grpRequest, 0, 0);
            this.tlpBorrow.Controls.Add(this.grpReceipt, 1, 0);
            this.tlpBorrow.Dock = DockStyle.Fill;
            this.tlpBorrow.RowCount = 1;
            this.tlpBorrow.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            this.grpRequest.AccentColor = Color.FromArgb(228, 36, 153);
            this.grpRequest.CardTitle = "NEW BORROW DISPATCH REQUEST";
            this.grpRequest.Controls.Add(this.lblBorrower);
            this.grpRequest.Controls.Add(this._cmbBorrowers);
            this.grpRequest.Controls.Add(this.lblEquip);
            this.grpRequest.Controls.Add(this._cmbAvailableEquipment);
            this.grpRequest.Controls.Add(this.lblRoom);
            this.grpRequest.Controls.Add(this._txtRoom);
            this.grpRequest.Controls.Add(this.lblCheck);
            this.grpRequest.Controls.Add(this._lstChecklistPreview);
            this.grpRequest.Controls.Add(this.btnIssueSlip);
            this.grpRequest.Dock = DockStyle.Fill;
            this.grpRequest.Margin = new Padding(0, 0, 10, 0);

            this.lblBorrower.AutoSize = true;
            this.lblBorrower.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblBorrower.Location = new Point(16, 48);
            this.lblBorrower.Text = "SELECT REGISTERED BORROWER";

            this._cmbBorrowers.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this._cmbBorrowers.BackColor = Color.FromArgb(36, 33, 56);
            this._cmbBorrowers.DropDownStyle = ComboBoxStyle.DropDownList;
            this._cmbBorrowers.FlatStyle = FlatStyle.Flat;
            this._cmbBorrowers.ForeColor = Color.FromArgb(245, 244, 252);
            this._cmbBorrowers.Location = new Point(16, 70);
            this._cmbBorrowers.Size = new Size(500, 24);

            this.lblEquip.AutoSize = true;
            this.lblEquip.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblEquip.Location = new Point(16, 108);
            this.lblEquip.Text = "SELECT AVAILABLE EQUIPMENT";

            this._cmbAvailableEquipment.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this._cmbAvailableEquipment.BackColor = Color.FromArgb(36, 33, 56);
            this._cmbAvailableEquipment.DropDownStyle = ComboBoxStyle.DropDownList;
            this._cmbAvailableEquipment.FlatStyle = FlatStyle.Flat;
            this._cmbAvailableEquipment.ForeColor = Color.FromArgb(245, 244, 252);
            this._cmbAvailableEquipment.Location = new Point(16, 130);
            this._cmbAvailableEquipment.Size = new Size(500, 24);
            this._cmbAvailableEquipment.SelectedIndexChanged += new EventHandler(this.CmbAvailableEquipment_SelectedIndexChanged);

            this.lblRoom.AutoSize = true;
            this.lblRoom.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblRoom.Location = new Point(16, 168);
            this.lblRoom.Text = "DESTINATION ROOM / LABORATORY";

            this._txtRoom.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this._txtRoom.BackColor = Color.FromArgb(36, 33, 56);
            this._txtRoom.BorderStyle = BorderStyle.FixedSingle;
            this._txtRoom.ForeColor = Color.FromArgb(245, 244, 252);
            this._txtRoom.Location = new Point(16, 190);
            this._txtRoom.Size = new Size(500, 24);
            this._txtRoom.Text = "Room 304 - Lecture Hall";

            this.lblCheck.AutoSize = true;
            this.lblCheck.ForeColor = Color.FromArgb(95, 216, 204);
            this.lblCheck.Location = new Point(16, 228);
            this.lblCheck.Text = "POLYMORPHIC RETURN CHECKLIST";

            this._lstChecklistPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this._lstChecklistPreview.BackColor = Color.FromArgb(18, 16, 30);
            this._lstChecklistPreview.BorderStyle = BorderStyle.FixedSingle;
            this._lstChecklistPreview.Font = new Font("Segoe UI", 9.25F);
            this._lstChecklistPreview.ForeColor = Color.FromArgb(245, 244, 252);
            this._lstChecklistPreview.Location = new Point(16, 250);
            this._lstChecklistPreview.Size = new Size(500, 150);

            this.btnIssueSlip.AccentColor = Color.FromArgb(62, 232, 168);
            this.btnIssueSlip.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.btnIssueSlip.Location = new Point(16, 412);
            this.btnIssueSlip.Size = new Size(500, 42);
            this.btnIssueSlip.Text = "ISSUE OFFICIAL BORROW SLIP";
            this.btnIssueSlip.Click += new EventHandler(this.BtnIssueSlip_Click);

            this.grpReceipt.AccentColor = Color.FromArgb(95, 216, 204);
            this.grpReceipt.CardTitle = "DISPATCH RECEIPT TERMINAL";
            this.grpReceipt.Controls.Add(this._txtBorrowReceipt);
            this.grpReceipt.Dock = DockStyle.Fill;
            this.grpReceipt.Margin = new Padding(10, 0, 0, 0);

            this._txtBorrowReceipt.BackColor = Color.FromArgb(12, 11, 20);
            this._txtBorrowReceipt.BorderStyle = BorderStyle.FixedSingle;
            this._txtBorrowReceipt.Dock = DockStyle.Fill;
            this._txtBorrowReceipt.Font = new Font("Consolas", 10F);
            this._txtBorrowReceipt.ForeColor = Color.FromArgb(95, 216, 204);
            this._txtBorrowReceipt.Multiline = true;
            this._txtBorrowReceipt.ReadOnly = true;
            this._txtBorrowReceipt.ScrollBars = ScrollBars.Vertical;
            this._txtBorrowReceipt.Text = ">> DISPATCH TERMINAL ONLINE.\r\n>> Select a borrower and available equipment, then click 'ISSUE OFFICIAL BORROW SLIP'.";

            // ==================== TAB 3: RETURNS & ACTIVE SLIPS ====================
            this.tabReturn.BackColor = Color.FromArgb(16, 15, 26);
            this.tabReturn.Controls.Add(this._dgvHistoricalSlips);
            this.tabReturn.Controls.Add(this.lblHist);
            this.tabReturn.Controls.Add(this.actionPanel);
            this.tabReturn.Controls.Add(this._dgvActiveSlips);
            this.tabReturn.Controls.Add(this.lblActive);
            this.tabReturn.Name = "tabReturn";
            this.tabReturn.Padding = new Padding(12);
            this.tabReturn.Text = "Returns and Active Slips";

            this.lblActive.Dock = DockStyle.Top;
            this.lblActive.Font = new Font("Bahnschrift SemiBold", 10F, FontStyle.Bold);
            this.lblActive.ForeColor = Color.FromArgb(95, 216, 204);
            this.lblActive.Height = 28;
            this.lblActive.Text = "ACTIVE BORROW SLIPS  //  SELECT A SLIP TO PROCESS RETURN";

            this._dgvActiveSlips.AllowUserToAddRows = false;
            this._dgvActiveSlips.AllowUserToDeleteRows = false;
            this._dgvActiveSlips.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._dgvActiveSlips.Dock = DockStyle.Top;
            this._dgvActiveSlips.Height = 185;
            this._dgvActiveSlips.MultiSelect = false;
            this._dgvActiveSlips.ReadOnly = true;
            this._dgvActiveSlips.Columns.Add("SlipID", "SLIP ID");
            this._dgvActiveSlips.Columns.Add("Borrower", "BORROWER");
            this._dgvActiveSlips.Columns.Add("Tag", "EQUIPMENT TAG");
            this._dgvActiveSlips.Columns.Add("Item", "EQUIPMENT NAME");
            this._dgvActiveSlips.Columns.Add("Room", "ROOM");
            this._dgvActiveSlips.Columns.Add("Due", "DUE TIME");
            this._dgvActiveSlips.Columns.Add("Status", "STATUS");

            this.actionPanel.BackColor = Color.FromArgb(25, 23, 39);
            this.actionPanel.Controls.Add(this._chkChecklistIntact);
            this.actionPanel.Controls.Add(this.lblCond);
            this.actionPanel.Controls.Add(this._cmbReturnCondition);
            this.actionPanel.Controls.Add(this.btnProcessReturn);
            this.actionPanel.Dock = DockStyle.Top;
            this.actionPanel.Height = 56;

            this._chkChecklistIntact.AutoSize = true;
            this._chkChecklistIntact.Checked = true;
            this._chkChecklistIntact.ForeColor = Color.FromArgb(245, 244, 252);
            this._chkChecklistIntact.Location = new Point(16, 18);
            this._chkChecklistIntact.Text = "All checklist components present and intact";

            this.lblCond.AutoSize = true;
            this.lblCond.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblCond.Location = new Point(365, 19);
            this.lblCond.Text = "RETURN CONDITION:";

            this._cmbReturnCondition.BackColor = Color.FromArgb(36, 33, 56);
            this._cmbReturnCondition.DropDownStyle = ComboBoxStyle.DropDownList;
            this._cmbReturnCondition.FlatStyle = FlatStyle.Flat;
            this._cmbReturnCondition.ForeColor = Color.FromArgb(245, 244, 252);
            this._cmbReturnCondition.Items.AddRange(new object[] { "Good", "Fair", "Damaged", "Needs Repair" });
            this._cmbReturnCondition.Location = new Point(512, 15);
            this._cmbReturnCondition.SelectedIndex = 0;
            this._cmbReturnCondition.Width = 165;

            this.btnProcessReturn.AccentColor = Color.FromArgb(255, 190, 146);
            this.btnProcessReturn.Location = new Point(698, 11);
            this.btnProcessReturn.Size = new Size(245, 34);
            this.btnProcessReturn.Text = "PROCESS EQUIPMENT RETURN";
            this.btnProcessReturn.Click += new EventHandler(this.BtnProcessReturn_Click);

            this.lblHist.Dock = DockStyle.Top;
            this.lblHist.Font = new Font("Bahnschrift SemiBold", 10F, FontStyle.Bold);
            this.lblHist.ForeColor = Color.FromArgb(255, 190, 146);
            this.lblHist.Height = 32;
            this.lblHist.Padding = new Padding(0, 8, 0, 0);
            this.lblHist.Text = "COMPLETED / HISTORICAL RETURN ARCHIVE";

            this._dgvHistoricalSlips.AllowUserToAddRows = false;
            this._dgvHistoricalSlips.AllowUserToDeleteRows = false;
            this._dgvHistoricalSlips.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._dgvHistoricalSlips.Dock = DockStyle.Fill;
            this._dgvHistoricalSlips.MultiSelect = false;
            this._dgvHistoricalSlips.ReadOnly = true;
            this._dgvHistoricalSlips.Columns.Add("SlipID", "SLIP ID");
            this._dgvHistoricalSlips.Columns.Add("Borrower", "BORROWER");
            this._dgvHistoricalSlips.Columns.Add("Item", "EQUIPMENT");
            this._dgvHistoricalSlips.Columns.Add("Returned", "TIME RETURNED");
            this._dgvHistoricalSlips.Columns.Add("Summary", "TRANSACTION RECORD");

            // ==================== TAB 4: BORROWERS & HOLDS ====================
            this.tabBorrowers.BackColor = Color.FromArgb(16, 15, 26);
            this.tabBorrowers.Controls.Add(this._dgvBorrowers);
            this.tabBorrowers.Controls.Add(this.pnlBorTop);
            this.tabBorrowers.Name = "tabBorrowers";
            this.tabBorrowers.Padding = new Padding(12);
            this.tabBorrowers.Text = "Borrowers and Holds";

            this.pnlBorTop.BackColor = Color.FromArgb(16, 15, 26);
            this.pnlBorTop.Controls.Add(this._txtSearchBorrowers);
            this.pnlBorTop.Controls.Add(this.btnApplyHold);
            this.pnlBorTop.Controls.Add(this.btnLiftHold);
            this.pnlBorTop.Dock = DockStyle.Top;
            this.pnlBorTop.Height = 48;

            this._txtSearchBorrowers.BackColor = Color.FromArgb(36, 33, 56);
            this._txtSearchBorrowers.BorderStyle = BorderStyle.FixedSingle;
            this._txtSearchBorrowers.Font = new Font("Segoe UI", 9.5F);
            this._txtSearchBorrowers.ForeColor = Color.FromArgb(245, 244, 252);
            this._txtSearchBorrowers.Location = new Point(0, 10);
            this._txtSearchBorrowers.Name = "_txtSearchBorrowers";
            this._txtSearchBorrowers.PlaceholderText = "Search ID, name, or department...";
            this._txtSearchBorrowers.Size = new Size(260, 24);
            this._txtSearchBorrowers.TextChanged += new EventHandler(this.TxtSearchBorrowers_TextChanged);

            this.btnApplyHold.AccentColor = Color.FromArgb(255, 68, 92);
            this.btnApplyHold.Location = new Point(276, 6);
            this.btnApplyHold.Size = new Size(215, 34);
            this.btnApplyHold.Text = "★ APPLY CLEARANCE HOLD";
            this.btnApplyHold.Click += new EventHandler(this.BtnApplyHold_Click);

            this.btnLiftHold.AccentColor = Color.FromArgb(62, 232, 168);
            this.btnLiftHold.Location = new Point(502, 6);
            this.btnLiftHold.Size = new Size(205, 34);
            this.btnLiftHold.Text = "LIFT CLEARANCE HOLD";
            this.btnLiftHold.Click += new EventHandler(this.BtnLiftHold_Click);

            this._dgvBorrowers.AllowUserToAddRows = false;
            this._dgvBorrowers.AllowUserToDeleteRows = false;
            this._dgvBorrowers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._dgvBorrowers.Dock = DockStyle.Fill;
            this._dgvBorrowers.MultiSelect = false;
            this._dgvBorrowers.ReadOnly = true;
            this._dgvBorrowers.Columns.Add("ID", "SCHOOL ID");
            this._dgvBorrowers.Columns.Add("Name", "FULL NAME");
            this._dgvBorrowers.Columns.Add("Role", "ROLE");
            this._dgvBorrowers.Columns.Add("Dept", "DEPARTMENT");
            this._dgvBorrowers.Columns.Add("Hold", "CLEARANCE STATUS");

            // ==================== TAB 5: AUTOMATED QA TESTS ====================
            this.tabQa.BackColor = Color.FromArgb(16, 15, 26);
            this.tabQa.Controls.Add(this._txtQaOutput);
            this.tabQa.Controls.Add(this.pnlQaTop);
            this.tabQa.Name = "tabQa";
            this.tabQa.Padding = new Padding(12);
            this.tabQa.Text = "Automated QA Tests";

            this.pnlQaTop.BackColor = Color.FromArgb(16, 15, 26);
            this.pnlQaTop.Controls.Add(this.btnRunQa);
            this.pnlQaTop.Controls.Add(this._lblQaBadge);
            this.pnlQaTop.Dock = DockStyle.Top;
            this.pnlQaTop.Height = 48;

            this.btnRunQa.AccentColor = Color.FromArgb(228, 36, 153);
            this.btnRunQa.Location = new Point(0, 6);
            this.btnRunQa.Size = new Size(290, 34);
            this.btnRunQa.Text = "RUN ALL 44 AUTOMATED QA TESTS";
            this.btnRunQa.Click += new EventHandler(this.BtnRunQa_Click);

            this._lblQaBadge.AutoSize = true;
            this._lblQaBadge.Font = new Font("Bahnschrift SemiBold", 10F, FontStyle.Bold);
            this._lblQaBadge.ForeColor = Color.FromArgb(168, 164, 194);
            this._lblQaBadge.Location = new Point(308, 14);
            this._lblQaBadge.Text = "QA DIAGNOSTIC DECK  //  STANDBY";

            this._txtQaOutput.BackColor = Color.FromArgb(10, 9, 18);
            this._txtQaOutput.BorderStyle = BorderStyle.FixedSingle;
            this._txtQaOutput.Dock = DockStyle.Fill;
            this._txtQaOutput.Font = new Font("Consolas", 9.75F);
            this._txtQaOutput.ForeColor = Color.FromArgb(62, 232, 168);
            this._txtQaOutput.Multiline = true;
            this._txtQaOutput.ReadOnly = true;
            this._txtQaOutput.ScrollBars = ScrollBars.Both;
            this._txtQaOutput.Text = ">> QA ENGINE READY.\r\n>> Click 'RUN ALL 44 AUTOMATED QA TESTS' to verify all OOP assertions.";

            // ==================== MAINFORM ====================
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.FromArgb(16, 15, 26);
            this.ClientSize = new Size(1180, 740);
            this.Controls.Add(this._toastBanner);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.pnlHudStrip);
            this.Controls.Add(this.pnlNavBar);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this._lblStatusBanner);
            this.Font = new Font("Segoe UI", 9.5F);
            this.KeyPreview = true;
            this.MinimumSize = new Size(1120, 680);
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "School Equipment Borrowing Management System — Vice Campus Edition";
            this.Shown += new EventHandler(this.MainForm_Shown);
            this.KeyDown += new KeyEventHandler(this.MainForm_KeyDown);

            this.headerPanel.ResumeLayout(false);
            this.pnlNavBar.ResumeLayout(false);
            this.pnlHudStrip.ResumeLayout(false);
            this.tlpHud.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.tabInventory.ResumeLayout(false);
            this.pnlInvTop.ResumeLayout(false);
            this.pnlInvTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvEquipment)).EndInit();
            this.tabBorrow.ResumeLayout(false);
            this.tlpBorrow.ResumeLayout(false);
            this.grpRequest.ResumeLayout(false);
            this.grpRequest.PerformLayout();
            this.grpReceipt.ResumeLayout(false);
            this.grpReceipt.PerformLayout();
            this.tabReturn.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgvActiveSlips)).EndInit();
            this.actionPanel.ResumeLayout(false);
            this.actionPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvHistoricalSlips)).EndInit();
            this.tabBorrowers.ResumeLayout(false);
            this.pnlBorTop.ResumeLayout(false);
            this.pnlBorTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvBorrowers)).EndInit();
            this.tabQa.ResumeLayout(false);
            this.pnlQaTop.ResumeLayout(false);
            this.pnlQaTop.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}