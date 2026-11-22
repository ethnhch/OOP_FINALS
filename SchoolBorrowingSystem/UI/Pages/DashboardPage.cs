using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.Transactions;

namespace SchoolBorrowingSystem.UI.Pages
{
    /// <summary>
    /// The first screen: a quick summary of everything plus shortcuts to the common tasks.
    /// </summary>
    public class DashboardPage : BasePage
    {
        private Label _totalNumber;
        private Label _totalNote;
        private Label _availableNumber;
        private Label _availableNote;
        private Label _onLoanNumber;
        private Label _onLoanNote;
        private Label _holdNumber;
        private Label _holdNote;

        private TableLayoutPanel _cardsTable;
        private List<CardPanel> _statCards = new List<CardPanel>();

        private DataGridView _loansGrid;
        private Label _noLoansLabel;

        public DashboardPage(EquipmentController equipment, BorrowingController borrowing, BorrowerController borrowers)
            : base("Dashboard", "Here's what is happening in the equipment office today.", equipment, borrowing, borrowers)
        {
            BuildLayout();
        }

        private void BuildLayout()
        {
            // ----- 4 summary cards (placed in ApplyLayout: 4 in a row, or 2 x 2 when narrow) -----
            _cardsTable = new TableLayoutPanel();
            _cardsTable.Dock = DockStyle.Top;

            _statCards.Add(MakeStatCard("TOTAL EQUIPMENT", Theme.Primary, out _totalNumber, out _totalNote));
            _statCards.Add(MakeStatCard("AVAILABLE NOW", Theme.Success, out _availableNumber, out _availableNote));
            _statCards.Add(MakeStatCard("ON LOAN", Theme.Warning, out _onLoanNumber, out _onLoanNote));
            _statCards.Add(MakeStatCard("BORROWERS ON HOLD", Theme.Danger, out _holdNumber, out _holdNote));

            foreach (CardPanel card in _statCards)
            {
                _cardsTable.Controls.Add(card);
            }

            // ----- Quick action buttons -----
            FlowLayoutPanel actions = MakeToolbar();
            actions.Padding = Theme.ScalePadding(0, 0, 0, 8);

            Button borrowButton = Theme.MakeButton("+  New Borrow", Theme.Primary);
            borrowButton.Click += delegate { GoToPage("Borrow"); };

            Button returnButton = Theme.MakeButton("Process a Return", Theme.Success);
            returnButton.Click += delegate { GoToPage("Return"); };

            Button inventoryButton = Theme.MakeSecondaryButton("View Inventory");
            inventoryButton.Click += delegate { GoToPage("Inventory"); };

            actions.Controls.Add(borrowButton);
            actions.Controls.Add(returnButton);
            actions.Controls.Add(inventoryButton);

            // ----- Card with the loans that need attention -----
            CardPanel loansCard = new CardPanel();
            loansCard.Dock = DockStyle.Fill;
            loansCard.Margin = new Padding(0);
            loansCard.Padding = Theme.ScalePadding(16, 16, 16, 16);

            Label loansTitle = Theme.MakeLabel("Current loans (overdue and due soonest first)", Theme.SectionFont, Theme.TextDark);
            loansTitle.Dock = DockStyle.Top;
            loansTitle.Padding = Theme.ScalePadding(0, 0, 0, 10);

            _loansGrid = new DataGridView();
            Theme.StyleGrid(_loansGrid);
            Theme.AddColumn(_loansGrid, "Slip", "SLIP", 70);
            Theme.AddColumn(_loansGrid, "Borrower", "BORROWER", 130);
            Theme.AddColumn(_loansGrid, "Equipment", "EQUIPMENT", 170);
            Theme.AddColumn(_loansGrid, "Room", "ROOM", 110);
            Theme.AddColumn(_loansGrid, "Due", "DUE BACK", 110);
            Theme.AddColumn(_loansGrid, "Status", "STATUS", 110);

            _noLoansLabel = Theme.MakeLabel("Nothing is on loan right now. Everything is back on the shelf!", Theme.NormalFont, Theme.TextMuted);
            _noLoansLabel.Dock = DockStyle.Top;
            _noLoansLabel.Padding = Theme.ScalePadding(0, 12, 0, 0);

            loansCard.Controls.Add(_loansGrid);
            loansCard.Controls.Add(_noLoansLabel);
            loansCard.Controls.Add(loansTitle);

            // Fill first, then the Top items from bottom to top (see note in BasePage)
            ContentArea.Controls.Add(loansCard);
            ContentArea.Controls.Add(actions);
            ContentArea.Controls.Add(_cardsTable);
        }

        /// <summary>
        /// Wide: 4 cards in one row. Narrow: 2 rows of 2 cards.
        /// Also hides the less important table columns when narrow.
        /// </summary>
        protected override void ApplyLayout(bool isNarrow)
        {
            int columns = isNarrow ? 2 : 4;
            int rows = isNarrow ? 2 : 1;
            int cardHeight = Theme.Scale(124);
            int gap = Theme.Scale(16);

            _cardsTable.ColumnStyles.Clear();
            _cardsTable.RowStyles.Clear();
            _cardsTable.ColumnCount = columns;
            _cardsTable.RowCount = rows;

            for (int c = 0; c < columns; c++)
            {
                _cardsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / columns));
            }
            for (int r = 0; r < rows; r++)
            {
                _cardsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, cardHeight + gap));
            }

            for (int i = 0; i < _statCards.Count; i++)
            {
                int column = i % columns;
                int row = i / columns;
                _cardsTable.SetCellPosition(_statCards[i], new TableLayoutPanelCellPosition(column, row));

                // A gap to the right of every card except the last one in its row
                bool lastInRow = column == columns - 1;
                _statCards[i].Margin = new Padding(0, 0, lastInRow ? 0 : gap, gap);
            }

            _cardsTable.Height = rows * (cardHeight + gap);

            Theme.SetColumnsVisible(_loansGrid, !isNarrow, "Slip", "Room");
        }

        /// <summary>
        /// Builds one summary card: a small colored caption, a big number and a note.
        /// The number and note labels are given back with "out" so we can update them later.
        /// </summary>
        private CardPanel MakeStatCard(string caption, Color accent, out Label numberLabel, out Label noteLabel)
        {
            CardPanel card = new CardPanel();
            card.Dock = DockStyle.Fill;
            card.Padding = Theme.ScalePadding(18, 14, 12, 10);

            // Caption, number and note stacked in a one-column table.
            // Each row is as tall as its text, so nothing overlaps at 125% / 150% screen scaling.
            TableLayoutPanel stack = new TableLayoutPanel();
            stack.Dock = DockStyle.Fill;
            stack.ColumnCount = 1;
            stack.RowCount = 3;
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Label captionLabel = Theme.MakeLabel(caption, new Font(Theme.FontName, 9F, FontStyle.Bold), accent);
            captionLabel.AutoEllipsis = true;
            captionLabel.Margin = new Padding(0);

            numberLabel = Theme.MakeLabel("0", Theme.BigNumberFont, Theme.TextDark);
            numberLabel.Margin = new Padding(0);

            noteLabel = Theme.MakeLabel("", Theme.SmallFont, Theme.TextMuted);
            noteLabel.Margin = new Padding(0);

            stack.Controls.Add(captionLabel, 0, 0);
            stack.Controls.Add(numberLabel, 0, 1);
            stack.Controls.Add(noteLabel, 0, 2);
            card.Controls.Add(stack);
            return card;
        }

        public override void RefreshData()
        {
            // ----- Count the equipment -----
            List<SchoolEquipment> allItems = Equipment.GetAllEquipment();
            int avCount = 0;
            int labCount = 0;
            int availableCount = 0;
            int needsAttention = 0;

            foreach (SchoolEquipment item in allItems)
            {
                if (item is AVEquipment)
                {
                    avCount++;
                }
                else
                {
                    labCount++;
                }

                string status = GetEquipmentStatus(item);
                if (status == "Available")
                {
                    availableCount++;
                }
                else if (status == "Under Repair" || status == "Missing Parts")
                {
                    needsAttention++;
                }
            }

            List<BorrowSlip> activeSlips = Borrowing.GetActiveSlips();
            int overdueCount = Borrowing.GetOverdueSlips().Count;

            _totalNumber.Text = allItems.Count.ToString();
            _totalNote.Text = avCount + " AV  ·  " + labCount + " Lab";

            _availableNumber.Text = availableCount.ToString();
            _availableNote.Text = needsAttention > 0 ? needsAttention + " in repair / missing parts" : "Ready to lend";

            _onLoanNumber.Text = activeSlips.Count.ToString();
            if (overdueCount > 0)
            {
                _onLoanNote.Text = overdueCount + " overdue!";
                _onLoanNote.ForeColor = Theme.Danger;
            }
            else
            {
                _onLoanNote.Text = "None overdue";
                _onLoanNote.ForeColor = Theme.TextMuted;
            }

            _holdNumber.Text = Borrowers.CountBorrowersOnHold().ToString();
            _holdNote.Text = "of " + Borrowers.GetAllBorrowers().Count + " registered";

            // ----- Fill the loans table, most urgent first -----
            activeSlips.Sort(CompareByDueTime);

            _loansGrid.Rows.Clear();
            foreach (BorrowSlip slip in activeSlips)
            {
                int rowIndex = _loansGrid.Rows.Add(
                    slip.SlipID,
                    slip.Borrower.FullName,
                    slip.Equipment.DisplayName,
                    slip.RoomToBeUsed,
                    Theme.FormatFriendlyDate(slip.DueTime),
                    "");

                DataGridViewRow row = _loansGrid.Rows[rowIndex];
                row.Tag = slip;
                Theme.PaintBadge(row.Cells["Status"], GetLoanStatus(slip));
            }
            _loansGrid.ClearSelection();

            // Show a friendly message instead of an empty table
            _loansGrid.Visible = activeSlips.Count > 0;
            _noLoansLabel.Visible = activeSlips.Count == 0;
        }

        private static int CompareByDueTime(BorrowSlip a, BorrowSlip b)
        {
            return a.DueTime.CompareTo(b.DueTime);
        }
    }
}
