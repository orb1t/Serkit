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
    /// Interaction logic for RSFlipFlop.xaml
    /// </summary>
    [Serializable]
    public partial class RSFlipFlop : UserControl
    {
        public RSFlipFlop()
        {
            InitializeComponent();
        }

        private void R_StateChanged(PinControl sender, LogicState s)
        {
            //if(Clock.State == LogicState.High)
                Eval();
        }

        public void Eval()
        {
            if(R.State == LogicState.Indeterminate || S.State == LogicState.Indeterminate)
            {
                Q.State = LogicState.Indeterminate;
                return;
            }
            if (R.State == LogicState.High && S.State == LogicState.High)
                Q.State = LogicState.Indeterminate;
            else if (R.State == LogicState.High && S.State == LogicState.Low)
                Q.State = LogicState.Low;
            else if (R.State == LogicState.Low && S.State == LogicState.High)
                Q.State = LogicState.High;
            else if (R.State == LogicState.Low && S.State == LogicState.Low)
                Q.State = Q.State;
        }

        private void Q_StateChanged(PinControl sender, LogicState s)
        {
            _Q.State = (!new Bit(Q.State)).Value;
        }
    }
}
