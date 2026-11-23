using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SchoolBorrowingSystem.Models;

namespace SchoolBorrowingSystem.UI
{
    public partial class BorrowerForm : Form
    {
        private readonly IList<SchoolBorrower> _borrowers;
        private readonly SchoolBorrower _existingBorrower;

        public SchoolBorrower ResultBorrower { get; private set; }

        public BorrowerForm()
        {
            InitializeComponent();
        }

        public BorrowerForm(IList<SchoolBorrower> borrowers, SchoolBorrower existingBorrower = null)
            : this()
        {
            _borrowers = borrowers;
            _existingBorrower = existingBorrower;

            if (_existingBorrower != null)
            {
                headerBar.TitleText = "EDIT BORROWER PROFILE";
                headerBar.SubtitleText = "MODIFY IDENTITY & CLEARANCE  //  " + _existingBorrower.SchoolIDNumber;
                btnSave.Text = "SAVE CHANGES";
                btnSave.AccentColor = Color.FromArgb(255, 196, 134);

                txtSchoolId.Text = _existingBorrower.SchoolIDNumber;
                txtSchoolId.ReadOnly = true;
                txtSchoolId.BackColor = Color.FromArgb(24, 22, 38);

                txtName.Text = _existingBorrower.FullName;
                txtDepartment.Text = _existingBorrower.Department;
                chkHold.Checked = _existingBorrower.HasClearanceHold;

                for (int i = 0; i < cmbRole.Items.Count; i++)
                {
                    if (string.Equals(cmbRole.Items[i].ToString(), _existingBorrower.Role, StringComparison.OrdinalIgnoreCase))
                    {
                        cmbRole.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            GtaPalette.EnableDarkTitleBar(this);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string id = txtSchoolId.Text.Trim();
            string name = txtName.Text.Trim();
            string dept = txtDepartment.Text.Trim();
            string role = cmbRole.SelectedItem != null ? cmbRole.SelectedItem.ToString() : "Student";
            bool hold = chkHold.Checked;

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
            {
                AudioHelper.PlayError();
                lblInlineAlert.Text = "ALERT  //  ENTER BOTH SCHOOL ID NUMBER AND FULL NAME.";
                lblInlineAlert.Visible = true;
                return;
            }

            if (string.IsNullOrEmpty(dept))
            {
                dept = "General Studies";
            }

            if (_existingBorrower == null && _borrowers != null)
            {
                foreach (var b in _borrowers)
                {
                    if (string.Equals(b.SchoolIDNumber, id, StringComparison.OrdinalIgnoreCase))
                    {
                        AudioHelper.PlayError();
                        lblInlineAlert.Text = string.Format("ALERT  //  BORROWER ID '{0}' IS ALREADY REGISTERED.", id);
                        lblInlineAlert.Visible = true;
                        return;
                    }
                }
            }

            if (_existingBorrower != null)
            {
                _existingBorrower.FullName = name;
                _existingBorrower.Role = role;
                _existingBorrower.Department = dept;
                _existingBorrower.HasClearanceHold = hold;
                ResultBorrower = _existingBorrower;
            }
            else
            {
                ResultBorrower = new SchoolBorrower(id, name, role, dept, hold);
            }

            AudioHelper.PlaySuccess();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            AudioHelper.PlayMenuClick();
            this.Close();
        }
    }
}
