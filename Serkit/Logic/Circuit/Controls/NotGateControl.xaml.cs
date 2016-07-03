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
    /// Interaction logic for NotGateControl.xaml
    /// </summary>
    [Serializable]
    public partial class NotGateControl : UserControl
    {
        public NotGateControl()
        {
            InitializeComponent();
            A.placed = true;
            F.placed = true;
        }

        private void A_StateChanged(PinControl sender, LogicState s)
        {
            Eval();
        }
        public void Eval()
        {
            if (A.State == LogicState.Indeterminate)
                F.State = LogicState.Indeterminate;
            else
            {
                if (A.State == LogicState.Low)
                    F.State = LogicState.High;
                else if (A.State == LogicState.High)
                    F.State = LogicState.Low;
            }
        }
    }
}
