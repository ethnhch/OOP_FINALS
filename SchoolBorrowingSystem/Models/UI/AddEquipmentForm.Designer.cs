using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolBorrowingSystem.UI
{
    partial class AddEquipmentForm
    {
        private System.ComponentModel.IContainer components = null;

        private GtaHeaderBar headerBar;
        private GtaCardPanel cardMain;
        private Label lblCategory;
        private ComboBox cmbCategory;
        private Label lblTag;
        private TextBox txtTag;
        private GtaTileButton btnAutoTag;
        private Label lblName;
        private TextBox txtName;
        private Panel pnlCategorySpecs;
        private Label lblSpecTitle;
        private CheckBox chkRemote;
        private CheckBox chkPowerCord;
        private CheckBox chkHdmi;
        private Label lblComponents;
        private NumericUpDown numComponents;
        private Label lblLabSpecs;
        private Label lblDuration;
        private NumericUpDown numDuration;
        private Label lblPreview;
        private Label lblInlineAlert;
        private GtaTileButton btnAdd;
        private GtaTileButton btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.headerBar = new GtaHeaderBar();
            this.cardMain = new GtaCardPanel();
            this.lblCategory = new Label();
            this.cmbCategory = new ComboBox();
            this.lblTag = new Label();
            this.txtTag = new TextBox();
            this.btnAutoTag = new GtaTileButton();
            this.lblName = new Label();
            this.txtName = new TextBox();
            this.pnlCategorySpecs = new Panel();
            this.lblSpecTitle = new Label();
            this.chkRemote = new CheckBox();
            this.chkPowerCord = new CheckBox();
            this.chkHdmi = new CheckBox();
            this.lblComponents = new Label();
            this.numComponents = new NumericUpDown();
            this.lblLabSpecs = new Label();
            this.lblDuration = new Label();
            this.numDuration = new NumericUpDown();
            this.lblPreview = new Label();
            this.lblInlineAlert = new Label();
            this.btnAdd = new GtaTileButton();
            this.btnCancel = new GtaTileButton();

            this.cardMain.SuspendLayout();
            this.pnlCategorySpecs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numComponents)).BeginInit();
            this.SuspendLayout();

            // ==================== HEADER BAR ====================
            this.headerBar.Dock = DockStyle.Top;
            this.headerBar.Location = new Point(0, 0);
            this.headerBar.Name = "headerBar";
            this.headerBar.Size = new Size(520, 72);
            this.headerBar.SubtitleText = "VICE CAMPUS ASSET INTAKE  //  AV & LAB HARDWARE";
            this.headerBar.TitleText = "REGISTER NEW EQUIPMENT";

            // ==================== MAIN FORM CARD ====================
            this.cardMain.AccentColor = Color.FromArgb(95, 216, 204);
            this.cardMain.CardTitle = "HARDWARE SPECIFICATION ENTRY";
            this.cardMain.Controls.Add(this.lblCategory);
            this.cardMain.Controls.Add(this.cmbCategory);
            this.cardMain.Controls.Add(this.lblTag);
            this.cardMain.Controls.Add(this.txtTag);
            this.cardMain.Controls.Add(this.btnAutoTag);
            this.cardMain.Controls.Add(this.lblName);
            this.cardMain.Controls.Add(this.txtName);
            this.cardMain.Controls.Add(this.pnlCategorySpecs);
            this.cardMain.Controls.Add(this.lblDuration);
            this.cardMain.Controls.Add(this.numDuration);
            this.cardMain.Controls.Add(this.lblPreview);
            this.cardMain.Controls.Add(this.lblInlineAlert);
            this.cardMain.Location = new Point(16, 86);
            this.cardMain.Name = "cardMain";
            this.cardMain.Size = new Size(488, 396);

            // Category
            this.lblCategory.AutoSize = true;
            this.lblCategory.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblCategory.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblCategory.Location = new Point(16, 48);
            this.lblCategory.Text = "EQUIPMENT CATEGORY";

            this.cmbCategory.BackColor = Color.FromArgb(36, 33, 56);
            this.cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbCategory.FlatStyle = FlatStyle.Flat;
            this.cmbCategory.Font = new Font("Segoe UI", 9.75F);
            this.cmbCategory.ForeColor = Color.FromArgb(245, 244, 252);
            this.cmbCategory.Items.AddRange(new object[] { "AV Equipment", "Lab Equipment" });
            this.cmbCategory.Location = new Point(16, 68);
            this.cmbCategory.SelectedIndex = 0;
            this.cmbCategory.Size = new Size(456, 26);
            this.cmbCategory.SelectedIndexChanged += new EventHandler(this.CmbCategory_SelectedIndexChanged);

            // Property Tag + Auto Tag Button
            this.lblTag.AutoSize = true;
            this.lblTag.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblTag.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblTag.Location = new Point(16, 104);
            this.lblTag.Text = "UNIQUE PROPERTY TAG (E.G., AV-103 OR LAB-203)";

            this.txtTag.BackColor = Color.FromArgb(36, 33, 56);
            this.txtTag.BorderStyle = BorderStyle.FixedSingle;
            this.txtTag.CharacterCasing = CharacterCasing.Upper;
            this.txtTag.Font = new Font("Consolas", 10F, FontStyle.Bold);
            this.txtTag.ForeColor = Color.FromArgb(95, 216, 204);
            this.txtTag.Location = new Point(16, 124);
            this.txtTag.PlaceholderText = "AV-103";
            this.txtTag.Size = new Size(340, 26);
            this.txtTag.TextChanged += new EventHandler(this.Input_Changed);

            this.btnAutoTag.AccentColor = Color.FromArgb(255, 190, 146);
            this.btnAutoTag.Location = new Point(364, 122);
            this.btnAutoTag.Name = "btnAutoTag";
            this.btnAutoTag.Size = new Size(108, 28);
            this.btnAutoTag.Text = "AUTO TAG";
            this.btnAutoTag.Click += new EventHandler(this.BtnAutoTag_Click);

            // Equipment Name
            this.lblName.AutoSize = true;
            this.lblName.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblName.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblName.Location = new Point(16, 160);
            this.lblName.Text = "EQUIPMENT ITEM NAME";

            this.txtName.BackColor = Color.FromArgb(36, 33, 56);
            this.txtName.BorderStyle = BorderStyle.FixedSingle;
            this.txtName.Font = new Font("Segoe UI", 9.75F);
            this.txtName.ForeColor = Color.FromArgb(245, 244, 252);
            this.txtName.Location = new Point(16, 180);
            this.txtName.PlaceholderText = "e.g., Epson 4K Laser Projector or Digital Microscope";
            this.txtName.Size = new Size(456, 26);
            this.txtName.TextChanged += new EventHandler(this.Input_Changed);

            // Category Specs Box (Swaps seamlessly between AV Accessories & Lab Profile)
            this.pnlCategorySpecs.BackColor = Color.FromArgb(18, 16, 30);
            this.pnlCategorySpecs.BorderStyle = BorderStyle.FixedSingle;
            this.pnlCategorySpecs.Controls.Add(this.lblSpecTitle);
            this.pnlCategorySpecs.Controls.Add(this.chkRemote);
            this.pnlCategorySpecs.Controls.Add(this.chkPowerCord);
            this.pnlCategorySpecs.Controls.Add(this.chkHdmi);
            this.pnlCategorySpecs.Controls.Add(this.lblComponents);
            this.pnlCategorySpecs.Controls.Add(this.numComponents);
            this.pnlCategorySpecs.Controls.Add(this.lblLabSpecs);
            this.pnlCategorySpecs.Location = new Point(16, 218);
            this.pnlCategorySpecs.Size = new Size(456, 74);

            this.lblSpecTitle.AutoSize = true;
            this.lblSpecTitle.Font = new Font("Bahnschrift SemiBold", 8.5F, FontStyle.Bold);
            this.lblSpecTitle.ForeColor = Color.FromArgb(95, 216, 204);
            this.lblSpecTitle.Location = new Point(12, 8);
            this.lblSpecTitle.Text = "INCLUDED AV ACCESSORIES (POLYMORPHIC CHECKLIST)";

            this.chkRemote.AutoSize = true;
            this.chkRemote.Checked = true;
            this.chkRemote.ForeColor = Color.FromArgb(245, 244, 252);
            this.chkRemote.Location = new Point(12, 34);
            this.chkRemote.Text = "Remote Control";

            this.chkPowerCord.AutoSize = true;
            this.chkPowerCord.Checked = true;
            this.chkPowerCord.ForeColor = Color.FromArgb(245, 244, 252);
            this.chkPowerCord.Location = new Point(158, 34);
            this.chkPowerCord.Text = "Power Cord";

            this.chkHdmi.AutoSize = true;
            this.chkHdmi.Checked = true;
            this.chkHdmi.ForeColor = Color.FromArgb(245, 244, 252);
            this.chkHdmi.Location = new Point(288, 34);
            this.chkHdmi.Text = "HDMI Cable";

            this.lblComponents.AutoSize = true;
            this.lblComponents.Font = new Font("Bahnschrift SemiBold", 8.5F, FontStyle.Bold);
            this.lblComponents.ForeColor = Color.FromArgb(245, 244, 252);
            this.lblComponents.Location = new Point(12, 36);
            this.lblComponents.Text = "COMPONENTS:";
            this.lblComponents.Visible = false;

            this.numComponents.BackColor = Color.FromArgb(36, 33, 56);
            this.numComponents.BorderStyle = BorderStyle.FixedSingle;
            this.numComponents.Font = new Font("Consolas", 10F, FontStyle.Bold);
            this.numComponents.ForeColor = Color.FromArgb(245, 244, 252);
            this.numComponents.Location = new Point(110, 33);
            this.numComponents.Maximum = 50;
            this.numComponents.Minimum = 1;
            this.numComponents.Size = new Size(65, 26);
            this.numComponents.Value = 3;
            this.numComponents.Visible = false;
            this.numComponents.ValueChanged += new EventHandler(this.Input_Changed);

            this.lblLabSpecs.Font = new Font("Segoe UI", 8.25F);
            this.lblLabSpecs.ForeColor = Color.FromArgb(255, 190, 146);
            this.lblLabSpecs.Location = new Point(185, 30);
            this.lblLabSpecs.Size = new Size(260, 38);
            this.lblLabSpecs.Text = "CALIBRATION: TODAY // SAFETY: TIER 3\r\nLab safety & component checklist auto-applied.";
            this.lblLabSpecs.Visible = false;

            // Max Borrow Duration + Live Preview Pill
            this.lblDuration.AutoSize = true;
            this.lblDuration.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblDuration.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblDuration.Location = new Point(16, 304);
            this.lblDuration.Text = "MAX BORROW DURATION (HOURS)";

            this.numDuration.BackColor = Color.FromArgb(36, 33, 56);
            this.numDuration.BorderStyle = BorderStyle.FixedSingle;
            this.numDuration.Font = new Font("Consolas", 10F, FontStyle.Bold);
            this.numDuration.ForeColor = Color.FromArgb(245, 244, 252);
            this.numDuration.Location = new Point(16, 324);
            this.numDuration.Maximum = 30;
            this.numDuration.Minimum = 1;
            this.numDuration.Size = new Size(125, 26);
            this.numDuration.Value = 4;
            this.numDuration.ValueChanged += new EventHandler(this.Input_Changed);

            this.lblPreview.BackColor = Color.FromArgb(18, 16, 30);
            this.lblPreview.BorderStyle = BorderStyle.FixedSingle;
            this.lblPreview.Font = new Font("Consolas", 8.75F, FontStyle.Bold);
            this.lblPreview.ForeColor = Color.FromArgb(62, 232, 168);
            this.lblPreview.Location = new Point(154, 324);
            this.lblPreview.Padding = new Padding(8, 0, 8, 0);
            this.lblPreview.Size = new Size(318, 26);
            this.lblPreview.Text = "[NEW TAG] UNNAMED UNIT  //  4 HRS";
            this.lblPreview.TextAlign = ContentAlignment.MiddleLeft;

            this.lblInlineAlert.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblInlineAlert.ForeColor = Color.FromArgb(255, 68, 92);
            this.lblInlineAlert.Location = new Point(16, 360);
            this.lblInlineAlert.Size = new Size(456, 22);
            this.lblInlineAlert.Text = "";
            this.lblInlineAlert.Visible = false;

            // ==================== ACTION BUTTONS ====================
            this.btnAdd.AccentColor = Color.FromArgb(62, 232, 168);
            this.btnAdd.Location = new Point(16, 496);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new Size(326, 42);
            this.btnAdd.Text = "+ REGISTER EQUIPMENT";
            this.btnAdd.Click += new EventHandler(this.BtnAdd_Click);

            this.btnCancel.AccentColor = Color.FromArgb(255, 68, 92);
            this.btnCancel.Location = new Point(354, 496);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(150, 42);
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.Click += new EventHandler(this.BtnCancel_Click);

            // ==================== FORM ====================
            this.AcceptButton = this.btnAdd;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.FromArgb(16, 15, 26);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new Size(520, 554);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.cardMain);
            this.Controls.Add(this.headerBar);
            this.Font = new Font("Segoe UI", 9.5F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddEquipmentForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Register New Equipment — Vice Campus";

            this.cardMain.ResumeLayout(false);
            this.cardMain.PerformLayout();
            this.pnlCategorySpecs.ResumeLayout(false);
            this.pnlCategorySpecs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numComponents)).EndInit();
            this.ResumeLayout(false);
        }
    }
}