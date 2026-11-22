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

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            GtaPalette.EnableDarkTitleBar(this);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            UpdateClockDisplay();
            _clockTimer.Start();
            AudioHelper.PlayStartup();
            _toastBanner.ShowBanner("SYSTEM ONLINE", "Welcome to Vice Campus Equipment Command.", GtaPalette.ViceTeal);
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            UpdateClockDisplay();
        }

        private void UpdateClockDisplay()
        {
            _lblClock.Text = "VICE TIME  //  " + DateTime.Now.ToString("hh:mm:ss tt");
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control) return;

            if (e.KeyCode == Keys.D1) SwitchTab(0);
            else if (e.KeyCode == Keys.D2) SwitchTab(1);
            else if (e.KeyCode == Keys.D3) SwitchTab(2);
            else if (e.KeyCode == Keys.D4) SwitchTab(3);
            else if (e.KeyCode == Keys.D5) SwitchTab(4);
        }

        private void BtnNav_Click(object sender, EventArgs e)
        {
            if (sender == btnNavInventory) SwitchTab(0);
            else if (sender == btnNavBorrow) SwitchTab(1);
            else if (sender == btnNavReturn) SwitchTab(2);
            else if (sender == btnNavBorrowers) SwitchTab(3);
            else if (sender == btnNavQa) SwitchTab(4);
        }

        private void SwitchTab(int index)
        {
            if (tabs.SelectedIndex != index)
                tabs.SelectedIndex = index;

            SyncNavButtons(index);
        }

        private void SyncNavButtons(int activeIndex)
        {
            btnNavInventory.IsActive = (activeIndex == 0);
            btnNavBorrow.IsActive = (activeIndex == 1);
            btnNavReturn.IsActive = (activeIndex == 2);
            btnNavBorrowers.IsActive = (activeIndex == 3);
            btnNavQa.IsActive = (activeIndex == 4);
        }

        private void BtnMute_Click(object sender, EventArgs e)
        {
            AudioHelper.IsMuted = !AudioHelper.IsMuted;
            _btnMute.Text = AudioHelper.IsMuted ? "AUDIO: MUTED" : "AUDIO: ON";
            _btnMute.AccentColor = AudioHelper.IsMuted ? GtaPalette.StatusWanted : GtaPalette.StatusClear;
            if (!AudioHelper.IsMuted) AudioHelper.PlayMenuClick();
        }

        private void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            SyncNavButtons(tabs.SelectedIndex);
            AudioHelper.PlayMenuClick();
        }

        private void ChkAvailableOnly_CheckedChanged(object sender, EventArgs e)
        {
            AudioHelper.PlayMenuClick();
            RefreshEquipmentGrid();
        }

        private void TxtSearchEquipment_TextChanged(object sender, EventArgs e)
        {
            RefreshEquipmentGrid();
        }

        private void TxtSearchBorrowers_TextChanged(object sender, EventArgs e)
        {
            RefreshBorrowersGrid();
        }

        private void BtnAddEquipment_Click(object sender, EventArgs e)
        {
            OpenAddEquipmentDialog();
        }

        private void DgvEquipment_SelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectedAssetDossier();
        }

        private void DgvBorrowers_SelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectedBorrowerProfile();
        }

        private void UpdateSelectedAssetDossier()
        {
            if (_equipmentController == null) return;

            string tag = GetSelectedFirstCell(_dgvEquipment);
            if (string.IsNullOrEmpty(tag))
            {
                _assetDossier.SetAsset(null, null, null, null, false, true, null);
                return;
            }

            var eq = _equipmentController.FindEquipmentByTag(tag);
            if (eq == null)
            {
                _assetDossier.SetAsset(null, null, null, null, false, true, null);
                return;
            }

            bool isAv = eq is AVEquipment;
            _assetDossier.SetAsset(
                eq.PropertyTag,
                eq.ItemName,
                isAv ? "AV Equipment" : "Lab Equipment",
                eq.Condition,
                eq.IsAvailable,
                isAv,
                eq.GetReturnChecklist());

            cardInvDossier.AccentColor = eq.IsAvailable ? (isAv ? GtaPalette.ViceTeal : GtaPalette.ViceMagenta) : GtaPalette.StatusWanted;
            btnQuickDispatch.AccentColor = eq.IsAvailable ? GtaPalette.StatusClear : GtaPalette.BorderSubtle;
        }

        private void UpdateSelectedBorrowerProfile()
        {
            if (_borrowers == null || _borrowingController == null) return;

            string id = GetSelectedFirstCell(_dgvBorrowers);
            if (string.IsNullOrEmpty(id))
            {
                _borrowerProfile.SetBorrower(null, null, null, null, false, 0);
                return;
            }

            SchoolBorrower found = null;
            foreach (var b in _borrowers)
            {
                if (string.Equals(b.SchoolIDNumber, id, StringComparison.OrdinalIgnoreCase))
                {
                    found = b;
                    break;
                }
            }

            if (found == null) return;

            int activeCount = 0;
            foreach (var slip in _borrowingController.GetActiveSlips())
            {
                if (slip.Borrower != null && string.Equals(slip.Borrower.SchoolIDNumber, found.SchoolIDNumber, StringComparison.OrdinalIgnoreCase))
                    activeCount++;
            }

            _borrowerProfile.SetBorrower(found.SchoolIDNumber, found.FullName, found.Role, found.Department, found.HasClearanceHold, activeCount);
            cardBorProfile.AccentColor = found.HasClearanceHold ? GtaPalette.StatusWanted : GtaPalette.StatusClear;
        }

        private void BtnQuickDispatch_Click(object sender, EventArgs e)
        {
            string tag = GetSelectedFirstCell(_dgvEquipment);
            if (string.IsNullOrEmpty(tag))
            {
                AudioHelper.PlayError();
                _toastBanner.ShowBanner("NO ASSET SELECTED", "Select an available equipment row first.", GtaPalette.StatusWanted);
                return;
            }

            var eq = _equipmentController.FindEquipmentByTag(tag);
            if (eq == null || !eq.IsAvailable)
            {
                AudioHelper.PlayWarning();
                _toastBanner.ShowBanner("UNIT UNAVAILABLE", tag + " is currently checked out or under repair.", GtaPalette.StatusWanted);
                return;
            }

            SwitchTab(1);
            for (int i = 0; i < _cmbAvailableEquipment.Items.Count; i++)
            {
                if (_cmbAvailableEquipment.Items[i].ToString().StartsWith(tag + " ", StringComparison.OrdinalIgnoreCase))
                {
                    _cmbAvailableEquipment.SelectedIndex = i;
                    break;
                }
            }
            _toastBanner.ShowBanner("DISPATCH LOADED", tag + " pre-selected in Borrow Request.", GtaPalette.ViceTeal);
        }

        private void CmbBorrowers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_borrowers == null || _cmbBorrowers.SelectedIndex < 0) return;

            var b = GetBorrowerAt(_borrowers, _cmbBorrowers.SelectedIndex);
            if (b == null) return;

            if (b.HasClearanceHold)
            {
                _lblBorrowClearance.Text = string.Format("★★★ ACTIVE CLEARANCE HOLD  //  {0} • DISPATCH LOCKED", b.Department.ToUpperInvariant());
                _lblBorrowClearance.ForeColor = GtaPalette.StatusWanted;
                grpRequest.AccentColor = GtaPalette.StatusWanted;
                btnIssueSlip.AccentColor = GtaPalette.StatusWanted;
            }
            else
            {
                _lblBorrowClearance.Text = string.Format("✓ CLEARANCE VERIFIED  //  {0} • {1}", b.Department.ToUpperInvariant(), b.Role.ToUpperInvariant());
                _lblBorrowClearance.ForeColor = GtaPalette.StatusClear;
                grpRequest.AccentColor = GtaPalette.ViceMagenta;
                btnIssueSlip.AccentColor = GtaPalette.StatusClear;
            }
        }

        private void BtnRoomPreset_Click(object sender, EventArgs e)
        {
            AudioHelper.PlayMenuClick();
            if (sender == btnRoomPreset1) _txtRoom.Text = "Room 304 - Lecture Hall";
            else if (sender == btnRoomPreset2) _txtRoom.Text = "Science Lab 201";
            else if (sender == btnRoomPreset3) _txtRoom.Text = "AVR Auditorium";
        }

        private void BtnCopyReceipt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtBorrowReceipt.Text)) return;
            try
            {
                Clipboard.SetText(_txtBorrowReceipt.Text);
                AudioHelper.PlayMenuClick();
                _toastBanner.ShowBanner("RECEIPT COPIED", "Official slip text copied to clipboard.", GtaPalette.SunsetPeach);
            }
            catch { }
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
                _toastBanner.ShowBanner("INVENTORY SAVED", "Synced safely to school_inventory.txt.", GtaPalette.StatusClear);
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                _toastBanner.ShowBanner("SAVE FAILED", ex.Message, GtaPalette.StatusWanted);
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
                _toastBanner.ShowBanner("INVENTORY LOADED", "Registry synced from school_inventory.txt.", GtaPalette.ViceTeal);
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                _toastBanner.ShowBanner("LOAD FAILED", ex.Message, GtaPalette.StatusWanted);
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
                _toastBanner.ShowBanner("NO TARGET SELECTED", "Select an equipment row first.", GtaPalette.StatusWanted);
                return;
            }

            try
            {
                _equipmentController.MarkForRepair(tag);
                RefreshAllViews();
                AudioHelper.PlayWarning();
                SetStatus(string.Format("Equipment '{0}' marked as Under Repair.", tag), true);
                _toastBanner.ShowBanner("UNIT FLAGGED FOR REPAIR", tag + " pulled from active circulation.", GtaPalette.SunsetPeach);
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
                _toastBanner.ShowBanner("NO TARGET SELECTED", "Select an equipment row first.", GtaPalette.StatusWanted);
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
                _toastBanner.ShowBanner("REPAIR COMPLETE", tag + " restored to Good / Available status.", GtaPalette.StatusClear);
            }
        }

        private void BtnIssueSlip_Click(object sender, EventArgs e)
        {
            if (_cmbBorrowers.SelectedIndex < 0 || _cmbAvailableEquipment.SelectedIndex < 0)
            {
                AudioHelper.PlayError();
                _toastBanner.ShowBanner("MISSING SELECTION", "Choose both a borrower and available equipment.", GtaPalette.StatusWanted);
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
                    "======================================================\r\n" +
                    "    LEONIDA CAMPUS  //  OFFICIAL BORROW SLIP ISSUED   \r\n" +
                    "======================================================\r\n" +
                    string.Format("SLIP ID     : {0}\r\n", slip.SlipID) +
                    string.Format("BORROWER    : {0} ({1})\r\n", borrower.FullName, borrower.SchoolIDNumber) +
                    string.Format("DEPARTMENT  : {0} ({1})\r\n", borrower.Department, borrower.Role) +
                    string.Format("EQUIPMENT   : {0} [{1}]\r\n", item.ItemName, item.PropertyTag) +
                    string.Format("ROOM / LAB  : {0}\r\n", slip.RoomToBeUsed) +
                    string.Format("BORROWED AT : {0:yyyy-MM-dd HH:mm}\r\n", slip.TimeBorrowed) +
                    string.Format("DUE TIME    : {0:yyyy-MM-dd HH:mm}\r\n", slip.DueTime) +
                    "------------------------------------------------------\r\n" +
                    "POLYMORPHIC RETURN CHECKLIST:\r\n" +
                    string.Join("\r\n", checklist.ConvertAll(c => "  [ ] " + c)) +
                    "\r\n======================================================";

                RefreshAllViews();
                SetStatus(string.Format("BorrowSlip {0} issued to {1}.", slip.SlipID, borrower.FullName), false);
                _toastBanner.ShowBanner("SLIP ISSUED  //  DISPATCHED", string.Format("{0} checked out to {1}.", item.PropertyTag, borrower.FullName), GtaPalette.StatusClear);
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                SetStatus(string.Format("Borrowing Rejected: {0}", ex.Message), true);
                _toastBanner.ShowBanner("DISPATCH BLOCKED", ex.Message, GtaPalette.StatusWanted);
                MessageBox.Show(ex.Message, "Borrowing Rejected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnProcessReturn_Click(object sender, EventArgs e)
        {
            string slipId = GetSelectedFirstCell(_dgvActiveSlips);
            if (string.IsNullOrEmpty(slipId))
            {
                AudioHelper.PlayError();
                _toastBanner.ShowBanner("NO SLIP SELECTED", "Select an active slip from the top table.", GtaPalette.StatusWanted);
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
                    _toastBanner.ShowBanner("PENALTY  //  CLEARANCE HOLD", string.Format("Slip {0}: PHP {1:N2} fine applied.", slipId, penalty), GtaPalette.StatusWanted);
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
                    _toastBanner.ShowBanner("RETURN COMPLETE", string.Format("Slip {0} cleared with PHP 0.00 penalty.", slipId), GtaPalette.StatusClear);
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
                _toastBanner.ShowBanner("NO BORROWER SELECTED", "Select a borrower row first.", GtaPalette.StatusWanted);
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
                _toastBanner.ShowBanner("★★★ CLEARANCE HOLD APPLIED", borrower.FullName + " is now locked from borrowing.", GtaPalette.StatusWanted);
            }
            else
            {
                _borrowingController.LiftClearanceHold(borrower);
                AudioHelper.PlaySuccess();
                SetStatus(string.Format("Clearance hold lifted for {0}.", borrower.FullName), false);
                _toastBanner.ShowBanner("CLEARANCE RESTORED", borrower.FullName + " is back in good standing.", GtaPalette.StatusClear);
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
                    _qaMeter.Percent = 100;
                    _qaMeter.AccentColor = GtaPalette.StatusClear;
                    _qaMeter.StatusText = "MISSION PASSED  //  44 / 44 OOP ASSERTIONS VERIFIED (100%)";
                    SetStatus("All 44 Automated QA Unit & Integration Tests PASSED!", false);
                    _toastBanner.ShowBanner("ALL 44 QA TESTS PASSED", "OOP encapsulation, inheritance & rules verified.", GtaPalette.ViceMagenta);
                }
                else
                {
                    AudioHelper.PlayError();
                    _qaMeter.Percent = 45;
                    _qaMeter.AccentColor = GtaPalette.StatusWanted;
                    _qaMeter.StatusText = "QA FAILURE DETECTED  //  INSPECT ASSERTION CONSOLE";
                    SetStatus("Some QA tests failed. Check output.", true);
                    _toastBanner.ShowBanner("QA TESTS FAILED", "Review console log for failed assertions.", GtaPalette.StatusWanted);
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
            UpdateHudTelemetry();
            UpdateSelectedAssetDossier();
            UpdateSelectedBorrowerProfile();
        }

        private void UpdateHudTelemetry()
        {
            int totalGear = 0;
            int availableCount = 0;
            int unavailableCount = 0;
            foreach (var eq in _equipmentController.GetAllEquipment())
            {
                totalGear++;
                if (eq.IsAvailable) availableCount++;
                else unavailableCount++;
            }

            int activeSlipCount = 0;
            foreach (var _ in _borrowingController.GetActiveSlips()) activeSlipCount++;

            int totalBorrowers = 0;
            int holdCount = 0;
            foreach (var b in _borrowers)
            {
                totalBorrowers++;
                if (b.HasClearanceHold) holdCount++;
            }

            int gearDiv = Math.Max(1, totalGear);
            int borDiv = Math.Max(1, totalBorrowers);

            hudAvailable.ValueText = availableCount.ToString();
            hudAvailable.FillPercent = (availableCount * 100) / gearDiv;

            hudActiveSlips.ValueText = activeSlipCount.ToString();
            hudActiveSlips.FillPercent = activeSlipCount == 0 ? 4 : Math.Min(100, (activeSlipCount * 100) / gearDiv);

            hudInRepair.ValueText = unavailableCount.ToString();
            hudInRepair.FillPercent = unavailableCount == 0 ? 4 : (unavailableCount * 100) / gearDiv;

            hudHolds.ValueText = holdCount.ToString();
            hudHolds.FillPercent = holdCount == 0 ? 4 : (holdCount * 100) / borDiv;
        }

        private void RefreshEquipmentGrid()
        {
            if (_equipmentController == null) return;

            _dgvEquipment.Rows.Clear();
            string query = _txtSearchEquipment != null ? _txtSearchEquipment.SearchText.Trim() : string.Empty;

            var items = _chkAvailableOnly.Checked
                ? _equipmentController.GetAvailableEquipment()
                : _equipmentController.GetAllEquipment();

            foreach (var eq in items)
            {
                string category = eq is AVEquipment ? "AV Equipment" : "Lab Equipment";
                if (!string.IsNullOrEmpty(query))
                {
                    bool match = eq.PropertyTag.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 eq.ItemName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 category.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 eq.Condition.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
                    if (!match) continue;
                }

                _dgvEquipment.Rows.Add(
                    eq.PropertyTag,
                    category,
                    eq.ItemName,
                    eq.Condition,
                    eq.IsAvailable ? "AVAILABLE" : "UNAVAILABLE",
                    eq.ToString()
                );
            }

            UpdateSelectedAssetDossier();
        }

        private void RefreshBorrowDropdowns()
        {
            _cmbBorrowers.Items.Clear();
            foreach (var b in _borrowers)
            {
                string holdTag = b.HasClearanceHold ? " (★ HOLD)" : " (CLEAR)";
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
                    _lstChecklistPreview.Items.Add("▸  " + item);
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
            if (_borrowers == null) return;

            _dgvBorrowers.Rows.Clear();
            string query = _txtSearchBorrowers != null ? _txtSearchBorrowers.SearchText.Trim() : string.Empty;

            foreach (var b in _borrowers)
            {
                if (!string.IsNullOrEmpty(query))
                {
                    bool match = b.SchoolIDNumber.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 b.FullName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 b.Department.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 b.Role.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
                    if (!match) continue;
                }

                _dgvBorrowers.Rows.Add(
                    b.SchoolIDNumber,
                    b.FullName,
                    b.Role,
                    b.Department,
                    b.HasClearanceHold ? "★★★ HOLD" : "✓ CLEAR"
                );
            }

            UpdateSelectedBorrowerProfile();
        }

        private void SetStatus(string text, bool isAlert)
        {
            _lblStatusBanner.Text = (isAlert ? "ALERT  //  " : "STATUS  //  ") + text.ToUpperInvariant();
            _lblStatusBanner.ForeColor = isAlert ? GtaPalette.StatusWanted : GtaPalette.ViceTeal;
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
                    _toastBanner.ShowBanner("NEW GEAR REGISTERED", "Inventory catalog and HUD updated.", GtaPalette.StatusClear);
                }
            }
        }
    }
}