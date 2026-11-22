using System;
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
        }

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isAv = cmbCategory.SelectedIndex == 0;
            chkRemote.Visible = isAv;
            chkPowerCord.Visible = isAv;
            chkHdmi.Visible = isAv;
            numDuration.Value = isAv ? 4 : 7;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string tag = txtTag.Text.Trim();
            string name = txtName.Text.Trim();
            int duration = (int)numDuration.Value;

            if (string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(name))
            {
                AudioHelper.PlayError();
                MessageBox.Show("Please enter both a Property Tag and an Equipment Name.", "Missing Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show(string.Format("Equipment '{0}' ({1}) added to inventory!", name, tag), "Equipment Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show(ex.Message, "Error Adding Equipment", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            AudioHelper.PlayMenuClick();
            this.Close();
        }
    }
}
