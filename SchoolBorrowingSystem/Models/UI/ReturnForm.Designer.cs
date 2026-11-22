using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolBorrowingSystem.UI
{
    partial class ReturnForm
    {
        private System.ComponentModel.IContainer components = null;

        private GtaHeaderBar headerBar;
        private GtaCardPanel cardInspection;
        private Label lblSlip;
        private ComboBox cmbActiveSlips;
        private Label lblSlipMeta;
        private Label lblChecklist;
        private CheckedListBox lstChecklist;
        private CheckBox chkChecklistIntact;
        private Label lblCondition;
        private ComboBox cmbCondition;
        private Label lblRiskBanner;
        private Label lblInlineAlert;
        private GtaTileButton btnProcess;
        private GtaTileButton btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.headerBar = new GtaHeaderBar();
            this.cardInspection = new GtaCardPanel();
            this.lblSlip = new Label();
            this.cmbActiveSlips = new ComboBox();
            this.lblSlipMeta = new Label();
            this.lblChecklist = new Label();
            this.lstChecklist = new CheckedListBox();
            this.chkChecklistIntact = new CheckBox();
            this.lblCondition = new Label();
            this.cmbCondition = new ComboBox();
            this.lblRiskBanner = new Label();
            this.lblInlineAlert = new Label();
            this.btnProcess = new GtaTileButton();
            this.btnClose = new GtaTileButton();

            this.cardInspection.SuspendLayout();
            this.SuspendLayout();

            // ==================== HEADER BAR ====================
            this.headerBar.Dock = DockStyle.Top;
            this.headerBar.Location = new Point(0, 0);
            this.headerBar.Name = "headerBar";
            this.headerBar.Size = new Size(540, 72);
            this.headerBar.SubtitleText = "LEONIDA CAMPUS  //  RETURN INSPECTION & FINE AUDIT";
            this.headerBar.TitleText = "PROCESS EQUIPMENT RETURN";

            // ==================== INSPECTION CARD ====================
            this.cardInspection.AccentColor = Color.FromArgb(255, 190, 146);
            this.cardInspection.CardTitle = "ACTIVE SLIP CHECK-IN & HARDWARE AUDIT";
            this.cardInspection.Controls.Add(this.lblSlip);
            this.cardInspection.Controls.Add(this.cmbActiveSlips);
            this.cardInspection.Controls.Add(this.lblSlipMeta);
            this.cardInspection.Controls.Add(this.lblChecklist);
            this.cardInspection.Controls.Add(this.lstChecklist);
            this.cardInspection.Controls.Add(this.chkChecklistIntact);
            this.cardInspection.Controls.Add(this.lblCondition);
            this.cardInspection.Controls.Add(this.cmbCondition);
            this.cardInspection.Controls.Add(this.lblRiskBanner);
            this.cardInspection.Controls.Add(this.lblInlineAlert);
            this.cardInspection.Location = new Point(16, 86);
            this.cardInspection.Name = "cardInspection";
            this.cardInspection.Size = new Size(508, 428);

            // Active Borrow Slip
            this.lblSlip.AutoSize = true;
            this.lblSlip.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblSlip.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblSlip.Location = new Point(16, 46);
            this.lblSlip.Text = "SELECT ACTIVE BORROW SLIP";

            this.cmbActiveSlips.BackColor = Color.FromArgb(36, 33, 56);
            this.cmbActiveSlips.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbActiveSlips.FlatStyle = FlatStyle.Flat;
            this.cmbActiveSlips.Font = new Font("Segoe UI", 9.5F);
            this.cmbActiveSlips.ForeColor = Color.FromArgb(245, 244, 252);
            this.cmbActiveSlips.Location = new Point(16, 66);
            this.cmbActiveSlips.Size = new Size(476, 26);
            this.cmbActiveSlips.SelectedIndexChanged += new EventHandler(this.CmbActiveSlips_SelectedIndexChanged);

            // Live Slip Telemetry Strip
            this.lblSlipMeta.BackColor = Color.FromArgb(18, 16, 30);
            this.lblSlipMeta.BorderStyle = BorderStyle.FixedSingle;
            this.lblSlipMeta.Font = new Font("Consolas", 8.5F, FontStyle.Bold);
            this.lblSlipMeta.ForeColor = Color.FromArgb(95, 216, 204);
            this.lblSlipMeta.Location = new Point(16, 98);
            this.lblSlipMeta.Padding = new Padding(8, 0, 8, 0);
            this.lblSlipMeta.Size = new Size(476, 24);
            this.lblSlipMeta.Text = "AWAITING ACTIVE SLIP SELECTION...";
            this.lblSlipMeta.TextAlign = ContentAlignment.MiddleLeft;

            // Interactive Return Checklist
            this.lblChecklist.AutoSize = true;
            this.lblChecklist.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblChecklist.ForeColor = Color.FromArgb(95, 216, 204);
            this.lblChecklist.Location = new Point(16, 132);
            this.lblChecklist.Text = "RETURN INSPECTION CHECKLIST (CLICK TO VERIFY ITEMS)";

            this.lstChecklist.BackColor = Color.FromArgb(14, 13, 24);
            this.lstChecklist.BorderStyle = BorderStyle.FixedSingle;
            this.lstChecklist.CheckOnClick = true;
            this.lstChecklist.Font = new Font("Segoe UI", 9.25F);
            this.lstChecklist.ForeColor = Color.FromArgb(245, 244, 252);
            this.lstChecklist.Location = new Point(16, 152);
            this.lstChecklist.Size = new Size(476, 108);
            this.lstChecklist.ItemCheck += new ItemCheckEventHandler(this.LstChecklist_ItemCheck);

            this.chkChecklistIntact.AutoSize = true;
            this.chkChecklistIntact.Checked = true;
            this.chkChecklistIntact.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            this.chkChecklistIntact.ForeColor = Color.FromArgb(245, 244, 252);
            this.chkChecklistIntact.Location = new Point(16, 268);
            this.chkChecklistIntact.Text = "All checklist components are present and intact";
            this.chkChecklistIntact.CheckedChanged += new EventHandler(this.ChkChecklistIntact_CheckedChanged);

            // Condition Dropdown
            this.lblCondition.AutoSize = true;
            this.lblCondition.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblCondition.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblCondition.Location = new Point(16, 298);
            this.lblCondition.Text = "RETURN EQUIPMENT CONDITION";

            this.cmbCondition.BackColor = Color.FromArgb(36, 33, 56);
            this.cmbCondition.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbCondition.FlatStyle = FlatStyle.Flat;
            this.cmbCondition.Font = new Font("Segoe UI", 9.5F);
            this.cmbCondition.ForeColor = Color.FromArgb(245, 244, 252);
            this.cmbCondition.Items.AddRange(new object[] { "Good", "Fair", "Damaged", "Needs Repair" });
            this.cmbCondition.Location = new Point(16, 318);
            this.cmbCondition.SelectedIndex = 0;
            this.cmbCondition.Size = new Size(476, 26);
            this.cmbCondition.SelectedIndexChanged += new EventHandler(this.CmbCondition_SelectedIndexChanged);

            // Live Penalty / Clearance Hold Risk Scanner
            this.lblRiskBanner.BackColor = Color.FromArgb(18, 16, 30);
            this.lblRiskBanner.BorderStyle = BorderStyle.FixedSingle;
            this.lblRiskBanner.Font = new Font("Bahnschrift SemiBold", 8.75F, FontStyle.Bold);
            this.lblRiskBanner.ForeColor = Color.FromArgb(62, 232, 168);
            this.lblRiskBanner.Location = new Point(16, 354);
            this.lblRiskBanner.Padding = new Padding(8, 0, 8, 0);
            this.lblRiskBanner.Size = new Size(476, 36);
            this.lblRiskBanner.Text = "✓ CLEAN INSPECTION  //  PHP 0.00 PENALTY EXPECTED";
            this.lblRiskBanner.TextAlign = ContentAlignment.MiddleLeft;

            // Inline Error Banner
            this.lblInlineAlert.Font = new Font("Bahnschrift SemiBold", 8.75F, FontStyle.Bold);
            this.lblInlineAlert.ForeColor = Color.FromArgb(255, 68, 92);
            this.lblInlineAlert.Location = new Point(16, 396);
            this.lblInlineAlert.Size = new Size(476, 22);
            this.lblInlineAlert.Text = "";
            this.lblInlineAlert.Visible = false;

            // ==================== ACTION BUTTONS ====================
            this.btnProcess.AccentColor = Color.FromArgb(62, 232, 168);
            this.btnProcess.Location = new Point(16, 526);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new Size(344, 42);
            this.btnProcess.Text = "COMPLETE RETURN & CHECK FINES";
            this.btnProcess.Click += new EventHandler(this.BtnProcess_Click);

            this.btnClose.AccentColor = Color.FromArgb(255, 68, 92);
            this.btnClose.Location = new Point(372, 526);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(152, 42);
            this.btnClose.Text = "CLOSE";
            this.btnClose.Click += new EventHandler(this.BtnClose_Click);

            // ==================== FORM ====================
            this.AcceptButton = this.btnProcess;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.FromArgb(16, 15, 26);
            this.CancelButton = this.btnClose;
            this.ClientSize = new Size(540, 584);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.cardInspection);
            this.Controls.Add(this.headerBar);
            this.Font = new Font("Segoe UI", 9.5F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReturnForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Process Equipment Return — Vice Campus";

            this.cardInspection.ResumeLayout(false);
            this.cardInspection.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}