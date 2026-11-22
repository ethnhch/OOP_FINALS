using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SchoolBorrowingSystem.UI;

namespace SchoolBorrowingSystem.Models.UI
{
    public partial class GtaLoadingForm : Form
    {
        private float _elapsed = 0f;
        private int _lastSlide = -1;
        private bool _fadingOut = false;

        // Timings synchronized with AudioHelper.PlayGtaLoadingSequence()
        private const float StarsDuration = 1.85f;   // 0.00s - 1.85s: 6 Neon Stars
        private const float EmblemDuration = 1.75f;  // 1.85s - 3.60s: Glowing Studio Badge
        private const float SlideDuration = 2.10f;   // 4 Slides x 2.10s = 8.40s (Total: 12.0s)

        private readonly (string Tag, string Title, string Desc, string Status)[] _slides =
        {
            (
                "LEONIDA CAMPUS  //  EQUIPMENT INVENTORY",
                "AV & LABORATORY GEAR DISPATCH",
                "Track Epson laser projectors, microscopes, and oscilloscopes in real time—complete with polymorphic return checklists.",
                "Loading Inventory Registry"
            ),
            (
                "BORROWER PROTOCOL  //  CLEARANCE HOLDS",
                "KEEP YOUR STANDING CLEAR",
                "Borrowers flagged with ACTIVE (HOLD) are blocked from checking out equipment until their clearance hold is lifted.",
                "Verifying Borrower Clearances"
            ),
            (
                "ACTIVE SLIPS  //  RETURNS & REPAIRS",
                "CHECKLIST BEFORE CHECK-IN",
                "Inspect every component upon return or flag damaged hardware for repair to pull it from circulation instantly.",
                "Syncing Active Borrow Slips"
            ),
            (
                "AUTOMATED QA  //  44 OOP ASSERTIONS",
                "BUILT TO PASS EVERY TEST",
                "Launch all 44 automated assertions directly from the QA tab to validate inheritance, encapsulation, and borrowing rules.",
                "Entering Main Dashboard"
            )
        };

        public GtaLoadingForm()
        {
            InitializeComponent();

            // Allow clicking or pressing Space/Enter/Esc to skip smoothly during demos
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
                    BeginFadeOut();
            };
            gtaCanvas.Click += (s, e) => BeginFadeOut();
        }

        private void GtaLoadingForm_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            gtaCanvas.LoadOptionalCustomSlides();
            SetSlideHudVisible(false);
            this.Opacity = 0;

            AudioHelper.PlayGtaLoadingSequence();
            animTimer.Start();
        }

        private void animTimer_Tick(object sender, EventArgs e)
        {
            if (_fadingOut)
            {
                this.Opacity -= 0.08;
                if (this.Opacity <= 0.04)
                {
                    animTimer.Stop();
                    AudioHelper.StopGtaLoadingAudio();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                return;
            }

            if (this.Opacity < 1.0)
                this.Opacity = Math.Min(1.0, this.Opacity + 0.08);

            _elapsed += 0.02f; // 20ms per tick
            float totalDuration = StarsDuration + EmblemDuration + (_slides.Length * SlideDuration);
            int overallPct = Math.Min(100, (int)((_elapsed / totalDuration) * 100f));

            if (_elapsed < StarsDuration)
            {
                SetSlideHudVisible(false);
                gtaCanvas.UpdateFrame(GtaLoadingCanvas.IntroPhase.StarsIntro, _elapsed, 0, 0f, overallPct);
            }
            else if (_elapsed < StarsDuration + EmblemDuration)
            {
                SetSlideHudVisible(false);
                float emblemT = _elapsed - StarsDuration;
                gtaCanvas.UpdateFrame(GtaLoadingCanvas.IntroPhase.EmblemReveal, emblemT, 0, 0f, overallPct);
            }
            else
            {
                float slideElapsed = _elapsed - (StarsDuration + EmblemDuration);
                int idx = (int)(slideElapsed / SlideDuration);

                if (idx >= _slides.Length)
                {
                    BeginFadeOut();
                    return;
                }

                float slideProg = (slideElapsed % SlideDuration) / SlideDuration;
                SetSlideHudVisible(true);

                if (idx != _lastSlide)
                {
                    _lastSlide = idx;
                    lblSlideTag.Text = _slides[idx].Tag;
                    lblSlideTitle.Text = _slides[idx].Title;
                    lblSlideDesc.Text = _slides[idx].Desc;
                }

                lblStatus.Text = $"{_slides[idx].Status}  {overallPct}%";
                gtaCanvas.UpdateFrame(GtaLoadingCanvas.IntroPhase.ArtSlideshow, slideElapsed, idx, slideProg, overallPct);
            }
        }

        private void SetSlideHudVisible(bool visible)
        {
            if (lblSlideTag.Visible == visible) return;
            lblSlideTag.Visible = visible;
            lblSlideTitle.Visible = visible;
            lblSlideDesc.Visible = visible;
            lblStatus.Visible = visible;
        }

        private void BeginFadeOut()
        {
            _fadingOut = true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            AudioHelper.StopGtaLoadingAudio();
            base.OnFormClosing(e);
        }
    }
}