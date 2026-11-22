using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.Transactions;

namespace SchoolBorrowingSystem.UI
{
    public partial class MainForm : Form
    {
        private readonly EquipmentController _equipmentController;
        private readonly BorrowingController _borrowingController;
        private readonly IList<SchoolBorrower> _borrowers;

        public MainForm(EquipmentController equipmentController, BorrowingController borrowingController, IList<SchoolBorrower> borrowers)
            : this()
        {
            _equipmentController = equipmentController;
            _borrowingController = borrowingController;
            _borrowers = borrowers;

            RefreshAllViews();
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            AudioHelper.PlayStartup();
        }

        private void BtnMute_Click(object sender, EventArgs e)
        {
            AudioHelper.IsMuted = !AudioHelper.IsMuted;
            _btnMute.Text = AudioHelper.IsMuted ? "Audio: MUTED" : "Audio: ON";
            _btnMute.BackColor = AudioHelper.IsMuted ? Color.FromArgb(183, 28, 28) : Color.FromArgb(46, 125, 50);
            if (!AudioHelper.IsMuted) AudioHelper.PlayMenuClick();
        }

        private void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            AudioHelper.PlayMenuClick();
        }

        private void ChkAvailableOnly_CheckedChanged(object sender, EventArgs e)
        {
            AudioHelper.PlayMenuClick();
            RefreshEquipmentGrid();
        }

        private void CmbAvailableEquipment_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateChecklistPreview();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _equipmentController.SaveInventory();
                AudioHelper.PlaySuccess();
                SetStatus("Inventory saved to 'school_inventory.txt'.", false);
                MessageBox.Show("Inventory successfully saved to 'school_inventory.txt'.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show(ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                _equipmentController.LoadInventory();
                RefreshAllViews();
                AudioHelper.PlaySuccess();
                SetStatus("Inventory loaded from disk.", false);
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show(ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnApplyHold_Click(object sender, EventArgs e)
        {
            ToggleSelectedBorrowerHold(true);
        }

        private void BtnLiftHold_Click(object sender, EventArgs e)
        {
            ToggleSelectedBorrowerHold(false);
        }

        private static string GetSelectedFirstCell(DataGridView dgv)
        {
            foreach (DataGridViewRow row in dgv.SelectedRows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    return cell.Value != null ? cell.Value.ToString() : string.Empty;
                }
            }
            return string.Empty;
        }

        private static SchoolEquipment GetItemAt(IEnumerable<SchoolEquipment> items, int targetIndex)
        {
            int idx = 0;
            foreach (var item in items)
            {
                if (idx == targetIndex) return item;
                idx++;
            }
            return null;
        }

        private static SchoolBorrower GetBorrowerAt(IEnumerable<SchoolBorrower> items, int targetIndex)
        {
            int idx = 0;
            foreach (var item in items)
            {
                if (idx == targetIndex) return item;
                idx++;
            }
            return null;
        }

        private void BtnMarkRepair_Click(object sender, EventArgs e)
        {
            string tag = GetSelectedFirstCell(_dgvEquipment);
            if (string.IsNullOrEmpty(tag))
            {
                AudioHelper.PlayError();
                MessageBox.Show("Please select an equipment row first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _equipmentController.MarkForRepair(tag);
                RefreshAllViews();
                AudioHelper.PlayWarning();
                SetStatus(string.Format("Equipment '{0}' marked as Under Repair.", tag), true);
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCompleteRepair_Click(object sender, EventArgs e)
        {
            string tag = GetSelectedFirstCell(_dgvEquipment);
            if (string.IsNullOrEmpty(tag))
            {
                AudioHelper.PlayError();
                MessageBox.Show("Please select an equipment row first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var item = _equipmentController.FindEquipmentByTag(tag);
            if (item != null)
            {
                item.Condition = "Good";
                item.IsAvailable = true;
                RefreshAllViews();
                AudioHelper.PlaySuccess();
                SetStatus(string.Format("Equipment '{0}' repaired and restored to Good / Available status.", tag), false);
            }
        }

        private void BtnIssueSlip_Click(object sender, EventArgs e)
        {
            if (_cmbBorrowers.SelectedIndex < 0 || _cmbAvailableEquipment.SelectedIndex < 0)
            {
                AudioHelper.PlayError();
                MessageBox.Show("Please select both a borrower and an available equipment item.", "Missing Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var borrower = GetBorrowerAt(_borrowers, _cmbBorrowers.SelectedIndex);
            var available = _equipmentController.GetAvailableEquipment();
            var item = GetItemAt(available, _cmbAvailableEquipment.SelectedIndex);
            if (borrower == null || item == null) return;

            try
            {
                var slip = _borrowingController.ProcessBorrow(borrower, item, _txtRoom.Text);
                AudioHelper.PlaySuccess();

                var checklist = item.GetReturnChecklist();
                _txtBorrowReceipt.Text =
                    "====================================================\r\n" +
                    "              OFFICIAL BORROW SLIP ISSUED           \r\n" +
                    "====================================================\r\n" +
                    string.Format("Slip ID     : {0}\r\n", slip.SlipID) +
                    string.Format("Borrower    : {0} ({1})\r\n", borrower.FullName, borrower.SchoolIDNumber) +
                    string.Format("Department  : {0} ({1})\r\n", borrower.Department, borrower.Role) +
                    string.Format("Equipment   : {0} ({1})\r\n", item.ItemName, item.PropertyTag) +
                    string.Format("Room        : {0}\r\n", slip.RoomToBeUsed) +
                    string.Format("Borrowed At : {0:yyyy-MM-dd HH:mm}\r\n", slip.TimeBorrowed) +
                    string.Format("Due Time    : {0:yyyy-MM-dd HH:mm}\r\n", slip.DueTime) +
                    "----------------------------------------------------\r\n" +
                    "EXPECTED RETURN CHECKLIST:\r\n" +
                    string.Join("\r\n", checklist.ConvertAll(c => "  - " + c));

                RefreshAllViews();
                SetStatus(string.Format("BorrowSlip {0} issued to {1}.", slip.SlipID, borrower.FullName), false);
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                SetStatus(string.Format("Borrowing Rejected: {0}", ex.Message), true);
                MessageBox.Show(ex.Message, "Borrowing Rejected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnProcessReturn_Click(object sender, EventArgs e)
        {
            string slipId = GetSelectedFirstCell(_dgvActiveSlips);
            if (string.IsNullOrEmpty(slipId))
            {
                AudioHelper.PlayError();
                MessageBox.Show("Please select an active borrow slip from the top table.", "No Slip Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string condition = _cmbReturnCondition.SelectedItem != null ? _cmbReturnCondition.SelectedItem.ToString() : "Good";
            bool intact = _chkChecklistIntact.Checked;

            try
            {
                double penalty = _borrowingController.ProcessReturn(slipId, condition, intact);
                RefreshAllViews();

                if (penalty > 0)
                {
                    AudioHelper.PlayWarning();
                    SetStatus(string.Format("Return processed with penalty: PHP {0:N2}. Clearance hold applied!", penalty), true);
                    MessageBox.Show(
string.Format("Return processed for {0}.\n\nPENALTY INCURRED: PHP {1:N2}\nNotice: Borrower has been placed on CLEARANCE HOLD.", slipId, penalty),
"Penalty & Clearance Hold",
MessageBoxButtons.OK,
MessageBoxIcon.Warning);
                }
                else
                {
                    AudioHelper.PlaySuccess();
                    SetStatus(string.Format("Slip {0} returned in good standing with PHP 0.00 penalty.", slipId), false);
                    MessageBox.Show(string.Format("Slip {0} returned in good standing with no penalty!", slipId), "Return Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show(ex.Message, "Return Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ToggleSelectedBorrowerHold(bool applyHold)
        {
            string id = GetSelectedFirstCell(_dgvBorrowers);
            if (string.IsNullOrEmpty(id))
            {
                AudioHelper.PlayError();
                MessageBox.Show("Please select a borrower first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SchoolBorrower borrower = null;
            foreach (var b in _borrowers)
            {
                if (string.Equals(b.SchoolIDNumber, id, StringComparison.OrdinalIgnoreCase))
                {
                    borrower = b;
                    break;
                }
            }
            if (borrower == null) return;
            if (applyHold)
            {
                _borrowingController.ApplyClearanceHold(borrower);
                AudioHelper.PlayWarning();
                SetStatus(string.Format("Clearance hold applied to {0}.", borrower.FullName), true);
            }
            else
            {
                _borrowingController.LiftClearanceHold(borrower);
                AudioHelper.PlaySuccess();
                SetStatus(string.Format("Clearance hold lifted for {0}.", borrower.FullName), false);
            }
            RefreshAllViews();
        }
        private void BtnRunQa_Click(object sender, EventArgs e)
        {
            AudioHelper.PlayMenuClick();
            var sw = new StringWriter();
            var originalOut = Console.Out;
            try
            {
                Console.SetOut(sw);
                bool passed = TestRunner.RunAllTests();
                _txtQaOutput.Text = sw.ToString();
                if (passed)
                {
                    AudioHelper.PlayTestVictory();
                    SetStatus("All 44 Automated QA Unit & Integration Tests PASSED!", false);
                }
                else
                {
                    AudioHelper.PlayError();
                    SetStatus("Some QA tests failed. Check output.", true);
                }
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
        private void RefreshAllViews()
        {
            if (_equipmentController == null || _borrowingController == null || _borrowers == null) return;
            RefreshEquipmentGrid();
            RefreshBorrowDropdowns();
            RefreshSlipsGrids();
            RefreshBorrowersGrid();
        }

        private void RefreshEquipmentGrid()
        {
            RefreshEquipmentGrid(_dgvEquipment);
        }

        private void RefreshEquipmentGrid(DataGridView _dgvEquipment)
        {
            _dgvEquipment.Rows.Clear();
            var items = _chkAvailableOnly.Checked
            ? _equipmentController.GetAvailableEquipment()
            : _equipmentController.GetAllEquipment();
            foreach (var eq in items)
            {
                int rowIdx = _dgvEquipment.Rows.Add(
                eq.PropertyTag,
                eq is AVEquipment ? "AV Equipment" : "Lab Equipment",
                eq.ItemName,
                eq.Condition,
                eq.IsAvailable ? "AVAILABLE" : "UNAVAILABLE",
                eq.ToString()
                );
                int r = 0;
                foreach (DataGridViewRow row in _dgvEquipment.Rows)
                {
                    if (r == rowIdx)
                    {
                        int c = 0;
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (c == 4)
                            {
                                cell.Style.ForeColor = eq.IsAvailable ? Color.FromArgb(27, 94, 32) : Color.FromArgb(183, 28, 28);
                                cell.Style.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                            }
                            c++;
                        }
                    }
                    r++;
                }
            }
        }
        private void RefreshBorrowDropdowns()
        {
            _cmbBorrowers.Items.Clear();
            foreach (var b in _borrowers)
            {
                string holdTag = b.HasClearanceHold ? " (HOLD)" : " (CLEAR)";
                _cmbBorrowers.Items.Add(string.Format("{0} - {1} ({2}){3}", b.SchoolIDNumber, b.FullName, b.Role, holdTag));
            }
            if (_cmbBorrowers.Items.Count > 0) _cmbBorrowers.SelectedIndex = 0;
            _cmbAvailableEquipment.Items.Clear();
            var available = _equipmentController.GetAvailableEquipment();
            foreach (var eq in available)
            {
                _cmbAvailableEquipment.Items.Add(string.Format("{0} - {1}", eq.PropertyTag, eq.ItemName));
            }
            if (_cmbAvailableEquipment.Items.Count > 0) _cmbAvailableEquipment.SelectedIndex = 0;
            else _lstChecklistPreview.Items.Clear();
        }
        private void UpdateChecklistPreview()
        {
            _lstChecklistPreview.Items.Clear();
            var available = _equipmentController.GetAvailableEquipment();
            var selectedItem = GetItemAt(available, _cmbAvailableEquipment.SelectedIndex);
            if (selectedItem != null)
            {
                foreach (var item in selectedItem.GetReturnChecklist())
                {
                    _lstChecklistPreview.Items.Add("• " + item);
                }
            }
        }
        private void RefreshSlipsGrids()
        {
            _dgvActiveSlips.Rows.Clear();
            foreach (var slip in _borrowingController.GetActiveSlips())
            {
                _dgvActiveSlips.Rows.Add(
                slip.SlipID,
                slip.Borrower != null ? slip.Borrower.FullName : "Unknown",
                slip.Equipment != null ? slip.Equipment.PropertyTag : "",
                slip.Equipment != null ? slip.Equipment.ItemName : "Unknown",
                slip.RoomToBeUsed,
                slip.DueTime.ToString("yyyy-MM-dd HH:mm"),
                slip.IsOverdue() ? "OVERDUE" : "ACTIVE"
                );
            }
            _dgvHistoricalSlips.Rows.Clear();
            foreach (var slip in _borrowingController.GetHistoricalSlips())
            {
                _dgvHistoricalSlips.Rows.Add(
                slip.SlipID,
                slip.Borrower != null ? slip.Borrower.FullName : "Unknown",
                slip.Equipment != null ? slip.Equipment.ItemName : "Unknown",
                slip.TimeReturned.HasValue ? slip.TimeReturned.Value.ToString("yyyy-MM-dd HH:mm") : "-",
                slip.ToString()
                );
            }
        }
        private void RefreshBorrowersGrid()
        {
            _dgvBorrowers.Rows.Clear();
            foreach (var b in _borrowers)
            {
                int rowIdx = _dgvBorrowers.Rows.Add(
                b.SchoolIDNumber,
                b.FullName,
                b.Role,
                b.Department,
                b.HasClearanceHold ? "ACTIVE (HOLD)" : "CLEAR"
                );
                int r = 0;
                foreach (DataGridViewRow row in _dgvBorrowers.Rows)
                {
                    if (r == rowIdx)
                    {
                        int c = 0;
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (c == 4)
                            {
                                cell.Style.ForeColor = b.HasClearanceHold ? Color.FromArgb(183, 28, 28) : Color.FromArgb(27, 94, 32);
                                cell.Style.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                            }
                            c++;
                        }
                    }
                    r++;
                }
            }
        }


        private void SetStatus(string text, bool isAlert)
        {
            _lblStatusBanner.Text = text;
            _lblStatusBanner.ForeColor = isAlert ? Color.FromArgb(183, 28, 28) : Color.FromArgb(27, 94, 32);
        }
        private void OpenBorrowDialog()
        {
            AudioHelper.PlayMenuClick();
            using (var dlg = new BorrowForm(_equipmentController, _borrowingController, _borrowers))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshAllViews();
                    SetStatus("New borrow slip issued.", false);
                }
            }
        }

        private void OpenReturnDialog()
        {
            AudioHelper.PlayMenuClick();
            using (var dlg = new ReturnForm(_borrowingController))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshAllViews();
                    SetStatus("Equipment return processed.", false);
                }
            }
        }

        private void OpenAddEquipmentDialog()
        {
            AudioHelper.PlayMenuClick();
            using (var dlg = new AddEquipmentForm(_equipmentController))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshAllViews();
                    SetStatus("New equipment registered in inventory.", false);
                }
            }
        }
    }
}
