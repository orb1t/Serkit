using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Logic.Circuit.Controls
{
    /// <summary>
    /// Interaction logic for Clock.xaml
    /// </summary>
    [Serializable]
    public partial class Clock : UserControl
    {
        private DispatcherTimer clockTimer;
        public static int clockSpeed = 500;
        public double cycles = 0;

        public DispatcherTimer Timer { get { return clockTimer; } }
        public int ClockSpeed { get { return clockSpeed; } set { clockSpeed = value; } }
        public double Cycles { get { return cycles; } set { cycles = value; } }

        public Clock()
        {
            InitializeComponent();
            clockTimer = new DispatcherTimer();
            clockTimer.Interval = new TimeSpan(0, 0, 0, 0, ClockSpeed);
            clockTimer.Tick += clockTimer_Tick;
            clockPin.State = LogicState.Low;
            clockPin.placed = true;
        }

        void clockTimer_Tick(object sender, EventArgs e)
        {
            cycles += 0.5;
            if (clockPin.State == LogicState.High)
                clockPin.State = LogicState.Low;
            else if (clockPin.State == LogicState.Low)
                clockPin.State = LogicState.High;
            else
                throw new ArgumentException("The clock is set To be Indeterminate!");
        }

        public void Start()
        {
            cycles = 0;
            clockTimer.Start();
        }

        public double Stop()
        {
            clockTimer.Stop();
            return cycles;
        }
    }
}
