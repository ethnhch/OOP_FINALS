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
    /// Screen for lending an item: pick a borrower and an item, then issue a slip.
    /// The form tells the user about problems (like a clearance hold) BEFORE they click.
    /// </summary>
    public class BorrowPage : BasePage
    {
        private ComboBox _borrowerBox;
        private Label _borrowerHint;
        private ComboBox _equipmentBox;
        private Label _equipmentHint;
        private TextBox _roomBox;
        private ListBox _checklistBox;
        private Button _issueButton;

        private TableLayoutPanel _columns;
        private CardPanel _formCard;
        private CardPanel _slipCard;

        private TextBox _slipText;
        private Button _copyButton;

        public BorrowPage(EquipmentController equipment, BorrowingController borrowing, BorrowerController borrowers)
            : base("Borrow Equipment", "Choose who is borrowing and what they need. A borrow slip is created right away.", equipment, borrowing, borrowers)
        {
            BuildLayout();
        }

        private void BuildLayout()
        {
            // Two cards: the form and the slip.
            // ApplyLayout puts them side by side (wide) or one above the other (narrow).
            _formCard = BuildFormCard();
            _slipCard = BuildSlipCard();

            _columns = new TableLayoutPanel();
            _columns.Controls.Add(_formCard);
            _columns.Controls.Add(_slipCard);

            ContentArea.Controls.Add(_columns);
        }

        protected override void ApplyLayout(bool isNarrow)
        {
            int gap = Theme.Scale(16);
            _columns.ColumnStyles.Clear();
            _columns.RowStyles.Clear();

            if (isNarrow)
            {
                // One column: form on top, slip underneath. The page scrolls if needed.
                int formHeight = Theme.Scale(620);
                int slipHeight = Theme.Scale(440);

                _columns.ColumnCount = 1;
                _columns.RowCount = 2;
                _columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                _columns.RowStyles.Add(new RowStyle(SizeType.Absolute, formHeight));
                _columns.RowStyles.Add(new RowStyle(SizeType.Absolute, slipHeight));
                _columns.SetCellPosition(_formCard, new TableLayoutPanelCellPosition(0, 0));
                _columns.SetCellPosition(_slipCard, new TableLayoutPanelCellPosition(0, 1));
                _formCard.Margin = new Padding(0, 0, 0, gap);
                _slipCard.Margin = new Padding(0, 0, 0, gap);

                _columns.Dock = DockStyle.Top;
                _columns.Height = formHeight + slipHeight;
                ContentArea.AutoScroll = true;
            }
            else
            {
                // Two columns side by side, filling the page
                _columns.ColumnCount = 2;
                _columns.RowCount = 1;
                _columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                _columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                _columns.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                _columns.SetCellPosition(_formCard, new TableLayoutPanelCellPosition(0, 0));
                _columns.SetCellPosition(_slipCard, new TableLayoutPanelCellPosition(1, 0));
                _formCard.Margin = new Padding(0, 0, gap, gap);
                _slipCard.Margin = new Padding(0, 0, 0, gap);

                ContentArea.AutoScroll = false;
                _columns.Dock = DockStyle.Fill;
            }
        }

        private CardPanel BuildFormCard()
        {
            CardPanel card = new CardPanel();
            card.Dock = DockStyle.Fill;

            // A one-column table: each control goes in the next row
            TableLayoutPanel form = new TableLayoutPanel();
            form.Dock = DockStyle.Fill;
            form.ColumnCount = 1;
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            AddRow(form, Theme.MakeLabel("1. Borrow details", Theme.SectionFont, Theme.TextDark), 12);

            // Borrower
            AddRow(form, Theme.MakeLabel("Borrower", Theme.BoldFont, Theme.TextDark), 4);
            _borrowerBox = Theme.MakeDropDown(300);
            _borrowerBox.Dock = DockStyle.Fill;
            _borrowerBox.DisplayMember = "DisplayName";
            _borrowerBox.SelectedIndexChanged += delegate { UpdateHints(); };
            AddRow(form, _borrowerBox, 2);
            _borrowerHint = Theme.MakeLabel("", Theme.SmallFont, Theme.TextMuted);
            AddRow(form, _borrowerHint, 14);

            // Equipment
            AddRow(form, Theme.MakeLabel("Equipment (only available items are listed)", Theme.BoldFont, Theme.TextDark), 4);
            _equipmentBox = Theme.MakeDropDown(300);
            _equipmentBox.Dock = DockStyle.Fill;
            _equipmentBox.DisplayMember = "DisplayName";
            _equipmentBox.SelectedIndexChanged += delegate { UpdateHints(); };
            AddRow(form, _equipmentBox, 2);
            _equipmentHint = Theme.MakeLabel("", Theme.SmallFont, Theme.TextMuted);
            AddRow(form, _equipmentHint, 14);

            // Room
            AddRow(form, Theme.MakeLabel("Room / location where it will be used", Theme.BoldFont, Theme.TextDark), 4);
            _roomBox = new TextBox();
            _roomBox.Font = Theme.NormalFont;
            _roomBox.Dock = DockStyle.Fill;
            _roomBox.PlaceholderText = "e.g. Room 304 - Lecture Hall";
            AddRow(form, _roomBox, 14);

            // Checklist preview
            AddRow(form, Theme.MakeLabel("The borrower must return all of these:", Theme.BoldFont, Theme.TextDark), 4);
            _checklistBox = new ListBox();
            _checklistBox.Font = Theme.NormalFont;
            _checklistBox.BorderStyle = BorderStyle.FixedSingle;
            _checklistBox.BackColor = Theme.PageBackground;
            _checklistBox.Dock = DockStyle.Fill;
            _checklistBox.Height = Theme.Scale(110);
            _checklistBox.SelectionMode = SelectionMode.None;
            AddRow(form, _checklistBox, 16);

            // Big button
            _issueButton = Theme.MakeButton("Issue Borrow Slip", Theme.Primary);
            _issueButton.Dock = DockStyle.Fill;
            _issueButton.Height = Theme.Scale(44);
            _issueButton.AutoSize = false;
            _issueButton.Margin = new Padding(0);
            _issueButton.Click += IssueButton_Click;
            AddRow(form, _issueButton, 0);

            // Empty last row that takes up the leftover space
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            form.RowCount++;

            card.Controls.Add(form);
            return card;
        }

        private CardPanel BuildSlipCard()
        {
            CardPanel card = new CardPanel();
            card.Dock = DockStyle.Fill;
            card.Margin = Theme.ScalePadding(0, 0, 0, 16);

            Label title = Theme.MakeLabel("2. Borrow slip", Theme.SectionFont, Theme.TextDark);
            title.Dock = DockStyle.Top;
            title.Padding = Theme.ScalePadding(0, 0, 0, 12);

            _slipText = new TextBox();
            _slipText.Multiline = true;
            _slipText.ReadOnly = true;
            _slipText.Dock = DockStyle.Fill;
            _slipText.Font = Theme.MonoFont;
            _slipText.BackColor = Color.FromArgb(254, 252, 232); // soft "paper" yellow
            _slipText.BorderStyle = BorderStyle.FixedSingle;
            _slipText.ScrollBars = ScrollBars.Vertical;
            _slipText.Text = "\r\n  No slip issued yet.\r\n\r\n  Fill in the form on the left and\r\n  click 'Issue Borrow Slip'.";

            FlowLayoutPanel buttons = new FlowLayoutPanel();
            buttons.Dock = DockStyle.Bottom;
            buttons.AutoSize = true;
            buttons.Padding = Theme.ScalePadding(0, 12, 0, 0);

            _copyButton = Theme.MakeSecondaryButton("Copy Slip");
            _copyButton.Enabled = false;
            _copyButton.Click += CopyButton_Click;
            buttons.Controls.Add(_copyButton);

            card.Controls.Add(_slipText);
            card.Controls.Add(buttons);
            card.Controls.Add(title);
            return card;
        }

        private void AddRow(TableLayoutPanel table, Control control, int spaceBelow)
        {
            control.Margin = Theme.ScalePadding(0, 0, 0, spaceBelow);

            // Fill the width of the column (this also lets long labels wrap onto 2 lines)
            control.Dock = DockStyle.Fill;

            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.Controls.Add(control, 0, table.RowCount);
            table.RowCount++;
        }

        public override void RefreshData()
        {
            // Keep the current choices if they are still valid
            SchoolBorrower selectedBorrower = _borrowerBox.SelectedItem as SchoolBorrower;
            SchoolEquipment selectedItem = _equipmentBox.SelectedItem as SchoolEquipment;

            _borrowerBox.Items.Clear();
            foreach (SchoolBorrower borrower in Borrowers.GetAllBorrowers())
            {
                _borrowerBox.Items.Add(borrower);
            }

            _equipmentBox.Items.Clear();
            foreach (SchoolEquipment item in Equipment.GetAvailableEquipment())
            {
                _equipmentBox.Items.Add(item);
            }

            SelectItemOrFirst(_borrowerBox, selectedBorrower);
            SelectItemOrFirst(_equipmentBox, selectedItem);
            UpdateHints();
        }

        private void SelectItemOrFirst(ComboBox box, object item)
        {
            if (item != null && box.Items.Contains(item))
            {
                box.SelectedItem = item;
            }
            else if (box.Items.Count > 0)
            {
                box.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Updates the small text under each drop-down, the checklist, and whether the button works.
        /// </summary>
        private void UpdateHints()
        {
            SchoolBorrower borrower = _borrowerBox.SelectedItem as SchoolBorrower;
            SchoolEquipment item = _equipmentBox.SelectedItem as SchoolEquipment;

            // Borrower hint
            if (borrower == null)
            {
                _borrowerHint.Text = "No borrowers registered yet. Add one in the Borrowers page.";
                _borrowerHint.ForeColor = Theme.TextMuted;
            }
            else if (borrower.HasClearanceHold)
            {
                _borrowerHint.Text = "This borrower is on clearance hold and can't borrow. Lift the hold in the Borrowers page.";
                _borrowerHint.ForeColor = Theme.Danger;
            }
            else
            {
                int loans = Borrowing.CountActiveLoans(borrower);
                _borrowerHint.Text = "Eligible to borrow  ·  " + borrower.Department + "  ·  " + loans + " item(s) currently borrowed";
                _borrowerHint.ForeColor = Theme.Success;
            }

            // Equipment hint + checklist
            _checklistBox.Items.Clear();
            if (item == null)
            {
                _equipmentHint.Text = "Nothing is available right now.";
                _equipmentHint.ForeColor = Theme.Danger;
            }
            else
            {
                DateTime due = item.CalculateDueTime(DateTime.Now);
                _equipmentHint.Text = "Loan period: " + item.GetLoanPeriodText() + "  ·  Due back " + Theme.FormatFriendlyDate(due);
                _equipmentHint.ForeColor = Theme.TextMuted;

                foreach (string checklistItem in item.GetReturnChecklist())
                {
                    _checklistBox.Items.Add("•  " + checklistItem);
                }
            }

            _issueButton.Enabled = borrower != null && item != null && borrower.CheckEligibility();
        }

        private void IssueButton_Click(object sender, EventArgs e)
        {
            SchoolBorrower borrower = _borrowerBox.SelectedItem as SchoolBorrower;
            SchoolEquipment item = _equipmentBox.SelectedItem as SchoolEquipment;
            if (borrower == null || item == null)
            {
                return;
            }

            try
            {
                BorrowSlip slip = Borrowing.ProcessBorrow(borrower, item, _roomBox.Text);
                _slipText.Text = BuildSlipText(slip);
                _copyButton.Enabled = true;

                AudioHelper.PlaySuccess();
                ShowStatus(slip.SlipID + " issued: " + item.ItemName + " to " + borrower.FullName + ". Due " + Theme.FormatFriendlyDate(slip.DueTime) + ".", false);

                _roomBox.Clear();
                RefreshData();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, "Can't borrow this item");
            }
        }

        private string BuildSlipText(BorrowSlip slip)
        {
            string line = "  ----------------------------------------\r\n";
            string text = "\r\n";
            text += "          SCHOOL EQUIPMENT OFFICE\r\n";
            text += "               BORROW SLIP\r\n";
            text += line;
            text += "  Slip no.   : " + slip.SlipID + "\r\n";
            text += "  Borrower   : " + slip.Borrower.FullName + "\r\n";
            text += "  School ID  : " + slip.Borrower.SchoolIDNumber + "\r\n";
            text += "  Department : " + slip.Borrower.Department + " (" + slip.Borrower.Role + ")\r\n";
            text += line;
            text += "  Equipment  : " + slip.Equipment.ItemName + "\r\n";
            text += "  Tag        : " + slip.Equipment.PropertyTag + "\r\n";
            text += "  Room       : " + slip.RoomToBeUsed + "\r\n";
            text += "  Borrowed   : " + slip.TimeBorrowed.ToString("MMM d, yyyy h:mm tt") + "\r\n";
            text += "  DUE BACK   : " + slip.DueTime.ToString("MMM d, yyyy h:mm tt") + "\r\n";
            text += line;
            text += "  Please return ALL of these:\r\n";

            List<string> checklist = slip.Equipment.GetReturnChecklist();
            foreach (string checklistItem in checklist)
            {
                text += "   [ ] " + checklistItem + "\r\n";
            }

            text += line;
            text += "  Late returns, missing parts and damage\r\n";
            text += "  are charged and put a clearance hold\r\n";
            text += "  on the borrower.\r\n";
            return text;
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(_slipText.Text);
            ShowStatus("Slip copied. You can paste it into an email or document.", false);
        }
    }
}
