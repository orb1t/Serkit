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
using Logic;
using Serkit;

namespace Logic.Circuit.Controls
{
    /// <summary>
    /// Interaction logic for PinControl.xaml
    /// </summary>
    [Serializable]
    public partial class PinControl : UserControl
    {
        public bool placed;
        protected string _label;
        protected PinControl _connection;
        protected LogicState _state;
        protected Point _location;
        protected bool _transp;

        //LOGISTIC
        public string Label { get { return _label; } set { _label = value; this.ToolTip = new ToolTip() { Content = Label }; } }
        public PinControl Connection { get { return _connection; } set { _connection = value; } }
        public virtual LogicState State
        {
            get { return _state; }
            set { _state = value; SetState(value, 0); }
        }
        public void SetState(LogicState s)
        {
            _state = s;
            SetState(s, 0);
        }
        public void SetState(LogicState s, int linkLevel)
        {
            _state = s;
            if (linkLevel <= 1 && Connection != null) Connection.SetState(_state, ++linkLevel);
            StateChanged(this, _state);
        }
        public void SetName(string s)
        {
            Name = s;
            ToolTip = new ToolTip() { Content = s };
        }
        public Brush Brush { get { return ellipse.Fill; } set { ellipse.Fill = value; } }
        public bool Transparency { 
            get { return _transp; }
            set
            {
                _transp = value;
                if (_transp)
                    ellipse.Fill = new SolidColorBrush(new Color() { A = 50, R = 0, G = 0, B = 0 });
                else
                    ellipse.Fill = new SolidColorBrush(new Color() { A = 255, R = 121, G = 157, B = 255 });
            }
        }

        public delegate void StateChangeHandler(PinControl sender, LogicState s);

        public event StateChangeHandler StateChanged;

        public PinControl()
        {
            InitializeComponent();
            StateChanged += PinControl_StateChanged;
        }

        void PinControl_StateChanged(PinControl sender, LogicState s)
        {
            Console.WriteLine(Name + " : " + State.ToString());
            ToolTip = new ToolTip() { Content = (State == LogicState.High) ? "High" : (State == LogicState.Low) ? "Low" : "Indeterminate" };
            if (_state == LogicState.High && !_transp)
                ellipse.Fill = new SolidColorBrush(new Color() { A = 255, R = 255, G = 188, B = 188 });
            else if (_state == LogicState.Low && !_transp)
                ellipse.Fill = new SolidColorBrush(new Color() { A = 255, R = 20, G = 20, B = 20 });
            else
                ellipse.Fill = new SolidColorBrush(new Color() { A = 255, R = 121, G = 157, B = 255 });
                
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            selectionArea.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            selectionArea.Visibility = System.Windows.Visibility.Visible;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            App.ActiveMouseOverPin = this;
            App.MouseOverPin = true;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            App.MouseOverPin = false;
        }
    }
}
