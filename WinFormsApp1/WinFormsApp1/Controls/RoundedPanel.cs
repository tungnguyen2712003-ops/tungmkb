using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinFormsApp1.Controls
{
    /// <summary>
    /// A panel with rounded corners and semi-transparent (frosted glass) appearance.
    /// Uses a translucent fill + visible rounded border to create a "glass box" effect.
    /// </summary>
    public class RoundedPanel : Panel
    {
        private int cornerRadius = 20;
        private Color borderColor = Color.FromArgb(100, 148, 163, 184);
        private float borderThickness = 2f;

        [Category("Appearance")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(20)]
        [Description("Radius of the rounded corners in pixels.")]
        public int CornerRadius
        {
            get => cornerRadius;
            set { cornerRadius = Math.Max(0, value); RecreateRegion(); Invalidate(); }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(2f)]
        public float BorderThickness
        {
            get => borderThickness;
            set { borderThickness = value; Invalidate(); }
        }

        /// <summary>Transparency of the panel fill (0 = fully transparent, 255 = fully opaque).</summary>
        [Category("Appearance")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(40)]
        [Description("Alpha (0-255) of the panel background fill.")]
        public int FillAlpha { get; set; } = 40;

        /// <summary>Base color used with FillAlpha for the translucent background.</summary>
        [Category("Appearance")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color FillColor { get; set; } = Color.FromArgb(30, 41, 59);

        public RoundedPanel()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Padding = new Padding(8);
            RecreateRegion();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RecreateRegion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Use a slightly inset rect so the border is fully visible
            float inset = borderThickness / 2f;
            var rect = new RectangleF(inset, inset, Width - borderThickness, Height - borderThickness);

            using var path = RoundedRectF(rect, cornerRadius);

            // Translucent fill
            using var fillBrush = new SolidBrush(Color.FromArgb(FillAlpha, FillColor.R, FillColor.G, FillColor.B));
            e.Graphics.FillPath(fillBrush, path);

            // Visible rounded border
            if (borderThickness > 0)
            {
                using var pen = new Pen(borderColor, borderThickness);
                e.Graphics.DrawPath(pen, path);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do nothing — OnPaint handles everything to keep transparency working.
        }

        private void RecreateRegion()
        {
            try
            {
                var rect = new Rectangle(0, 0, Width, Height);
                using var path = RoundedRect(rect, cornerRadius);
                this.Region = new Region(path);
            }
            catch { }
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }
            int diameter = radius * 2;
            var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static GraphicsPath RoundedRectF(RectangleF bounds, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }
            int diameter = radius * 2;
            var arc = new RectangleF(bounds.Location, new SizeF(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
