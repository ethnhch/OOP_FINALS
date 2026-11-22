using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Transactions;

namespace SchoolBorrowingSystem.UI
{
    public partial class ReturnForm : Form
    {
        private readonly BorrowingController _borrowingController;
        private bool _syncingChecklist = false;

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

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            GtaPalette.EnableDarkTitleBar(this);
        }

        private void LoadActiveSlips()
        {
            if (_borrowingController == null) return;

            cmbActiveSlips.Items.Clear();
            foreach (var slip in _borrowingController.GetActiveSlips())
            {
                string borrowerName = slip.Borrower != null ? slip.Borrower.FullName : "Unknown";
                string tag = slip.Equipment != null ? slip.Equipment.PropertyTag : "";
                string itemName = slip.Equipment != null ? slip.Equipment.ItemName : "Unknown";
                cmbActiveSlips.Items.Add(string.Format("{0} | {1} - {2} ({3})", slip.SlipID, tag, itemName, borrowerName));
            }

            if (cmbActiveSlips.Items.Count > 0)
            {
                cmbActiveSlips.SelectedIndex = 0;
            }
            else
            {
                lstChecklist.Items.Clear();
                lblSlipMeta.Text = "NO ACTIVE BORROW SLIPS CURRENTLY OUT";
                lblSlipMeta.ForeColor = GtaPalette.TextSecondary;
                lblRiskBanner.Text = "STANDBY  //  ALL EQUIPMENT ACCOUNTED FOR";
                lblRiskBanner.ForeColor = GtaPalette.ViceTeal;
            }
        }

        private BorrowSlip GetSelectedSlip()
        {
            if (_borrowingController == null || cmbActiveSlips.SelectedIndex < 0) return null;

            int idx = 0;
            foreach (var slip in _borrowingController.GetActiveSlips())
            {
                if (idx == cmbActiveSlips.SelectedIndex) return slip;
                idx++;
            }
            return null;
        }

        private void CmbActiveSlips_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_borrowingController == null) return;

            lblInlineAlert.Visible = false;
            _syncingChecklist = true;
            lstChecklist.Items.Clear();

            BorrowSlip selected = GetSelectedSlip();
            if (selected != null)
            {
                if (selected.Equipment != null)
                {
                    foreach (var item in selected.Equipment.GetReturnChecklist())
                    {
                        lstChecklist.Items.Add(item, true);
                    }
                }

                chkChecklistIntact.Checked = true;

                bool overdue = selected.IsOverdue();
                string room = string.IsNullOrWhiteSpace(selected.RoomToBeUsed) ? "N/A" : selected.RoomToBeUsed.ToUpperInvariant();
                lblSlipMeta.Text = string.Format("ROOM: {0}  //  DUE: {1:MM/dd HH:mm}  //  {2}",
                    room, selected.DueTime, overdue ? "★★★ OVERDUE" : "ON TIME");
                lblSlipMeta.ForeColor = overdue ? GtaPalette.StatusWanted : GtaPalette.ViceTeal;
            }

            _syncingChecklist = false;
            UpdateRiskIndicator(chkChecklistIntact.Checked);
        }

        private void LstChecklist_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_syncingChecklist) return;

            // Compute whether all items will be checked after this click completes
            bool allChecked = true;
            for (int i = 0; i < lstChecklist.Items.Count; i++)
            {
                bool isChecked = (i == e.Index) ? (e.NewValue == CheckState.Checked) : lstChecklist.GetItemChecked(i);
                if (!isChecked)
                {
                    allChecked = false;
                    break;
                }
            }

            _syncingChecklist = true;
            chkChecklistIntact.Checked = allChecked;
            _syncingChecklist = false;

            UpdateRiskIndicator(allChecked);
        }

        private void ChkChecklistIntact_CheckedChanged(object sender, EventArgs e)
        {
            if (_syncingChecklist) return;

            _syncingChecklist = true;
            for (int i = 0; i < lstChecklist.Items.Count; i++)
            {
                lstChecklist.SetItemChecked(i, chkChecklistIntact.Checked);
            }
            _syncingChecklist = false;

            UpdateRiskIndicator(chkChecklistIntact.Checked);
        }

        private void CmbCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_borrowingController == null) return;
            UpdateRiskIndicator(chkChecklistIntact.Checked);
        }

        private void UpdateRiskIndicator(bool checklistIntact)
        {
            BorrowSlip selected = GetSelectedSlip();
            if (selected == null) return;

            string condition = cmbCondition.SelectedItem != null ? cmbCondition.SelectedItem.ToString() : "Good";
            bool isDamaged = condition == "Damaged" || condition == "Needs Repair";
            bool isOverdue = selected.IsOverdue();

            var flags = new List<string>();
            if (isOverdue) flags.Add("OVERDUE");
            if (!checklistIntact) flags.Add("MISSING PARTS");
            if (isDamaged) flags.Add(condition.ToUpperInvariant());

            if (flags.Count > 0)
            {
                lblRiskBanner.Text = "★★★ PENALTY & CLEARANCE HOLD RISK  //  " + string.Join(" + ", flags);
                lblRiskBanner.ForeColor = GtaPalette.StatusWanted;
                cardInspection.AccentColor = GtaPalette.StatusWanted;
                btnProcess.AccentColor = GtaPalette.StatusWanted;
            }
            else
            {
                lblRiskBanner.Text = "✓ CLEAN INSPECTION  //  GOOD STANDING • PHP 0.00 PENALTY EXPECTED";
                lblRiskBanner.ForeColor = GtaPalette.StatusClear;
                cardInspection.AccentColor = GtaPalette.SunsetPeach;
                btnProcess.AccentColor = GtaPalette.StatusClear;
            }
        }

        private void BtnProcess_Click(object sender, EventArgs e)
        {
            if (cmbActiveSlips.SelectedIndex < 0)
            {
                AudioHelper.PlayError();
                lblInlineAlert.Text = "ALERT  //  THERE ARE NO ACTIVE BORROW SLIPS TO RETURN.";
                lblInlineAlert.Visible = true;
                return;
            }

            BorrowSlip selectedSlip = GetSelectedSlip();
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
                lblInlineAlert.Text = "RETURN ERROR  //  " + ex.Message.ToUpperInvariant();
                lblInlineAlert.Visible = true;
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            AudioHelper.PlayMenuClick();
            this.Close();
        }
    }
}