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
    /// Interaction logic for LED.xaml
    /// </summary>
    [Serializable]
    public partial class LED : UserControl
    {
        private SolidColorBrush _brush;

        public SolidColorBrush Brush { get { return _brush; } set { _brush = value; } }

        public LED()
        {
            InitializeComponent();
            ledPin.placed = true;
            ledPin.Label = "LED Pin";
        }
    }
}
