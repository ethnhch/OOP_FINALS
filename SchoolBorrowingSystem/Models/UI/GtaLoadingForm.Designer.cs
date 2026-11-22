namespace SchoolBorrowingSystem.Models.UI
{
    partial class GtaLoadingForm
    {
        private System.ComponentModel.IContainer components = null;
        private GtaLoadingCanvas gtaCanvas;
        private System.Windows.Forms.Label lblSlideTag;
        private System.Windows.Forms.Label lblSlideTitle;
        private System.Windows.Forms.Label lblSlideDesc;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Timer animTimer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gtaCanvas = new SchoolBorrowingSystem.Models.UI.GtaLoadingCanvas();
            this.lblSlideTag = new System.Windows.Forms.Label();
            this.lblSlideTitle = new System.Windows.Forms.Label();
            this.lblSlideDesc = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.animTimer = new System.Windows.Forms.Timer(this.components);
            this.gtaCanvas.SuspendLayout();
            this.SuspendLayout();

            // gtaCanvas
            this.gtaCanvas.Controls.Add(this.lblStatus);
            this.gtaCanvas.Controls.Add(this.lblSlideDesc);
            this.gtaCanvas.Controls.Add(this.lblSlideTitle);
            this.gtaCanvas.Controls.Add(this.lblSlideTag);
            this.gtaCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gtaCanvas.Location = new System.Drawing.Point(0, 0);
            this.gtaCanvas.Name = "gtaCanvas";
            this.gtaCanvas.OverallProgress = 68;
            this.gtaCanvas.PreviewPhase = SchoolBorrowingSystem.Models.UI.GtaLoadingCanvas.IntroPhase.ArtSlideshow;
            this.gtaCanvas.Size = new System.Drawing.Size(960, 540);
            this.gtaCanvas.TabIndex = 0;

            // lblSlideTag (Neon Vice Accent Tag)
            this.lblSlideTag.AutoSize = true;
            this.lblSlideTag.BackColor = System.Drawing.Color.Transparent;
            this.lblSlideTag.Font = new System.Drawing.Font("Bahnschrift SemiBold", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblSlideTag.ForeColor = System.Drawing.Color.FromArgb(0, 245, 212);
            this.lblSlideTag.Location = new System.Drawing.Point(42, 366);
            this.lblSlideTag.Name = "lblSlideTag";
            this.lblSlideTag.Text = "LEONIDA CAMPUS  //  EQUIPMENT INVENTORY";

            // lblSlideTitle (Bold GTA Headline)
            this.lblSlideTitle.AutoSize = true;
            this.lblSlideTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSlideTitle.Font = new System.Drawing.Font("Impact", 26F, System.Drawing.FontStyle.Regular);
            this.lblSlideTitle.ForeColor = System.Drawing.Color.White;
            this.lblSlideTitle.Location = new System.Drawing.Point(38, 386);
            this.lblSlideTitle.Name = "lblSlideTitle";
            this.lblSlideTitle.Text = "AV & LABORATORY GEAR DISPATCH";

            // lblSlideDesc (Clean Tip Copy)
            this.lblSlideDesc.BackColor = System.Drawing.Color.Transparent;
            this.lblSlideDesc.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.lblSlideDesc.ForeColor = System.Drawing.Color.FromArgb(232, 228, 246);
            this.lblSlideDesc.Location = new System.Drawing.Point(42, 434);
            this.lblSlideDesc.Name = "lblSlideDesc";
            this.lblSlideDesc.Size = new System.Drawing.Size(540, 46);
            this.lblSlideDesc.Text = "Track Epson laser projectors, microscopes, and oscilloscopes in real time—complete with polymorphic return checklists.";

            // lblStatus (Bottom-Right Loading Label Next to Spinner)
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Font = new System.Drawing.Font("Bahnschrift SemiBold", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(626, 502);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(278, 24);
            this.lblStatus.Text = "Loading Dashboard  68%";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // animTimer (~50 FPS smooth pan & zoom)
            this.animTimer.Interval = 20;
            this.animTimer.Tick += new System.EventHandler(this.animTimer_Tick);

            // GtaLoadingForm
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(20, 18, 34);
            this.ClientSize = new System.Drawing.Size(960, 540);
            this.Controls.Add(this.gtaCanvas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "GtaLoadingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading School Equipment Borrowing System";
            this.Load += new System.EventHandler(this.GtaLoadingForm_Load);

            this.gtaCanvas.ResumeLayout(false);
            this.gtaCanvas.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}