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

        private DataGridView _loansGrid;
        private Label _noLoansLabel;

        public DashboardPage(EquipmentController equipment, BorrowingController borrowing, BorrowerController borrowers)
            : base("Dashboard", "Here's what is happening in the equipment office today.", equipment, borrowing, borrowers)
        {
            BuildLayout();
        }

        private void BuildLayout()
        {
            // ----- Row of 4 summary cards -----
            TableLayoutPanel cards = new TableLayoutPanel();
            cards.Dock = DockStyle.Top;
            cards.Height = 136;
            cards.ColumnCount = 4;
            for (int i = 0; i < 4; i++)
            {
                cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            }

            cards.Controls.Add(MakeStatCard("TOTAL EQUIPMENT", Theme.Primary, out _totalNumber, out _totalNote), 0, 0);
            cards.Controls.Add(MakeStatCard("AVAILABLE NOW", Theme.Success, out _availableNumber, out _availableNote), 1, 0);
            cards.Controls.Add(MakeStatCard("ON LOAN", Theme.Warning, out _onLoanNumber, out _onLoanNote), 2, 0);
            cards.Controls.Add(MakeStatCard("BORROWERS ON HOLD", Theme.Danger, out _holdNumber, out _holdNote), 3, 0);

            // ----- Quick action buttons -----
            FlowLayoutPanel actions = MakeToolbar();
            actions.Padding = new Padding(0, 0, 0, 16);

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

            Label loansTitle = Theme.MakeLabel("Current loans (overdue and due soonest first)", Theme.SectionFont, Theme.TextDark);
            loansTitle.Dock = DockStyle.Top;
            loansTitle.Padding = new Padding(0, 0, 0, 10);

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
            _noLoansLabel.Padding = new Padding(0, 12, 0, 0);

            loansCard.Controls.Add(_loansGrid);
            loansCard.Controls.Add(_noLoansLabel);
            loansCard.Controls.Add(loansTitle);

            // Fill first, then the Top items from bottom to top (see note in BasePage)
            ContentArea.Controls.Add(loansCard);
            ContentArea.Controls.Add(actions);
            ContentArea.Controls.Add(cards);
        }

        /// <summary>
        /// Builds one summary card: a small colored caption, a big number and a note.
        /// The number and note labels are given back with "out" so we can update them later.
        /// </summary>
        private CardPanel MakeStatCard(string caption, Color accent, out Label numberLabel, out Label noteLabel)
        {
            CardPanel card = new CardPanel();
            card.Dock = DockStyle.Fill;
            card.Padding = new Padding(20, 16, 16, 12);

            Label captionLabel = Theme.MakeLabel(caption, new Font(Theme.FontName, 9F, FontStyle.Bold), accent);
            captionLabel.Location = new Point(20, 16);

            numberLabel = Theme.MakeLabel("0", Theme.BigNumberFont, Theme.TextDark);
            numberLabel.Location = new Point(16, 36);

            noteLabel = Theme.MakeLabel("", Theme.SmallFont, Theme.TextMuted);
            noteLabel.Location = new Point(20, 88);

            card.Controls.Add(captionLabel);
            card.Controls.Add(numberLabel);
            card.Controls.Add(noteLabel);
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
