using System;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Models;

namespace SchoolBorrowingSystem.UI.Dialogs
{
    /// <summary>
    /// Pop-up for registering a new student, faculty member or staff member.
    /// </summary>
    public class AddBorrowerForm : BaseDialog
    {
        private readonly BorrowerController _borrowerController;

        private TextBox _idBox;
        private TextBox _nameBox;
        private ComboBox _roleBox;
        private TextBox _departmentBox;

        public SchoolBorrower CreatedBorrower { get; private set; }

        public AddBorrowerForm()
            : this(new BorrowerController())
        {
        }

        public AddBorrowerForm(BorrowerController borrowerController)
            : base("Add Borrower", "Register")
        {
            _borrowerController = borrowerController;

            _idBox = new TextBox();
            _idBox.PlaceholderText = "e.g. 2024-10001";
            AddField("School ID", _idBox);

            _nameBox = new TextBox();
            _nameBox.PlaceholderText = "e.g. Juan Dela Cruz";
            AddField("Full name", _nameBox);

            _roleBox = new ComboBox();
            _roleBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _roleBox.Items.Add("Student");
            _roleBox.Items.Add("Faculty");
            _roleBox.Items.Add("Staff");
            _roleBox.SelectedIndex = 0;
            AddField("Role", _roleBox);

            _departmentBox = new TextBox();
            _departmentBox.PlaceholderText = "e.g. Computer Science";
            AddField("Department", _departmentBox);
        }

        protected override bool TrySave()
        {
            string id = _idBox.Text.Trim();
            string name = _nameBox.Text.Trim();
            string department = _departmentBox.Text.Trim();

            if (id.Length == 0)
            {
                ShowError("Please enter the school ID.");
                _idBox.Focus();
                return false;
            }

            if (name.Length == 0)
            {
                ShowError("Please enter the full name.");
                _nameBox.Focus();
                return false;
            }

            if (department.Length == 0)
            {
                ShowError("Please enter the department.");
                _departmentBox.Focus();
                return false;
            }

            SchoolBorrower borrower = new SchoolBorrower(id, name, _roleBox.SelectedItem.ToString(), department);

            try
            {
                _borrowerController.AddBorrower(borrower);
            }
            catch (Exception ex)
            {
                // e.g. the ID is already registered
                ShowError(ex.Message);
                return false;
            }

            CreatedBorrower = borrower;
            return true;
        }
    }
}
