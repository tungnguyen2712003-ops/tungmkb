using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinFormsApp1.Controls
{
    public class RoundedValueDisplay : UserControl
    {
        private int cornerRadius = 12;
        private double value = 0;

        [Category("Appearance")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(12)]
        public int CornerRadius
        {
            get => cornerRadius;
            set { cornerRadius = Math.Max(0, value); UpdateRegion(); Invalidate(); }
        }

        [Category("Data")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(0.0)]
        public double Value
        {
            get => value;
            set { this.value = value; Invalidate(); }
        }

        public RoundedValueDisplay()
        {
            DoubleBuffered = true;
            Width = 140;
            Height = 50;
            BackColor = Color.Transparent;
            ForeColor = Color.Black;
            // ensure a Times New Roman font for the number display
            Font = new Font("Times New Roman", 16F, FontStyle.Regular, GraphicsUnit.Point);
            UpdateRegion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var path = RoundedRect(rect, cornerRadius))
            using (var brush = new SolidBrush(Color.White))
            using (var pen = new Pen(Color.FromArgb(200, 200, 200)))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }

            string text = Value.ToString();
            var textRect = rect;
            // draw number centered, using Times New Roman
            using (var textBrush = new SolidBrush(ForeColor))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                e.Graphics.DrawString(text, Font, textBrush, textRect, sf);
            }
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }
            int diameter = radius * 2;
            var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            // top left
            path.AddArc(arc, 180, 90);
            // top right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            // bottom right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            // bottom left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void UpdateRegion()
        {
            try
            {
                using (var gp = RoundedRect(new Rectangle(0, 0, Width, Height), cornerRadius))
                {
                    this.Region = new Region(gp);
                }
            }
            catch { }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateRegion();
            Invalidate();
        }
    }
}
