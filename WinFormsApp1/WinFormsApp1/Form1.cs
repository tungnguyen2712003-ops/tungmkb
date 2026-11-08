using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp1.Controls;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Start unlocked (green). Operator clicks LOTO to engage the lock.
            lockButton.IsLocked = false;
            ApplyLockState();
        }

        private void lockButton_LockStateChanged(object sender, EventArgs e)
        {
            ApplyLockState();
        }

        private void ApplyLockState()
        {
            if (lockButton.IsLocked)
            {
                this.Text = "Form1 - LOCKOUT ACTIVE";
            }
            else
            {
                this.Text = "Form1 - Safety Lock Demo";
            }
        }
    }
}
