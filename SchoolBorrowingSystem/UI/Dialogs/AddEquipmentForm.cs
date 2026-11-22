using System;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Models;

namespace SchoolBorrowingSystem.UI.Dialogs
{
    /// <summary>
    /// Pop-up for registering a new AV or Lab item.
    /// The fields change depending on which type is picked.
    /// </summary>
    public class AddEquipmentForm : BaseDialog
    {
        private readonly EquipmentController _equipmentController;

        private RadioButton _avRadio;
        private RadioButton _labRadio;
        private TextBox _tagBox;
        private TextBox _nameBox;
        private NumericUpDown _durationBox;
        private Label _durationUnit;

        // AV only
        private CheckBox _remoteCheck;
        private CheckBox _cordCheck;
        private CheckBox _hdmiCheck;

        // Lab only
        private NumericUpDown _componentsBox;
        private DateTimePicker _calibrationPicker;

        /// <summary>The item that was added (only set after the user clicks Save).</summary>
        public SchoolEquipment CreatedItem { get; private set; }

        public AddEquipmentForm()
            : this(new EquipmentController())
        {
        }

        public AddEquipmentForm(EquipmentController equipmentController)
            : base("Add Equipment", "Add to Inventory")
        {
            _equipmentController = equipmentController;
            BuildFields();
            UpdateFieldsForType();
        }

        private void BuildFields()
        {
            // Type: two radio buttons are easier to see than a drop-down
            _avRadio = new RadioButton();
            _avRadio.Text = "AV (hours)";
            _avRadio.AutoSize = true;
            _avRadio.Checked = true;
            _avRadio.CheckedChanged += delegate { UpdateFieldsForType(); };

            _labRadio = new RadioButton();
            _labRadio.Text = "Lab (days)";
            _labRadio.AutoSize = true;

            FlowLayoutPanel typeRow = new FlowLayoutPanel();
            typeRow.AutoSize = true;
            typeRow.Controls.Add(_avRadio);
            typeRow.Controls.Add(_labRadio);
            AddField("Type", typeRow);

            _tagBox = new TextBox();
            AddField("Property tag", _tagBox);

            _nameBox = new TextBox();
            _nameBox.PlaceholderText = "e.g. Epson Projector";
            AddField("Name", _nameBox);

            _durationBox = new NumericUpDown();
            _durationBox.Minimum = 1;
            _durationBox.Maximum = 60;
            _durationBox.Width = Theme.Scale(70);
            _durationUnit = Theme.MakeLabel("hours", Theme.NormalFont, Theme.TextMuted);
            _durationUnit.Margin = Theme.ScalePadding(6, 4, 0, 0);

            FlowLayoutPanel durationRow = new FlowLayoutPanel();
            durationRow.AutoSize = true;
            durationRow.Controls.Add(_durationBox);
            durationRow.Controls.Add(_durationUnit);
            AddField("Loan period", durationRow);

            // AV accessories
            _remoteCheck = MakeCheckBox("Comes with a remote", false);
            _cordCheck = MakeCheckBox("Comes with a power cord", true);
            _hdmiCheck = MakeCheckBox("Comes with an HDMI cable", true);
            AddField("Accessories", _remoteCheck);
            AddField("", _cordCheck);
            AddField("", _hdmiCheck);

            // Lab details
            _componentsBox = new NumericUpDown();
            _componentsBox.Minimum = 1;
            _componentsBox.Maximum = 100;
            _componentsBox.Value = 3;
            AddField("Number of parts", _componentsBox);

            _calibrationPicker = new DateTimePicker();
            _calibrationPicker.Format = DateTimePickerFormat.Short;
            _calibrationPicker.Value = DateTime.Today;
            AddField("Last calibrated", _calibrationPicker);
        }

        private CheckBox MakeCheckBox(string text, bool isChecked)
        {
            CheckBox box = new CheckBox();
            box.Text = text;
            box.Checked = isChecked;
            box.AutoSize = true;
            return box;
        }

        /// <summary>
        /// Shows only the fields that matter for the chosen type
        /// and suggests the next free property tag (AV-103, LAB-203, ...).
        /// </summary>
        private void UpdateFieldsForType()
        {
            bool isAv = _avRadio.Checked;

            _remoteCheck.Enabled = isAv;
            _cordCheck.Enabled = isAv;
            _hdmiCheck.Enabled = isAv;
            _componentsBox.Enabled = !isAv;
            _calibrationPicker.Enabled = !isAv;

            if (isAv)
            {
                _durationUnit.Text = "hours";
                _durationBox.Value = AVEquipment.DefaultHours;
                _tagBox.Text = SuggestNextTag("AV-", 101);
            }
            else
            {
                _durationUnit.Text = "days";
                _durationBox.Value = LabEquipment.DefaultDays;
                _tagBox.Text = SuggestNextTag("LAB-", 201);
            }
        }

        private string SuggestNextTag(string prefix, int startNumber)
        {
            int number = startNumber;
            while (_equipmentController.FindEquipmentByTag(prefix + number) != null)
            {
                number++;
            }
            return prefix + number;
        }

        protected override bool TrySave()
        {
            string tag = _tagBox.Text.Trim().ToUpper();
            string name = _nameBox.Text.Trim();
            int duration = (int)_durationBox.Value;

            if (tag.Length == 0)
            {
                ShowError("Please enter a property tag.");
                _tagBox.Focus();
                return false;
            }

            if (name.Length == 0)
            {
                ShowError("Please enter the equipment name.");
                _nameBox.Focus();
                return false;
            }

            SchoolEquipment newItem;
            if (_avRadio.Checked)
            {
                newItem = new AVEquipment(tag, name, _remoteCheck.Checked, _cordCheck.Checked, _hdmiCheck.Checked, duration);
            }
            else
            {
                newItem = new LabEquipment(tag, name, _calibrationPicker.Value, (int)_componentsBox.Value, duration);
            }

            try
            {
                _equipmentController.AddEquipment(newItem);
            }
            catch (Exception ex)
            {
                // e.g. the tag is already used
                ShowError(ex.Message);
                return false;
            }

            CreatedItem = newItem;
            return true;
        }
    }
}
