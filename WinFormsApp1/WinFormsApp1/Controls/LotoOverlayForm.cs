using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinFormsApp1.Controls
{
    /// <summary>
    /// Lockout / Tagout overlay.
    ///
    /// - When locking (currentlyUnlocked = true):
    ///     Operator enters a NEW password (single field, no confirm) and presses ACTIVATE LOTO.
    ///     That password becomes the lock password.
    ///
    /// - When unlocking (currentlyUnlocked = false):
    ///     Operator must enter the SAME password that was used to lock.
    ///     Presses RELEASE LOTO to unlock. The lock password is then cleared.
    /// </summary>
    public class LotoOverlayForm : Form
    {
        // ---- Palette -------------------------------------------------------
        private static readonly Color BgDark = Color.FromArgb(30, 41, 59);
        private static readonly Color BgPanel = Color.FromArgb(15, 23, 42);
        private static readonly Color BgHeader = Color.FromArgb(30, 41, 59);
        private static readonly Color BgImage = Color.FromArgb(240, 244, 248);
        private static readonly Color BgField = Color.FromArgb(30, 41, 59);
        private static readonly Color AccentRed = Color.FromArgb(239, 68, 68);
        private static readonly Color AccentGreen = Color.FromArgb(34, 197, 94);
        private static readonly Color MutedText = Color.FromArgb(148, 163, 184);
        private static readonly Color LightText = Color.FromArgb(226, 232, 240);

        private readonly string lockPassword;       // password to match (empty when locking)
        private readonly bool currentlyUnlocked;    // true = locking, false = unlocking

        /// <summary>The password the operator typed (used by caller when locking).</summary>
        public string EnteredPassword { get; private set; } = "";

        // ---- Controls ------------------------------------------------------
        private DangerTagImage dangerImage = null!;
        private TextBox passwordBox = null!;
        private Label passwordHint = null!;
        private Button activateButton = null!;
        private Button cancelButton = null!;

        public LotoOverlayForm(string lockPassword, bool currentlyUnlocked)
        {
            this.lockPassword = lockPassword;
            this.currentlyUnlocked = currentlyUnlocked;

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            MinimizeBox = false;
            MaximizeBox = false;
            BackColor = BgDark;
            KeyPreview = true;

            BuildLayout();

            // Esc to cancel — only when locking (can't escape while unlocking)
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape && currentlyUnlocked)
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
            };
        }

        // -------------------------------------------------------------------
        // Layout
        // -------------------------------------------------------------------
        private void BuildLayout()
        {
            const int pad = 20;
            int headerH = 74;
            // Single password field for both lock and unlock → same footer height
            int footerH = 78;

            // ---------- Header ----------
            var header = new Panel
            {
                BackColor = BgHeader,
                Dock = DockStyle.Top,
                Height = headerH
            };

            var lockIcon = new Panel
            {
                BackColor = Color.Transparent,
                Size = new Size(34, 34),
                Location = new Point(pad, (headerH - 34) / 2)
            };
            lockIcon.Paint += PaintLockIcon;

            var title = new Label
            {
                Text = currentlyUnlocked ? "LOCKOUT / TAGOUT" : "RELEASE LOCKOUT",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(pad + 46, 12)
            };
            var subtitle = new Label
            {
                Text = currentlyUnlocked
                    ? "Nhập mật khẩu mới để khóa thiết bị"
                    : "Nhập mật khẩu đã dùng khi khóa để mở khóa",
                ForeColor = MutedText,
                Font = new Font("Segoe UI", 9F),
                AutoSize = true,
                Location = new Point(pad + 48, 44)
            };

            header.Controls.Add(lockIcon);
            header.Controls.Add(title);
            header.Controls.Add(subtitle);

            // ---------- Body ----------
            var body = new Panel
            {
                BackColor = BgDark,
                Dock = DockStyle.Fill,
                Padding = new Padding(pad)
            };

            var imageHost = new Panel
            {
                BackColor = BgImage,
                Dock = DockStyle.Fill,
                Padding = new Padding(6)
            };
            dangerImage = new DangerTagImage
            {
                Dock = DockStyle.Fill,
                BackColor = BgImage
            };
            imageHost.Controls.Add(dangerImage);

            var infoStrip = new Panel
            {
                BackColor = BgPanel,
                Dock = DockStyle.Bottom,
                Height = 46
            };
            var infoText = new Label
            {
                Text = currentlyUnlocked
                    ? "Thiết bị sẽ bị khóa. Mọi nút điều khiển sẽ bị vô hiệu hóa cho đến khi mở khóa."
                    : "Nhập đúng mật khẩu đã dùng khi khóa để mở khóa thiết bị.",
                ForeColor = MutedText,
                Font = new Font("Segoe UI", 9F),
                AutoSize = true,
                Location = new Point(14, 14)
            };
            infoStrip.Controls.Add(infoText);

            body.Controls.Add(imageHost);
            body.Controls.Add(infoStrip);

            // ---------- Footer ----------
            var footer = new Panel
            {
                BackColor = BgPanel,
                Dock = DockStyle.Bottom,
                Height = footerH,
                Padding = new Padding(pad)
            };

            // Password label + textbox (single field for both modes)
            var pwdLabel = new Label
            {
                Text = currentlyUnlocked ? "MẬT KHẨU KHÓA:" : "MẬT KHẨU MỞ KHÓA:",
                ForeColor = LightText,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(pad, 10)
            };

            passwordBox = new TextBox
            {
                Location = new Point(pad, 30),
                Size = new Size(260, 30),
                Font = new Font("Segoe UI", 11F),
                UseSystemPasswordChar = true,
                BackColor = BgField,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            passwordBox.TextChanged += (s, e) =>
            {
                passwordHint.Visible = false;
                activateButton.Enabled = true;
            };

            passwordHint = new Label
            {
                Text = "",
                ForeColor = AccentRed,
                Font = new Font("Segoe UI", 8.5F),
                AutoSize = true,
                Location = new Point(pad + 270, 34),
                Visible = false
            };

            // Cancel — only available when LOCKING (entering new password to lock).
            // When UNLOCKING (device is locked), user cannot cancel — must enter correct password.
            cancelButton = MakeFlatButton("✕  Cancel", 130, Color.FromArgb(51, 65, 85), Color.White);
            cancelButton.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            cancelButton.Visible = currentlyUnlocked;

            // Activate / Release button
            if (currentlyUnlocked)
            {
                activateButton = MakeFlatButton("🔒  ACTIVATE LOTO", 200, AccentRed, Color.White);
            }
            else
            {
                activateButton = MakeFlatButton("🔓  RELEASE LOTO", 200, AccentGreen, Color.White);
            }
            activateButton.Click += ActivateButton_Click;

            footer.Controls.Add(pwdLabel);
            footer.Controls.Add(passwordBox);
            footer.Controls.Add(passwordHint);
            footer.Controls.Add(cancelButton);
            footer.Controls.Add(activateButton);

            // ---------- Assemble ----------
            Controls.Add(body);
            Controls.Add(footer);
            Controls.Add(header);

            Resize += (s, e) => LayoutFooter(footerH);
            LayoutFooter(footerH);

            Shown += (s, e) => passwordBox.Focus();
        }

        private void LayoutFooter(int footerH)
        {
            int pad = 20;
            int y = footerH - 50;

            int rightX = ClientSize.Width - pad;
            activateButton.Location = new Point(rightX - 200, y);

            if (cancelButton.Visible)
            {
                cancelButton.Location = new Point(rightX - 200 - 10 - 130, y);
            }
        }

        private Button MakeFlatButton(string text, int width, Color back, Color fore)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(width, 40),
                BackColor = back,
                ForeColor = fore,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(back, 0.12f);
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(back, 0.12f);
            return btn;
        }

        private void PaintLockIcon(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var iconColor = currentlyUnlocked ? AccentRed : AccentGreen;
            using var pen = new Pen(iconColor, 2.4f);
            g.DrawArc(pen, 8, 3, 16, 18, 180, 180);
            g.DrawRectangle(pen, 5, 13, 22, 17);
            using var brush = new SolidBrush(iconColor);
            g.FillEllipse(brush, 13.5f, 18f, 5, 5);
        }

        // -------------------------------------------------------------------
        // Activation logic
        // -------------------------------------------------------------------
        private void ActivateButton_Click(object? sender, EventArgs e)
        {
            if (currentlyUnlocked)
            {
                // LOCKING: single password field, no confirm needed
                if (string.IsNullOrWhiteSpace(passwordBox.Text))
                {
                    ShowError("⚠ Vui lòng nhập mật khẩu.");
                    passwordBox.Focus();
                    return;
                }
                if (passwordBox.Text.Length < 4)
                {
                    ShowError("⚠ Mật khẩu phải có ít nhất 4 ký tự.");
                    passwordBox.Focus();
                    return;
                }

                EnteredPassword = passwordBox.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                // UNLOCKING: must match the lock password
                if (passwordBox.Text != lockPassword)
                {
                    ShowError("⚠ Sai mật khẩu! Không thể mở khóa.");
                    passwordBox.Clear();
                    passwordBox.Focus();
                    activateButton.Enabled = false;
                    return;
                }

                EnteredPassword = passwordBox.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void ShowError(string msg)
        {
            passwordHint.Text = msg;
            passwordHint.ForeColor = AccentRed;
            passwordHint.Visible = true;
        }
    }
}
