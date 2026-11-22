using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolBorrowingSystem.UI.Dialogs
{
    /// <summary>
    /// Shared look for the small "Add ..." pop-up windows.
    /// Child classes call AddField(...) for each input and override TrySave().
    /// Errors are shown in red inside the window instead of in a separate message box.
    /// </summary>
    public class BaseDialog : Form
    {
        protected readonly TableLayoutPanel Fields;
        private readonly Label _errorLabel;
        private readonly Button _saveButton;

        public BaseDialog()
            : this("Dialog", "Save")
        {
        }

        public BaseDialog(string title, string saveText)
        {
            Text = title;
            Font = Theme.NormalFont;
            BackColor = Theme.CardBackground;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = Theme.ScalePadding(24, 20, 24, 16);

            Label heading = Theme.MakeLabel(title, Theme.SectionFont, Theme.TextDark);
            heading.Margin = Theme.ScalePadding(0, 0, 0, 14);

            // Two columns: label on the left, input on the right
            Fields = new TableLayoutPanel();
            Fields.AutoSize = true;
            Fields.ColumnCount = 2;
            Fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Theme.Scale(150)));
            Fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Theme.Scale(280)));

            _errorLabel = Theme.MakeLabel("", Theme.SmallFont, Theme.Danger);
            _errorLabel.MaximumSize = new Size(Theme.Scale(430), 0);
            _errorLabel.Margin = Theme.ScalePadding(0, 6, 0, 6);

            _saveButton = Theme.MakeButton(saveText, Theme.Primary);
            _saveButton.Click += SaveButton_Click;

            Button cancelButton = Theme.MakeSecondaryButton("Cancel");
            cancelButton.DialogResult = DialogResult.Cancel;

            FlowLayoutPanel buttons = new FlowLayoutPanel();
            buttons.AutoSize = true;
            buttons.FlowDirection = FlowDirection.RightToLeft;
            buttons.Dock = DockStyle.Fill;
            buttons.Margin = Theme.ScalePadding(0, 8, 0, 0);
            cancelButton.Margin = new Padding(0);
            buttons.Controls.Add(cancelButton);
            buttons.Controls.Add(_saveButton);

            // Enter = save, Esc = cancel
            AcceptButton = _saveButton;
            CancelButton = cancelButton;

            TableLayoutPanel root = new TableLayoutPanel();
            root.AutoSize = true;
            root.ColumnCount = 1;
            root.Controls.Add(heading, 0, 0);
            root.Controls.Add(Fields, 0, 1);
            root.Controls.Add(_errorLabel, 0, 2);
            root.Controls.Add(buttons, 0, 3);
            Controls.Add(root);
        }

        /// <summary>Adds one "Label: [input]" row.</summary>
        protected void AddField(string labelText, Control input)
        {
            Label label = Theme.MakeLabel(labelText, Theme.NormalFont, Theme.TextDark);
            label.Anchor = AnchorStyles.Left;
            label.Margin = Theme.ScalePadding(0, 6, 8, 6);

            input.Font = Theme.NormalFont;
            input.Margin = Theme.ScalePadding(0, 4, 0, 4);
            if (!(input is CheckBox) && !(input is FlowLayoutPanel))
            {
                input.Width = Theme.Scale(280);
            }

            Fields.Controls.Add(label, 0, Fields.RowCount);
            Fields.Controls.Add(input, 1, Fields.RowCount);
            Fields.RowCount++;
        }

        protected void ShowError(string message)
        {
            _errorLabel.Text = message;
            AudioHelper.PlayError();
        }

        /// <summary>
        /// Child classes check the inputs and save here.
        /// Return true to close the window, or call ShowError(...) and return false.
        /// </summary>
        protected virtual bool TrySave()
        {
            return true;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            _errorLabel.Text = "";
            if (TrySave())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
