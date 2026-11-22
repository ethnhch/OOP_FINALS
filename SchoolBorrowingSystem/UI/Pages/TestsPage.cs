using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;

namespace SchoolBorrowingSystem.UI.Pages
{
    /// <summary>
    /// Runs the automated tests from TestRunner.cs and shows the output.
    /// Handy for showing the teacher that all the business rules work.
    /// </summary>
    public class TestsPage : BasePage
    {
        private Label _resultLabel;
        private TextBox _outputBox;

        public TestsPage(EquipmentController equipment, BorrowingController borrowing, BorrowerController borrowers)
            : base("Automated Tests", "Checks every business rule (fines, holds, repairs, saving) using separate test data.", equipment, borrowing, borrowers)
        {
            BuildLayout();
        }

        private void BuildLayout()
        {
            FlowLayoutPanel toolbar = MakeToolbar();

            Button runButton = Theme.MakeButton("Run All Tests", Theme.Primary);
            runButton.Click += RunButton_Click;

            _resultLabel = Theme.MakeLabel("Not run yet.", Theme.SectionFont, Theme.TextMuted);
            _resultLabel.Margin = Theme.ScalePadding(8, 8, 0, 0);

            toolbar.Controls.Add(runButton);
            toolbar.Controls.Add(_resultLabel);

            _outputBox = new TextBox();
            _outputBox.Dock = DockStyle.Fill;
            _outputBox.Multiline = true;
            _outputBox.ReadOnly = true;
            _outputBox.ScrollBars = ScrollBars.Both;
            _outputBox.WordWrap = false;
            _outputBox.Font = Theme.MonoFont;
            _outputBox.BackColor = Theme.Sidebar;
            _outputBox.ForeColor = Color.FromArgb(134, 239, 172);
            _outputBox.BorderStyle = BorderStyle.None;
            _outputBox.Text = "Click 'Run All Tests' to start.";

            ContentArea.Controls.Add(_outputBox);
            ContentArea.Controls.Add(toolbar);
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            // TestRunner writes to Console, so we temporarily send Console output into a string
            StringWriter output = new StringWriter();
            TextWriter originalOutput = Console.Out;
            bool allPassed;

            try
            {
                Console.SetOut(output);
                allPassed = TestRunner.RunAllTests();
            }
            finally
            {
                Console.SetOut(originalOutput);
            }

            _outputBox.Text = output.ToString();

            int total = TestRunner.PassedCount + TestRunner.FailedCount;
            if (allPassed)
            {
                _resultLabel.Text = "All " + total + " tests passed";
                _resultLabel.ForeColor = Theme.Success;
                AudioHelper.PlayTestVictory();
                ShowStatus("All " + total + " automated tests passed.", false);
            }
            else
            {
                _resultLabel.Text = TestRunner.FailedCount + " of " + total + " tests failed";
                _resultLabel.ForeColor = Theme.Danger;
                AudioHelper.PlayError();
                ShowStatus(TestRunner.FailedCount + " test(s) failed. Look for [FAIL] in the output.", true);
            }
        }
    }
}
