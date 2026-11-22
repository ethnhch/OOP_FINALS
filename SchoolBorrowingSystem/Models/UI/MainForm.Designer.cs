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

        // Tab 1: Equipment Inventory + Live Asset Dossier
        private TabPage tabInventory;
        private Panel pnlInvTop;
        private GtaSearchBox _txtSearchEquipment;
        private GtaToggleSwitch _chkAvailableOnly;
        private GtaTileButton btnAddEquipment;
        private GtaTileButton btnSave;
        private GtaTileButton btnLoad;
        private TableLayoutPanel tlpInvBody;
        private GtaCardPanel cardInvGrid;
        private GtaDataGridView _dgvEquipment;
        private GtaCardPanel cardInvDossier;
        private GtaAssetDossierPanel _assetDossier;
        private Panel pnlDossierActions;
        private TableLayoutPanel tlpDossierBtns;
        private GtaTileButton btnQuickDispatch;
        private GtaTileButton btnMarkRepair;
        private GtaTileButton btnCompleteRepair;

        // Tab 2: Borrow Equipment
        private TabPage tabBorrow;
        private TableLayoutPanel tlpBorrow;
        private GtaCardPanel grpRequest;
        private TableLayoutPanel tlpRequestForm;
        private Label lblBorrower;
        private ComboBox _cmbBorrowers;
        private Label _lblBorrowClearance;
        private Label lblEquip;
        private ComboBox _cmbAvailableEquipment;
        private Label lblRoom;
        private TextBox _txtRoom;
        private TableLayoutPanel tlpRoomPresets;
        private GtaTileButton btnRoomPreset1;
        private GtaTileButton btnRoomPreset2;
        private GtaTileButton btnRoomPreset3;
        private Label lblCheck;
        private ListBox _lstChecklistPreview;
        private GtaTileButton btnIssueSlip;
        private GtaCardPanel grpReceipt;
        private TableLayoutPanel tlpReceipt;
        private TextBox _txtBorrowReceipt;
        private GtaTileButton btnCopyReceipt;

        // Tab 3: Returns & Active Slips
        private TabPage tabReturn;
        private GtaCardPanel cardActiveSlips;
        private GtaDataGridView _dgvActiveSlips;
        private Panel actionPanel;
        private GtaToggleSwitch _chkChecklistIntact;
        private Label lblCond;
        private ComboBox _cmbReturnCondition;
        private GtaTileButton btnProcessReturn;
        private GtaCardPanel cardHistSlips;
        private GtaDataGridView _dgvHistoricalSlips;

        // Tab 4: Borrowers & Holds + Wanted Profile
        private TabPage tabBorrowers;
        private Panel pnlBorTop;
        private GtaSearchBox _txtSearchBorrowers;
        private GtaTileButton btnApplyHold;
        private GtaTileButton btnLiftHold;
        private TableLayoutPanel tlpBorBody;
        private GtaCardPanel cardBorGrid;
        private GtaDataGridView _dgvBorrowers;
        private GtaCardPanel cardBorProfile;
        private GtaBorrowerProfilePanel _borrowerProfile;

        // Tab 5: Automated QA Tests
        private TabPage tabQa;
        private Panel pnlQaTop;
        private TableLayoutPanel tlpQaTop;
        private GtaTileButton btnRunQa;
        private GtaQaMeterPanel _qaMeter;
        private GtaCardPanel cardQaConsole;
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

            // Tab 1
            this.tabInventory = new TabPage();
            this.pnlInvTop = new Panel();
            this._txtSearchEquipment = new GtaSearchBox();
            this._chkAvailableOnly = new GtaToggleSwitch();
            this.btnAddEquipment = new GtaTileButton();
            this.btnSave = new GtaTileButton();
            this.btnLoad = new GtaTileButton();
            this.tlpInvBody = new TableLayoutPanel();
            this.cardInvGrid = new GtaCardPanel();
            this._dgvEquipment = new GtaDataGridView();
            this.cardInvDossier = new GtaCardPanel();
            this._assetDossier = new GtaAssetDossierPanel();
            this.pnlDossierActions = new Panel();
            this.tlpDossierBtns = new TableLayoutPanel();
            this.btnQuickDispatch = new GtaTileButton();
            this.btnMarkRepair = new GtaTileButton();
            this.btnCompleteRepair = new GtaTileButton();

            // Tab 2
            this.tabBorrow = new TabPage();
            this.tlpBorrow = new TableLayoutPanel();
            this.grpRequest = new GtaCardPanel();
            this.tlpRequestForm = new TableLayoutPanel();
            this.lblBorrower = new Label();
            this._cmbBorrowers = new ComboBox();
            this._lblBorrowClearance = new Label();
            this.lblEquip = new Label();
            this._cmbAvailableEquipment = new ComboBox();
            this.lblRoom = new Label();
            this._txtRoom = new TextBox();
            this.tlpRoomPresets = new TableLayoutPanel();
            this.btnRoomPreset1 = new GtaTileButton();
            this.btnRoomPreset2 = new GtaTileButton();
            this.btnRoomPreset3 = new GtaTileButton();
            this.lblCheck = new Label();
            this._lstChecklistPreview = new ListBox();
            this.btnIssueSlip = new GtaTileButton();
            this.grpReceipt = new GtaCardPanel();
            this.tlpReceipt = new TableLayoutPanel();
            this._txtBorrowReceipt = new TextBox();
            this.btnCopyReceipt = new GtaTileButton();

            // Tab 3
            this.tabReturn = new TabPage();
            this.cardActiveSlips = new GtaCardPanel();
            this._dgvActiveSlips = new GtaDataGridView();
            this.actionPanel = new Panel();
            this._chkChecklistIntact = new GtaToggleSwitch();
            this.lblCond = new Label();
            this._cmbReturnCondition = new ComboBox();
            this.btnProcessReturn = new GtaTileButton();
            this.cardHistSlips = new GtaCardPanel();
            this._dgvHistoricalSlips = new GtaDataGridView();

            // Tab 4
            this.tabBorrowers = new TabPage();
            this.pnlBorTop = new Panel();
            this._txtSearchBorrowers = new GtaSearchBox();
            this.btnApplyHold = new GtaTileButton();
            this.btnLiftHold = new GtaTileButton();
            this.tlpBorBody = new TableLayoutPanel();
            this.cardBorGrid = new GtaCardPanel();
            this._dgvBorrowers = new GtaDataGridView();
            this.cardBorProfile = new GtaCardPanel();
            this._borrowerProfile = new GtaBorrowerProfilePanel();

            // Tab 5
            this.tabQa = new TabPage();
            this.pnlQaTop = new Panel();
            this.tlpQaTop = new TableLayoutPanel();
            this.btnRunQa = new GtaTileButton();
            this._qaMeter = new GtaQaMeterPanel();
            this.cardQaConsole = new GtaCardPanel();
            this._txtQaOutput = new TextBox();

            this.headerPanel.SuspendLayout();
            this.pnlNavBar.SuspendLayout();
            this.pnlHudStrip.SuspendLayout();
            this.tlpHud.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabInventory.SuspendLayout();
            this.pnlInvTop.SuspendLayout();
            this.tlpInvBody.SuspendLayout();
            this.cardInvGrid.SuspendLayout();
            this.cardInvDossier.SuspendLayout();
            this.pnlDossierActions.SuspendLayout();
            this.tlpDossierBtns.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvEquipment)).BeginInit();
            this.tabBorrow.SuspendLayout();
            this.tlpBorrow.SuspendLayout();
            this.grpRequest.SuspendLayout();
            this.tlpRequestForm.SuspendLayout();
            this.tlpRoomPresets.SuspendLayout();
            this.grpReceipt.SuspendLayout();
            this.tlpReceipt.SuspendLayout();
            this.tabReturn.SuspendLayout();
            this.cardActiveSlips.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvActiveSlips)).BeginInit();
            this.actionPanel.SuspendLayout();
            this.cardHistSlips.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvHistoricalSlips)).BeginInit();
            this.tabBorrowers.SuspendLayout();
            this.pnlBorTop.SuspendLayout();
            this.tlpBorBody.SuspendLayout();
            this.cardBorGrid.SuspendLayout();
            this.cardBorProfile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvBorrowers)).BeginInit();
            this.tabQa.SuspendLayout();
            this.pnlQaTop.SuspendLayout();
            this.tlpQaTop.SuspendLayout();
            this.cardQaConsole.SuspendLayout();
            this.SuspendLayout();

            // ==================== HEADER BAR ====================
            this.headerPanel.Controls.Add(this._lblClock);
            this.headerPanel.Controls.Add(this._btnMute);
            this.headerPanel.Dock = DockStyle.Top;
            this.headerPanel.Location = new Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new Size(1240, 82);
            this.headerPanel.SubtitleText = "LEONIDA CAMPUS COMMAND  //  AV & LABORATORY INVENTORY";
            this.headerPanel.TitleText = "SCHOOL EQUIPMENT BORROWING SYSTEM";

            this._lblClock.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this._lblClock.BackColor = Color.Transparent;
            this._lblClock.Font = new Font("Bahnschrift SemiBold", 10F, FontStyle.Bold);
            this._lblClock.ForeColor = Color.FromArgb(255, 196, 134);
            this._lblClock.Location = new Point(880, 29);
            this._lblClock.Name = "_lblClock";
            this._lblClock.Size = new Size(195, 24);
            this._lblClock.Text = "VICE TIME  //  11:30 PM";
            this._lblClock.TextAlign = ContentAlignment.MiddleRight;

            this._btnMute.AccentColor = Color.FromArgb(56, 238, 170);
            this._btnMute.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this._btnMute.Location = new Point(1088, 22);
            this._btnMute.Name = "_btnMute";
            this._btnMute.Size = new Size(134, 38);
            this._btnMute.Text = "AUDIO: ON";
            this._btnMute.Click += new EventHandler(this.BtnMute_Click);

            // ==================== GTA 6 PAUSE-MENU NAV BAR ====================
            this.pnlNavBar.BackColor = Color.FromArgb(18, 16, 30);
            this.pnlNavBar.Controls.Add(this.btnNavQa);
            this.pnlNavBar.Controls.Add(this.btnNavBorrowers);
            this.pnlNavBar.Controls.Add(this.btnNavReturn);
            this.pnlNavBar.Controls.Add(this.btnNavBorrow);
            this.pnlNavBar.Controls.Add(this.btnNavInventory);
            this.pnlNavBar.Dock = DockStyle.Top;
            this.pnlNavBar.Height = 46;
            this.pnlNavBar.Name = "pnlNavBar";
            this.pnlNavBar.Padding = new Padding(10, 0, 10, 0);

            this.btnNavInventory.Dock = DockStyle.Left;
            this.btnNavInventory.IsActive = true;
            this.btnNavInventory.Size = new Size(218, 46);
            this.btnNavInventory.Text = "EQUIPMENT INVENTORY";
            this.btnNavInventory.Click += new EventHandler(this.BtnNav_Click);

            this.btnNavBorrow.Dock = DockStyle.Left;
            this.btnNavBorrow.IsActive = false;
            this.btnNavBorrow.Size = new Size(200, 46);
            this.btnNavBorrow.Text = "BORROW EQUIPMENT";
            this.btnNavBorrow.Click += new EventHandler(this.BtnNav_Click);

            this.btnNavReturn.Dock = DockStyle.Left;
            this.btnNavReturn.IsActive = false;
            this.btnNavReturn.Size = new Size(224, 46);
            this.btnNavReturn.Text = "RETURNS & ACTIVE SLIPS";
            this.btnNavReturn.Click += new EventHandler(this.BtnNav_Click);

            this.btnNavBorrowers.Dock = DockStyle.Left;
            this.btnNavBorrowers.IsActive = false;
            this.btnNavBorrowers.Size = new Size(206, 46);
            this.btnNavBorrowers.Text = "BORROWERS & HOLDS";
            this.btnNavBorrowers.Click += new EventHandler(this.BtnNav_Click);

            this.btnNavQa.Dock = DockStyle.Left;
            this.btnNavQa.IsActive = false;
            this.btnNavQa.Size = new Size(206, 46);
            this.btnNavQa.Text = "AUTOMATED QA TESTS";
            this.btnNavQa.Click += new EventHandler(this.BtnNav_Click);

            // ==================== LIVE HUD TELEMETRY STRIP ====================
            this.pnlHudStrip.BackColor = Color.FromArgb(14, 13, 24);
            this.pnlHudStrip.Controls.Add(this.tlpHud);
            this.pnlHudStrip.Dock = DockStyle.Top;
            this.pnlHudStrip.Height = 76;
            this.pnlHudStrip.Name = "pnlHudStrip";
            this.pnlHudStrip.Padding = new Padding(10, 8, 10, 2);

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

            this.hudAvailable.AccentColor = Color.FromArgb(56, 238, 170);
            this.hudAvailable.CaptionText = "READY FOR DISPATCH";
            this.hudAvailable.Dock = DockStyle.Fill;
            this.hudAvailable.FillPercent = 100;
            this.hudAvailable.LabelText = "AVAILABLE GEAR";
            this.hudAvailable.ValueText = "4";

            this.hudActiveSlips.AccentColor = Color.FromArgb(72, 232, 214);
            this.hudActiveSlips.CaptionText = "CURRENTLY CHECKED OUT";
            this.hudActiveSlips.Dock = DockStyle.Fill;
            this.hudActiveSlips.FillPercent = 12;
            this.hudActiveSlips.LabelText = "ACTIVE SLIPS";
            this.hudActiveSlips.ValueText = "0";

            this.hudInRepair.AccentColor = Color.FromArgb(255, 196, 134);
            this.hudInRepair.CaptionText = "OFFLINE / MAINTENANCE";
            this.hudInRepair.Dock = DockStyle.Fill;
            this.hudInRepair.FillPercent = 10;
            this.hudInRepair.LabelText = "UNAVAILABLE / REPAIR";
            this.hudInRepair.ValueText = "0";

            this.hudHolds.AccentColor = Color.FromArgb(255, 68, 96);
            this.hudHolds.CaptionText = "WANTED CLEARANCE LOCK";
            this.hudHolds.Dock = DockStyle.Fill;
            this.hudHolds.FillPercent = 25;
            this.hudHolds.LabelText = "CLEARANCE HOLDS";
            this.hudHolds.ValueText = "1";

            // ==================== STATUS BAR & TOAST BANNER ====================
            this._lblStatusBanner.BackColor = Color.FromArgb(11, 10, 19);
            this._lblStatusBanner.Dock = DockStyle.Bottom;
            this._lblStatusBanner.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this._lblStatusBanner.ForeColor = Color.FromArgb(72, 232, 214);
            this._lblStatusBanner.Name = "_lblStatusBanner";
            this._lblStatusBanner.Padding = new Padding(16, 0, 16, 0);
            this._lblStatusBanner.Size = new Size(1240, 30);
            this._lblStatusBanner.Text = "READY  //  CTRL+1..5 SWITCHES SECTIONS  •  SELECT ANY ROW TO INSPECT DOSSIER";
            this._lblStatusBanner.TextAlign = ContentAlignment.MiddleLeft;

            this._toastBanner.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this._toastBanner.Location = new Point(802, 92);
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
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.SelectedIndexChanged += new EventHandler(this.Tabs_SelectedIndexChanged);

            // ==================== TAB 1: EQUIPMENT INVENTORY + ASSET DOSSIER ====================
            this.tabInventory.BackColor = Color.FromArgb(14, 13, 24);
            this.tabInventory.Controls.Add(this.tlpInvBody);
            this.tabInventory.Controls.Add(this.pnlInvTop);
            this.tabInventory.Name = "tabInventory";
            this.tabInventory.Padding = new Padding(14, 8, 14, 12);
            this.tabInventory.Text = "Equipment Inventory";

            this.pnlInvTop.BackColor = Color.FromArgb(14, 13, 24);
            this.pnlInvTop.Controls.Add(this._txtSearchEquipment);
            this.pnlInvTop.Controls.Add(this._chkAvailableOnly);
            this.pnlInvTop.Controls.Add(this.btnAddEquipment);
            this.pnlInvTop.Controls.Add(this.btnSave);
            this.pnlInvTop.Controls.Add(this.btnLoad);
            this.pnlInvTop.Dock = DockStyle.Top;
            this.pnlInvTop.Height = 50;

            this._txtSearchEquipment.AccentColor = Color.FromArgb(72, 232, 214);
            this._txtSearchEquipment.Location = new Point(0, 8);
            this._txtSearchEquipment.Placeholder = "Search tag, name, or category...";
            this._txtSearchEquipment.Size = new Size(275, 34);
            this._txtSearchEquipment.SearchTextChanged += new EventHandler(this.TxtSearchEquipment_TextChanged);

            this._chkAvailableOnly.Location = new Point(290, 11);
            this._chkAvailableOnly.OnColor = Color.FromArgb(56, 238, 170);
            this._chkAvailableOnly.Size = new Size(165, 28);
            this._chkAvailableOnly.Text = "AVAILABLE ONLY";
            this._chkAvailableOnly.CheckedChanged += new EventHandler(this.ChkAvailableOnly_CheckedChanged);

            this.btnAddEquipment.AccentColor = Color.FromArgb(72, 232, 214);
            this.btnAddEquipment.Location = new Point(470, 7);
            this.btnAddEquipment.Size = new Size(150, 36);
            this.btnAddEquipment.Text = "+ ADD GEAR";
            this.btnAddEquipment.Click += new EventHandler(this.BtnAddEquipment_Click);

            this.btnSave.AccentColor = Color.FromArgb(255, 114, 107);
            this.btnSave.Location = new Point(630, 7);
            this.btnSave.Size = new Size(140, 36);
            this.btnSave.Text = "SAVE FILE";
            this.btnSave.Click += new EventHandler(this.BtnSave_Click);

            this.btnLoad.AccentColor = Color.FromArgb(255, 196, 134);
            this.btnLoad.Location = new Point(780, 7);
            this.btnLoad.Size = new Size(135, 36);
            this.btnLoad.Text = "LOAD FILE";
            this.btnLoad.Click += new EventHandler(this.BtnLoad_Click);

            this.tlpInvBody.ColumnCount = 2;
            this.tlpInvBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67F));
            this.tlpInvBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            this.tlpInvBody.Controls.Add(this.cardInvGrid, 0, 0);
            this.tlpInvBody.Controls.Add(this.cardInvDossier, 1, 0);
            this.tlpInvBody.Dock = DockStyle.Fill;
            this.tlpInvBody.RowCount = 1;
            this.tlpInvBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            this.cardInvGrid.AccentColor = Color.FromArgb(72, 232, 214);
            this.cardInvGrid.CardTitle = "EQUIPMENT REGISTRY  //  LIVE CATALOG";
            this.cardInvGrid.Controls.Add(this._dgvEquipment);
            this.cardInvGrid.Dock = DockStyle.Fill;
            this.cardInvGrid.Margin = new Padding(0, 4, 8, 0);

            this._dgvEquipment.AllowUserToAddRows = false;
            this._dgvEquipment.AllowUserToDeleteRows = false;
            this._dgvEquipment.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._dgvEquipment.Dock = DockStyle.Fill;
            this._dgvEquipment.MultiSelect = false;
            this._dgvEquipment.ReadOnly = true;
            this._dgvEquipment.Columns.Add("Tag", "TAG");
            this._dgvEquipment.Columns.Add("Type", "CATEGORY");
            this._dgvEquipment.Columns.Add("Name", "EQUIPMENT NAME");
            this._dgvEquipment.Columns.Add("Condition", "CONDITION");
            this._dgvEquipment.Columns.Add("Status", "STATUS");
            this._dgvEquipment.Columns.Add("Details", "SPECIFICATIONS");
            this._dgvEquipment.SelectionChanged += new EventHandler(this.DgvEquipment_SelectionChanged);

            this.cardInvDossier.AccentColor = Color.FromArgb(236, 46, 158);
            this.cardInvDossier.CardTitle = "SELECTED ASSET DOSSIER";
            this.cardInvDossier.Controls.Add(this._assetDossier);
            this.cardInvDossier.Controls.Add(this.pnlDossierActions);
            this.cardInvDossier.Dock = DockStyle.Fill;
            this.cardInvDossier.Margin = new Padding(8, 4, 0, 0);

            this._assetDossier.Dock = DockStyle.Fill;

            // Responsive 2x2 Action Grid inside Dossier
            this.pnlDossierActions.BackColor = Color.FromArgb(22, 20, 38);
            this.pnlDossierActions.Controls.Add(this.tlpDossierBtns);
            this.pnlDossierActions.Dock = DockStyle.Bottom;
            this.pnlDossierActions.Height = 92;
            this.pnlDossierActions.Padding = new Padding(0, 8, 0, 0);

            this.tlpDossierBtns.BackColor = Color.FromArgb(22, 20, 38);
            this.tlpDossierBtns.ColumnCount = 2;
            this.tlpDossierBtns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tlpDossierBtns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tlpDossierBtns.RowCount = 2;
            this.tlpDossierBtns.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            this.tlpDossierBtns.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            this.tlpDossierBtns.Controls.Add(this.btnQuickDispatch, 0, 0);
            this.tlpDossierBtns.SetColumnSpan(this.btnQuickDispatch, 2);
            this.tlpDossierBtns.Controls.Add(this.btnMarkRepair, 0, 1);
            this.tlpDossierBtns.Controls.Add(this.btnCompleteRepair, 1, 1);
            this.tlpDossierBtns.Dock = DockStyle.Fill;

            this.btnQuickDispatch.AccentColor = Color.FromArgb(56, 238, 170);
            this.btnQuickDispatch.Dock = DockStyle.Fill;
            this.btnQuickDispatch.Margin = new Padding(0, 0, 0, 4);
            this.btnQuickDispatch.Text = "DISPATCH SELECTED UNIT →";
            this.btnQuickDispatch.Click += new EventHandler(this.BtnQuickDispatch_Click);

            this.btnMarkRepair.AccentColor = Color.FromArgb(255, 68, 96);
            this.btnMarkRepair.Dock = DockStyle.Fill;
            this.btnMarkRepair.Margin = new Padding(0, 4, 4, 0);
            this.btnMarkRepair.Text = "FLAG REPAIR";
            this.btnMarkRepair.Click += new EventHandler(this.BtnMarkRepair_Click);

            this.btnCompleteRepair.AccentColor = Color.FromArgb(72, 232, 214);
            this.btnCompleteRepair.Dock = DockStyle.Fill;
            this.btnCompleteRepair.Margin = new Padding(4, 4, 0, 0);
            this.btnCompleteRepair.Text = "RESTORE UNIT";
            this.btnCompleteRepair.Click += new EventHandler(this.BtnCompleteRepair_Click);

            // ==================== TAB 2: BORROW EQUIPMENT (FULLY RESPONSIVE) ====================
            this.tabBorrow.BackColor = Color.FromArgb(14, 13, 24);
            this.tabBorrow.Controls.Add(this.tlpBorrow);
            this.tabBorrow.Name = "tabBorrow";
            this.tabBorrow.Padding = new Padding(14, 10, 14, 12);
            this.tabBorrow.Text = "Borrow Equipment";

            this.tlpBorrow.ColumnCount = 2;
            this.tlpBorrow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
            this.tlpBorrow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
            this.tlpBorrow.Controls.Add(this.grpRequest, 0, 0);
            this.tlpBorrow.Controls.Add(this.grpReceipt, 1, 0);
            this.tlpBorrow.Dock = DockStyle.Fill;
            this.tlpBorrow.RowCount = 1;
            this.tlpBorrow.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            this.grpRequest.AccentColor = Color.FromArgb(236, 46, 158);
            this.grpRequest.CardTitle = "NEW BORROW DISPATCH REQUEST";
            this.grpRequest.Controls.Add(this.tlpRequestForm);
            this.grpRequest.Dock = DockStyle.Fill;
            this.grpRequest.Margin = new Padding(0, 0, 10, 0);

            this.tlpRequestForm.BackColor = Color.FromArgb(22, 20, 38);
            this.tlpRequestForm.ColumnCount = 1;
            this.tlpRequestForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tlpRequestForm.Dock = DockStyle.Fill;
            this.tlpRequestForm.RowCount = 11;
            this.tlpRequestForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));  // 0: Borrower label
            this.tlpRequestForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));  // 1: Borrower combo
            this.tlpRequestForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));  // 2: Clearance badge
            this.tlpRequestForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));  // 3: Equip label
            this.tlpRequestForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));  // 4: Equip combo
            this.tlpRequestForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));  // 5: Room label
            this.tlpRequestForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));  // 6: Room textbox
            this.tlpRequestForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));  // 7: Room presets
            this.tlpRequestForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));  // 8: Checklist label
            this.tlpRequestForm.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // 9: Checklist box
            this.tlpRequestForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));  // 10: Issue button

            this.tlpRequestForm.Controls.Add(this.lblBorrower, 0, 0);
            this.tlpRequestForm.Controls.Add(this._cmbBorrowers, 0, 1);
            this.tlpRequestForm.Controls.Add(this._lblBorrowClearance, 0, 2);
            this.tlpRequestForm.Controls.Add(this.lblEquip, 0, 3);
            this.tlpRequestForm.Controls.Add(this._cmbAvailableEquipment, 0, 4);
            this.tlpRequestForm.Controls.Add(this.lblRoom, 0, 5);
            this.tlpRequestForm.Controls.Add(this._txtRoom, 0, 6);
            this.tlpRequestForm.Controls.Add(this.tlpRoomPresets, 0, 7);
            this.tlpRequestForm.Controls.Add(this.lblCheck, 0, 8);
            this.tlpRequestForm.Controls.Add(this._lstChecklistPreview, 0, 9);
            this.tlpRequestForm.Controls.Add(this.btnIssueSlip, 0, 10);

            this.lblBorrower.Dock = DockStyle.Fill;
            this.lblBorrower.Font = new Font("Bahnschrift SemiBold", 8.75F, FontStyle.Bold);
            this.lblBorrower.ForeColor = Color.FromArgb(164, 158, 196);
            this.lblBorrower.Text = "SELECT REGISTERED BORROWER";
            this.lblBorrower.TextAlign = ContentAlignment.BottomLeft;

            this._cmbBorrowers.BackColor = Color.FromArgb(34, 30, 56);
            this._cmbBorrowers.Dock = DockStyle.Fill;
            this._cmbBorrowers.DropDownStyle = ComboBoxStyle.DropDownList;
            this._cmbBorrowers.FlatStyle = FlatStyle.Flat;
            this._cmbBorrowers.ForeColor = Color.FromArgb(246, 245, 255);
            this._cmbBorrowers.Margin = new Padding(0, 4, 0, 2);
            this._cmbBorrowers.SelectedIndexChanged += new EventHandler(this.CmbBorrowers_SelectedIndexChanged);

            this._lblBorrowClearance.BackColor = Color.FromArgb(16, 14, 28);
            this._lblBorrowClearance.BorderStyle = BorderStyle.FixedSingle;
            this._lblBorrowClearance.Dock = DockStyle.Fill;
            this._lblBorrowClearance.Font = new Font("Bahnschrift SemiBold", 8.5F, FontStyle.Bold);
            this._lblBorrowClearance.ForeColor = Color.FromArgb(56, 238, 170);
            this._lblBorrowClearance.Margin = new Padding(0, 2, 0, 4);
            this._lblBorrowClearance.Padding = new Padding(8, 0, 8, 0);
            this._lblBorrowClearance.Text = "✓ CLEARANCE VERIFIED  //  AUTHORIZED TO BORROW";
            this._lblBorrowClearance.TextAlign = ContentAlignment.MiddleLeft;

            this.lblEquip.Dock = DockStyle.Fill;
            this.lblEquip.Font = new Font("Bahnschrift SemiBold", 8.75F, FontStyle.Bold);
            this.lblEquip.ForeColor = Color.FromArgb(164, 158, 196);
            this.lblEquip.Text = "SELECT AVAILABLE EQUIPMENT";
            this.lblEquip.TextAlign = ContentAlignment.BottomLeft;

            this._cmbAvailableEquipment.BackColor = Color.FromArgb(34, 30, 56);
            this._cmbAvailableEquipment.Dock = DockStyle.Fill;
            this._cmbAvailableEquipment.DropDownStyle = ComboBoxStyle.DropDownList;
            this._cmbAvailableEquipment.FlatStyle = FlatStyle.Flat;
            this._cmbAvailableEquipment.ForeColor = Color.FromArgb(246, 245, 255);
            this._cmbAvailableEquipment.Margin = new Padding(0, 4, 0, 4);
            this._cmbAvailableEquipment.SelectedIndexChanged += new EventHandler(this.CmbAvailableEquipment_SelectedIndexChanged);

            this.lblRoom.Dock = DockStyle.Fill;
            this.lblRoom.Font = new Font("Bahnschrift SemiBold", 8.75F, FontStyle.Bold);
            this.lblRoom.ForeColor = Color.FromArgb(164, 158, 196);
            this.lblRoom.Text = "DESTINATION ROOM / LABORATORY";
            this.lblRoom.TextAlign = ContentAlignment.BottomLeft;

            this._txtRoom.BackColor = Color.FromArgb(34, 30, 56);
            this._txtRoom.BorderStyle = BorderStyle.FixedSingle;
            this._txtRoom.Dock = DockStyle.Fill;
            this._txtRoom.ForeColor = Color.FromArgb(246, 245, 255);
            this._txtRoom.Margin = new Padding(0, 4, 0, 4);
            this._txtRoom.Text = "Room 304 - Lecture Hall";

            this.tlpRoomPresets.BackColor = Color.FromArgb(22, 20, 38);
            this.tlpRoomPresets.ColumnCount = 3;
            this.tlpRoomPresets.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            this.tlpRoomPresets.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            this.tlpRoomPresets.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            this.tlpRoomPresets.RowCount = 1;
            this.tlpRoomPresets.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tlpRoomPresets.Controls.Add(this.btnRoomPreset1, 0, 0);
            this.tlpRoomPresets.Controls.Add(this.btnRoomPreset2, 1, 0);
            this.tlpRoomPresets.Controls.Add(this.btnRoomPreset3, 2, 0);
            this.tlpRoomPresets.Dock = DockStyle.Fill;
            this.tlpRoomPresets.Margin = new Padding(0, 2, 0, 4);

            this.btnRoomPreset1.AccentColor = Color.FromArgb(72, 232, 214);
            this.btnRoomPreset1.Dock = DockStyle.Fill;
            this.btnRoomPreset1.Font = new Font("Bahnschrift SemiBold", 8.5F, FontStyle.Bold);
            this.btnRoomPreset1.Margin = new Padding(0, 0, 4, 0);
            this.btnRoomPreset1.Text = "RM 304 LECTURE";
            this.btnRoomPreset1.Click += new EventHandler(this.BtnRoomPreset_Click);

            this.btnRoomPreset2.AccentColor = Color.FromArgb(255, 196, 134);
            this.btnRoomPreset2.Dock = DockStyle.Fill;
            this.btnRoomPreset2.Font = new Font("Bahnschrift SemiBold", 8.5F, FontStyle.Bold);
            this.btnRoomPreset2.Margin = new Padding(4, 0, 4, 0);
            this.btnRoomPreset2.Text = "SCIENCE LAB 201";
            this.btnRoomPreset2.Click += new EventHandler(this.BtnRoomPreset_Click);

            this.btnRoomPreset3.AccentColor = Color.FromArgb(236, 46, 158);
            this.btnRoomPreset3.Dock = DockStyle.Fill;
            this.btnRoomPreset3.Font = new Font("Bahnschrift SemiBold", 8.5F, FontStyle.Bold);
            this.btnRoomPreset3.Margin = new Padding(4, 0, 0, 0);
            this.btnRoomPreset3.Text = "AVR AUDITORIUM";
            this.btnRoomPreset3.Click += new EventHandler(this.BtnRoomPreset_Click);

            this.lblCheck.Dock = DockStyle.Fill;
            this.lblCheck.Font = new Font("Bahnschrift SemiBold", 8.75F, FontStyle.Bold);
            this.lblCheck.ForeColor = Color.FromArgb(72, 232, 214);
            this.lblCheck.Text = "POLYMORPHIC RETURN CHECKLIST";
            this.lblCheck.TextAlign = ContentAlignment.BottomLeft;

            this._lstChecklistPreview.BackColor = Color.FromArgb(14, 13, 24);
            this._lstChecklistPreview.BorderStyle = BorderStyle.FixedSingle;
            this._lstChecklistPreview.Dock = DockStyle.Fill;
            this._lstChecklistPreview.Font = new Font("Segoe UI", 9.5F);
            this._lstChecklistPreview.ForeColor = Color.FromArgb(246, 245, 255);
            this._lstChecklistPreview.IntegralHeight = false;
            this._lstChecklistPreview.Margin = new Padding(0, 4, 0, 10);
            this._lstChecklistPreview.SelectionMode = SelectionMode.None;

            this.btnIssueSlip.AccentColor = Color.FromArgb(56, 238, 170);
            this.btnIssueSlip.Dock = DockStyle.Fill;
            this.btnIssueSlip.Margin = new Padding(0, 2, 0, 0);
            this.btnIssueSlip.Text = "ISSUE OFFICIAL BORROW SLIP";
            this.btnIssueSlip.Click += new EventHandler(this.BtnIssueSlip_Click);

            // Right Receipt Card (Matched bottom baseline)
            this.grpReceipt.AccentColor = Color.FromArgb(72, 232, 214);
            this.grpReceipt.CardTitle = "DISPATCH RECEIPT TERMINAL";
            this.grpReceipt.Controls.Add(this.tlpReceipt);
            this.grpReceipt.Dock = DockStyle.Fill;
            this.grpReceipt.Margin = new Padding(10, 0, 0, 0);

            this.tlpReceipt.BackColor = Color.FromArgb(22, 20, 38);
            this.tlpReceipt.ColumnCount = 1;
            this.tlpReceipt.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tlpReceipt.RowCount = 2;
            this.tlpReceipt.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tlpReceipt.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            this.tlpReceipt.Controls.Add(this._txtBorrowReceipt, 0, 0);
            this.tlpReceipt.Controls.Add(this.btnCopyReceipt, 0, 1);
            this.tlpReceipt.Dock = DockStyle.Fill;

            this._txtBorrowReceipt.BackColor = Color.FromArgb(11, 10, 19);
            this._txtBorrowReceipt.BorderStyle = BorderStyle.FixedSingle;
            this._txtBorrowReceipt.Dock = DockStyle.Fill;
            this._txtBorrowReceipt.Font = new Font("Consolas", 10F);
            this._txtBorrowReceipt.ForeColor = Color.FromArgb(72, 232, 214);
            this._txtBorrowReceipt.Margin = new Padding(0, 0, 0, 10);
            this._txtBorrowReceipt.Multiline = true;
            this._txtBorrowReceipt.ReadOnly = true;
            this._txtBorrowReceipt.ScrollBars = ScrollBars.Vertical;
            this._txtBorrowReceipt.Text = ">> LEONIDA DISPATCH TERMINAL ONLINE.\r\n>> Select a borrower and available unit, then click 'ISSUE OFFICIAL BORROW SLIP'.";

            this.btnCopyReceipt.AccentColor = Color.FromArgb(255, 196, 134);
            this.btnCopyReceipt.Dock = DockStyle.Fill;
            this.btnCopyReceipt.Margin = new Padding(0, 2, 0, 0);
            this.btnCopyReceipt.Text = "COPY RECEIPT TO CLIPBOARD";
            this.btnCopyReceipt.Click += new EventHandler(this.BtnCopyReceipt_Click);

            // ==================== TAB 3: RETURNS & ACTIVE SLIPS ====================
            this.tabReturn.BackColor = Color.FromArgb(14, 13, 24);
            this.tabReturn.Controls.Add(this.cardHistSlips);
            this.tabReturn.Controls.Add(this.actionPanel);
            this.tabReturn.Controls.Add(this.cardActiveSlips);
            this.tabReturn.Name = "tabReturn";
            this.tabReturn.Padding = new Padding(14, 10, 14, 12);
            this.tabReturn.Text = "Returns and Active Slips";

            this.cardActiveSlips.AccentColor = Color.FromArgb(72, 232, 214);
            this.cardActiveSlips.CardTitle = "ACTIVE BORROW SLIPS  //  SELECT A SLIP TO CHECK IN";
            this.cardActiveSlips.Controls.Add(this._dgvActiveSlips);
            this.cardActiveSlips.Dock = DockStyle.Top;
            this.cardActiveSlips.Height = 220;

            this._dgvActiveSlips.AllowUserToAddRows = false;
            this._dgvActiveSlips.AllowUserToDeleteRows = false;
            this._dgvActiveSlips.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._dgvActiveSlips.Dock = DockStyle.Fill;
            this._dgvActiveSlips.MultiSelect = false;
            this._dgvActiveSlips.ReadOnly = true;
            this._dgvActiveSlips.Columns.Add("SlipID", "SLIP ID");
            this._dgvActiveSlips.Columns.Add("Borrower", "BORROWER");
            this._dgvActiveSlips.Columns.Add("Tag", "TAG");
            this._dgvActiveSlips.Columns.Add("Item", "EQUIPMENT NAME");
            this._dgvActiveSlips.Columns.Add("Room", "ROOM");
            this._dgvActiveSlips.Columns.Add("Due", "DUE TIME");
            this._dgvActiveSlips.Columns.Add("Status", "STATUS");

            this.actionPanel.BackColor = Color.FromArgb(14, 13, 24);
            this.actionPanel.Controls.Add(this._chkChecklistIntact);
            this.actionPanel.Controls.Add(this.lblCond);
            this.actionPanel.Controls.Add(this._cmbReturnCondition);
            this.actionPanel.Controls.Add(this.btnProcessReturn);
            this.actionPanel.Dock = DockStyle.Top;
            this.actionPanel.Height = 58;

            this._chkChecklistIntact.Checked = true;
            this._chkChecklistIntact.Location = new Point(6, 15);
            this._chkChecklistIntact.OnColor = Color.FromArgb(56, 238, 170);
            this._chkChecklistIntact.Size = new Size(340, 28);
            this._chkChecklistIntact.Text = "ALL CHECKLIST COMPONENTS INTACT";

            this.lblCond.AutoSize = true;
            this.lblCond.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblCond.ForeColor = Color.FromArgb(164, 158, 196);
            this.lblCond.Location = new Point(368, 20);
            this.lblCond.Text = "RETURN CONDITION:";

            this._cmbReturnCondition.BackColor = Color.FromArgb(34, 30, 56);
            this._cmbReturnCondition.DropDownStyle = ComboBoxStyle.DropDownList;
            this._cmbReturnCondition.FlatStyle = FlatStyle.Flat;
            this._cmbReturnCondition.ForeColor = Color.FromArgb(246, 245, 255);
            this._cmbReturnCondition.Items.AddRange(new object[] { "Good", "Fair", "Damaged", "Needs Repair" });
            this._cmbReturnCondition.Location = new Point(512, 16);
            this._cmbReturnCondition.SelectedIndex = 0;
            this._cmbReturnCondition.Width = 175;

            this.btnProcessReturn.AccentColor = Color.FromArgb(255, 196, 134);
            this.btnProcessReturn.Location = new Point(706, 11);
            this.btnProcessReturn.Size = new Size(255, 36);
            this.btnProcessReturn.Text = "PROCESS EQUIPMENT RETURN";
            this.btnProcessReturn.Click += new EventHandler(this.BtnProcessReturn_Click);

            this.cardHistSlips.AccentColor = Color.FromArgb(255, 196, 134);
            this.cardHistSlips.CardTitle = "COMPLETED / HISTORICAL TRANSACTION ARCHIVE";
            this.cardHistSlips.Controls.Add(this._dgvHistoricalSlips);
            this.cardHistSlips.Dock = DockStyle.Fill;

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

            // ==================== TAB 4: BORROWERS & HOLDS + WANTED PROFILE ====================
            this.tabBorrowers.BackColor = Color.FromArgb(14, 13, 24);
            this.tabBorrowers.Controls.Add(this.tlpBorBody);
            this.tabBorrowers.Controls.Add(this.pnlBorTop);
            this.tabBorrowers.Name = "tabBorrowers";
            this.tabBorrowers.Padding = new Padding(14, 8, 14, 12);
            this.tabBorrowers.Text = "Borrowers and Holds";

            this.pnlBorTop.BackColor = Color.FromArgb(14, 13, 24);
            this.pnlBorTop.Controls.Add(this._txtSearchBorrowers);
            this.pnlBorTop.Controls.Add(this.btnApplyHold);
            this.pnlBorTop.Controls.Add(this.btnLiftHold);
            this.pnlBorTop.Dock = DockStyle.Top;
            this.pnlBorTop.Height = 50;

            this._txtSearchBorrowers.AccentColor = Color.FromArgb(236, 46, 158);
            this._txtSearchBorrowers.Location = new Point(0, 8);
            this._txtSearchBorrowers.Placeholder = "Search ID, name, or department...";
            this._txtSearchBorrowers.Size = new Size(285, 34);
            this._txtSearchBorrowers.SearchTextChanged += new EventHandler(this.TxtSearchBorrowers_TextChanged);

            this.btnApplyHold.AccentColor = Color.FromArgb(255, 68, 96);
            this.btnApplyHold.Location = new Point(300, 7);
            this.btnApplyHold.Size = new Size(220, 36);
            this.btnApplyHold.Text = "★ APPLY CLEARANCE HOLD";
            this.btnApplyHold.Click += new EventHandler(this.BtnApplyHold_Click);

            this.btnLiftHold.AccentColor = Color.FromArgb(56, 238, 170);
            this.btnLiftHold.Location = new Point(530, 7);
            this.btnLiftHold.Size = new Size(210, 36);
            this.btnLiftHold.Text = "LIFT CLEARANCE HOLD";
            this.btnLiftHold.Click += new EventHandler(this.BtnLiftHold_Click);

            this.tlpBorBody.ColumnCount = 2;
            this.tlpBorBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67F));
            this.tlpBorBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            this.tlpBorBody.Controls.Add(this.cardBorGrid, 0, 0);
            this.tlpBorBody.Controls.Add(this.cardBorProfile, 1, 0);
            this.tlpBorBody.Dock = DockStyle.Fill;
            this.tlpBorBody.RowCount = 1;
            this.tlpBorBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            this.cardBorGrid.AccentColor = Color.FromArgb(236, 46, 158);
            this.cardBorGrid.CardTitle = "REGISTERED BORROWER DIRECTORY";
            this.cardBorGrid.Controls.Add(this._dgvBorrowers);
            this.cardBorGrid.Dock = DockStyle.Fill;
            this.cardBorGrid.Margin = new Padding(0, 4, 8, 0);

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
            this._dgvBorrowers.SelectionChanged += new EventHandler(this.DgvBorrowers_SelectionChanged);

            this.cardBorProfile.AccentColor = Color.FromArgb(255, 68, 96);
            this.cardBorProfile.CardTitle = "BORROWER CLEARANCE DOSSIER";
            this.cardBorProfile.Controls.Add(this._borrowerProfile);
            this.cardBorProfile.Dock = DockStyle.Fill;
            this.cardBorProfile.Margin = new Padding(8, 4, 0, 0);

            this._borrowerProfile.Dock = DockStyle.Fill;

            // ==================== TAB 5: AUTOMATED QA TESTS (RESPONSIVE TOP BAR) ====================
            this.tabQa.BackColor = Color.FromArgb(14, 13, 24);
            this.tabQa.Controls.Add(this.cardQaConsole);
            this.tabQa.Controls.Add(this.pnlQaTop);
            this.tabQa.Name = "tabQa";
            this.tabQa.Padding = new Padding(14, 8, 14, 12);
            this.tabQa.Text = "Automated QA Tests";

            this.pnlQaTop.BackColor = Color.FromArgb(14, 13, 24);
            this.pnlQaTop.Controls.Add(this.tlpQaTop);
            this.pnlQaTop.Dock = DockStyle.Top;
            this.pnlQaTop.Height = 54;

            this.tlpQaTop.BackColor = Color.FromArgb(14, 13, 24);
            this.tlpQaTop.ColumnCount = 2;
            this.tlpQaTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 306F));
            this.tlpQaTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tlpQaTop.RowCount = 1;
            this.tlpQaTop.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tlpQaTop.Controls.Add(this.btnRunQa, 0, 0);
            this.tlpQaTop.Controls.Add(this._qaMeter, 1, 0);
            this.tlpQaTop.Dock = DockStyle.Fill;

            this.btnRunQa.AccentColor = Color.FromArgb(236, 46, 158);
            this.btnRunQa.Dock = DockStyle.Fill;
            this.btnRunQa.Margin = new Padding(0, 6, 12, 8);
            this.btnRunQa.Text = "RUN ALL 44 AUTOMATED QA TESTS";
            this.btnRunQa.Click += new EventHandler(this.BtnRunQa_Click);

            this._qaMeter.AccentColor = Color.FromArgb(72, 232, 214);
            this._qaMeter.Dock = DockStyle.Fill;
            this._qaMeter.Margin = new Padding(4, 7, 0, 7);
            this._qaMeter.Percent = 0;
            this._qaMeter.StatusText = "QA DIAGNOSTIC DECK  //  STANDBY (READY TO VERIFY 44 ASSERTIONS)";

            this.cardQaConsole.AccentColor = Color.FromArgb(236, 46, 158);
            this.cardQaConsole.CardTitle = "AUTOMATED UNIT & INTEGRATION ASSERTION LOG";
            this.cardQaConsole.Controls.Add(this._txtQaOutput);
            this.cardQaConsole.Dock = DockStyle.Fill;

            this._txtQaOutput.BackColor = Color.FromArgb(10, 9, 18);
            this._txtQaOutput.BorderStyle = BorderStyle.FixedSingle;
            this._txtQaOutput.Dock = DockStyle.Fill;
            this._txtQaOutput.Font = new Font("Consolas", 9.75F);
            this._txtQaOutput.ForeColor = Color.FromArgb(56, 238, 170);
            this._txtQaOutput.Multiline = true;
            this._txtQaOutput.ReadOnly = true;
            this._txtQaOutput.ScrollBars = ScrollBars.Both;
            this._txtQaOutput.Text = ">> QA ENGINE STANDBY.\r\n>> Click 'RUN ALL 44 AUTOMATED QA TESTS' to execute full OOP verification.";

            // ==================== MAINFORM ====================
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.FromArgb(14, 13, 24);
            this.ClientSize = new Size(1240, 760);
            this.Controls.Add(this._toastBanner);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.pnlHudStrip);
            this.Controls.Add(this.pnlNavBar);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this._lblStatusBanner);
            this.Font = new Font("Segoe UI", 9.5F);
            this.KeyPreview = true;
            this.MinimumSize = new Size(1140, 680);
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
            this.tlpInvBody.ResumeLayout(false);
            this.cardInvGrid.ResumeLayout(false);
            this.cardInvDossier.ResumeLayout(false);
            this.pnlDossierActions.ResumeLayout(false);
            this.tlpDossierBtns.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgvEquipment)).EndInit();
            this.tabBorrow.ResumeLayout(false);
            this.tlpBorrow.ResumeLayout(false);
            this.grpRequest.ResumeLayout(false);
            this.tlpRequestForm.ResumeLayout(false);
            this.tlpRequestForm.PerformLayout();
            this.tlpRoomPresets.ResumeLayout(false);
            this.grpReceipt.ResumeLayout(false);
            this.tlpReceipt.ResumeLayout(false);
            this.tlpReceipt.PerformLayout();
            this.tabReturn.ResumeLayout(false);
            this.cardActiveSlips.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgvActiveSlips)).EndInit();
            this.actionPanel.ResumeLayout(false);
            this.actionPanel.PerformLayout();
            this.cardHistSlips.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgvHistoricalSlips)).EndInit();
            this.tabBorrowers.ResumeLayout(false);
            this.pnlBorTop.ResumeLayout(false);
            this.tlpBorBody.ResumeLayout(false);
            this.cardBorGrid.ResumeLayout(false);
            this.cardBorProfile.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgvBorrowers)).EndInit();
            this.tabQa.ResumeLayout(false);
            this.pnlQaTop.ResumeLayout(false);
            this.tlpQaTop.ResumeLayout(false);
            this.cardQaConsole.ResumeLayout(false);
            this.cardQaConsole.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}