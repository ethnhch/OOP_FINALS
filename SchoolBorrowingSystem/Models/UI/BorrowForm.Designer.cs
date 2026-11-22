using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolBorrowingSystem.UI
{
    partial class BorrowForm
    {
        private System.ComponentModel.IContainer components = null;

        private GtaHeaderBar headerBar;
        private GtaCardPanel cardDispatch;
        private Label lblBorrower;
        private ComboBox cmbBorrowers;
        private Label lblClearanceBadge;
        private Label lblEquip;
        private ComboBox cmbEquipment;
        private Label lblEquipMeta;
        private Label lblRoom;
        private TextBox txtRoom;
        private GtaTileButton btnRoomLecture;
        private GtaTileButton btnRoomLab;
        private GtaTileButton btnRoomAvr;
        private Label lblChecklist;
        private ListBox lstChecklist;
        private Label lblInlineAlert;
        private GtaTileButton btnIssue;
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
            this.cardDispatch = new GtaCardPanel();
            this.lblBorrower = new Label();
            this.cmbBorrowers = new ComboBox();
            this.lblClearanceBadge = new Label();
            this.lblEquip = new Label();
            this.cmbEquipment = new ComboBox();
            this.lblEquipMeta = new Label();
            this.lblRoom = new Label();
            this.txtRoom = new TextBox();
            this.btnRoomLecture = new GtaTileButton();
            this.btnRoomLab = new GtaTileButton();
            this.btnRoomAvr = new GtaTileButton();
            this.lblChecklist = new Label();
            this.lstChecklist = new ListBox();
            this.lblInlineAlert = new Label();
            this.btnIssue = new GtaTileButton();
            this.btnCancel = new GtaTileButton();

            this.cardDispatch.SuspendLayout();
            this.SuspendLayout();

            // ==================== HEADER BAR ====================
            this.headerBar.Dock = DockStyle.Top;
            this.headerBar.Location = new Point(0, 0);
            this.headerBar.Name = "headerBar";
            this.headerBar.Size = new Size(540, 72);
            this.headerBar.SubtitleText = "LEONIDA CAMPUS  //  EQUIPMENT CHECKOUT PROTOCOL";
            this.headerBar.TitleText = "NEW BORROW REQUEST";

            // ==================== DISPATCH CARD ====================
            this.cardDispatch.AccentColor = Color.FromArgb(228, 36, 153);
            this.cardDispatch.CardTitle = "BORROWER & ASSET ASSIGNMENT";
            this.cardDispatch.Controls.Add(this.lblBorrower);
            this.cardDispatch.Controls.Add(this.cmbBorrowers);
            this.cardDispatch.Controls.Add(this.lblClearanceBadge);
            this.cardDispatch.Controls.Add(this.lblEquip);
            this.cardDispatch.Controls.Add(this.cmbEquipment);
            this.cardDispatch.Controls.Add(this.lblEquipMeta);
            this.cardDispatch.Controls.Add(this.lblRoom);
            this.cardDispatch.Controls.Add(this.txtRoom);
            this.cardDispatch.Controls.Add(this.btnRoomLecture);
            this.cardDispatch.Controls.Add(this.btnRoomLab);
            this.cardDispatch.Controls.Add(this.btnRoomAvr);
            this.cardDispatch.Controls.Add(this.lblChecklist);
            this.cardDispatch.Controls.Add(this.lstChecklist);
            this.cardDispatch.Controls.Add(this.lblInlineAlert);
            this.cardDispatch.Location = new Point(16, 86);
            this.cardDispatch.Name = "cardDispatch";
            this.cardDispatch.Size = new Size(508, 456);

            // Registered Borrower
            this.lblBorrower.AutoSize = true;
            this.lblBorrower.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblBorrower.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblBorrower.Location = new Point(16, 46);
            this.lblBorrower.Text = "SELECT REGISTERED BORROWER";

            this.cmbBorrowers.BackColor = Color.FromArgb(36, 33, 56);
            this.cmbBorrowers.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBorrowers.FlatStyle = FlatStyle.Flat;
            this.cmbBorrowers.Font = new Font("Segoe UI", 9.5F);
            this.cmbBorrowers.ForeColor = Color.FromArgb(245, 244, 252);
            this.cmbBorrowers.Location = new Point(16, 66);
            this.cmbBorrowers.Size = new Size(476, 26);
            this.cmbBorrowers.SelectedIndexChanged += new EventHandler(this.CmbBorrowers_SelectedIndexChanged);

            // Live Clearance Scanner Strip
            this.lblClearanceBadge.BackColor = Color.FromArgb(18, 16, 30);
            this.lblClearanceBadge.BorderStyle = BorderStyle.FixedSingle;
            this.lblClearanceBadge.Font = new Font("Bahnschrift SemiBold", 8.75F, FontStyle.Bold);
            this.lblClearanceBadge.ForeColor = Color.FromArgb(62, 232, 168);
            this.lblClearanceBadge.Location = new Point(16, 98);
            this.lblClearanceBadge.Padding = new Padding(8, 0, 8, 0);
            this.lblClearanceBadge.Size = new Size(476, 24);
            this.lblClearanceBadge.Text = "✓ CLEARANCE VERIFIED  //  BORROWER IN GOOD STANDING";
            this.lblClearanceBadge.TextAlign = ContentAlignment.MiddleLeft;

            // Available Equipment
            this.lblEquip.AutoSize = true;
            this.lblEquip.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblEquip.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblEquip.Location = new Point(16, 132);
            this.lblEquip.Text = "SELECT AVAILABLE EQUIPMENT";

            this.cmbEquipment.BackColor = Color.FromArgb(36, 33, 56);
            this.cmbEquipment.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbEquipment.FlatStyle = FlatStyle.Flat;
            this.cmbEquipment.Font = new Font("Segoe UI", 9.5F);
            this.cmbEquipment.ForeColor = Color.FromArgb(245, 244, 252);
            this.cmbEquipment.Location = new Point(16, 152);
            this.cmbEquipment.Size = new Size(476, 26);
            this.cmbEquipment.SelectedIndexChanged += new EventHandler(this.CmbEquipment_SelectedIndexChanged);

            // Live Asset Telemetry Strip
            this.lblEquipMeta.BackColor = Color.FromArgb(18, 16, 30);
            this.lblEquipMeta.BorderStyle = BorderStyle.FixedSingle;
            this.lblEquipMeta.Font = new Font("Consolas", 8.5F, FontStyle.Bold);
            this.lblEquipMeta.ForeColor = Color.FromArgb(95, 216, 204);
            this.lblEquipMeta.Location = new Point(16, 184);
            this.lblEquipMeta.Padding = new Padding(8, 0, 8, 0);
            this.lblEquipMeta.Size = new Size(476, 24);
            this.lblEquipMeta.Text = "CATEGORY: AV EQUIPMENT  //  CONDITION: GOOD";
            this.lblEquipMeta.TextAlign = ContentAlignment.MiddleLeft;

            // Destination Room + Quick Preset Chips
            this.lblRoom.AutoSize = true;
            this.lblRoom.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblRoom.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblRoom.Location = new Point(16, 218);
            this.lblRoom.Text = "DESTINATION ROOM / LABORATORY";

            this.txtRoom.BackColor = Color.FromArgb(36, 33, 56);
            this.txtRoom.BorderStyle = BorderStyle.FixedSingle;
            this.txtRoom.Font = new Font("Segoe UI", 9.5F);
            this.txtRoom.ForeColor = Color.FromArgb(245, 244, 252);
            this.txtRoom.Location = new Point(16, 238);
            this.txtRoom.PlaceholderText = "Enter room or lab location...";
            this.txtRoom.Size = new Size(476, 26);
            this.txtRoom.Text = "Room 304 - Lecture Hall";

            this.btnRoomLecture.AccentColor = Color.FromArgb(95, 216, 204);
            this.btnRoomLecture.Font = new Font("Bahnschrift SemiBold", 8.25F, FontStyle.Bold);
            this.btnRoomLecture.Location = new Point(16, 270);
            this.btnRoomLecture.Size = new Size(154, 26);
            this.btnRoomLecture.Text = "RM 304 LECTURE";
            this.btnRoomLecture.Click += new EventHandler(this.BtnRoomPreset_Click);

            this.btnRoomLab.AccentColor = Color.FromArgb(255, 190, 146);
            this.btnRoomLab.Font = new Font("Bahnschrift SemiBold", 8.25F, FontStyle.Bold);
            this.btnRoomLab.Location = new Point(177, 270);
            this.btnRoomLab.Size = new Size(154, 26);
            this.btnRoomLab.Text = "SCIENCE LAB 201";
            this.btnRoomLab.Click += new EventHandler(this.BtnRoomPreset_Click);

            this.btnRoomAvr.AccentColor = Color.FromArgb(228, 36, 153);
            this.btnRoomAvr.Font = new Font("Bahnschrift SemiBold", 8.25F, FontStyle.Bold);
            this.btnRoomAvr.Location = new Point(338, 270);
            this.btnRoomAvr.Size = new Size(154, 26);
            this.btnRoomAvr.Text = "AVR AUDITORIUM";
            this.btnRoomAvr.Click += new EventHandler(this.BtnRoomPreset_Click);

            // Polymorphic Return Checklist
            this.lblChecklist.AutoSize = true;
            this.lblChecklist.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblChecklist.ForeColor = Color.FromArgb(95, 216, 204);
            this.lblChecklist.Location = new Point(16, 306);
            this.lblChecklist.Text = "EXPECTED RETURN CHECKLIST (POLYMORPHIC)";

            this.lstChecklist.BackColor = Color.FromArgb(14, 13, 24);
            this.lstChecklist.BorderStyle = BorderStyle.FixedSingle;
            this.lstChecklist.Font = new Font("Segoe UI", 9.25F);
            this.lstChecklist.ForeColor = Color.FromArgb(245, 244, 252);
            this.lstChecklist.Location = new Point(16, 326);
            this.lstChecklist.Size = new Size(476, 94);

            // Inline Rejection / Validation Banner
            this.lblInlineAlert.Font = new Font("Bahnschrift SemiBold", 8.75F, FontStyle.Bold);
            this.lblInlineAlert.ForeColor = Color.FromArgb(255, 68, 92);
            this.lblInlineAlert.Location = new Point(16, 426);
            this.lblInlineAlert.Size = new Size(476, 22);
            this.lblInlineAlert.Text = "";
            this.lblInlineAlert.Visible = false;

            // ==================== ACTION BUTTONS ====================
            this.btnIssue.AccentColor = Color.FromArgb(62, 232, 168);
            this.btnIssue.Location = new Point(16, 554);
            this.btnIssue.Name = "btnIssue";
            this.btnIssue.Size = new Size(344, 42);
            this.btnIssue.Text = "ISSUE OFFICIAL BORROW SLIP";
            this.btnIssue.Click += new EventHandler(this.BtnIssue_Click);

            this.btnCancel.AccentColor = Color.FromArgb(255, 68, 92);
            this.btnCancel.Location = new Point(372, 554);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(152, 42);
            this.btnCancel.Text = "CLOSE";
            this.btnCancel.Click += new EventHandler(this.BtnCancel_Click);

            // ==================== FORM ====================
            this.AcceptButton = this.btnIssue;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.FromArgb(16, 15, 26);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new Size(540, 612);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnIssue);
            this.Controls.Add(this.cardDispatch);
            this.Controls.Add(this.headerBar);
            this.Font = new Font("Segoe UI", 9.5F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BorrowForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Borrow Equipment — Vice Campus";

            this.cardDispatch.ResumeLayout(false);
            this.cardDispatch.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}