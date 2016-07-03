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
    public partial class Sequence : UserControl
    {
        private DispatcherTimer clockTimer;
        public static int clockSpeed = 500;
        public double cycles = 0;
        public string _sequence = "0";
        private int Iteration = 0;

        public DispatcherTimer Timer { get { return clockTimer; } }
        public int ClockSpeed { get { return clockSpeed; } set { clockSpeed = value; } }
        public double Cycles { get { return cycles; } set { cycles = value; } }
        public string SequenceString { get { return _sequence; } set { _sequence = value; } }

        public Sequence()
        {
            InitializeComponent();
            clockTimer = new DispatcherTimer();
            clockTimer.Interval = new TimeSpan(0, 0, 0, 0, ClockSpeed);
            clockTimer.Tick += clockTimer_Tick;
            seqPin.State = LogicState.Low;
            seqPin.placed = true;
        }

        void clockTimer_Tick(object sender, EventArgs e)
        {
            cycles += 0.5;
            Iteration = (Iteration + 1) % _sequence.Length;
            char c = _sequence[Iteration];
            if (c == '0')
                seqPin.State = LogicState.Low;
            else if (c == '1')
                seqPin.State = LogicState.High;
            else
                throw new ArgumentException("The sequence has some Invalid Arguments!");
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SequenceString = textseq.Text;
            textseq.Width = SequenceString.Length * (textseq.FontSize);
            if (textseq.Width == 0)
            {
                textseq.Width = 10;
            }
        }
    }
}
