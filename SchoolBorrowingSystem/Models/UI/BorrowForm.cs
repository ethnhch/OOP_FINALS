using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Models;

namespace SchoolBorrowingSystem.UI
{
    public partial class BorrowForm : Form
    {
        private readonly EquipmentController _equipmentController;
        private readonly BorrowingController _borrowingController;
        private readonly IList<SchoolBorrower> _borrowers;

        public BorrowForm()
        {
            InitializeComponent();
        }

        public BorrowForm(EquipmentController equipmentController, BorrowingController borrowingController, IList<SchoolBorrower> borrowers)
            : this()
        {
            _equipmentController = equipmentController;
            _borrowingController = borrowingController;
            _borrowers = borrowers;

            LoadDropdowns();
        }

        private void LoadDropdowns()
        {
            cmbBorrowers.Items.Clear();
            foreach (var b in _borrowers)
            {
                string hold = b.HasClearanceHold ? " (HOLD)" : " (CLEAR)";
                cmbBorrowers.Items.Add(string.Format("{0} - {1} ({2}){3}", b.SchoolIDNumber, b.FullName, b.Role, hold));
            }
            if (cmbBorrowers.Items.Count > 0) cmbBorrowers.SelectedIndex = 0;

            cmbEquipment.Items.Clear();
            foreach (var eq in _equipmentController.GetAvailableEquipment())
            {
                cmbEquipment.Items.Add(string.Format("{0} - {1}", eq.PropertyTag, eq.ItemName));
            }
            if (cmbEquipment.Items.Count > 0) cmbEquipment.SelectedIndex = 0;
        }

        private void CmbEquipment_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstChecklist.Items.Clear();
            int idx = 0;
            foreach (var eq in _equipmentController.GetAvailableEquipment())
            {
                if (idx == cmbEquipment.SelectedIndex)
                {
                    foreach (var item in eq.GetReturnChecklist())
                    {
                        lstChecklist.Items.Add("• " + item);
                    }
                    break;
                }
                idx++;
            }
        }

        private void BtnIssue_Click(object sender, EventArgs e)
        {
            if (cmbBorrowers.SelectedIndex < 0 || cmbEquipment.SelectedIndex < 0)
            {
                AudioHelper.PlayError();
                MessageBox.Show("Please select both a borrower and an available equipment item.", "Missing Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SchoolBorrower selectedBorrower = null;
            int bIdx = 0;
            foreach (var b in _borrowers)
            {
                if (bIdx == cmbBorrowers.SelectedIndex) { selectedBorrower = b; break; }
                bIdx++;
            }

            SchoolEquipment selectedItem = null;
            int eIdx = 0;
            foreach (var eq in _equipmentController.GetAvailableEquipment())
            {
                if (eIdx == cmbEquipment.SelectedIndex) { selectedItem = eq; break; }
                eIdx++;
            }

            if (selectedBorrower == null || selectedItem == null) return;

            try
            {
                var slip = _borrowingController.ProcessBorrow(selectedBorrower, selectedItem, txtRoom.Text);
                AudioHelper.PlaySuccess();
                MessageBox.Show(
                    string.Format("Borrow Slip {0} issued to {1}!\nEquipment: {2} ({3})\nDue: {4:yyyy-MM-dd HH:mm}",
                        slip.SlipID, selectedBorrower.FullName, selectedItem.ItemName, selectedItem.PropertyTag, slip.DueTime),
                    "Borrow Slip Issued",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show(ex.Message, "Borrowing Rejected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            AudioHelper.PlayMenuClick();
            this.Close();
        }
    }
}
