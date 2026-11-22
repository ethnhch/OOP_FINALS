using System;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.UI.Dialogs;

namespace SchoolBorrowingSystem.UI.Pages
{
    /// <summary>
    /// List of students / faculty / staff. Register new borrowers and manage clearance holds.
    /// </summary>
    public class BorrowersPage : BasePage
    {
        private TextBox _searchBox;
        private ComboBox _filterBox;
        private Button _holdButton;
        private Button _liftButton;
        private DataGridView _grid;
        private Label _countLabel;

        public BorrowersPage(EquipmentController equipment, BorrowingController borrowing, BorrowerController borrowers)
            : base("Borrowers", "Registered students, faculty and staff. A clearance hold blocks someone from borrowing.", equipment, borrowing, borrowers)
        {
            BuildLayout();
        }

        private void BuildLayout()
        {
            FlowLayoutPanel searchRow = MakeToolbar();

            _searchBox = Theme.MakeSearchBox("Search by ID, name or department...");
            _searchBox.TextChanged += delegate { RefreshData(); };

            _filterBox = Theme.MakeDropDown(150);
            _filterBox.Items.Add("Everyone");
            _filterBox.Items.Add("On hold");
            _filterBox.Items.Add("Clear");
            _filterBox.SelectedIndex = 0;
            _filterBox.SelectedIndexChanged += delegate { RefreshData(); };

            Button addButton = Theme.MakeButton("+  Add Borrower", Theme.Primary);
            addButton.Click += AddButton_Click;

            searchRow.Controls.Add(_searchBox);
            searchRow.Controls.Add(_filterBox);
            searchRow.Controls.Add(addButton);

            FlowLayoutPanel actionRow = MakeToolbar();

            _holdButton = Theme.MakeButton("Place on Hold", Theme.Danger);
            _holdButton.Click += HoldButton_Click;

            _liftButton = Theme.MakeButton("Lift Hold", Theme.Success);
            _liftButton.Click += LiftButton_Click;

            actionRow.Controls.Add(_holdButton);
            actionRow.Controls.Add(_liftButton);

            CardPanel card = new CardPanel();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(0);
            card.Padding = new Padding(1);

            _grid = new DataGridView();
            Theme.StyleGrid(_grid);
            Theme.AddColumn(_grid, "ID", "SCHOOL ID", 90);
            Theme.AddColumn(_grid, "Name", "FULL NAME", 160);
            Theme.AddColumn(_grid, "Role", "ROLE", 70);
            Theme.AddColumn(_grid, "Department", "DEPARTMENT", 130);
            Theme.AddColumn(_grid, "Loans", "ITEMS BORROWED", 80);
            Theme.AddColumn(_grid, "Status", "CLEARANCE", 80);
            _grid.SelectionChanged += delegate { UpdateButtons(); };
            card.Controls.Add(_grid);

            _countLabel = Theme.MakeLabel("", Theme.SmallFont, Theme.TextMuted);
            _countLabel.Dock = DockStyle.Bottom;
            _countLabel.Padding = Theme.ScalePadding(0, 8, 0, 0);

            ContentArea.Controls.Add(card);
            ContentArea.Controls.Add(_countLabel);
            ContentArea.Controls.Add(actionRow);
            ContentArea.Controls.Add(searchRow);
        }

        /// <summary>
        /// Narrow window: hide Role and Department so names and clearance status stay readable.
        /// </summary>
        protected override void ApplyLayout(bool isNarrow)
        {
            Theme.SetColumnsVisible(_grid, !isNarrow, "Role", "Department");
            _searchBox.Width = Theme.Scale(isNarrow ? 200 : 260);
        }

        public override void RefreshData()
        {
            SchoolBorrower previouslySelected = GetSelectedRowObject(_grid) as SchoolBorrower;
            string search = _searchBox.Text.Trim().ToLower();
            string filter = _filterBox.SelectedItem.ToString();
            int shownCount = 0;
            int totalCount = 0;

            _grid.Rows.Clear();
            foreach (SchoolBorrower borrower in Borrowers.GetAllBorrowers())
            {
                totalCount++;

                bool matchesSearch = search.Length == 0
                    || borrower.SchoolIDNumber.ToLower().Contains(search)
                    || borrower.FullName.ToLower().Contains(search)
                    || borrower.Department.ToLower().Contains(search);

                bool matchesFilter = filter == "Everyone"
                    || (filter == "On hold" && borrower.HasClearanceHold)
                    || (filter == "Clear" && !borrower.HasClearanceHold);

                if (!matchesSearch || !matchesFilter)
                {
                    continue;
                }

                int rowIndex = _grid.Rows.Add(
                    borrower.SchoolIDNumber,
                    borrower.FullName,
                    borrower.Role,
                    borrower.Department,
                    Borrowing.CountActiveLoans(borrower),
                    "");

                DataGridViewRow row = _grid.Rows[rowIndex];
                row.Tag = borrower;
                Theme.PaintBadge(row.Cells["Status"], borrower.HasClearanceHold ? "On Hold" : "Clear");

                if (borrower == previouslySelected)
                {
                    row.Selected = true;
                }
                shownCount++;
            }

            if (previouslySelected == null)
            {
                _grid.ClearSelection();
            }

            _countLabel.Text = "Showing " + shownCount + " of " + totalCount + " borrowers";
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            SchoolBorrower borrower = GetSelectedRowObject(_grid) as SchoolBorrower;
            _holdButton.Enabled = borrower != null && !borrower.HasClearanceHold;
            _liftButton.Enabled = borrower != null && borrower.HasClearanceHold;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            using (AddBorrowerForm dialog = new AddBorrowerForm(Borrowers))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    AudioHelper.PlaySuccess();
                    ShowStatus(dialog.CreatedBorrower.FullName + " was registered as a borrower.", false);
                    RefreshData();
                }
            }
        }

        private void HoldButton_Click(object sender, EventArgs e)
        {
            SchoolBorrower borrower = GetSelectedRowObject(_grid) as SchoolBorrower;
            if (borrower == null)
            {
                return;
            }

            if (!AskYesNo("Place " + borrower.FullName + " on clearance hold?\n\nThey won't be able to borrow until the hold is lifted.", "Place on Hold"))
            {
                return;
            }

            Borrowing.ApplyClearanceHold(borrower);
            AudioHelper.PlayWarning();
            ShowStatus(borrower.FullName + " is now on clearance hold.", true);
            RefreshData();
        }

        private void LiftButton_Click(object sender, EventArgs e)
        {
            SchoolBorrower borrower = GetSelectedRowObject(_grid) as SchoolBorrower;
            if (borrower == null)
            {
                return;
            }

            if (!AskYesNo("Lift the clearance hold for " + borrower.FullName + "?\n\nMake sure any fines have been paid first.", "Lift Hold"))
            {
                return;
            }

            Borrowing.LiftClearanceHold(borrower);
            AudioHelper.PlaySuccess();
            ShowStatus("Clearance hold lifted for " + borrower.FullName + ". They can borrow again.", false);
            RefreshData();
        }
    }
}
