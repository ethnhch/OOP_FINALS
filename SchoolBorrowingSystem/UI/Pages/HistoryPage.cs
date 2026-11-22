using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Transactions;

namespace SchoolBorrowingSystem.UI.Pages
{
    /// <summary>
    /// Every completed return, newest first, with the fine that was charged.
    /// </summary>
    public class HistoryPage : BasePage
    {
        private TextBox _searchBox;
        private Label _summaryLabel;
        private DataGridView _grid;
        private Label _emptyLabel;

        public HistoryPage(EquipmentController equipment, BorrowingController borrowing, BorrowerController borrowers)
            : base("Return History", "All items that have been returned, newest first.", equipment, borrowing, borrowers)
        {
            BuildLayout();
        }

        private void BuildLayout()
        {
            FlowLayoutPanel toolbar = MakeToolbar();

            _searchBox = Theme.MakeSearchBox("Search by slip, borrower or item...");
            _searchBox.TextChanged += delegate { RefreshData(); };

            _summaryLabel = Theme.MakeLabel("", Theme.BoldFont, Theme.TextMuted);
            _summaryLabel.Margin = new Padding(8, 10, 0, 0);

            toolbar.Controls.Add(_searchBox);
            toolbar.Controls.Add(_summaryLabel);

            CardPanel card = new CardPanel();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(0);
            card.Padding = new Padding(1);

            _grid = new DataGridView();
            Theme.StyleGrid(_grid);
            Theme.AddColumn(_grid, "Slip", "SLIP", 70);
            Theme.AddColumn(_grid, "Borrower", "BORROWER", 120);
            Theme.AddColumn(_grid, "Equipment", "EQUIPMENT", 160);
            Theme.AddColumn(_grid, "Borrowed", "BORROWED", 110);
            Theme.AddColumn(_grid, "Returned", "RETURNED", 110);
            Theme.AddColumn(_grid, "OnTime", "ON TIME?", 70);
            Theme.AddColumn(_grid, "Condition", "CONDITION", 90);
            Theme.AddColumn(_grid, "Fine", "FINE", 90);

            _emptyLabel = Theme.MakeLabel("No returns yet. Completed returns will show up here.", Theme.NormalFont, Theme.TextMuted);
            _emptyLabel.Dock = DockStyle.Top;
            _emptyLabel.Padding = new Padding(16);

            card.Controls.Add(_grid);
            card.Controls.Add(_emptyLabel);

            ContentArea.Controls.Add(card);
            ContentArea.Controls.Add(toolbar);
        }

        public override void RefreshData()
        {
            List<BorrowSlip> history = Borrowing.GetHistoricalSlips();
            history.Reverse(); // newest first

            string search = _searchBox.Text.Trim().ToLower();

            _grid.Rows.Clear();
            foreach (BorrowSlip slip in history)
            {
                bool matches = search.Length == 0
                    || slip.SlipID.ToLower().Contains(search)
                    || slip.Borrower.FullName.ToLower().Contains(search)
                    || slip.Equipment.DisplayName.ToLower().Contains(search);
                if (!matches)
                {
                    continue;
                }

                int rowIndex = _grid.Rows.Add(
                    slip.SlipID,
                    slip.Borrower.FullName,
                    slip.Equipment.DisplayName,
                    Theme.FormatFriendlyDate(slip.TimeBorrowed),
                    Theme.FormatFriendlyDate(slip.TimeReturned.Value),
                    "",
                    slip.ReturnCondition + (slip.ChecklistComplete ? "" : " (incomplete)"),
                    Theme.FormatMoney(slip.PenaltyCharged));

                DataGridViewRow row = _grid.Rows[rowIndex];
                row.Tag = slip;
                Theme.PaintBadge(row.Cells["OnTime"], slip.IsOverdue() ? "Late" : "On time");

                if (slip.PenaltyCharged > 0)
                {
                    row.Cells["Fine"].Style.ForeColor = Theme.Danger;
                }
            }
            _grid.ClearSelection();

            _summaryLabel.Text = history.Count + " return(s)  ·  " + Theme.FormatMoney(Borrowing.GetTotalFinesCollected()) + " in fines";
            _grid.Visible = history.Count > 0;
            _emptyLabel.Visible = history.Count == 0;
        }
    }
}
