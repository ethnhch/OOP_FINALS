using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolBorrowingSystem.UI
{
    partial class AddEquipmentForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblHeader;
        private Label lblCategory;
        private ComboBox cmbCategory;
        private Label lblTag;
        private TextBox txtTag;
        private Label lblName;
        private TextBox txtName;
        private CheckBox chkRemote;
        private CheckBox chkPowerCord;
        private CheckBox chkHdmi;
        private Label lblDuration;
        private NumericUpDown numDuration;
        private Button btnAdd;
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
            this.lblCategory = new Label();
            this.cmbCategory = new ComboBox();
            this.lblTag = new Label();
            this.txtTag = new TextBox();
            this.lblName = new Label();
            this.txtName = new TextBox();
            this.chkRemote = new CheckBox();
            this.chkPowerCord = new CheckBox();
            this.chkHdmi = new CheckBox();
            this.lblDuration = new Label();
            this.numDuration = new NumericUpDown();
            this.btnAdd = new Button();
            this.btnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).BeginInit();
            this.SuspendLayout();

            this.lblHeader.BackColor = Color.FromArgb(24, 49, 83);
            this.lblHeader.Dock = DockStyle.Top;
            this.lblHeader.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblHeader.ForeColor = Color.White;
            this.lblHeader.Height = 48;
            this.lblHeader.Padding = new Padding(14, 0, 0, 0);
            this.lblHeader.Text = "REGISTER NEW SCHOOL EQUIPMENT";
            this.lblHeader.TextAlign = ContentAlignment.MiddleLeft;

            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new Point(20, 66);
            this.lblCategory.Text = "Equipment Category:";

            this.cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbCategory.Items.AddRange(new object[] { "AV Equipment", "Lab Equipment" });
            this.cmbCategory.Location = new Point(20, 90);
            this.cmbCategory.SelectedIndex = 0;
            this.cmbCategory.Size = new Size(420, 28);
            this.cmbCategory.SelectedIndexChanged += new EventHandler(this.CmbCategory_SelectedIndexChanged);

            this.lblTag.AutoSize = true;
            this.lblTag.Location = new Point(20, 130);
            this.lblTag.Text = "Unique Property Tag (e.g., AV-103 or LAB-203):";

            this.txtTag.Location = new Point(20, 154);
            this.txtTag.Size = new Size(420, 27);

            this.lblName.AutoSize = true;
            this.lblName.Location = new Point(20, 194);
            this.lblName.Text = "Equipment Item Name:";

            this.txtName.Location = new Point(20, 218);
            this.txtName.Size = new Size(420, 27);

            this.chkRemote.AutoSize = true;
            this.chkRemote.Checked = true;
            this.chkRemote.Location = new Point(20, 260);
            this.chkRemote.Text = "Includes Remote Control";

            this.chkPowerCord.AutoSize = true;
            this.chkPowerCord.Checked = true;
            this.chkPowerCord.Location = new Point(20, 288);
            this.chkPowerCord.Text = "Includes Power Cord";

            this.chkHdmi.AutoSize = true;
            this.chkHdmi.Checked = true;
            this.chkHdmi.Location = new Point(220, 260);
            this.chkHdmi.Text = "Includes HDMI Cable";

            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new Point(20, 326);
            this.lblDuration.Text = "Max Borrow Duration (Hours for AV / Days for Lab):";

            this.numDuration.Location = new Point(20, 350);
            this.numDuration.Maximum = 30;
            this.numDuration.Minimum = 1;
            this.numDuration.Size = new Size(140, 27);
            this.numDuration.Value = 4;

            this.btnAdd.BackColor = Color.FromArgb(46, 125, 50);
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = FlatStyle.Flat;
            this.btnAdd.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.btnAdd.ForeColor = Color.White;
            this.btnAdd.Location = new Point(20, 398);
            this.btnAdd.Size = new Size(260, 38);
            this.btnAdd.Text = "Add Equipment to Inventory";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new EventHandler(this.BtnAdd_Click);

            this.btnCancel.BackColor = Color.FromArgb(100, 116, 139);
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.Location = new Point(295, 398);
            this.btnCancel.Size = new Size(145, 38);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new EventHandler(this.BtnCancel_Click);

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.White;
            this.ClientSize = new Size(462, 458);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.numDuration);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.chkHdmi);
            this.Controls.Add(this.chkPowerCord);
            this.Controls.Add(this.chkRemote);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtTag);
            this.Controls.Add(this.lblTag);
            this.Controls.Add(this.cmbCategory);
            this.Controls.Add(this.lblCategory);
            this.Controls.Add(this.lblHeader);
            this.Font = new Font("Segoe UI", 9.5F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddEquipmentForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Add New Equipment";
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
