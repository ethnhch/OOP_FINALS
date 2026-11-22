using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolBorrowingSystem.UI
{
    partial class BorrowForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblHeader;
        private Label lblBorrower;
        private ComboBox cmbBorrowers;
        private Label lblEquip;
        private ComboBox cmbEquipment;
        private Label lblRoom;
        private TextBox txtRoom;
        private Label lblChecklist;
        private ListBox lstChecklist;
        private Button btnIssue;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblHeader = new Label();
            this.lblBorrower = new Label();
            this.cmbBorrowers = new ComboBox();
            this.lblEquip = new Label();
            this.cmbEquipment = new ComboBox();
            this.lblRoom = new Label();
            this.txtRoom = new TextBox();
            this.lblChecklist = new Label();
            this.lstChecklist = new ListBox();
            this.btnIssue = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            this.lblHeader.BackColor = Color.FromArgb(24, 49, 83);
            this.lblHeader.Dock = DockStyle.Top;
            this.lblHeader.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblHeader.ForeColor = Color.White;
            this.lblHeader.Height = 48;
            this.lblHeader.Padding = new Padding(14, 0, 0, 0);
            this.lblHeader.Text = "NEW EQUIPMENT BORROW REQUEST";
            this.lblHeader.TextAlign = ContentAlignment.MiddleLeft;

            this.lblBorrower.AutoSize = true;
            this.lblBorrower.Location = new Point(20, 66);
            this.lblBorrower.Text = "Select Registered Borrower:";

            this.cmbBorrowers.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBorrowers.Location = new Point(20, 90);
            this.cmbBorrowers.Size = new Size(440, 28);

            this.lblEquip.AutoSize = true;
            this.lblEquip.Location = new Point(20, 132);
            this.lblEquip.Text = "Select Available Equipment:";

            this.cmbEquipment.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbEquipment.Location = new Point(20, 156);
            this.cmbEquipment.Size = new Size(440, 28);
            this.cmbEquipment.SelectedIndexChanged += new EventHandler(this.CmbEquipment_SelectedIndexChanged);

            this.lblRoom.AutoSize = true;
            this.lblRoom.Location = new Point(20, 198);
            this.lblRoom.Text = "Room / Laboratory Where Equipment Will Be Used:";

            this.txtRoom.Location = new Point(20, 222);
            this.txtRoom.Size = new Size(440, 27);
            this.txtRoom.Text = "Room 304 - Lecture Hall";

            this.lblChecklist.AutoSize = true;
            this.lblChecklist.Location = new Point(20, 264);
            this.lblChecklist.Text = "Expected Return Checklist:";

            this.lstChecklist.BackColor = Color.FromArgb(248, 250, 252);
            this.lstChecklist.Location = new Point(20, 288);
            this.lstChecklist.Size = new Size(440, 120);

            this.btnIssue.BackColor = Color.FromArgb(46, 125, 50);
            this.btnIssue.FlatAppearance.BorderSize = 0;
            this.btnIssue.FlatStyle = FlatStyle.Flat;
            this.btnIssue.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.btnIssue.ForeColor = Color.White;
            this.btnIssue.Location = new Point(20, 424);
            this.btnIssue.Size = new Size(280, 38);
            this.btnIssue.Text = "Issue Official Borrow Slip";
            this.btnIssue.UseVisualStyleBackColor = false;
            this.btnIssue.Click += new EventHandler(this.BtnIssue_Click);

            this.btnCancel.BackColor = Color.FromArgb(100, 116, 139);
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.Location = new Point(315, 424);
            this.btnCancel.Size = new Size(145, 38);
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new EventHandler(this.BtnCancel_Click);

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.White;
            this.ClientSize = new Size(482, 482);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnIssue);
            this.Controls.Add(this.lstChecklist);
            this.Controls.Add(this.lblChecklist);
            this.Controls.Add(this.txtRoom);
            this.Controls.Add(this.lblRoom);
            this.Controls.Add(this.cmbEquipment);
            this.Controls.Add(this.lblEquip);
            this.Controls.Add(this.cmbBorrowers);
            this.Controls.Add(this.lblBorrower);
            this.Controls.Add(this.lblHeader);
            this.Font = new Font("Segoe UI", 9.5F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BorrowForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Borrow Equipment";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
