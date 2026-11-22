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
    /// Screen for checking items back in.
    /// Staff tick each checklist item, choose the condition, and SEE the fine before confirming.
    /// </summary>
    public class ReturnPage : BasePage
    {
        // Containers that ApplyLayout rearranges
        private TableLayoutPanel _rows;
        private CardPanel _loansCard;
        private CardPanel _checkCard;
        private TableLayoutPanel _checkColumns;
        private Panel _checklistColumn;
        private Control _conditionColumn;
        private Panel _fineColumn;

        private DataGridView _grid;
        private Label _noLoansLabel;

        private Panel _checkPanel;
        private Label _selectedLabel;
        private CheckedListBox _checklistBox;
        private ComboBox _conditionBox;
        private Label _lateFineLabel;
        private Label _missingFineLabel;
        private Label _damageFineLabel;
        private Label _totalLabel;
        private Label _holdWarningLabel;
        private Button _confirmButton;

        public ReturnPage(EquipmentController equipment, BorrowingController borrowing, BorrowerController borrowers)
            : base("Process a Return", "Pick the loan, tick off each item that came back, then confirm.", equipment, borrowing, borrowers)
        {
            BuildLayout();
        }

        private void BuildLayout()
        {
            // Top: the loans table. Bottom: the "check the item" card.
            _loansCard = BuildLoansCard();
            _checkCard = BuildCheckCard();

            _rows = new TableLayoutPanel();
            _rows.ColumnCount = 1;
            _rows.RowCount = 2;
            _rows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _rows.Controls.Add(_loansCard, 0, 0);
            _rows.Controls.Add(_checkCard, 0, 1);

            ContentArea.Controls.Add(_rows);
        }

        /// <summary>
        /// Wide:   loans table on top, then  [checklist | condition | fine]  side by side.
        /// Narrow: everything stacked and the page scrolls:
        ///         loans table, then the checklist full width, then [condition | fine].
        /// </summary>
        protected override void ApplyLayout(bool isNarrow)
        {
            _rows.RowStyles.Clear();
            _checkColumns.ColumnStyles.Clear();
            _checkColumns.RowStyles.Clear();

            if (isNarrow)
            {
                int loansHeight = Theme.Scale(280);
                int checkHeight = Theme.Scale(600);
                _rows.RowStyles.Add(new RowStyle(SizeType.Absolute, loansHeight));
                _rows.RowStyles.Add(new RowStyle(SizeType.Absolute, checkHeight));
                _rows.Dock = DockStyle.Top;
                _rows.Height = loansHeight + checkHeight;
                ContentArea.AutoScroll = true;

                _checkColumns.ColumnCount = 2;
                _checkColumns.RowCount = 2;
                _checkColumns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
                _checkColumns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
                _checkColumns.RowStyles.Add(new RowStyle(SizeType.Absolute, Theme.Scale(230)));
                _checkColumns.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                _checkColumns.SetCellPosition(_checklistColumn, new TableLayoutPanelCellPosition(0, 0));
                _checkColumns.SetColumnSpan(_checklistColumn, 2);
                _checkColumns.SetCellPosition(_conditionColumn, new TableLayoutPanelCellPosition(0, 1));
                _checkColumns.SetCellPosition(_fineColumn, new TableLayoutPanelCellPosition(1, 1));
                _checklistColumn.Padding = Theme.ScalePadding(0, 0, 0, 12);
            }
            else
            {
                _rows.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
                _rows.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
                ContentArea.AutoScroll = false;
                _rows.Dock = DockStyle.Fill;

                _checkColumns.ColumnCount = 3;
                _checkColumns.RowCount = 1;
                _checkColumns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
                _checkColumns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
                _checkColumns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
                _checkColumns.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                _checkColumns.SetColumnSpan(_checklistColumn, 1);
                _checkColumns.SetCellPosition(_checklistColumn, new TableLayoutPanelCellPosition(0, 0));
                _checkColumns.SetCellPosition(_conditionColumn, new TableLayoutPanelCellPosition(1, 0));
                _checkColumns.SetCellPosition(_fineColumn, new TableLayoutPanelCellPosition(2, 0));
                _checklistColumn.Padding = Theme.ScalePadding(0, 0, 20, 0);
            }

            Theme.SetColumnsVisible(_grid, !isNarrow, "Slip", "Room");
        }

        private CardPanel BuildLoansCard()
        {
            CardPanel card = new CardPanel();
            card.Dock = DockStyle.Fill;
            card.Margin = Theme.ScalePadding(0, 0, 0, 16);

            Label title = Theme.MakeLabel("Items currently on loan", Theme.SectionFont, Theme.TextDark);
            title.Dock = DockStyle.Top;
            title.Padding = Theme.ScalePadding(0, 0, 0, 10);

            _grid = new DataGridView();
            Theme.StyleGrid(_grid);
            Theme.AddColumn(_grid, "Slip", "SLIP", 70);
            Theme.AddColumn(_grid, "Borrower", "BORROWER", 130);
            Theme.AddColumn(_grid, "Equipment", "EQUIPMENT", 170);
            Theme.AddColumn(_grid, "Room", "ROOM", 110);
            Theme.AddColumn(_grid, "Due", "DUE BACK", 110);
            Theme.AddColumn(_grid, "Status", "STATUS", 110);
            _grid.SelectionChanged += delegate { ShowSelectedSlip(); };

            _noLoansLabel = Theme.MakeLabel("Nothing to return - no items are on loan right now.", Theme.NormalFont, Theme.TextMuted);
            _noLoansLabel.Dock = DockStyle.Top;

            card.Controls.Add(_grid);
            card.Controls.Add(_noLoansLabel);
            card.Controls.Add(title);
            return card;
        }

        private CardPanel BuildCheckCard()
        {
            CardPanel card = new CardPanel();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(0);

            _checkPanel = new Panel();
            _checkPanel.Dock = DockStyle.Fill;

            _selectedLabel = Theme.MakeLabel("Select a loan above.", Theme.SectionFont, Theme.TextDark);
            _selectedLabel.Dock = DockStyle.Top;
            _selectedLabel.Padding = Theme.ScalePadding(0, 0, 0, 10);

            // Checklist, condition and fine summary. ApplyLayout decides where each one goes.
            _checklistColumn = BuildChecklistColumn();
            _conditionColumn = BuildConditionColumn();
            _fineColumn = BuildFineColumn();

            _checkColumns = new TableLayoutPanel();
            _checkColumns.Dock = DockStyle.Fill;
            _checkColumns.Controls.Add(_checklistColumn);
            _checkColumns.Controls.Add(_conditionColumn);
            _checkColumns.Controls.Add(_fineColumn);

            _checkPanel.Controls.Add(_checkColumns);
            _checkPanel.Controls.Add(_selectedLabel);
            card.Controls.Add(_checkPanel);
            return card;
        }

        private Panel BuildChecklistColumn()
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = Theme.ScalePadding(0, 0, 20, 0);

            Label label = Theme.MakeLabel("Tick every item that came back:", Theme.BoldFont, Theme.TextDark);
            label.Dock = DockStyle.Top;
            label.Padding = Theme.ScalePadding(0, 0, 0, 6);

            _checklistBox = new CheckedListBox();
            _checklistBox.Dock = DockStyle.Fill;
            _checklistBox.Font = Theme.NormalFont;
            _checklistBox.CheckOnClick = true;
            _checklistBox.BorderStyle = BorderStyle.FixedSingle;
            _checklistBox.IntegralHeight = false;
            _checklistBox.HorizontalScrollbar = true;
            // ItemCheck fires BEFORE the box changes, so we wait until it's done
            _checklistBox.ItemCheck += delegate { BeginInvoke(new Action(UpdateFinePreview)); };

            LinkLabel checkAllLink = new LinkLabel();
            checkAllLink.Text = "Everything is here (tick all)";
            checkAllLink.Font = Theme.SmallFont;
            checkAllLink.LinkColor = Theme.Primary;
            checkAllLink.Dock = DockStyle.Bottom;
            checkAllLink.Padding = Theme.ScalePadding(0, 6, 0, 0);
            checkAllLink.Height = Theme.Scale(28);
            checkAllLink.LinkClicked += delegate { TickAll(); };

            panel.Controls.Add(_checklistBox);
            panel.Controls.Add(checkAllLink);
            panel.Controls.Add(label);
            return panel;
        }

        private Control BuildConditionColumn()
        {
            // Top-to-bottom flow, so the pieces stack nicely at any screen scale
            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.FlowDirection = FlowDirection.TopDown;
            panel.WrapContents = false;
            panel.Padding = Theme.ScalePadding(0, 0, 20, 0);

            Label label = Theme.MakeLabel("Condition:", Theme.BoldFont, Theme.TextDark);
            label.Margin = Theme.ScalePadding(0, 0, 0, 6);

            _conditionBox = Theme.MakeDropDown(160);
            _conditionBox.Items.Add(SchoolEquipment.ConditionGood);
            _conditionBox.Items.Add(SchoolEquipment.ConditionFair);
            _conditionBox.Items.Add(SchoolEquipment.ConditionDamaged);
            _conditionBox.Items.Add(SchoolEquipment.ConditionNeedsRepair);
            _conditionBox.SelectedIndex = 0;
            _conditionBox.SelectedIndexChanged += delegate { UpdateFinePreview(); };

            Label help = Theme.MakeLabel("Damaged / Needs Repair sends\nthe item to repair automatically.", Theme.SmallFont, Theme.TextMuted);
            help.Margin = Theme.ScalePadding(0, 4, 0, 0);

            panel.Controls.Add(label);
            panel.Controls.Add(_conditionBox);
            panel.Controls.Add(help);
            return panel;
        }

        private Panel BuildFineColumn()
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Theme.PageBackground;
            panel.Padding = Theme.ScalePadding(16, 12, 16, 12);

            TableLayoutPanel lines = new TableLayoutPanel();
            lines.Dock = DockStyle.Top;
            lines.AutoSize = true;
            lines.ColumnCount = 2;
            lines.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            lines.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _lateFineLabel = AddFineLine(lines, "Late return");
            _missingFineLabel = AddFineLine(lines, "Missing items");
            _damageFineLabel = AddFineLine(lines, "Damage");
            _totalLabel = AddFineLine(lines, "TOTAL FINE");
            _totalLabel.Font = Theme.SectionFont;

            _holdWarningLabel = Theme.MakeLabel("", Theme.SmallFont, Theme.Danger);
            _holdWarningLabel.Dock = DockStyle.Top;
            _holdWarningLabel.Padding = Theme.ScalePadding(0, 6, 0, 0);

            _confirmButton = Theme.MakeButton("Confirm Return", Theme.Success);
            _confirmButton.Dock = DockStyle.Bottom;
            _confirmButton.AutoSize = false;
            _confirmButton.Height = Theme.Scale(42);
            _confirmButton.Click += ConfirmButton_Click;

            panel.Controls.Add(_holdWarningLabel);
            panel.Controls.Add(lines);
            panel.Controls.Add(_confirmButton);
            return panel;
        }

        /// <summary>Adds "name ........ PHP 0.00" and gives back the amount label.</summary>
        private Label AddFineLine(TableLayoutPanel table, string name)
        {
            Label nameLabel = Theme.MakeLabel(name, Theme.NormalFont, Theme.TextMuted);
            nameLabel.Margin = Theme.ScalePadding(0, 4, 0, 4);

            Label amountLabel = Theme.MakeLabel(Theme.FormatMoney(0), Theme.BoldFont, Theme.TextDark);
            amountLabel.Margin = Theme.ScalePadding(0, 4, 0, 4);
            amountLabel.Anchor = AnchorStyles.Right;

            table.Controls.Add(nameLabel, 0, table.RowCount);
            table.Controls.Add(amountLabel, 1, table.RowCount);
            table.RowCount++;
            return amountLabel;
        }

        // ---------------------------------------------------------------

        public override void RefreshData()
        {
            BorrowSlip previouslySelected = GetSelectedRowObject(_grid) as BorrowSlip;
            List<BorrowSlip> slips = Borrowing.GetActiveSlips();

            _grid.Rows.Clear();
            foreach (BorrowSlip slip in slips)
            {
                int rowIndex = _grid.Rows.Add(
                    slip.SlipID,
                    slip.Borrower.FullName,
                    slip.Equipment.DisplayName,
                    slip.RoomToBeUsed,
                    Theme.FormatFriendlyDate(slip.DueTime),
                    "");

                DataGridViewRow row = _grid.Rows[rowIndex];
                row.Tag = slip;
                Theme.PaintBadge(row.Cells["Status"], GetLoanStatus(slip));

                if (slip == previouslySelected)
                {
                    row.Selected = true;
                }
            }

            _grid.Visible = slips.Count > 0;
            _noLoansLabel.Visible = slips.Count == 0;
            ShowSelectedSlip();
        }

        private BorrowSlip GetSelectedSlip()
        {
            return GetSelectedRowObject(_grid) as BorrowSlip;
        }

        /// <summary>Fills the bottom card with the checklist of the selected loan.</summary>
        private void ShowSelectedSlip()
        {
            BorrowSlip slip = GetSelectedSlip();
            _checklistBox.Items.Clear();
            _conditionBox.SelectedIndex = 0;

            if (slip == null)
            {
                _selectedLabel.Text = "Select a loan above to check it in.";
                _checkPanel.Enabled = false;
            }
            else
            {
                _selectedLabel.Text = "Checking in " + slip.SlipID + ":  " + slip.Equipment.ItemName + "  (" + slip.Borrower.FullName + ")";
                _checkPanel.Enabled = true;

                foreach (string checklistItem in slip.Equipment.GetReturnChecklist())
                {
                    _checklistBox.Items.Add(checklistItem, false);
                }
            }

            UpdateFinePreview();
        }

        private void TickAll()
        {
            for (int i = 0; i < _checklistBox.Items.Count; i++)
            {
                _checklistBox.SetItemChecked(i, true);
            }
            UpdateFinePreview();
        }

        private bool IsChecklistComplete()
        {
            return _checklistBox.Items.Count > 0 && _checklistBox.CheckedItems.Count == _checklistBox.Items.Count;
        }

        /// <summary>
        /// Shows how much the borrower will be charged if staff confirm right now.
        /// The real total comes from BorrowingController.CalculatePenalty so the
        /// preview always matches what ProcessReturn will charge.
        /// </summary>
        private void UpdateFinePreview()
        {
            BorrowSlip slip = GetSelectedSlip();
            if (slip == null)
            {
                _lateFineLabel.Text = Theme.FormatMoney(0);
                _missingFineLabel.Text = Theme.FormatMoney(0);
                _damageFineLabel.Text = Theme.FormatMoney(0);
                _totalLabel.Text = Theme.FormatMoney(0);
                _totalLabel.ForeColor = Theme.TextDark;
                _holdWarningLabel.Text = "";
                _confirmButton.Enabled = false;
                return;
            }

            DateTime now = DateTime.Now;
            string condition = _conditionBox.SelectedItem.ToString();
            bool complete = IsChecklistComplete();

            double lateFine = 0;
            if (now > slip.DueTime)
            {
                lateFine = slip.Equipment.CalculateLateFine(now - slip.DueTime);
            }
            double missingFine = complete ? 0 : BorrowingController.MissingPartsFee;
            double damageFine = BorrowingController.IsDamagedCondition(condition) ? BorrowingController.DamageFee : 0;
            double total = Borrowing.CalculatePenalty(slip, condition, complete, now);

            _confirmButton.Enabled = true;

            _lateFineLabel.Text = Theme.FormatMoney(lateFine);
            _missingFineLabel.Text = Theme.FormatMoney(missingFine);
            _damageFineLabel.Text = Theme.FormatMoney(damageFine);
            _totalLabel.Text = Theme.FormatMoney(total);

            if (total > 0)
            {
                _totalLabel.ForeColor = Theme.Danger;
                _holdWarningLabel.Text = slip.Borrower.FullName + " will be placed on clearance hold.";
                _confirmButton.BackColor = Theme.Warning;
                _confirmButton.Text = "Confirm Return with Fine";
            }
            else
            {
                _totalLabel.ForeColor = Theme.Success;
                _holdWarningLabel.Text = "";
                _confirmButton.BackColor = Theme.Success;
                _confirmButton.Text = "Confirm Return";
            }
            _confirmButton.FlatAppearance.MouseOverBackColor = Theme.Darken(_confirmButton.BackColor, 0.12);
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            BorrowSlip slip = GetSelectedSlip();
            if (slip == null)
            {
                return;
            }

            string condition = _conditionBox.SelectedItem.ToString();
            bool complete = IsChecklistComplete();
            double expectedFine = Borrowing.CalculatePenalty(slip, condition, complete, DateTime.Now);

            // Charging someone money deserves a "are you sure?"
            if (expectedFine > 0)
            {
                string question = "This return has a fine of " + Theme.FormatMoney(expectedFine) + ".\n\n"
                                + slip.Borrower.FullName + " will be put on clearance hold until it is settled.\n\nContinue?";
                if (!AskYesNo(question, "Confirm fine"))
                {
                    return;
                }
            }

            try
            {
                double fine = Borrowing.ProcessReturn(slip.SlipID, condition, complete);

                if (fine > 0)
                {
                    AudioHelper.PlayWarning();
                    ShowStatus(slip.SlipID + " returned with a fine of " + Theme.FormatMoney(fine) + ". " + slip.Borrower.FullName + " is now on clearance hold.", true);
                }
                else
                {
                    AudioHelper.PlaySuccess();
                    ShowStatus(slip.SlipID + " returned on time and complete. No fine.", false);
                }

                RefreshData();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, "Could not process return");
            }
        }
    }
}
