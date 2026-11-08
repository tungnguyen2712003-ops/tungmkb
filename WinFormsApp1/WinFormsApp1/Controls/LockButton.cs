using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace WinFormsApp1.Controls
{
    /// <summary>
    /// Safety lock button.
    /// - Green when unlocked, red when locked.
    /// - When UNLOCKED: clicking opens LOTO overlay → operator enters a password
    ///   and presses ACTIVATE LOTO to LOCK. That password becomes the lock password.
    /// - When LOCKED: clicking opens LOTO overlay → operator must enter the SAME
    ///   password that was used to lock, then presses RELEASE LOTO to UNLOCK.
    /// - The lock password persists to a file so it survives app restarts.
    /// </summary>
    public class LockButton : UserControl
    {
        private bool isLocked = false;
        private bool isHovered;
        private bool isPressed;
        private string lockPassword = "";     // password used to lock

        // File used to persist the lock password between app restarts
        private static readonly string PasswordFile =
            Path.Combine(AppContext.BaseDirectory, "loto_password.dat");

        /// <summary>Raised whenever the lock state changes.</summary>
        public event EventHandler? LockStateChanged;

        [Category("Behavior")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        [Description("Whether the equipment is currently locked out.")]
        public bool IsLocked
        {
            get => isLocked;
            set
            {
                if (isLocked == value) return;
                isLocked = value;
                UpdateAppearance();
                ApplyLockState();
                LockStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The password that was used to engage the lock.
        /// Empty when not locked. Persisted to disk.
        /// </summary>
        [Browsable(false)]
        public string LockPassword
        {
            get => lockPassword;
            private set
            {
                lockPassword = value ?? "";
                SavePasswordToFile(lockPassword);
            }
        }

        [Category("Appearance")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("UNLOCKED")]
        public string ButtonText { get; set; } = "UNLOCKED";

        private Color fillColor = Color.FromArgb(40, 170, 70);

        [Category("Appearance")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color FillColor
        {
            get => fillColor;
            set { fillColor = value; Invalidate(); }
        }

        private static readonly Color LockedColor = Color.FromArgb(200, 42, 42);
        private static readonly Color UnlockedColor = Color.FromArgb(40, 170, 70);

        public LockButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            DoubleBuffered = true;
            Size = new Size(120, 40);
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            Cursor = Cursors.Hand;

            // Restore saved password from disk (if any)
            lockPassword = LoadPasswordFromFile();

            UpdateAppearance();
            UpdateRegion();
        }

        private void UpdateAppearance()
        {
            if (isLocked)
            {
                fillColor = LockedColor;
                ButtonText = "LOCKED";
            }
            else
            {
                fillColor = UnlockedColor;
                ButtonText = "UNLOCKED";
            }
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color fill = fillColor;
            if (isPressed) fill = ControlPaint.Dark(fill);
            else if (isHovered) fill = ControlPaint.Light(fill);

            int radius = 12;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var gp = RoundedRect(rect, radius))
            using (var brush = new SolidBrush(fill))
            using (var pen = new Pen(ControlPaint.Dark(fill, 0.3f)))
            {
                e.Graphics.FillPath(brush, gp);
                e.Graphics.DrawPath(pen, gp);
            }

            DrawPadlockGlyph(e.Graphics, rect);
            TextRenderer.DrawText(e.Graphics, ButtonText, Font, rect, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private void DrawPadlockGlyph(Graphics g, Rectangle rect)
        {
            int s = 13;
            int x = 11;
            int y = (Height - s) / 2;
            using var pen = new Pen(Color.White, 1.5f);
            g.DrawRectangle(pen, x, y + 5, s, s - 5);
            g.DrawArc(pen, x + 2, y, s - 4, 7, 180, 180);
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
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

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateRegion();
            Invalidate();
        }

        private void UpdateRegion()
        {
            try
            {
                int radius = 12;
                var rect = new Rectangle(0, 0, Width, Height);
                using (var gp = RoundedRect(rect, radius))
                    this.Region = new Region(gp);
            }
            catch { }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left) isPressed = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (isPressed && e.Button == MouseButtons.Left)
            {
                isPressed = false;
                OnClick(EventArgs.Empty);
            }
            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            var host = this.FindForm();
            if (host == null) return;

            // isLocked = false → currentlyUnlocked = true  → overlay to LOCK (enter new password)
            // isLocked = true  → currentlyUnlocked = false → overlay to UNLOCK (must match lockPassword)
            using (var overlay = new LotoOverlayForm(lockPassword, !isLocked))
            {
                int overlayW = host.Width * 2 / 3;
                int overlayH = host.Height * 2 / 3;
                int overlayX = host.Left + (host.Width - overlayW) / 2;
                int overlayY = host.Top + (host.Height - overlayH) / 2;

                overlay.FormBorderStyle = FormBorderStyle.None;
                overlay.StartPosition = FormStartPosition.Manual;
                overlay.Location = new Point(overlayX, overlayY);
                overlay.Size = new Size(overlayW, overlayH);
                overlay.ShowInTaskbar = false;
                overlay.TopMost = true;

                if (overlay.ShowDialog(host) == DialogResult.OK)
                {
                    if (!isLocked)
                    {
                        // LOCKING: the password entered becomes the lock password
                        LockPassword = overlay.EnteredPassword;
                        IsLocked = true;
                    }
                    else
                    {
                        // UNLOCKING: password already verified inside overlay.
                        // Clear the saved password so the next lock uses a fresh one.
                        LockPassword = "";
                        DeletePasswordFile();
                        IsLocked = false;
                    }
                }
            }
        }

        private void ApplyLockState()
        {
            var host = this.FindForm();
            if (host == null) return;

            foreach (Control c in host.Controls)
            {
                if (c == this) continue;

                var tag = c.Tag?.ToString();
                bool hasSaved = tag != null && tag.StartsWith(LockTagPrefix);

                if (isLocked)
                {
                    if (!hasSaved)
                        c.Tag = LockTagPrefix + c.Enabled.ToString();
                    c.Enabled = false;
                }
                else
                {
                    if (hasSaved)
                    {
                        c.Enabled = bool.Parse(tag!.Substring(LockTagPrefix.Length));
                        c.Tag = null;
                    }
                    else
                    {
                        c.Enabled = true;
                    }
                }
            }
        }

        private const string LockTagPrefix = "__lock_saved_";

        // -------------------------------------------------------------------
        // Password persistence (simple file-based, Base64)
        // -------------------------------------------------------------------
        private static string LoadPasswordFromFile()
        {
            try
            {
                if (File.Exists(PasswordFile))
                {
                    var b64 = File.ReadAllText(PasswordFile).Trim();
                    if (!string.IsNullOrEmpty(b64))
                    {
                        var bytes = Convert.FromBase64String(b64);
                        return System.Text.Encoding.UTF8.GetString(bytes);
                    }
                }
            }
            catch { }
            return "";
        }

        private static void SavePasswordToFile(string pwd)
        {
            try
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(pwd);
                var b64 = Convert.ToBase64String(bytes);
                File.WriteAllText(PasswordFile, b64);
            }
            catch { }
        }

        private static void DeletePasswordFile()
        {
            try
            {
                if (File.Exists(PasswordFile))
                    File.Delete(PasswordFile);
            }
            catch { }
        }
    }
}
