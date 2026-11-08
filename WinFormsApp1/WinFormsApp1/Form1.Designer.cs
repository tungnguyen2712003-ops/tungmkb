namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private WinFormsApp1.Controls.RoundedPanel controlPanel;
        private WinFormsApp1.Controls.LockButton lockButton;
        private WinFormsApp1.Controls.StylishButton btnStart;
        private WinFormsApp1.Controls.StylishButton btnStop;
        private WinFormsApp1.Controls.StylishButton btnReset;
        private WinFormsApp1.Controls.StylishButton btnJogLeft;
        private WinFormsApp1.Controls.StylishButton btnJogRight;
        private WinFormsApp1.Controls.ToggleSwitch toggleSwitch1;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            toggleSwitch1 = new WinFormsApp1.Controls.ToggleSwitch();
            btnJogRight = new WinFormsApp1.Controls.StylishButton();
            btnJogLeft = new WinFormsApp1.Controls.StylishButton();
            btnReset = new WinFormsApp1.Controls.StylishButton();
            btnStop = new WinFormsApp1.Controls.StylishButton();
            btnStart = new WinFormsApp1.Controls.StylishButton();
            lockButton = new WinFormsApp1.Controls.LockButton();
            roundedPanel1 = new WinFormsApp1.Controls.RoundedPanel();
            floatingLabelTextBox1 = new WinFormsApp1.Controls.FloatingLabelTextBox();
            floatingLabelTextBox2 = new WinFormsApp1.Controls.FloatingLabelTextBox();
            roundedPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // toggleSwitch1
            // 
            toggleSwitch1.BackColor = Color.Black;
            toggleSwitch1.Location = new Point(1518, 779);
            toggleSwitch1.Name = "toggleSwitch1";
            toggleSwitch1.Size = new Size(60, 28);
            toggleSwitch1.TabIndex = 6;
            // 
            // btnJogRight
            // 
            btnJogRight.BackColor = Color.Transparent;
            btnJogRight.ButtonText = "JOG RIGHT";
            btnJogRight.ExpandedWidth = 240;
            btnJogRight.ExpandOnHover = false;
            btnJogRight.FillColor = Color.FromArgb(100, 149, 237);
            btnJogRight.Font = new Font("Segoe UI", 9F);
            btnJogRight.ForeColor = Color.White;
            btnJogRight.IsDraggable = false;
            btnJogRight.Location = new Point(774, 216);
            btnJogRight.Name = "btnJogRight";
            btnJogRight.OriginalWidth = 120;
            btnJogRight.Size = new Size(120, 40);
            btnJogRight.TabIndex = 5;
            // 
            // btnJogLeft
            // 
            btnJogLeft.BackColor = Color.Transparent;
            btnJogLeft.ButtonText = "JOG LEFT";
            btnJogLeft.ExpandedWidth = 240;
            btnJogLeft.ExpandOnHover = false;
            btnJogLeft.FillColor = Color.FromArgb(100, 149, 237);
            btnJogLeft.Font = new Font("Segoe UI", 9F);
            btnJogLeft.ForeColor = Color.White;
            btnJogLeft.IsDraggable = false;
            btnJogLeft.Location = new Point(727, 11);
            btnJogLeft.Name = "btnJogLeft";
            btnJogLeft.OriginalWidth = 120;
            btnJogLeft.Size = new Size(120, 40);
            btnJogLeft.TabIndex = 4;
            // 
            // btnReset
            // 
            btnReset.BackColor = Color.Transparent;
            btnReset.ButtonText = "RESET";
            btnReset.ExpandedWidth = 240;
            btnReset.ExpandOnHover = false;
            btnReset.FillColor = Color.FromArgb(255, 165, 0);
            btnReset.Font = new Font("Segoe UI", 9F);
            btnReset.ForeColor = Color.White;
            btnReset.IsDraggable = false;
            btnReset.Location = new Point(919, 274);
            btnReset.Name = "btnReset";
            btnReset.OriginalWidth = 120;
            btnReset.Size = new Size(120, 40);
            btnReset.TabIndex = 3;
            // 
            // btnStop
            // 
            btnStop.BackColor = Color.Transparent;
            btnStop.ButtonText = "STOP";
            btnStop.ExpandedWidth = 240;
            btnStop.ExpandOnHover = false;
            btnStop.FillColor = Color.FromArgb(220, 40, 40);
            btnStop.Font = new Font("Segoe UI", 9F);
            btnStop.ForeColor = Color.White;
            btnStop.IsDraggable = false;
            btnStop.Location = new Point(1240, 844);
            btnStop.Name = "btnStop";
            btnStop.OriginalWidth = 120;
            btnStop.Size = new Size(120, 40);
            btnStop.TabIndex = 2;
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.Transparent;
            btnStart.ButtonText = "START";
            btnStart.ExpandedWidth = 240;
            btnStart.ExpandOnHover = false;
            btnStart.FillColor = Color.FromArgb(30, 144, 255);
            btnStart.Font = new Font("Segoe UI", 9F);
            btnStart.ForeColor = Color.White;
            btnStart.IsDraggable = false;
            btnStart.Location = new Point(585, 251);
            btnStart.Name = "btnStart";
            btnStart.OriginalWidth = 120;
            btnStart.Size = new Size(120, 40);
            btnStart.TabIndex = 1;
            // 
            // lockButton
            // 
            lockButton.BackColor = Color.Transparent;
            lockButton.FillColor = Color.FromArgb(40, 170, 70);
            lockButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lockButton.Location = new Point(727, 61);
            lockButton.Name = "lockButton";
            lockButton.Size = new Size(112, 37);
            lockButton.TabIndex = 1;
            lockButton.LockStateChanged += lockButton_LockStateChanged;
            // 
            // roundedPanel1
            // 
            roundedPanel1.BackColor = Color.Transparent;
            roundedPanel1.BorderColor = Color.FromArgb(80, 120, 180, 255);
            roundedPanel1.Controls.Add(lockButton);
            roundedPanel1.Controls.Add(btnJogLeft);
            roundedPanel1.FillColor = Color.FromArgb(30, 41, 59);
            roundedPanel1.Location = new Point(658, 940);
            roundedPanel1.Name = "roundedPanel1";
            roundedPanel1.Padding = new Padding(8);
            roundedPanel1.Size = new Size(920, 98);
            roundedPanel1.TabIndex = 8;
            // 
            // floatingLabelTextBox1
            // 
            floatingLabelTextBox1.BackColor = Color.Black;
            floatingLabelTextBox1.Location = new Point(1038, 779);
            floatingLabelTextBox1.Name = "floatingLabelTextBox1";
            floatingLabelTextBox1.Size = new Size(200, 49);
            floatingLabelTextBox1.TabIndex = 9;
            // 
            // floatingLabelTextBox2
            // 
            floatingLabelTextBox2.Location = new Point(684, 514);
            floatingLabelTextBox2.Name = "floatingLabelTextBox2";
            floatingLabelTextBox2.Size = new Size(180, 44);
            floatingLabelTextBox2.TabIndex = 9;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(1904, 1041);
            Controls.Add(floatingLabelTextBox1);
            Controls.Add(btnStop);
            Controls.Add(toggleSwitch1);
            Controls.Add(floatingLabelTextBox2);
            Controls.Add(roundedPanel1);
            Controls.Add(btnJogRight);
            Controls.Add(btnStart);
            Controls.Add(btnReset);
            Name = "Form1";
            Text = "Form1 - Safety Lock Demo";
            Load += Form1_Load;
            roundedPanel1.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private Controls.RoundedPanel roundedPanel1;
        private Controls.FloatingLabelTextBox floatingLabelTextBox1;
        private Controls.FloatingLabelTextBox floatingLabelTextBox2;
    }
}
