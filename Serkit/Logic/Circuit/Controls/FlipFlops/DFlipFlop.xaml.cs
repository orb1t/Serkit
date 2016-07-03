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

namespace Logic.Circuit.Controls.FlipFlops
{
    /// <summary>
    /// Interaction logic for DFlipFlop.xaml
    /// </summary>
    [Serializable]
    public partial class DFlipFlop : UserControl
    {
        public DFlipFlop()
        {
            InitializeComponent();
        }

        private void Clock_StateChanged(PinControl sender, LogicState s)
        {
            if (Clock.State == LogicState.Indeterminate)
                Q.State = LogicState.Indeterminate;
            else
            {
                if (Clock.State == LogicState.Low)
                    Q.State = D.State;
                else if (Clock.State == LogicState.High && D.State == LogicState.Low)
                    Q.State = LogicState.Low;
                else if (Clock.State == LogicState.High && D.State == LogicState.High)
                    Q.State = LogicState.High;
            }
        }

        private void Q_StateChanged(PinControl sender, LogicState s)
        {
            _Q.State = (!(Bit)new Bit(Q.State)).Value;
        }
    }
}
