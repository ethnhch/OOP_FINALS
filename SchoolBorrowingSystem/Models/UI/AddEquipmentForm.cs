using System;
using System.Drawing;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Models;

namespace SchoolBorrowingSystem.UI
{
    public partial class AddEquipmentForm : Form
    {
        private readonly EquipmentController _equipmentController;

        public AddEquipmentForm()
        {
            InitializeComponent();
        }

        public AddEquipmentForm(EquipmentController equipmentController)
            : this()
        {
            _equipmentController = equipmentController;
            UpdatePreview();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            GtaPalette.EnableDarkTitleBar(this);
        }

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isAv = cmbCategory.SelectedIndex == 0;

            chkRemote.Visible = isAv;
            chkPowerCord.Visible = isAv;
            chkHdmi.Visible = isAv;
            lblLabSpecs.Visible = !isAv;

            cardMain.AccentColor = isAv ? GtaPalette.ViceTeal : GtaPalette.ViceMagenta;
            lblSpecTitle.ForeColor = isAv ? GtaPalette.ViceTeal : GtaPalette.ViceMagenta;
            lblSpecTitle.Text = isAv
                ? "INCLUDED AV ACCESSORIES (POLYMORPHIC CHECKLIST)"
                : "LAB EQUIPMENT TELEMETRY & SAFETY PROFILE";
            lblDuration.Text = isAv
                ? "MAX BORROW DURATION (HOURS)"
                : "MAX BORROW DURATION (DAYS)";
            txtTag.PlaceholderText = isAv ? "AV-103" : "LAB-203";

            numDuration.Value = isAv ? 4 : 7;
            UpdatePreview();

            if (Visible) AudioHelper.PlayMenuClick();
        }

        private void BtnAutoTag_Click(object sender, EventArgs e)
        {
            AudioHelper.PlayMenuClick();
            bool isAv = cmbCategory.SelectedIndex == 0;
            string prefix = isAv ? "AV-" : "LAB-";
            int nextNum = isAv ? 101 : 201;

            if (_equipmentController != null)
            {
                while (_equipmentController.FindEquipmentByTag(prefix + nextNum) != null)
                {
                    nextNum++;
                }
            }

            txtTag.Text = prefix + nextNum;
            lblInlineAlert.Visible = false;
            txtName.Focus();
        }

        private void Input_Changed(object sender, EventArgs e)
        {
            lblInlineAlert.Visible = false;
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            string tag = string.IsNullOrWhiteSpace(txtTag.Text) ? "NEW TAG" : txtTag.Text.Trim().ToUpperInvariant();
            string name = string.IsNullOrWhiteSpace(txtName.Text) ? "UNNAMED UNIT" : txtName.Text.Trim();
            if (name.Length > 22) name = name.Substring(0, 22) + "...";

            string unit = cmbCategory.SelectedIndex == 0 ? "HRS" : "DAYS";
            lblPreview.Text = string.Format("[{0}] {1}  //  {2} {3}", tag, name.ToUpperInvariant(), (int)numDuration.Value, unit);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string tag = txtTag.Text.Trim();
            string name = txtName.Text.Trim();
            int duration = (int)numDuration.Value;

            if (string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(name))
            {
                AudioHelper.PlayError();
                lblInlineAlert.Text = "ALERT  //  ENTER BOTH A PROPERTY TAG AND AN EQUIPMENT NAME.";
                lblInlineAlert.Visible = true;
                return;
            }

            try
            {
                SchoolEquipment newItem;
                if (cmbCategory.SelectedIndex == 0)
                {
                    newItem = new AVEquipment(tag, name, chkRemote.Checked, chkPowerCord.Checked, chkHdmi.Checked, duration);
                }
                else
                {
                    newItem = new LabEquipment(tag, name, DateTime.Now, 3, duration);
                }

                _equipmentController.AddEquipment(newItem);
                AudioHelper.PlaySuccess();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                lblInlineAlert.Text = "ERROR  //  " + ex.Message.ToUpperInvariant();
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