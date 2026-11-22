using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.UI.Dialogs;

namespace SchoolBorrowingSystem.UI.Pages
{
    /// <summary>
    /// List of all equipment with search, filter, repairs, and save/load.
    /// </summary>
    public class InventoryPage : BasePage
    {
        private TextBox _searchBox;
        private ComboBox _filterBox;
        private Button _repairButton;
        private Button _fixedButton;
        private DataGridView _grid;
        private Label _countLabel;

        public InventoryPage(EquipmentController equipment, BorrowingController borrowing, BorrowerController borrowers)
            : base("Inventory", "Every item the school can lend. Select a row to send it to repair or bring it back.", equipment, borrowing, borrowers)
        {
            BuildLayout();
        }

        private void BuildLayout()
        {
            // ----- Top row: search, filter, add -----
            FlowLayoutPanel searchRow = MakeToolbar();

            _searchBox = Theme.MakeSearchBox("Search by tag or name...");
            _searchBox.TextChanged += delegate { RefreshData(); };

            _filterBox = Theme.MakeDropDown(170);
            _filterBox.Items.Add("All items");
            _filterBox.Items.Add("Available");
            _filterBox.Items.Add("On Loan");
            _filterBox.Items.Add("Needs attention");
            _filterBox.SelectedIndex = 0;
            _filterBox.SelectedIndexChanged += delegate { RefreshData(); };

            Button addButton = Theme.MakeButton("+  Add Equipment", Theme.Primary);
            addButton.Click += AddButton_Click;

            searchRow.Controls.Add(_searchBox);
            searchRow.Controls.Add(_filterBox);
            searchRow.Controls.Add(addButton);

            // ----- Second row: actions for the selected item -----
            FlowLayoutPanel actionRow = MakeToolbar();

            _repairButton = Theme.MakeButton("Send to Repair", Theme.Warning);
            _repairButton.Click += RepairButton_Click;

            _fixedButton = Theme.MakeButton("Mark as Fixed / Available", Theme.Success);
            _fixedButton.Click += FixedButton_Click;

            Button saveButton = Theme.MakeSecondaryButton("Save to File");
            saveButton.Click += SaveButton_Click;

            Button loadButton = Theme.MakeSecondaryButton("Load from File");
            loadButton.Click += LoadButton_Click;

            actionRow.Controls.Add(_repairButton);
            actionRow.Controls.Add(_fixedButton);
            actionRow.Controls.Add(saveButton);
            actionRow.Controls.Add(loadButton);

            // ----- Table -----
            CardPanel card = new CardPanel();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(0);
            card.Padding = new Padding(1);

            _grid = new DataGridView();
            Theme.StyleGrid(_grid);
            Theme.AddColumn(_grid, "Tag", "TAG", 70);
            Theme.AddColumn(_grid, "Name", "NAME", 180);
            Theme.AddColumn(_grid, "Category", "CATEGORY", 90);
            Theme.AddColumn(_grid, "Loan", "LOAN PERIOD", 80);
            Theme.AddColumn(_grid, "Condition", "CONDITION", 90);
            Theme.AddColumn(_grid, "Status", "STATUS", 90);
            Theme.AddColumn(_grid, "Details", "DETAILS", 170);
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
        /// Narrow window: hide the columns people need least, and make the search box smaller.
        /// Tag, Name, Condition and Status always stay visible.
        /// </summary>
        protected override void ApplyLayout(bool isNarrow)
        {
            Theme.SetColumnsVisible(_grid, !isNarrow, "Category", "Loan", "Details");
            _searchBox.Width = Theme.Scale(isNarrow ? 200 : 260);
        }

        public override void RefreshData()
        {
            // Remember which item was selected so the selection survives a refresh
            SchoolEquipment previouslySelected = GetSelectedRowObject(_grid) as SchoolEquipment;

            string search = _searchBox.Text.Trim().ToLower();
            string filter = _filterBox.SelectedItem.ToString();

            List<SchoolEquipment> allItems = Equipment.GetAllEquipment();
            int shownCount = 0;

            _grid.Rows.Clear();
            foreach (SchoolEquipment item in allItems)
            {
                string status = GetEquipmentStatus(item);

                if (!MatchesSearch(item, search) || !MatchesFilter(status, filter))
                {
                    continue;
                }

                int rowIndex = _grid.Rows.Add(
                    item.PropertyTag,
                    item.ItemName,
                    item.GetCategoryName(),
                    item.GetLoanPeriodText(),
                    item.Condition,
                    "",
                    item.GetDetailsText());

                DataGridViewRow row = _grid.Rows[rowIndex];
                row.Tag = item;
                Theme.PaintBadge(row.Cells["Status"], status);

                if (item == previouslySelected)
                {
                    row.Selected = true;
                }
                shownCount++;
            }

            if (previouslySelected == null)
            {
                _grid.ClearSelection();
            }

            _countLabel.Text = "Showing " + shownCount + " of " + allItems.Count + " items";
            UpdateButtons();
        }

        private bool MatchesSearch(SchoolEquipment item, string search)
        {
            if (search.Length == 0)
            {
                return true;
            }
            return item.PropertyTag.ToLower().Contains(search) || item.ItemName.ToLower().Contains(search);
        }

        private bool MatchesFilter(string status, string filter)
        {
            if (filter == "Available")
            {
                return status == "Available";
            }
            if (filter == "On Loan")
            {
                return status == "On Loan";
            }
            if (filter == "Needs attention")
            {
                return status == "Under Repair" || status == "Missing Parts" || status == "Unavailable";
            }
            return true; // "All items"
        }

        /// <summary>
        /// Only enable the buttons that make sense for the selected item.
        /// This is better than letting people click and then showing an error.
        /// </summary>
        private void UpdateButtons()
        {
            SchoolEquipment item = GetSelectedRowObject(_grid) as SchoolEquipment;

            if (item == null)
            {
                _repairButton.Enabled = false;
                _fixedButton.Enabled = false;
                return;
            }

            string status = GetEquipmentStatus(item);
            _repairButton.Enabled = status == "Available";
            _fixedButton.Enabled = status == "Under Repair" || status == "Missing Parts" || status == "Unavailable";
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            using (AddEquipmentForm dialog = new AddEquipmentForm(Equipment))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    AudioHelper.PlaySuccess();
                    ShowStatus("Added " + dialog.CreatedItem.DisplayName + " to the inventory.", false);
                    RefreshData();
                }
            }
        }

        private void RepairButton_Click(object sender, EventArgs e)
        {
            SchoolEquipment item = GetSelectedRowObject(_grid) as SchoolEquipment;
            if (item == null)
            {
                return;
            }

            if (!AskYesNo("Send " + item.DisplayName + " to repair?\n\nIt won't be available for borrowing until you mark it as fixed.", "Send to Repair"))
            {
                return;
            }

            Equipment.MarkForRepair(item.PropertyTag);
            AudioHelper.PlayWarning();
            ShowStatus(item.DisplayName + " was sent to repair.", true);
            RefreshData();
        }

        private void FixedButton_Click(object sender, EventArgs e)
        {
            SchoolEquipment item = GetSelectedRowObject(_grid) as SchoolEquipment;
            if (item == null)
            {
                return;
            }

            Equipment.RestoreToService(item.PropertyTag);
            AudioHelper.PlaySuccess();
            ShowStatus(item.DisplayName + " is back in Good condition and available.", false);
            RefreshData();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                Equipment.SaveInventory();
                AudioHelper.PlaySuccess();
                ShowStatus("Inventory saved to " + Equipment.StorageFilePath + ".", false);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, "Could not save");
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            // Loading replaces every item with a new object. Open slips would then point
            // to items that are no longer in the list, so we don't allow it during loans.
            if (Borrowing.GetActiveSlips().Count > 0)
            {
                ShowError("Please process all returns before loading from the file.\nItems that are on loan would lose their borrow slips.", "Items still on loan");
                return;
            }

            if (!System.IO.File.Exists(Equipment.StorageFilePath))
            {
                ShowError("There is no saved file yet. Click 'Save to File' first.", "Nothing to load");
                return;
            }

            if (!AskYesNo("Replace the current inventory with the saved file?", "Load from File"))
            {
                return;
            }

            try
            {
                Equipment.LoadInventory();
                AudioHelper.PlaySuccess();
                ShowStatus("Inventory loaded from " + Equipment.StorageFilePath + ".", false);
                RefreshData();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, "Could not load");
            }
        }
    }
}
