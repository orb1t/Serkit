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

namespace Logic.Circuit.Controls
{
    /// <summary>
    /// Interaction logic for AndGateControl.xaml
    /// </summary>
    [Serializable]
    public partial class AndGateControl : UserControl
    {
        public AndGateControl()
        {
            InitializeComponent();
            A.placed = true;
            B.placed = true;
            F.placed = true;
        }

        private void A_StateChanged(PinControl sender, LogicState s)
        {
            Eval();
        }

        private void B_StateChanged(PinControl sender, LogicState s)
        {
            Eval();
        }

        public void Eval()
        {
            if (A.State == LogicState.Indeterminate || B.State == LogicState.Indeterminate)
                F.State = LogicState.Indeterminate;
            else
            {
                if (A.State == LogicState.High && B.State == LogicState.High)
                    F.State = LogicState.High;
                else if (A.State == LogicState.High && B.State == LogicState.Low)
                    F.State = LogicState.Low;
                else if (A.State == LogicState.Low && B.State == LogicState.High)
                    F.State = LogicState.Low;
                else if (A.State == LogicState.Low && B.State == LogicState.Low)
                    F.State = LogicState.Low;
            }
        }

        public override string ToString()
        {

            return base.ToString();
        }
    }
}
