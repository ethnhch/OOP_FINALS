using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace SchoolBorrowingSystem.Models.UI
{
    public class GtaLoadingCanvas : Panel
    {
        public enum IntroPhase
        {
            StarsIntro,
            EmblemReveal,
            ArtSlideshow
        }

        private IntroPhase _phase = IntroPhase.ArtSlideshow; // Preview full slide look in Designer
        private float _phaseTime = 0.45f;
        private int _slideIndex = 0;
        private float _slideProgress = 0.35f; // 0..1 across current slide
        private float _spinnerAngle = 0f;
        private int _overallProgress = 68;
        private readonly List<Image> _customSlides = new List<Image>();

        [Category("GTA 6 Loading")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int OverallProgress
        {
            get => _overallProgress;
            set { _overallProgress = Math.Max(0, Math.Min(100, value)); Invalidate(); }
        }

        [Category("GTA 6 Loading")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public IntroPhase PreviewPhase
        {
            get => _phase;
            set { _phase = value; Invalidate(); }
        }

        public GtaLoadingCanvas()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            BackColor = Color.FromArgb(34, 34, 52);
        }

        public void LoadOptionalCustomSlides()
        {
            try
            {
                string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoadingSlides");
                if (!Directory.Exists(folder)) return;

                foreach (string file in Directory.GetFiles(folder))
                {
                    string ext = Path.GetExtension(file).ToLowerInvariant();
                    if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                        _customSlides.Add(Image.FromFile(file));
                }
            }
            catch { }
        }

        public void UpdateFrame(IntroPhase phase, float phaseTime, int slideIndex, float slideProgress, int overallPct)
        {
            _phase = phase;
            _phaseTime = phaseTime;
            _slideIndex = slideIndex;
            _slideProgress = slideProgress;
            _overallProgress = overallPct;
            _spinnerAngle = (_spinnerAngle + 8.5f) % 360f;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var img in _customSlides) img.Dispose();
                _customSlides.Clear();
            }
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width < 50 || Height < 50) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            switch (_phase)
            {
                case IntroPhase.StarsIntro:
                    DrawSixStarsIntro(g, Width, Height);
                    break;
                case IntroPhase.EmblemReveal:
                    DrawEmblemReveal(g, Width, Height);
                    break;
                case IntroPhase.ArtSlideshow:
                    DrawViceArtSlide(g, Width, Height);
                    break;
            }
        }

        // ============================================================
        // ACT 1: 6 NEON WANTED STARS (Matches 00:05 in reference video)
        // ============================================================
        private void DrawSixStarsIntro(Graphics g, int w, int h)
        {
            g.Clear(Color.FromArgb(38, 38, 56));

            int starCount = 6;
            float baseSpacing = 44f;
            float collapse = _phaseTime > 1.25f ? Math.Min(1f, (_phaseTime - 1.25f) / 0.45f) : 0f;
            float spacing = baseSpacing * (1f - collapse);
            bool turnWhite = _phaseTime > 0.92f;

            float totalW = (starCount - 1) * spacing;
            float startX = (w - totalW) / 2f;
            float cy = h / 2f;

            for (int i = 0; i < starCount; i++)
            {
                float appear = Math.Max(0f, Math.Min(1f, (_phaseTime - (i * 0.11f)) / 0.18f));
                if (appear <= 0f) continue;

                float cx = startX + (i * spacing);
                float radius = (17f + (1f - appear) * 8f) * (1f - (collapse * 0.15f));

                // Coral -> Hot Pink -> Vice Purple gradient across the 6 stars
                Color starColor = turnWhite
                    ? Color.White
                    : LerpColor(Color.FromArgb(255, 138, 92), Color.FromArgb(236, 46, 214), i / 5f);

                using (GraphicsPath starPath = CreateStarPath(cx, cy, radius, radius * 0.44f))
                {
                    if (!turnWhite)
                    {
                        // Outer neon bloom
                        for (int glow = 3; glow >= 1; glow--)
                        {
                            using (var pen = new Pen(Color.FromArgb(40 * (4 - glow), starColor), glow * 4.5f))
                                g.DrawPath(pen, starPath);
                        }
                    }

                    using (var fill = new SolidBrush(Color.FromArgb((int)(255 * appear), starColor)))
                        g.FillPath(fill, starPath);
                }
            }
        }

        // ============================================================
        // ACT 2: GLOWING STUDIO EMBLEM (Matches 00:17 in reference video)
        // ============================================================
        private void DrawEmblemReveal(Graphics g, int w, int h)
        {
            g.Clear(Color.FromArgb(38, 38, 56));

            float t = _phaseTime; // 0..1.7s
            float pop = Math.Min(1f, t / 0.25f);
            float neonFade = t < 0.75f ? 1f : Math.Max(0f, 1f - ((t - 0.75f) / 0.35f));
            float overallAlpha = t > 1.35f ? Math.Max(0f, 1f - ((t - 1.35f) / 0.35f)) : 1f;
            int alpha = (int)(255 * overallAlpha);
            if (alpha <= 2) return;

            int boxSize = (int)(116 * (0.85f + 0.15f * pop));
            Rectangle rect = new Rectangle((w - boxSize) / 2, (h - boxSize) / 2, boxSize, boxSize);

            using (GraphicsPath rounded = CreateRoundedRect(rect, 20))
            {
                // Glowing Vice Gradient Aura (orange -> magenta)
                if (neonFade > 0.01f)
                {
                    for (int i = 5; i >= 1; i--)
                    {
                        Rectangle gRect = Rectangle.Inflate(rect, i * 4, i * 4);
                        using (GraphicsPath gPath = CreateRoundedRect(gRect, 20 + i * 2))
                        using (var grad = new LinearGradientBrush(gRect,
                            Color.FromArgb((int)(34 * neonFade * overallAlpha), 255, 162, 68),
                            Color.FromArgb((int)(38 * neonFade * overallAlpha), 232, 34, 214),
                            45f))
                        using (var glowPen = new Pen(grad, i * 3.5f))
                        {
                            g.DrawPath(glowPen, gPath);
                        }
                    }
                }

                // Badge Dark Fill
                using (var bg = new SolidBrush(Color.FromArgb(alpha, 24, 22, 38)))
                    g.FillPath(bg, rounded);

                // Badge Border (Transitions from neon sunset rim to crisp white like the video)
                if (neonFade > 0.05f)
                {
                    using (var rimBrush = new LinearGradientBrush(rect,
                        Color.FromArgb(alpha, 255, 188, 72),
                        Color.FromArgb(alpha, 236, 46, 214), 45f))
                    using (var rimPen = new Pen(rimBrush, 3.5f))
                        g.DrawPath(rimPen, rounded);
                }
                else
                {
                    using (var whitePen = new Pen(Color.FromArgb(alpha, 255, 255, 255), 3f))
                        g.DrawPath(whitePen, rounded);
                }

                // Stylized "S★" / OOP Emblem Inside Badge
                using (var letterFont = new Font("Impact", 46f * (0.85f + 0.15f * pop), FontStyle.Italic))
                using (var textBrush = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255)))
                {
                    g.DrawString("S", letterFont, textBrush, rect.X + 16, rect.Y + 18);
                }

                using (GraphicsPath miniStar = CreateStarPath(rect.Right - 30, rect.Bottom - 30, 17, 7.5f))
                using (var starPen = new Pen(Color.FromArgb(alpha, 255, 255, 255), 2.4f))
                {
                    if (neonFade > 0.1f)
                    {
                        using (var pinkFill = new SolidBrush(Color.FromArgb((int)(alpha * 0.45f * neonFade), 255, 42, 133)))
                            g.FillPath(pinkFill, miniStar);
                    }
                    g.DrawPath(starPen, miniStar);
                }
            }

            // Subtitle below badge
            using (var subFont = new Font("Bahnschrift SemiBold", 10.5f, FontStyle.Bold))
            using (var subBrush = new SolidBrush(Color.FromArgb((int)(alpha * 0.85f), 220, 216, 240)))
            {
                string label = "SCHOOL EQUIPMENT BORROWING SYSTEM  //  OOP FINALS";
                SizeF sz = g.MeasureString(label, subFont);
                g.DrawString(label, subFont, subBrush, (w - sz.Width) / 2f, rect.Bottom + 24);
            }
        }

        // ============================================================
        // ACT 3: SLANTED-LETTERBOX VICE ART SLIDES (Matches 00:20 - 00:59)
        // ============================================================
        private void DrawViceArtSlide(Graphics g, int w, int h)
        {
            g.Clear(Color.FromArgb(20, 18, 34));

            // 1. Draw either user's custom slide image OR procedural Vice City artwork with Ken Burns pan/zoom
            if (_customSlides.Count > 0)
            {
                Image img = _customSlides[_slideIndex % _customSlides.Count];
                DrawKenBurnsImage(g, img, w, h, _slideProgress);
            }
            else
            {
                DrawProceduralViceArtwork(g, w, h, _slideIndex % 4, _slideProgress);
            }

            // 2. Signature Angled Black Letterbox Bars (Just like the GTA 6 slides at 00:20-00:59!)
            int slant = (int)(h * 0.048f);
            int topBar = (int)(h * 0.055f);
            int botBar = (int)(h * 0.068f);

            using (var barBrush = new SolidBrush(Color.FromArgb(14, 14, 22)))
            {
                // Top slightly slanted cinematic bar
                Point[] topPoly =
                {
                    new Point(0, 0),
                    new Point(w, 0),
                    new Point(w, topBar),
                    new Point(0, topBar + slant)
                };
                g.FillPolygon(barBrush, topPoly);

                // Bottom slightly slanted cinematic bar
                Point[] botPoly =
                {
                    new Point(0, h - botBar),
                    new Point(w, h - botBar - slant),
                    new Point(w, h),
                    new Point(0, h)
                };
                g.FillPolygon(barBrush, botPoly);
            }

            // 3. Smooth Dark Slate Cross-Fade at Start & End of Each Slide
            float fadeAlpha = 0f;
            if (_slideProgress < 0.14f)
                fadeAlpha = 1f - (_slideProgress / 0.14f);
            else if (_slideProgress > 0.86f)
                fadeAlpha = (_slideProgress - 0.86f) / 0.14f;

            if (fadeAlpha > 0.01f)
            {
                using (var fadeBrush = new SolidBrush(Color.FromArgb((int)(235 * fadeAlpha), 28, 28, 44)))
                    g.FillRectangle(fadeBrush, 0, 0, w, h);
            }

            // 4. Minimal Rockstar Bottom-Right Spinner + Subtle Neon Progress Hairline
            int spinSize = 22;
            Rectangle spinRect = new Rectangle(w - 46, h - 38, spinSize, spinSize);
            using (var trackPen = new Pen(Color.FromArgb(65, 255, 255, 255), 2.5f))
                g.DrawEllipse(trackPen, spinRect);
            using (var spinPen = new Pen(Color.FromArgb(255, 196, 86), 2.8f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                g.DrawArc(spinPen, spinRect, _spinnerAngle, 240f);

            int fillW = (int)(w * (_overallProgress / 100f));
            if (fillW > 0)
            {
                using (var barGrad = new LinearGradientBrush(
                    new Rectangle(0, h - 3, w, 3),
                    Color.FromArgb(255, 42, 133),
                    Color.FromArgb(0, 245, 212),
                    LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(barGrad, 0, h - 3, fillW, 3);
                }
            }
        }

        private void DrawKenBurnsImage(Graphics g, Image img, int w, int h, float progress)
        {
            float zoom = 1.04f + (progress * 0.08f);
            float scale = Math.Max((float)w / img.Width, (float)h / img.Height) * zoom;
            float drawW = img.Width * scale;
            float drawH = img.Height * scale;
            float panX = ((w - drawW) / 2f) + ((progress - 0.5f) * 34f);
            float panY = (h - drawH) / 2f;

            g.DrawImage(img, panX, panY, drawW, drawH);
        }

        private void DrawProceduralViceArtwork(Graphics g, int w, int h, int themeIdx, float p)
        {
            // 4 Distinct GTA 6 Pastel-Sunset Color Palettes (Inspired by the 4 art slides in the video)
            Color[][] palettes =
            {
                // Slide 0: Leonida Harbor Sunset (Violet -> Coral -> Warm Gold)
                new[] { Color.FromArgb(62, 42, 118), Color.FromArgb(204, 82, 148), Color.FromArgb(255, 186, 120) },
                // Slide 1: Vice Beach Pastel (Magenta -> Peach -> Lemonade Yellow)
                new[] { Color.FromArgb(148, 48, 128), Color.FromArgb(255, 118, 142), Color.FromArgb(255, 224, 132) },
                // Slide 2: Ocean Bridge Dusk (Indigo -> Cyan Glow -> Soft Lavender)
                new[] { Color.FromArgb(32, 46, 98), Color.FromArgb(108, 126, 198), Color.FromArgb(248, 182, 168) },
                // Slide 3: Motel Neon Twilight (Deep Plum -> Hot Pink -> Sunset Orange)
                new[] { Color.FromArgb(46, 20, 74), Color.FromArgb(196, 48, 122), Color.FromArgb(255, 146, 86) }
            };

            Color[] pal = palettes[themeIdx];
            int horizonY = (int)(h * 0.64f);
            float bgPan = (p - 0.5f) * 36f;   // Background drifts slowly
            float fgPan = (p - 0.5f) * -72f;  // Foreground drifts opposite (2.5D GTA parallax!)

            // Sky
            using (var sky = new LinearGradientBrush(new Rectangle(0, 0, w, h), pal[0], pal[2], LinearGradientMode.Vertical))
            {
                sky.InterpolationColors = new ColorBlend
                {
                    Colors = pal,
                    Positions = new[] { 0.0f, 0.52f, 1.0f }
                };
                g.FillRectangle(sky, 0, 0, w, h);
            }

            // Sun Glow
            int sunR = (int)(h * 0.42f);
            int sunX = (int)((w * (themeIdx % 2 == 0 ? 0.62f : 0.36f)) - sunR / 2f + bgPan);
            int sunY = horizonY - (int)(sunR * 0.62f);
            using (var sunBrush = new LinearGradientBrush(new Rectangle(sunX, sunY, sunR, sunR),
                Color.FromArgb(230, 255, 245, 190), Color.FromArgb(160, 255, 110, 145), 90f))
            {
                g.FillEllipse(sunBrush, sunX, sunY, sunR, sunR);
            }

            // Distant Vice City Skyline / Bridge Silhouette (Background Parallax)
            using (var cityBrush = new SolidBrush(Color.FromArgb(120, 44, 24, 72)))
            {
                int[] bHeights = { 55, 92, 68, 128, 84, 108, 52, 74, 138, 86, 62, 116, 78, 94, 58 };
                float bw = (w + 140f) / bHeights.Length;
                for (int i = 0; i < bHeights.Length; i++)
                {
                    float bx = -70f + (i * bw) + bgPan;
                    g.FillRectangle(cityBrush, bx, horizonY - bHeights[i], bw - 3, bHeights[i] + 10);
                }
            }

            // Shimmering Bay Water Reflection
            using (var water = new LinearGradientBrush(
                new Rectangle(0, horizonY, w, h - horizonY),
                Color.FromArgb(210, pal[1]), Color.FromArgb(245, 24, 16, 46), LinearGradientMode.Vertical))
            {
                g.FillRectangle(water, 0, horizonY, w, h - horizonY);
            }

            using (var ripplePen = new Pen(Color.FromArgb(75, 255, 232, 175), 2f))
            {
                for (int r = 0; r < 14; r++)
                {
                    int ry = horizonY + 8 + (r * 13);
                    float rw = 90 + (r * 22);
                    g.DrawLine(ripplePen, sunX + sunR / 2f - rw / 2f, ry, sunX + sunR / 2f + rw / 2f, ry);
                }
            }

            // Sweeping Foreground Palm Trees (Foreground 2.5D Parallax!)
            DrawPalmSilhouette(g, (int)(w * 0.12f + fgPan), h + 15, 1.15f, true);
            DrawPalmSilhouette(g, (int)(w * 0.88f + fgPan * 0.85f), h + 20, 1.05f, false);

            // Bold GTA-Style Large Background Typography Accent
            string[] heroWords = { "INVENTORY", "CLEARANCE", "DISPATCH", "SYSTEM QA" };
            using (var bigFont = new Font("Impact", Math.Max(68f, h * 0.18f), FontStyle.Italic))
            using (var bigBrush = new SolidBrush(Color.FromArgb(34, 255, 255, 255)))
            {
                g.DrawString(heroWords[themeIdx], bigFont, bigBrush, 52 + (fgPan * 0.35f), h * 0.16f);
            }

            // Soft Bottom-Left Vignette for Slide Caption Readability
            using (var vig = new LinearGradientBrush(
                new Rectangle(0, (int)(h * 0.55f), w, (int)(h * 0.45f)),
                Color.FromArgb(0, 14, 10, 26),
                Color.FromArgb(210, 14, 10, 26),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(vig, 0, (int)(h * 0.55f), w, (int)(h * 0.45f));
            }
        }

        private void DrawPalmSilhouette(Graphics g, int baseX, int baseY, float scale, bool leanRight)
        {
            float dir = leanRight ? 1f : -1f;
            float topX = baseX + (56f * dir * scale);
            float topY = baseY - (255f * scale);

            using (var trunkPen = new Pen(Color.FromArgb(22, 12, 38), 13f * scale) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            using (var frondPen = new Pen(Color.FromArgb(22, 12, 38), 6.5f * scale) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                g.DrawBezier(trunkPen, baseX, baseY,
                    baseX + (12f * dir * scale), baseY - (100f * scale),
                    baseX + (30f * dir * scale), baseY - (180f * scale),
                    topX, topY);

                float[,] fronds =
                {
                    { -92, -26, -124,  20 },
                    { -64, -62,  -98, -32 },
                    { -14, -80,  -38, -64 },
                    {  44, -68,   92, -28 },
                    {  88, -28,  126,  22 },
                    {  56,  14,   80,  62 },
                    { -56,  14,  -82,  58 }
                };

                for (int i = 0; i < fronds.GetLength(0); i++)
                {
                    g.DrawBezier(frondPen, topX, topY,
                        topX + fronds[i, 0] * scale, topY + fronds[i, 1] * scale,
                        topX + fronds[i, 0] * scale, topY + fronds[i, 1] * scale,
                        topX + fronds[i, 2] * scale, topY + fronds[i, 3] * scale);
                }
            }
        }

        private static GraphicsPath CreateStarPath(float cx, float cy, float outerR, float innerR)
        {
            var path = new GraphicsPath();
            PointF[] pts = new PointF[10];
            double angle = -Math.PI / 2.0;
            double step = Math.PI / 5.0;

            for (int i = 0; i < 10; i++)
            {
                float r = (i % 2 == 0) ? outerR : innerR;
                pts[i] = new PointF(cx + (float)(Math.Cos(angle) * r), cy + (float)(Math.Sin(angle) * r));
                angle += step;
            }
            path.AddPolygon(pts);
            return path;
        }

        private static GraphicsPath CreateRoundedRect(Rectangle r, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static Color LerpColor(Color a, Color b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            return Color.FromArgb(
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }
    }
}