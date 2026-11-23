using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolBorrowingSystem.UI
{
    partial class BorrowerForm
    {
        private System.ComponentModel.IContainer components = null;

        private GtaHeaderBar headerBar;
        private GtaCardPanel cardMain;
        private Label lblSchoolId;
        private TextBox txtSchoolId;
        private Label lblName;
        private TextBox txtName;
        private Label lblRole;
        private ComboBox cmbRole;
        private Label lblDepartment;
        private TextBox txtDepartment;
        private CheckBox chkHold;
        private Label lblInlineAlert;
        private GtaTileButton btnSave;
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
            this.lblSchoolId = new Label();
            this.txtSchoolId = new TextBox();
            this.lblName = new Label();
            this.txtName = new TextBox();
            this.lblRole = new Label();
            this.cmbRole = new ComboBox();
            this.lblDepartment = new Label();
            this.txtDepartment = new TextBox();
            this.chkHold = new CheckBox();
            this.lblInlineAlert = new Label();
            this.btnSave = new GtaTileButton();
            this.btnCancel = new GtaTileButton();

            this.cardMain.SuspendLayout();
            this.SuspendLayout();

            // ==================== HEADER BAR ====================
            this.headerBar.Dock = DockStyle.Top;
            this.headerBar.Location = new Point(0, 0);
            this.headerBar.Name = "headerBar";
            this.headerBar.Size = new Size(500, 72);
            this.headerBar.SubtitleText = "CAMPUS REGISTRATION  //  STUDENT & FACULTY PROFILE";
            this.headerBar.TitleText = "REGISTER NEW BORROWER";

            // ==================== MAIN FORM CARD ====================
            this.cardMain.AccentColor = Color.FromArgb(72, 232, 214);
            this.cardMain.CardTitle = "BORROWER CREDENTIAL DETAILS";
            this.cardMain.Controls.Add(this.lblSchoolId);
            this.cardMain.Controls.Add(this.txtSchoolId);
            this.cardMain.Controls.Add(this.lblName);
            this.cardMain.Controls.Add(this.txtName);
            this.cardMain.Controls.Add(this.lblRole);
            this.cardMain.Controls.Add(this.cmbRole);
            this.cardMain.Controls.Add(this.lblDepartment);
            this.cardMain.Controls.Add(this.txtDepartment);
            this.cardMain.Controls.Add(this.chkHold);
            this.cardMain.Controls.Add(this.lblInlineAlert);
            this.cardMain.Location = new Point(16, 86);
            this.cardMain.Name = "cardMain";
            this.cardMain.Size = new Size(468, 330);

            // School ID
            this.lblSchoolId.AutoSize = true;
            this.lblSchoolId.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblSchoolId.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblSchoolId.Location = new Point(16, 42);
            this.lblSchoolId.Text = "SCHOOL ID NUMBER (E.G. 2024-10005 OR FAC-5010)";

            this.txtSchoolId.BackColor = Color.FromArgb(36, 33, 56);
            this.txtSchoolId.BorderStyle = BorderStyle.FixedSingle;
            this.txtSchoolId.CharacterCasing = CharacterCasing.Upper;
            this.txtSchoolId.Font = new Font("Consolas", 10F, FontStyle.Bold);
            this.txtSchoolId.ForeColor = Color.FromArgb(72, 232, 214);
            this.txtSchoolId.Location = new Point(16, 62);
            this.txtSchoolId.PlaceholderText = "2024-10005";
            this.txtSchoolId.Size = new Size(436, 26);

            // Full Name
            this.lblName.AutoSize = true;
            this.lblName.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblName.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblName.Location = new Point(16, 96);
            this.lblName.Text = "FULL NAME";

            this.txtName.BackColor = Color.FromArgb(36, 33, 56);
            this.txtName.BorderStyle = BorderStyle.FixedSingle;
            this.txtName.Font = new Font("Segoe UI", 9.75F);
            this.txtName.ForeColor = Color.FromArgb(245, 244, 252);
            this.txtName.Location = new Point(16, 116);
            this.txtName.PlaceholderText = "e.g., Jennifer Connelly";
            this.txtName.Size = new Size(436, 26);

            // Role
            this.lblRole.AutoSize = true;
            this.lblRole.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblRole.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblRole.Location = new Point(16, 150);
            this.lblRole.Text = "ACADEMIC / CAMPUS ROLE";

            this.cmbRole.BackColor = Color.FromArgb(36, 33, 56);
            this.cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbRole.FlatStyle = FlatStyle.Flat;
            this.cmbRole.Font = new Font("Segoe UI", 9.75F);
            this.cmbRole.ForeColor = Color.FromArgb(245, 244, 252);
            this.cmbRole.Items.AddRange(new object[] { "Student", "Faculty", "Staff", "Researcher" });
            this.cmbRole.Location = new Point(16, 170);
            this.cmbRole.SelectedIndex = 0;
            this.cmbRole.Size = new Size(436, 26);

            // Department
            this.lblDepartment.AutoSize = true;
            this.lblDepartment.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblDepartment.ForeColor = Color.FromArgb(168, 164, 194);
            this.lblDepartment.Location = new Point(16, 204);
            this.lblDepartment.Text = "DEPARTMENT / FACULTY UNIT";

            this.txtDepartment.BackColor = Color.FromArgb(36, 33, 56);
            this.txtDepartment.BorderStyle = BorderStyle.FixedSingle;
            this.txtDepartment.Font = new Font("Segoe UI", 9.75F);
            this.txtDepartment.ForeColor = Color.FromArgb(245, 244, 252);
            this.txtDepartment.Location = new Point(16, 224);
            this.txtDepartment.PlaceholderText = "e.g., Computer Science, Biology, Physics";
            this.txtDepartment.Size = new Size(436, 26);

            // Clearance Hold Checkbox
            this.chkHold.AutoSize = true;
            this.chkHold.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.chkHold.ForeColor = Color.FromArgb(255, 68, 96);
            this.chkHold.Location = new Point(16, 260);
            this.chkHold.Text = "PLACE UNDER ADMINISTRATIVE CLEARANCE HOLD";

            // Alert Label
            this.lblInlineAlert.Font = new Font("Bahnschrift SemiBold", 9F, FontStyle.Bold);
            this.lblInlineAlert.ForeColor = Color.FromArgb(255, 68, 92);
            this.lblInlineAlert.Location = new Point(16, 292);
            this.lblInlineAlert.Size = new Size(436, 22);
            this.lblInlineAlert.Text = "";
            this.lblInlineAlert.Visible = false;

            // ==================== ACTION BUTTONS ====================
            this.btnSave.AccentColor = Color.FromArgb(72, 232, 214);
            this.btnSave.Location = new Point(16, 430);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(310, 42);
            this.btnSave.Text = "+ REGISTER BORROWER";
            this.btnSave.Click += new EventHandler(this.BtnSave_Click);

            this.btnCancel.AccentColor = Color.FromArgb(255, 68, 92);
            this.btnCancel.Location = new Point(334, 430);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(150, 42);
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.Click += new EventHandler(this.BtnCancel_Click);

            // ==================== FORM ====================
            this.AcceptButton = this.btnSave;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.FromArgb(16, 15, 26);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new Size(500, 488);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cardMain);
            this.Controls.Add(this.headerBar);
            this.Font = new Font("Segoe UI", 9.5F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Borrower Management";

            this.cardMain.ResumeLayout(false);
            this.cardMain.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
