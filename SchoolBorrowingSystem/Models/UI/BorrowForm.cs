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

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            GtaPalette.EnableDarkTitleBar(this);
        }

        private void LoadDropdowns()
        {
            if (_borrowers == null || _equipmentController == null) return;

            cmbBorrowers.Items.Clear();
            foreach (var b in _borrowers)
            {
                string hold = b.HasClearanceHold ? " (★ HOLD)" : " (CLEAR)";
                cmbBorrowers.Items.Add(string.Format("{0} - {1} ({2}){3}", b.SchoolIDNumber, b.FullName, b.Role, hold));
            }
            if (cmbBorrowers.Items.Count > 0) cmbBorrowers.SelectedIndex = 0;

            cmbEquipment.Items.Clear();
            foreach (var eq in _equipmentController.GetAvailableEquipment())
            {
                cmbEquipment.Items.Add(string.Format("{0} - {1}", eq.PropertyTag, eq.ItemName));
            }
            if (cmbEquipment.Items.Count > 0)
            {
                cmbEquipment.SelectedIndex = 0;
            }
            else
            {
                lstChecklist.Items.Clear();
                lblEquipMeta.Text = "NO AVAILABLE UNITS IN INVENTORY";
                lblEquipMeta.ForeColor = GtaPalette.StatusWanted;
            }
        }

        private void CmbBorrowers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_borrowers == null || cmbBorrowers.SelectedIndex < 0) return;

            lblInlineAlert.Visible = false;
            SchoolBorrower b = GetSelectedBorrower();
            if (b == null) return;

            if (b.HasClearanceHold)
            {
                lblClearanceBadge.Text = string.Format("★★★ ACTIVE CLEARANCE HOLD  //  {0} • DISPATCH LOCKED", b.Department.ToUpperInvariant());
                lblClearanceBadge.ForeColor = GtaPalette.StatusWanted;
                cardDispatch.AccentColor = GtaPalette.StatusWanted;
                btnIssue.AccentColor = GtaPalette.StatusWanted;
            }
            else
            {
                lblClearanceBadge.Text = string.Format("✓ CLEARANCE VERIFIED  //  {0} • {1}", b.Department.ToUpperInvariant(), b.Role.ToUpperInvariant());
                lblClearanceBadge.ForeColor = GtaPalette.StatusClear;
                cardDispatch.AccentColor = GtaPalette.ViceMagenta;
                btnIssue.AccentColor = GtaPalette.StatusClear;
            }
        }

        private void CmbEquipment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_equipmentController == null) return;

            lblInlineAlert.Visible = false;
            lstChecklist.Items.Clear();

            SchoolEquipment selected = GetSelectedEquipment();
            if (selected == null) return;

            var checklist = selected.GetReturnChecklist();
            foreach (var item in checklist)
            {
                lstChecklist.Items.Add("▸  " + item);
            }

            string cat = selected is AVEquipment ? "AV EQUIPMENT" : "LAB EQUIPMENT";
            lblEquipMeta.ForeColor = GtaPalette.ViceTeal;
            lblEquipMeta.Text = string.Format("CATEGORY: {0}  //  CONDITION: {1}  //  {2} CHECKLIST ITEMS",
                cat, selected.Condition.ToUpperInvariant(), checklist.Count);
        }

        private void BtnRoomPreset_Click(object sender, EventArgs e)
        {
            AudioHelper.PlayMenuClick();
            if (sender == btnRoomLecture) txtRoom.Text = "Room 304 - Lecture Hall";
            else if (sender == btnRoomLab) txtRoom.Text = "Science Lab 201";
            else if (sender == btnRoomAvr) txtRoom.Text = "AVR Auditorium";
            lblInlineAlert.Visible = false;
        }

        private SchoolBorrower GetSelectedBorrower()
        {
            if (_borrowers == null) return null;
            int idx = 0;
            foreach (var b in _borrowers)
            {
                if (idx == cmbBorrowers.SelectedIndex) return b;
                idx++;
            }
            return null;
        }

        private SchoolEquipment GetSelectedEquipment()
        {
            if (_equipmentController == null) return null;
            int idx = 0;
            foreach (var eq in _equipmentController.GetAvailableEquipment())
            {
                if (idx == cmbEquipment.SelectedIndex) return eq;
                idx++;
            }
            return null;
        }

        private void BtnIssue_Click(object sender, EventArgs e)
        {
            if (cmbBorrowers.SelectedIndex < 0 || cmbEquipment.SelectedIndex < 0)
            {
                AudioHelper.PlayError();
                lblInlineAlert.Text = "ALERT  //  SELECT BOTH A BORROWER AND AN AVAILABLE EQUIPMENT UNIT.";
                lblInlineAlert.Visible = true;
                return;
            }

            SchoolBorrower selectedBorrower = GetSelectedBorrower();
            SchoolEquipment selectedItem = GetSelectedEquipment();
            if (selectedBorrower == null || selectedItem == null) return;

            try
            {
                var slip = _borrowingController.ProcessBorrow(selectedBorrower, selectedItem, txtRoom.Text);
                AudioHelper.PlaySuccess();
                MessageBox.Show(
                    string.Format("OFFICIAL BORROW SLIP ISSUED\n\nSlip ID: {0}\nBorrower: {1}\nEquipment: {2} ({3})\nDue: {4:yyyy-MM-dd HH:mm}",
                        slip.SlipID, selectedBorrower.FullName, selectedItem.ItemName, selectedItem.PropertyTag, slip.DueTime),
                    "Dispatch Confirmed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                lblInlineAlert.Text = "DISPATCH BLOCKED  //  " + ex.Message.ToUpperInvariant();
                lblInlineAlert.Visible = true;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            AudioHelper.PlayMenuClick();
            this.Close();
        }
    }
}