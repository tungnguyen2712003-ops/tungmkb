using System;
using System.Collections.Generic;
using System.Text;

namespace WinFormsApp1
{
    internal class ReadAxisConfig
    {
        private class AxisConfig
        {
            public int Card { get; set; }
            public int Slave { get; set; }
            public int AxisId { get; set; }
            public long Encoder { get; set; }
            public int Lead { get; set; }
            public int Eqiv { get; set; }
            public int JogSpeed { get; set; }
            public int HomeMethod { get; set; }
            public int HomeLowSpeed { get; set; }
            public int HomeHighSpeed { get; set; }
            public int MinSpeed { get; set; }
            public int MaxSpeed { get; set; }
            public double Acceleration { get; set; }
            public double Deceleration { get; set; }
            public int LimitNegative { get; set; }
            public int LimitPositive { get; set; }
        }
    }
}
