using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinFormsApp1.Controls
{
    /// <summary>
    /// Paints the classic DANGER "DO NOT OPERATE" lockout tag with a padlock.
    /// Drawn entirely with GDI+ so it stays crisp at any size and needs no image asset.
    /// </summary>
    public class DangerTagImage : Control
    {
        private Color tagRed = Color.FromArgb(196, 30, 38);
        private Color tagBorder = Color.FromArgb(150, 20, 25);

        public DangerTagImage()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            DoubleBuffered = true;
            BackColor = Color.White;
            Size = new Size(900, 470);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            float w = Width;
            float h = Height;

            // Tag occupies the left ~62%, padlock on the right ~38%
            float tagW = w * 0.60f;
            float tagH = h * 0.80f;
            float tagX = (w - tagW) / 2f - w * 0.06f;
            float tagY = (h - tagH) / 2f + h * 0.02f;

            DrawTag(g, tagX, tagY, tagW, tagH);
            DrawPadlock(g, w * 0.60f, h * 0.50f, w * 0.36f, h * 0.52f);
        }

        // ------------------------------------------------------------------
        // The safety tag
        // ------------------------------------------------------------------
        private void DrawTag(Graphics g, float x, float y, float w, float h)
        {
            float angle = -18f; // slight tilt, like the reference
            GraphicsState state = g.Save();
            g.TranslateTransform(x + w / 2f, y + h / 2f);
            g.RotateTransform(angle);
            g.TranslateTransform(-w / 2f, -h / 2f);

            // Outer tag body with rounded corners
            var body = new RectangleF(0, 0, w, h);
            using (var path = RoundedRect(body, w * 0.10f))
            using (var fill = new SolidBrush(Color.White))
            using (var pen = new Pen(tagBorder, Math.Max(3f, w * 0.022f)))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }

            // Top "hole" area – dark bar where the tag is punched
            float punchW = w * 0.30f;
            float punchH = h * 0.11f;
            var punch = new RectangleF((w - punchW) / 2f, -punchH * 0.35f, punchW, punchH);
            using (var brush = new SolidBrush(Color.FromArgb(235, 235, 235)))
                g.FillEllipse(brush, punch);
            using (var pen = new Pen(tagBorder, Math.Max(2f, w * 0.012f)))
                g.DrawEllipse(pen, punch);

            float pad = w * 0.07f;
            float cy = y + 0; // working in translated space now

            // ---- DANGER header band ----
            float bandY = pad + h * 0.09f;
            float bandH = h * 0.155f;
            var band = new RectangleF(pad, bandY, w - pad * 2, bandH);
            using (var brush = new SolidBrush(tagRed))
                g.FillRectangle(brush, band);

            using (var font = new Font("Arial Black", bandH * 0.62f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var brush = new SolidBrush(Color.White))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString("DANGER", font, brush, band, sf);
            }

            // ---- DO NOT OPERATE ----
            float opY = bandY + bandH + h * 0.045f;
            float opH = h * 0.145f;
            using (var font = new Font("Arial Black", opH * 0.72f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var brush = new SolidBrush(Color.Black))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString("DO NOT", font, brush,
                    new RectangleF(pad, opY, w - pad * 2, opH), sf);
                g.DrawString("OPERATE", font, brush,
                    new RectangleF(pad, opY + opH * 0.95f, w - pad * 2, opH), sf);
            }

            // ---- EQUIPMENT LOCKED OUT ----
            float eqY = opY + opH * 2.05f;
            float eqH = h * 0.115f;
            using (var font = new Font("Arial Black", eqH * 0.60f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var brush = new SolidBrush(Color.Black))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString("EQUIPMENT", font, brush,
                    new RectangleF(pad, eqY, w - pad * 2, eqH), sf);
                g.DrawString("LOCKED OUT", font, brush,
                    new RectangleF(pad, eqY + eqH * 1.02f, w - pad * 2, eqH), sf);
            }

            // ---- Footer: NAME / DATE lines ----
            float footY = h - pad - h * 0.155f;
            using (var pen = new Pen(Color.Black, Math.Max(1f, w * 0.006f)))
            {
                g.DrawLine(pen, pad * 1.4f, footY + h * 0.052f, w - pad * 1.4f, footY + h * 0.052f);
                g.DrawLine(pen, pad * 1.4f, footY + h * 0.112f, w - pad * 1.4f, footY + h * 0.112f);
            }
            using (var font = new Font("Arial", h * 0.042f, FontStyle.Regular, GraphicsUnit.Pixel))
            using (var brush = new SolidBrush(Color.Black))
            {
                g.DrawString("NAME", font, brush, pad * 1.5f, footY);
                g.DrawString("DATE", font, brush, w - pad * 1.5f - h * 0.12f, footY);
                g.DrawString("© 2026 Creative Safety Supply", font, brush,
                    new RectangleF(pad * 1.5f, footY + h * 0.062f, w - pad * 3, h * 0.045f),
                    new StringFormat { Alignment = StringAlignment.Center });
            }

            g.Restore(state);
        }

        // ------------------------------------------------------------------
        // The padlock
        // ------------------------------------------------------------------
        private void DrawPadlock(Graphics g, float cx, float cy, float w, float h)
        {
            GraphicsState state = g.Save();
            g.TranslateTransform(cx, cy);
            g.RotateTransform(8f);

            float bodyW = w * 0.62f;
            float bodyH = h * 0.56f;
            var body = new RectangleF(-bodyW / 2f, -h * 0.06f, bodyW, bodyH);

            // Shackle
            float shackleW = bodyW * 0.52f;
            float shackleH = h * 0.42f;
            var shackle = new RectangleF(-shackleW / 2f, -h * 0.46f, shackleW, shackleH);
            using (var pen = new Pen(Color.FromArgb(70, 70, 75), Math.Max(4f, w * 0.045f)))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawArc(pen, shackle, 180, 180);
            }

            // Lock body
            using (var path = RoundedRect(body, w * 0.07f))
            using (var fill = new SolidBrush(Color.FromArgb(205, 32, 40)))
            using (var pen = new Pen(Color.FromArgb(140, 20, 26), Math.Max(3f, w * 0.030f)))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }

            // Holes in the body
            float holeR = bodyW * 0.075f;
            using (var brush = new SolidBrush(Color.FromArgb(245, 245, 245)))
            using (var pen = new Pen(Color.FromArgb(120, 15, 20), Math.Max(1.5f, w * 0.012f)))
            {
                for (int row = 0; row < 2; row++)
                {
                    for (int col = 0; col < 2; col++)
                    {
                        float hx = -bodyW / 2f + bodyW * (0.28f + col * 0.44f);
                        float hy = body.Top + bodyH * (0.28f + row * 0.44f);
                        var hole = new RectangleF(hx - holeR, hy - holeR, holeR * 2, holeR * 2);
                        g.FillEllipse(brush, hole);
                        g.DrawEllipse(pen, hole);
                    }
                }
            }

            // Brass key cylinder
            float cylW = bodyW * 0.92f;
            float cylH = bodyH * 0.34f;
            var cyl = new RectangleF(-cylW / 2f, body.Bottom - cylH * 0.55f, cylW, cylH);
            using (var path = RoundedRect(cyl, cylH * 0.18f))
            using (var fill = new SolidBrush(Color.FromArgb(185, 190, 195)))
            using (var pen = new Pen(Color.FromArgb(120, 125, 130), Math.Max(2f, w * 0.014f)))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);
            }
            // Cylinder ridges
            using (var pen = new Pen(Color.FromArgb(140, 145, 150), Math.Max(1f, w * 0.008f)))
            {
                for (int i = 1; i < 7; i++)
                {
                    float rx = cyl.Left + cylW * i / 7f;
                    g.DrawLine(pen, rx, cyl.Top + cylH * 0.15f, rx, cyl.Bottom - cylH * 0.15f);
                }
            }
            // Keyhole
            float kR = cylH * 0.22f;
            using (var brush = new SolidBrush(Color.FromArgb(70, 70, 75)))
            {
                g.FillEllipse(brush, cyl.Left + cylW * 0.5f - kR, cyl.Top + cylH * 0.5f - kR, kR * 2, kR * 2);
            }

            g.Restore(state);
        }

        private GraphicsPath RoundedRect(RectangleF r, float radius)
        {
            var path = new GraphicsPath();
            float d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
            path.AddArc(r.Left, r.Top, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
