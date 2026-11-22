using System;
using System.Drawing;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.Transactions;

namespace SchoolBorrowingSystem.UI.Pages
{
    /// <summary>
    /// Every screen in the sidebar (Dashboard, Inventory, Borrow, ...) inherits from this class.
    /// It gives each page:
    ///   - the three controllers
    ///   - a title + subtitle at the top
    ///   - a ContentArea panel where the page puts its own controls
    ///   - a way to show a message in the status bar, or jump to another page
    /// </summary>
    public class BasePage : UserControl
    {
        protected readonly EquipmentController Equipment;
        protected readonly BorrowingController Borrowing;
        protected readonly BorrowerController Borrowers;

        protected readonly Panel ContentArea;

        // MainForm listens to these two events
        public event Action<string, bool> StatusChanged;
        public event Action<string> NavigateRequested;

        // Needed so the Visual Studio designer can open the class
        public BasePage()
            : this("Page", "", null, null, null)
        {
        }

        public BasePage(string title, string subtitle, EquipmentController equipment, BorrowingController borrowing, BorrowerController borrowers)
        {
            Equipment = equipment;
            Borrowing = borrowing;
            Borrowers = borrowers;

            Dock = DockStyle.Fill;
            BackColor = Theme.PageBackground;
            Padding = new Padding(28, 24, 28, 20);
            Font = Theme.NormalFont;
            DoubleBuffered = true;

            // Header: big title with a gray subtitle under it
            Panel header = new Panel();
            header.Dock = DockStyle.Top;
            header.Height = 72;

            Label titleLabel = Theme.MakeLabel(title, Theme.TitleFont, Theme.TextDark);
            titleLabel.Location = new Point(0, 0);

            Label subtitleLabel = Theme.MakeLabel(subtitle, Theme.NormalFont, Theme.TextMuted);
            subtitleLabel.Location = new Point(2, 38);

            header.Controls.Add(titleLabel);
            header.Controls.Add(subtitleLabel);

            ContentArea = new Panel();
            ContentArea.Dock = DockStyle.Fill;

            // NOTE about docking: WinForms docks the LAST added control first.
            // So we add the Fill panel first and the Top header second,
            // which makes the header take the top and the content fill the rest.
            Controls.Add(ContentArea);
            Controls.Add(header);
        }

        /// <summary>
        /// MainForm calls this every time the page is shown, so the data is always fresh.
        /// Each page overrides it to reload its tables.
        /// </summary>
        public virtual void RefreshData()
        {
        }

        protected void ShowStatus(string message, bool isWarning)
        {
            if (StatusChanged != null)
            {
                StatusChanged(message, isWarning);
            }
        }

        protected void GoToPage(string pageName)
        {
            if (NavigateRequested != null)
            {
                NavigateRequested(pageName);
            }
        }

        /// <summary>
        /// A row of controls laid out left to right (search box, buttons, ...).
        /// </summary>
        protected FlowLayoutPanel MakeToolbar()
        {
            FlowLayoutPanel toolbar = new FlowLayoutPanel();
            toolbar.Dock = DockStyle.Top;
            toolbar.AutoSize = true;
            toolbar.WrapContents = true;
            toolbar.FlowDirection = FlowDirection.LeftToRight;
            toolbar.Padding = new Padding(0, 0, 0, 12);
            return toolbar;
        }

        /// <summary>
        /// Every row we add to a grid stores the real object in row.Tag.
        /// This gets that object back for the selected row (or null).
        /// </summary>
        protected object GetSelectedRowObject(DataGridView grid)
        {
            if (grid.SelectedRows.Count == 0)
            {
                return null;
            }
            return grid.SelectedRows[0].Tag;
        }

        /// <summary>
        /// One word that describes what is going on with an item right now.
        /// </summary>
        protected string GetEquipmentStatus(SchoolEquipment item)
        {
            if (item.IsAvailable)
            {
                return "Available";
            }

            BorrowSlip slip = Borrowing.FindActiveSlipForEquipment(item.PropertyTag);
            if (slip != null)
            {
                return "On Loan";
            }

            if (item.Condition == SchoolEquipment.ConditionUnderRepair)
            {
                return "Under Repair";
            }

            if (item.Condition == SchoolEquipment.ConditionMissingParts)
            {
                return "Missing Parts";
            }

            return "Unavailable";
        }

        /// <summary>
        /// Status text for an open loan: "Overdue", "Due soon" (less than 1 hour) or "Due in ...".
        /// </summary>
        protected string GetLoanStatus(BorrowSlip slip)
        {
            if (slip.IsOverdue())
            {
                return slip.GetTimeLeftText();
            }

            if ((slip.DueTime - DateTime.Now).TotalHours < 1)
            {
                return "Due soon";
            }

            return slip.GetTimeLeftText();
        }

        protected bool AskYesNo(string question, string title)
        {
            DialogResult answer = MessageBox.Show(this, question, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return answer == DialogResult.Yes;
        }

        protected void ShowError(string message, string title)
        {
            AudioHelper.PlayError();
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
