using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolBorrowingSystem.UI
{
    partial class ReturnForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblHeader;
        private Label lblSlip;
        private ComboBox cmbActiveSlips;
        private Label lblChecklist;
        private ListBox lstChecklist;
        private CheckBox chkChecklistIntact;
        private Label lblCondition;
        private ComboBox cmbCondition;
        private Button btnProcess;
        private Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblHeader = new Label();
            this.lblSlip = new Label();
            this.cmbActiveSlips = new ComboBox();
            this.lblChecklist = new Label();
            this.lstChecklist = new ListBox();
            this.chkChecklistIntact = new CheckBox();
            this.lblCondition = new Label();
            this.cmbCondition = new ComboBox();
            this.btnProcess = new Button();
            this.btnClose = new Button();
            this.SuspendLayout();

            this.lblHeader.BackColor = Color.FromArgb(24, 49, 83);
            this.lblHeader.Dock = DockStyle.Top;
            this.lblHeader.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblHeader.ForeColor = Color.White;
            this.lblHeader.Height = 48;
            this.lblHeader.Padding = new Padding(14, 0, 0, 0);
            this.lblHeader.Text = "PROCESS EQUIPMENT RETURN";
            this.lblHeader.TextAlign = ContentAlignment.MiddleLeft;

            this.lblSlip.AutoSize = true;
            this.lblSlip.Location = new Point(20, 66);
            this.lblSlip.Text = "Select Active Borrow Slip:";

            this.cmbActiveSlips.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbActiveSlips.Location = new Point(20, 90);
            this.cmbActiveSlips.Size = new Size(440, 28);
            this.cmbActiveSlips.SelectedIndexChanged += new EventHandler(this.CmbActiveSlips_SelectedIndexChanged);

            this.lblChecklist.AutoSize = true;
            this.lblChecklist.Location = new Point(20, 132);
            this.lblChecklist.Text = "Return Inspection Checklist:";

            this.lstChecklist.BackColor = Color.FromArgb(248, 250, 252);
            this.lstChecklist.Location = new Point(20, 156);
            this.lstChecklist.Size = new Size(440, 115);

            this.chkChecklistIntact.AutoSize = true;
            this.chkChecklistIntact.Checked = true;
            this.chkChecklistIntact.Location = new Point(20, 284);
            this.chkChecklistIntact.Text = "All checklist components are present and intact";

            this.lblCondition.AutoSize = true;
            this.lblCondition.Location = new Point(20, 322);
            this.lblCondition.Text = "Return Equipment Condition:";

            this.cmbCondition.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbCondition.Items.AddRange(new object[] { "Good", "Fair", "Damaged", "Needs Repair" });
            this.cmbCondition.Location = new Point(20, 346);
            this.cmbCondition.SelectedIndex = 0;
            this.cmbCondition.Size = new Size(440, 28);

            this.btnProcess.BackColor = Color.FromArgb(25, 118, 210);
            this.btnProcess.FlatAppearance.BorderSize = 0;
            this.btnProcess.FlatStyle = FlatStyle.Flat;
            this.btnProcess.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.btnProcess.ForeColor = Color.White;
            this.btnProcess.Location = new Point(20, 396);
            this.btnProcess.Size = new Size(280, 38);
            this.btnProcess.Text = "Complete Return & Check Fines";
            this.btnProcess.UseVisualStyleBackColor = false;
            this.btnProcess.Click += new EventHandler(this.BtnProcess_Click);

            this.btnClose.BackColor = Color.FromArgb(100, 116, 139);
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.btnClose.ForeColor = Color.White;
            this.btnClose.Location = new Point(315, 396);
            this.btnClose.Size = new Size(145, 38);
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new EventHandler(this.BtnClose_Click);

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.White;
            this.ClientSize = new Size(482, 456);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.cmbCondition);
            this.Controls.Add(this.lblCondition);
            this.Controls.Add(this.chkChecklistIntact);
            this.Controls.Add(this.lstChecklist);
            this.Controls.Add(this.lblChecklist);
            this.Controls.Add(this.cmbActiveSlips);
            this.Controls.Add(this.lblSlip);
            this.Controls.Add(this.lblHeader);
            this.Font = new Font("Segoe UI", 9.5F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReturnForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Process Equipment Return";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
