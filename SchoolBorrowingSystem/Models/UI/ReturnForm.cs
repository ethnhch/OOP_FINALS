using System;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Transactions;

namespace SchoolBorrowingSystem.UI
{
    public partial class ReturnForm : Form
    {
        private readonly BorrowingController _borrowingController;

        public ReturnForm()
        {
            InitializeComponent();
        }

        public ReturnForm(BorrowingController borrowingController)
            : this()
        {
            _borrowingController = borrowingController;
            LoadActiveSlips();
        }

        private void LoadActiveSlips()
        {
            cmbActiveSlips.Items.Clear();
            foreach (var slip in _borrowingController.GetActiveSlips())
            {
                string borrowerName = slip.Borrower != null ? slip.Borrower.FullName : "Unknown";
                string tag = slip.Equipment != null ? slip.Equipment.PropertyTag : "";
                string itemName = slip.Equipment != null ? slip.Equipment.ItemName : "Unknown";
                cmbActiveSlips.Items.Add(string.Format("{0} | {1} - {2} ({3})", slip.SlipID, tag, itemName, borrowerName));
            }
            if (cmbActiveSlips.Items.Count > 0) cmbActiveSlips.SelectedIndex = 0;
        }

        private void CmbActiveSlips_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstChecklist.Items.Clear();
            int idx = 0;
            foreach (var slip in _borrowingController.GetActiveSlips())
            {
                if (idx == cmbActiveSlips.SelectedIndex && slip.Equipment != null)
                {
                    foreach (var item in slip.Equipment.GetReturnChecklist())
                    {
                        lstChecklist.Items.Add("• " + item);
                    }
                    break;
                }
                idx++;
            }
        }

        private void BtnProcess_Click(object sender, EventArgs e)
        {
            if (cmbActiveSlips.SelectedIndex < 0)
            {
                AudioHelper.PlayError();
                MessageBox.Show("There are no active borrow slips to return.", "No Active Slips", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            BorrowSlip selectedSlip = null;
            int idx = 0;
            foreach (var slip in _borrowingController.GetActiveSlips())
            {
                if (idx == cmbActiveSlips.SelectedIndex) { selectedSlip = slip; break; }
                idx++;
            }
            if (selectedSlip == null) return;

            string condition = cmbCondition.SelectedItem != null ? cmbCondition.SelectedItem.ToString() : "Good";
            bool intact = chkChecklistIntact.Checked;

            try
            {
                double penalty = _borrowingController.ProcessReturn(selectedSlip.SlipID, condition, intact);
                if (penalty > 0)
                {
                    AudioHelper.PlayWarning();
                    MessageBox.Show(
                        string.Format("Return processed for {0}.\n\nPENALTY INCURRED: PHP {1:N2}\nNotice: Borrower has been placed on CLEARANCE HOLD.", selectedSlip.SlipID, penalty),
                        "Penalty & Clearance Hold",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    AudioHelper.PlaySuccess();
                    MessageBox.Show(
                        string.Format("Slip {0} returned in good standing with PHP 0.00 penalty!", selectedSlip.SlipID),
                        "Return Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show(ex.Message, "Return Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            AudioHelper.PlayMenuClick();
            this.Close();
        }
    }
}
