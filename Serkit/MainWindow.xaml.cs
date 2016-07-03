using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Serkit;
using System.Runtime.Serialization.Formatters.Binary;
using Logic.Circuit;
using Logic.Circuit.Controls;
using Logic.Circuit.Controls.FlipFlops;
using Logic.Circuit.Controls.Memory;

namespace Serkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //COMPONENT COUNT
        int pinCount = 0;
        int gateCount = 0;
        int ffCount = 0;
        int memCount = 0;
        int ledCount = 0;
        int textCount = 0;
        int gatePinCount = 0;
        int clockCount = 0;
        int seqCount = 0;
        List<PinControl> pins = new List<PinControl>();
        List<Wire> wires = new List<Wire>();
        Wire currentWire;

        #region VARIABLES

            #region BOOLEAN
            bool save = true;
            bool clockSet = false;
            bool maximised;
            bool pointFixed = false;
            bool penModeOn = false;
            bool Ctrl = false;
            bool mouseDown = false;
            bool wireOpen = false;
            bool pointerHand = true;
            #endregion

            #region INTEGERS and DOUBLES
            int gridWidth = 5;
            int gridHeight = 5;
            int selectedCircuitTab = 0;
            int upd = 0;
            const double ScaleRate = 1.05;
            double prevh, prevw, prevl, prevt;
            double CellHeight, CellWidth;
            #endregion

            #region STRINGS
            string circuitName = "Unnamed";
            string pointerMode = "Normal";
            string keyPressed = "";
            string keyReleased = "";
            string logicTextBuffer = "";
            #endregion

            #region OTHERS
            private HwndSource _hwndSource;
            Point penLinkHistory;
            Cursor current;
            PinControl prevPin;
            List<Clock> clocks = new List<Clock>();
            List<Sequence> sequences = new List<Sequence>();
            Control selectedControl;
            #endregion

        #endregion

        #region BACKGROUNDS
        SolidColorBrush menuSel = new SolidColorBrush(new Color() { A = 255, R = 245, G = 245, B = 245 });
        #endregion


        public MainWindow()
        {
            InitializeComponent();
            logicPanel.Visibility = System.Windows.Visibility.Visible;
            flipFlopPanel.Visibility = System.Windows.Visibility.Hidden;
            memoryPanel.Visibility = System.Windows.Visibility.Hidden;
            maximised = true;
            prevh = window.Height;
            prevw = window.Width;
            prevl = window.Left;
            prevt = window.Top;
            window.Height = SystemParameters.WorkArea.Height;
            window.Width = SystemParameters.WorkArea.Width;
            window.Left = 0;
            window.BorderThickness = new Thickness(0);
            window.Top = 0;
            maxres.Content = "";
        }

        #region PEN MODES

        void pinMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            pt = RoundToNearest(pt);
            PinControl pin = new PinControl();
            pin.Name = "Pin_" + pinCount++;
            Canvas.SetLeft(pin, pt.X);
            Canvas.SetTop(pin, pt.Y);
            pin.MouseDown += controlMouseDown;
            pin.MouseUp += controlMouseUp;
            canvas.Children.Add(pin);
            pin.placed = true;
            pointerMode = "Normal";
            ShowStatus("Placed PIN at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void andGateMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            pt = RoundToNearest(pt);
            AndGateControl gate1 = new AndGateControl();
            gate1.Name = "Gate_" + gateCount++;
            gate1.A.Name = "Gate_Pin_" + gatePinCount++;
            gate1.B.Name = "Gate_Pin_" + gatePinCount++;
            gate1.F.Name = "Gate_Pin_" + gatePinCount++;
            Canvas.SetLeft(gate1, pt.X - 40);
            Canvas.SetTop(gate1, pt.Y - 20);
            gate1.MouseDown += controlMouseDown;
            gate1.MouseUp += controlMouseUp;
            canvas.Children.Add(gate1);
            pointerMode = "Normal";
            ShowStatus("Placed AND gate at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void orGateMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            pt = RoundToNearest(pt);
            OrGateControl gate1 = new OrGateControl();
            gate1.Name = "Gate_" + gateCount++;
            gate1.A.Name = "Gate_Pin_" + gatePinCount++;
            gate1.B.Name = "Gate_Pin_" + gatePinCount++;
            gate1.F.Name = "Gate_Pin_" + gatePinCount++;
            Canvas.SetLeft(gate1, pt.X - 40);
            Canvas.SetTop(gate1, pt.Y - 20);
            gate1.MouseDown += controlMouseDown;
            gate1.MouseUp += controlMouseUp;
            canvas.Children.Add(gate1);
            pointerMode = "Normal";
            ShowStatus("Placed OR gate at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void notGateMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            pt = RoundToNearest(pt);
            NotGateControl gate1 = new NotGateControl();
            gate1.Name = "Gate_" + gateCount++;
            gate1.A.Name = "Gate_Pin_" + gatePinCount++;
            gate1.F.Name = "Gate_Pin_" + gatePinCount++;
            Canvas.SetLeft(gate1, pt.X - 40);
            Canvas.SetTop(gate1, pt.Y - 20);
            gate1.MouseDown += controlMouseDown;
            gate1.MouseUp += controlMouseUp;
            canvas.Children.Add(gate1);
            pointerMode = "Normal";
            ShowStatus("Placed NOT gate at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void xorGateMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            pt = RoundToNearest(pt);
            XorGateControl gate1 = new XorGateControl();
            gate1.Name = "Gate_" + gateCount++;
            gate1.A.Name = "Gate_Pin_" + gatePinCount++;
            gate1.B.Name = "Gate_Pin_" + gatePinCount++;
            gate1.F.Name = "Gate_Pin_" + gatePinCount++;
            Canvas.SetLeft(gate1, pt.X - 40);
            Canvas.SetTop(gate1, pt.Y - 20);
            gate1.MouseDown += controlMouseDown;
            gate1.MouseUp += controlMouseUp;
            canvas.Children.Add(gate1);
            pointerMode = "Normal";
            ShowStatus("Placed XOR gate at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void xnorGateMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            pt = RoundToNearest(pt);
            XnorGateControl gate1 = new XnorGateControl();
            gate1.Name = "Gate_" + gateCount++;
            gate1.A.Name = "Gate_Pin_" + gatePinCount++;
            gate1.B.Name = "Gate_Pin_" + gatePinCount++;
            gate1.F.Name = "Gate_Pin_" + gatePinCount++;
            Canvas.SetLeft(gate1, pt.X - 40);
            Canvas.SetTop(gate1, pt.Y - 20);
            gate1.MouseDown += controlMouseDown;
            gate1.MouseUp += controlMouseUp;
            canvas.Children.Add(gate1);
            pointerMode = "Normal";
            ShowStatus("Placed XNOR gate at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void norGateMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            pt = RoundToNearest(pt);
            NorGateControl gate1 = new NorGateControl();
            gate1.Name = "Gate_" + gateCount++;
            gate1.A.Name = "Gate_Pin_" + gatePinCount++;
            gate1.B.Name = "Gate_Pin_" + gatePinCount++;
            gate1.F.Name = "Gate_Pin_" + gatePinCount++;
            Canvas.SetLeft(gate1, pt.X - 40);
            Canvas.SetTop(gate1, pt.Y - 20);
            gate1.MouseDown += controlMouseDown;
            gate1.MouseUp += controlMouseUp;
            canvas.Children.Add(gate1);
            pointerMode = "Normal";
            ShowStatus("Placed NOR gate at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void nandGateMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            pt = RoundToNearest(pt);
            NandGateControl gate1 = new NandGateControl();
            gate1.Name = "Gate_" + gateCount++;
            gate1.A.Name = "Gate_Pin_" + gatePinCount++;
            gate1.B.Name = "Gate_Pin_" + gatePinCount++;
            gate1.F.Name = "Gate_Pin_" + gatePinCount++;
            Canvas.SetLeft(gate1, pt.X - 40);
            Canvas.SetTop(gate1, pt.Y - 20);
            gate1.MouseDown += controlMouseDown;
            gate1.MouseUp += controlMouseUp;
            canvas.Children.Add(gate1);
            pointerMode = "Normal";
            ShowStatus("Placed NAND gate at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void rsFFMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            pt = RoundToNearest(pt);
            RSFlipFlop gate1 = new RSFlipFlop();
            gate1.Name = "FF_" + ffCount++;
            gate1.R.Name = "FF_Pin_" + gatePinCount++;
            gate1.S.Name = "FF_Pin_" + gatePinCount++;
            gate1.Q.Name = "FF_Pin_" + gatePinCount++;
            gate1._Q.Name = "FF_Pin_" + gatePinCount++;
            gate1.Clock.Name = "FF_Pin_" + gatePinCount++;
            Canvas.SetLeft(gate1, pt.X - 50);
            Canvas.SetTop(gate1, pt.Y - 30);
            gate1.MouseDown += controlMouseDown;
            gate1.MouseUp += controlMouseUp;
            canvas.Children.Add(gate1);
            pointerMode = "Normal";
            ShowStatus("Placed RS Flip Flop at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void jkFFMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            JKFlipFlop gate1 = new JKFlipFlop();
            gate1.Name = "FF_" + ffCount++;
            gate1.J.Name = "FF_Pin_" + gatePinCount++;
            gate1.K.Name = "FF_Pin_" + gatePinCount++;
            gate1.Q.Name = "FF_Pin_" + gatePinCount++;
            gate1._Q.Name = "FF_Pin_" + gatePinCount++;
            gate1.Clock.Name = "FF_Pin_" + gatePinCount++;
            Canvas.SetLeft(gate1, pt.X - 50);
            Canvas.SetTop(gate1, pt.Y - 30);
            gate1.MouseDown += controlMouseDown;
            gate1.MouseUp += controlMouseUp;
            canvas.Children.Add(gate1);
            pointerMode = "Normal";
            ShowStatus("Placed JK Flip Flop at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void dFFMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            DFlipFlop gate1 = new DFlipFlop();
            gate1.Name = "FF_" + ffCount++;
            gate1.D.Name = "FF_Pin_" + gatePinCount++;
            gate1.Q.Name = "FF_Pin_" + gatePinCount++;
            gate1._Q.Name = "FF_Pin_" + gatePinCount++;
            gate1.Clock.Name = "FF_Pin_" + gatePinCount++;
            Canvas.SetLeft(gate1, pt.X - 50);
            Canvas.SetTop(gate1, pt.Y - 30);
            gate1.MouseDown += controlMouseDown;
            gate1.MouseUp += controlMouseUp;
            canvas.Children.Add(gate1);
            pointerMode = "Normal";
            ShowStatus("Placed D Flip Flop at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void tFFMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            TFlipFlop gate1 = new TFlipFlop();
            gate1.Name = "FF_" + ffCount++;
            gate1.T.Name = "FF_Pin_" + gatePinCount++;
            gate1.Q.Name = "FF_Pin_" + gatePinCount++;
            gate1._Q.Name = "FF_Pin_" + gatePinCount++;
            gate1.Clock.Name = "FF_Pin_" + gatePinCount++;
            Canvas.SetLeft(gate1, pt.X - 50);
            Canvas.SetTop(gate1, pt.Y - 30);
            gate1.MouseDown += controlMouseDown;
            gate1.MouseUp += controlMouseUp;
            canvas.Children.Add(gate1);
            pointerMode = "Normal";
            ShowStatus("Placed T Flip Flop at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void clockMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            Clock clock = new Clock();
            clock.clockPin.Name = "Clock_" + clockCount;
            clock.Name = "Clock_" + clockCount++;
            Canvas.SetLeft(clock, pt.X - 25);
            Canvas.SetTop(clock, pt.Y - 10);
            clock.MouseDown += controlMouseDown;
            clock.MouseUp += controlMouseUp;
            canvas.Children.Add(clock);
            clockSet = true;
            clock.Timer.Tick += clockTimer_Tick;
            clocks.Add(clock);
            ShowStatus("Placed CLOCK at : (" + pt.X + "," + pt.Y + ")");
            pointerMode = "Normal";
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void seqMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            Sequence seq = new Sequence();
            seq.Focusable = false;
            seq.seqPin.Name = "Sequence_" + seqCount;
            seq.Name = "Sequence_" + seqCount++;
            Canvas.SetLeft(seq, pt.X - 25);
            Canvas.SetTop(seq, pt.Y - 10);
            seq.MouseDown += controlMouseDown;
            seq.MouseUp += controlMouseUp;
            canvas.Children.Add(seq);
            ShowStatus("Placed SEQ at : (" + pt.X + "," + pt.Y + ")");
            pointerMode = "Normal";
            sequences.Add(seq);
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void mem16x8Mode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            Memory16x8 gate1 = new Memory16x8();
            gate1.Name = "Mem_" + memCount++;
            Canvas.SetLeft(gate1, pt.X - 50);
            Canvas.SetTop(gate1, pt.Y - 50);
            gate1.MouseDown += controlMouseDown;
            gate1.MouseUp += controlMouseUp;
            canvas.Children.Add(gate1);
            pointerMode = "Normal";
            ShowStatus("Placed 16 x 8 RAM at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void ledMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            LED gate1 = new LED();
            gate1.ledPin.Name = "LED_" + ledCount;
            gate1.Name = "LED_" + ledCount++;
            Canvas.SetLeft(gate1, pt.X - 10.5);
            Canvas.SetTop(gate1, pt.Y - 10.5);
            gate1.MouseDown += controlMouseDown;
            gate1.MouseUp += controlMouseUp;
            gate1.ToolTip = new ToolTip() { Content = "LED :\n" + (pt.X - 10.5) + "," + (pt.Y - 10.5) };
            canvas.Children.Add(gate1);
            pointerMode = "Normal";
            ShowStatus("Placed LED at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void textMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas);
            TextBox text = new TextBox() { Text = "" };
            text.Name = "TEXT_" + textCount++;
            Canvas.SetLeft(text, pt.X - 10.5);
            Canvas.SetTop(text, pt.Y - 10.5);
            text.IsReadOnly = true;
            text.Focusable = false;
            text.FontSize = 16;
            text.Height = 26;
            text.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            text.Width = 120;
            text.BorderThickness = new Thickness(1);
            text.FontFamily = new FontFamily("Segoe UI");
            text.MouseDown += textBoxMouseDown;
            text.MouseUp += controlMouseUp;
            text.MouseDoubleClick += text_MouseDoubleClick;
            text.GotFocus += text_GotFocus;
            text.LostFocus += text_LostFocus;
            text.MouseEnter += text_MouseEnter;
            text.MouseLeave += text_MouseLeave;
            text.Padding = new Thickness(2);
            text.Tag = "TEXT";
            canvas.Children.Add(text);
            pointerMode = "Normal";
            ShowStatus("Placed TEXT Box at : (" + pt.X + "," + pt.Y + ")");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }

        void text_GotFocus(object sender, RoutedEventArgs e)
        {
            switchMenu("EDTMNU");
        }
        void text_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.Background = new SolidColorBrush(Colors.White);
        }
        void text_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.Background = new SolidColorBrush(Color.FromRgb(245,245,245));
        }
        void text_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.Focusable = false;
            t.IsReadOnly = true;
        }
        void text_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.IsReadOnly = false;
            t.Focusable = true;
            t.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            switchMenu("EDTMNU");
        }
        void canvasPenMode(object sender, MouseButtonEventArgs e)
        {
            Point pt = Mouse.GetPosition(canvas);
            double X = pt.X;
            double Y = pt.Y;
            //double X = Canvas.GetLeft(App.ActiveMouseOverPin);
            //double Y = Canvas.GetTop(App.ActiveMouseOverPin);
            if(penModeOn)
            {
                if (App.MouseOverPin)
                {
                    if (!App.PinSelected)
                    {
                        currentWire = new Wire();
                        Line l = currentWire.Add(new Point() { X = X, Y = Y }, canvas);
                        if(l!=null)
                            l.MouseDown += shapeMouseDown;
                        ShowStatus("Connected Wire End to Pin : " + App.ActiveMouseOverPin.Name);
                        wireOpen = true;
                        App.SelectedPin = App.ActiveMouseOverPin;
                        App.PinSelected = true;
                    }
                    else
                    {
                        Line l = currentWire.Add(new Point() { X = X, Y = Y }, canvas);
                        if (l != null)
                            l.MouseDown += shapeMouseDown;
                        wires.Add(currentWire);
                        PinControl p1 = App.SelectedPin;
                        PinControl p2 = App.ActiveMouseOverPin;
                        p1.Connection = p2;
                        p2.Connection = p1;
                        ShowStatus("Connected Pin" + p1.Name + " to Pin : " + p2.Name);
                        App.SelectedPin = null;
                        App.PinSelected = false;
                        wireOpen = false;
                        penModeOn = false;
                        penIcon.Foreground = new SolidColorBrush(new Color() { A = 100, R = 121, G = 121, B = 121 });
                        penIcon.Text = "";
                        penLabel.Text = "Wire";
                        MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
                        current = new Cursor(memoryStream);
                        Cursor = current;
                    }

                }
                else if (wireOpen)
                {
                    Line l = currentWire.Add(pt, canvas);
                    if (l != null)
                        l.MouseDown += shapeMouseDown;
                    ShowStatus(currentWire.Count + "");
                }
            }
        }
        void controlMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (pointerMode.Equals("Erase"))
            {
                canvas.Children.Remove((UserControl)sender);
            }
            else
            {
                selectedControl = (UserControl)sender;
            }
            mouseDown = true;
        }
        void textBoxMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (pointerMode.Equals("Erase"))
            {
                canvas.Children.Remove((TextBox)sender);
            }
            else
            {
                selectedControl = (TextBox)sender;
                switchMenu("EDTMNU");
            }
            mouseDown = true;
        }
        void controlMouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
        }
        void shapeMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (pointerMode.Equals("Erase"))
            {
                canvas.Children.Remove((Shape)sender);
            }
        }

        #endregion

        #region GATES and BUTTONS

        private void pin_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "PIN";
            ShowStatus("Insert PIN");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointerHand);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void andGate_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "AND";
            ShowStatus("Insert AND Gate");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.addAnd);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void orGate_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "OR";
            ShowStatus("Insert OR Gate");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.addOr);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void notGate_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "NOT";
            ShowStatus("Insert NOT Gate");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.addNot);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void nandGate_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "NAND";
            ShowStatus("Insert NAND Gate");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.addNand);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void norGate_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "NOR";
            ShowStatus("Insert NOR Gate");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.addNor);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void xorGate_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "XOR";
            ShowStatus("Insert XOR Gate");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.addXor);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void xnorGate_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "XNOR";
            ShowStatus("Insert XNOR Gate");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.addXnor);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void rsFlipFlop_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "RSFF";
            ShowStatus("Insert R S Flip Gate");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.rsFF);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void jkFlipFlop_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "JKFF";
            ShowStatus("Insert J K Flip Gate");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.jkFF);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void dFlipFlop_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "DFF";
            ShowStatus("Insert D Flip Gate");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.dFF);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void tFlipFlop_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "TFF";
            ShowStatus("Insert T Flip Gate");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.tFF);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void mem16x8_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "MEM16X8";
            ShowStatus("Insert Memory 16 x 8");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources._16x8);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void clock_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "CLOCK";
            ShowStatus("Insert CLOCK");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pen);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void sequence_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "SEQ";
            ShowStatus("Insert Sequence");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pen);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void led_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "LED";
            ShowStatus("Insert LED");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pen);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void text_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "TEXT";
            ShowStatus("Insert TEXT");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointerHand);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void eraseMode_Click(object sender, RoutedEventArgs e)
        {
            MemoryStream memoryStream;
            if (wireOpen)
            {
                penIcon.Foreground = new SolidColorBrush(new Color() { A = 100, R = 121, G = 121, B = 121 });
                penIcon.Text = "";
                penLabel.Text = "Wire";
                wireOpen = false;
                pointFixed = false;
                memoryStream = new MemoryStream(Properties.Resources.pointer);
                current = new Cursor(memoryStream);
                Cursor = current;
            }
            pointerMode = "Erase";
            ShowStatus("Pointer Mode : Erase");
            memoryStream = new MemoryStream(Properties.Resources.delete);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void penMode_Click(object sender, RoutedEventArgs e)
        {
            pointerMode = "Pen";
            ShowStatus("Pointer Mode : Pen");
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pen);
            current = new Cursor(memoryStream);
            Cursor = current;
            penModeOn = !penModeOn;
            if (penModeOn)
            {
                penIcon.Foreground = new SolidColorBrush(new Color() { A = 100, R = 236, G = 70, B = 70 });
                penIcon.Text = "";
                penLabel.Text = "Done";
            }
            else
            {
                wires.Add(currentWire);
                currentWire = null;
                wireOpen = false;
                penModeOn = false;
                penIcon.Foreground = new SolidColorBrush(new Color() { A = 100, R = 121, G = 121, B = 121 });
                penIcon.Text = "";
                penLabel.Text = "Wire";
                memoryStream = new MemoryStream(Properties.Resources.pointer);
                current = new Cursor(memoryStream);
                Cursor = current;
            }
        }
        private void pointerHandMode_Click(object sender, RoutedEventArgs e)
        {
            MemoryStream memoryStream;
            if (wireOpen)
            {
                penIcon.Foreground = new SolidColorBrush(new Color() { A = 100, R = 121, G = 121, B = 121 });
                penIcon.Text = "";
                penLabel.Text = "Wire";
                wireOpen = false;
                pointFixed = false;
                memoryStream = new MemoryStream(Properties.Resources.pointer);
                current = new Cursor(memoryStream);
                Cursor = current;
            }
            if (pointerHand)
            {
                pointerMode = "PointerHand";
                ShowStatus("Pointer Mode : Pointer Hand");
                memoryStream = new MemoryStream(Properties.Resources.pointerHand);
                current = new Cursor(memoryStream);
                Cursor = current;
            }
            else
            {
                pointerMode = "Pointer";
                ShowStatus("Pointer Mode : Pointer");
                memoryStream = new MemoryStream(Properties.Resources.pointer);
                current = new Cursor(memoryStream);
                Cursor = current;
            }
        }
        private void pointerHandMode_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            pointerHand = !pointerHand;
            if (pointerHand)
            {
                pointerMode = "PointerHand";
                ShowStatus("Pointer Mode : Pointer Hand");
                MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointerHand);
                current = new Cursor(memoryStream);
                Cursor = current;
            }
            else
            {
                pointerMode = "Pointer";
                ShowStatus("Pointer Mode : Pointer");
                MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
                current = new Cursor(memoryStream);
                Cursor = current;
            }
        }

        #endregion

        //Imported
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);
        
        void ShowStatus(string s)
        {
            if (infoLabel != null)
                infoLabel.Content = s;
        }
        private Point RoundToNearest(Point pt)
        {
            Point newPoint = new Point();
            int X = (int)pt.X;
            int Y = (int)pt.Y;
            if (pt.X % gridWidth < gridWidth / 2)
                newPoint.X = X - (X % gridWidth);
            else
                newPoint.X = X + (gridWidth - X % gridWidth);

            if (pt.Y % gridHeight < gridHeight / 2)
                newPoint.Y = Y - (Y % gridHeight);
            else
                newPoint.Y = Y + (gridHeight - Y % gridHeight);
            newPoint.X += 1;
            newPoint.Y += 1;
            return newPoint;
        }


        private void exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            exit.Background = new SolidColorBrush(new Color() { A = 100, R = 245, G = 245, B = 245 });
        }
        private void window_StateChanged(object sender, EventArgs e)
        {
            if (window.WindowState == System.Windows.WindowState.Maximized)
            {
                window.WindowState = System.Windows.WindowState.Normal;
                maximised = true;
                prevh = window.Height;
                prevw = window.Width;
                prevl = window.Left;
                prevt = window.Top;
                window.Height = SystemParameters.WorkArea.Height;
                window.Width = SystemParameters.WorkArea.Width;
                window.Left = 0;
                window.Top = 0;
                maxres.Content = "";
            }
        }
        private void min_Click(object sender, RoutedEventArgs e)
        {
            window.WindowState = System.Windows.WindowState.Minimized;
        }
        private void maxres_Click(object sender, RoutedEventArgs e)
        {
            if (maximised)
            {
                maximised = false;
                window.WindowState = System.Windows.WindowState.Normal;
                window.BorderThickness = new Thickness(1);
                window.Height = prevh;
                window.Width = prevw;
                window.Left = prevl;
                window.Top = prevt;
                maxres.Content = "";
            }
            else
            {
                maximised = true;
                prevh = window.Height;
                prevw = window.Width;
                prevl = window.Left;
                prevt = window.Top;
                window.Height = SystemParameters.WorkArea.Height;
                window.Width = SystemParameters.WorkArea.Width;
                window.Left = 0;
                window.BorderThickness = new Thickness(0);
                window.Top = 0;
                maxres.Content = "";
            }
        }
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }
        private void moveRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        protected void ResizeRectangle_MouseMove(Object sender, MouseEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name)
            {
                case "top":
                    Cursor = Cursors.SizeNS;
                    break;
                case "bottom":
                    Cursor = Cursors.SizeNS;
                    break;
                case "left":
                    Cursor = Cursors.SizeWE;
                    break;
                case "right":
                    Cursor = Cursors.SizeWE;
                    break;
                case "topLeft":
                    Cursor = Cursors.SizeNWSE;
                    break;
                case "topRight":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "bottomLeft":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "bottomRight":
                    Cursor = Cursors.SizeNWSE;
                    break;
                default:
                    Cursor = current;
                    break;
            }
        }
        private void window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                Cursor = current;
        }
        private void window_SourceInitialized(object sender, EventArgs e)
        {
            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
        }
        protected void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name)
            {
                case "top":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "bottom":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "left":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "right":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "topLeft":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "topRight":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "bottomLeft":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "bottomRight":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                default:
                    Cursor = current;
                    break;
            }
        }
        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
        }
        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }
        private void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem m = (MenuItem)sender;
            m.Background = null;
            m.BorderBrush = null;
        }
        private void exit_MouseEnter(object sender, MouseEventArgs e)
        {
            exit.Background = new SolidColorBrush(new Color() { A = 100, R = 255, G = 165, B = 165 });
        }
        private void exit_MouseLeave(object sender, MouseEventArgs e)
        {
            exit.Background = null;
        }
        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                Ctrl = true;
            }
            if(!keyPressed.Contains("<"+e.Key.ToString()+">"))
                keyPressed += "<" + e.Key.ToString() + ">";
            ShowStatus("Key Pressed : " + keyPressed);
        }
        private void window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                Ctrl = false;
            }
            if(keyPressed.Contains("<"+e.Key.ToString()+">"))
                keyPressed = keyPressed.Remove(keyPressed.IndexOf("<" + e.Key.ToString()+">"), e.Key.ToString().Length + 2);
            ShowStatus("Key Released : " + keyPressed);
        }
        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }
        private void canvas_GotMouseCapture(object sender, MouseEventArgs e)
        {
            if (pointerMode.Equals("Normal"))
                current = Cursors.Arrow;
            if (pointerMode.Equals("Pen"))
                current = Cursors.Pen;
            if (pointerMode.Equals("Erase"))
                current = Cursors.No;
            if (pointerMode.Equals("AND"))
                current = Cursors.Cross;
            Cursor = current;
        }
        private void desk_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = current;
        }
        public void CloseWindow()
        {
            Close();
            Application.Current.Shutdown(0);
        }

        //CANVAS
        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            pointerMode = "NORMAL";
            if (currentWire != null)
            {
                wires.Add(currentWire);
                currentWire = null;
            }
            wires.Add(currentWire);
            currentWire = null;
            penModeOn = false;
            wireOpen = false;
            penIcon.Foreground = new SolidColorBrush(new Color() { A = 100, R = 121, G = 121, B = 121 });
            penIcon.Text = "";
            penLabel.Text = "Wire";
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        private void canvas_KeyDown(object sender, KeyEventArgs e)
        {
        }
        private void canvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                Ctrl = false;
            }
            ShowStatus("Key Released : " + e.Key.ToString());
        }
        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(canvas);
            if (!pointFixed && wireOpen)
            {
                penLinkHistory = Mouse.GetPosition(canvas);
                pointFixed = true;
            }
        }
        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Ctrl)
            {
                if (e.Delta > 0)
                {
                    canvasScaleTransform.ScaleX *= ScaleRate;
                    canvasScaleTransform.ScaleY *= ScaleRate;
                }
                else
                {
                    canvasScaleTransform.ScaleX /= ScaleRate;
                    canvasScaleTransform.ScaleY /= ScaleRate;
                }
            }
        }
        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (pointerMode.Equals("Normal"))
            {

            }
            else if (pointerMode.Equals("Pen"))
            {
                canvasPenMode(sender, e);
            }
            else if (pointerMode.Equals("AND"))
            {
                andGateMode(sender, e);
            }
            else if (pointerMode.Equals("OR"))
            {
                orGateMode(sender, e);
            }
            else if (pointerMode.Equals("NOT"))
            {
                notGateMode(sender, e);
            }
            else if (pointerMode.Equals("NAND"))
            {
                nandGateMode(sender, e);
            }
            else if (pointerMode.Equals("NOR"))
            {
                norGateMode(sender, e);
            }
            else if (pointerMode.Equals("XOR"))
            {
                xorGateMode(sender, e);
            }
            else if (pointerMode.Equals("XNOR"))
            {
                xnorGateMode(sender, e);
            }
            else if (pointerMode.Equals("PIN"))
            {
                pinMode(sender, e);
            }
            else if (pointerMode.Equals("RSFF"))
            {
                rsFFMode(sender, e);
            }
            else if (pointerMode.Equals("JKFF"))
            {
                jkFFMode(sender, e);
            }
            else if (pointerMode.Equals("DFF"))
            {
                dFFMode(sender, e);
            }
            else if (pointerMode.Equals("TFF"))
            {
                tFFMode(sender, e);
            }
            else if (pointerMode.Equals("CLOCK"))
            {
                clockMode(sender, e);
            }
            else if (pointerMode.Equals("SEQ"))
            {
                seqMode(sender, e);
            }
            else if (pointerMode.Equals("MEM16X8"))
            {
                mem16x8Mode(sender, e);
            }
            else if (pointerMode.Equals("LED"))
            {
                ledMode(sender, e);
            }
            else if (pointerMode.Equals("TEXT"))
            {
                textMode(sender, e);
            }
        }
        private void canvas_GotFocus(object sender, RoutedEventArgs e)
        {
            switchMenu("CKTMNU");
        }
        private void canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown && selectedControl != null)
            {
                Point pt = RoundToNearest(Mouse.GetPosition(canvas));
                double w = selectedControl.Width;
                double h = selectedControl.Height;
                Canvas.SetLeft(selectedControl, pt.X - w / 2);
                Canvas.SetTop(selectedControl, pt.Y - h / 2);
            }
        }

        
        //TABS
        private void logicPanelButton_Click(object sender, RoutedEventArgs e)
        {
            selectedCircuitTab = 0;
            logicPanelButton.Background = new SolidColorBrush(new Color() { A = 255, R = 193, G = 255, B = 207 });
            flipFlopPanelButton.Background = new SolidColorBrush(new Color() { A = 255, R = 234, G = 234, B = 234 });
            memoryPanelButton.Background = new SolidColorBrush(new Color() { A = 255, R = 234, G = 234, B = 234 });
            logicPanel.Visibility = System.Windows.Visibility.Visible;
            flipFlopPanel.Visibility = System.Windows.Visibility.Hidden;
            memoryPanel.Visibility = System.Windows.Visibility.Hidden;
        }
        private void flipFlopPanelButton_Click(object sender, RoutedEventArgs e)
        {
            selectedCircuitTab = 1;
            flipFlopPanelButton.Background = new SolidColorBrush(new Color() { A = 255, R = 255, G = 216, B = 216 });
            logicPanelButton.Background = new SolidColorBrush(new Color() { A = 255, R = 234, G = 234, B = 234 });
            memoryPanelButton.Background = new SolidColorBrush(new Color() { A = 255, R = 234, G = 234, B = 234 });
            logicPanel.Visibility = System.Windows.Visibility.Hidden;
            flipFlopPanel.Visibility = System.Windows.Visibility.Visible;
            memoryPanel.Visibility = System.Windows.Visibility.Hidden;
        }
        private void memoryPanelButton_Click(object sender, RoutedEventArgs e)
        {
            selectedCircuitTab = 2;
            memoryPanelButton.Background = new SolidColorBrush(new Color() { A = 255, R = 250, G = 255, B = 195 });
            logicPanelButton.Background = new SolidColorBrush(new Color() { A = 255, R = 234, G = 234, B = 234 });
            flipFlopPanelButton.Background = new SolidColorBrush(new Color() { A = 255, R = 234, G = 234, B = 234 });
            logicPanel.Visibility = System.Windows.Visibility.Hidden;
            flipFlopPanel.Visibility = System.Windows.Visibility.Hidden;
            memoryPanel.Visibility = System.Windows.Visibility.Visible;
        }


        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowStatus(Environment.CurrentDirectory);
            MemoryStream memoryStream = new MemoryStream(Properties.Resources.pointer);
            current = new Cursor(memoryStream);
            Cursor = current;
        }
        void clockTimer_Tick(object sender, EventArgs e)
        {
            ShowStatus("Cycle Count : " + clocks[0].Cycles);
        }
        public void demo()
        {

        }
        private void properties_LostFocus(object sender, RoutedEventArgs e)
        {
        }
        private void propertiesGrid_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        //CLOCK
        private void runCircuit_Click(object sender, RoutedEventArgs e)
        {
            StartAllClocks();
        }
        private void stopCircuit_Click(object sender, RoutedEventArgs e)
        {
            StopAllClocks();
        }
        private void StartAllClocks()
        {
            foreach (Clock c in clocks)
            {
                c.Stop();
                c.Start();
            }
            foreach (Sequence c in sequences)
            {
                c.Stop();
                c.Start();
            }
        }
        private void StopAllClocks()
        {
            foreach (Clock c in clocks)
                c.Stop();
            foreach (Sequence c in sequences)
            {
                c.Stop();
            }
        }


        //MENU
        private void fileMenu_Click(object sender, RoutedEventArgs e)
        {
            switchMenu("FILMNU");
        }
        private void circuitMenu_Click(object sender, RoutedEventArgs e)
        {
            switchMenu("CKTMNU");
        }
        private void switchMenu(string p)
        {
            if (p.Equals("CKTMNU"))
            {
                circuitGrid.Visibility = System.Windows.Visibility.Visible;
                fileGrid.Visibility = System.Windows.Visibility.Collapsed;
                editGrid.Visibility = System.Windows.Visibility.Collapsed;
                circuitMenu.Background = menuSel;
                fileMenu.Background = null;
                editMenu.Background = null;
            }
            else if (p.Equals("FILMNU"))
            {
                circuitGrid.Visibility = System.Windows.Visibility.Collapsed;
                fileGrid.Visibility = System.Windows.Visibility.Visible;
                editGrid.Visibility = System.Windows.Visibility.Collapsed;
                circuitMenu.Background = null;
                fileMenu.Background = menuSel;
                editMenu.Background = null;
            }
            else if (p.Equals("EDTMNU"))
            {
                circuitGrid.Visibility = System.Windows.Visibility.Collapsed;
                fileGrid.Visibility = System.Windows.Visibility.Collapsed;
                editGrid.Visibility = System.Windows.Visibility.Visible;
                switchEditMenu();
                circuitMenu.Background = null;
                fileMenu.Background = null;
                editMenu.Background = menuSel;
            }
        }
        private void switchEditMenu()
        {
            if(selectedControl.Tag.Equals("TEXT"))
            {
                seqEdit.Visibility = System.Windows.Visibility.Collapsed;
                textBoxEdit.Visibility = System.Windows.Visibility.Visible;
            }
            else if(selectedControl.Tag.Equals("SEQ"))
            {
                seqEdit.Visibility = System.Windows.Visibility.Visible;
                textBoxEdit.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        private void fontChoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedControl != null)
            {
                TextBox t = (TextBox)selectedControl;
                FontFamily f = (FontFamily)fontChoose.SelectedValue;
                t.FontFamily = f;
            }
        }
        private void fontSizeChoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedControl != null)
            {
                TextBox t = (TextBox)selectedControl;
                t.FontSize = (Int32)fontSizeChoose.SelectedValue;
            }
        }
        private void fontBold_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedControl != null)
            {
                TextBox t = (TextBox)selectedControl;
                t.FontWeight = FontWeights.Bold;
            }
        }
        private void fontBold_Unchecked(object sender, RoutedEventArgs e)
        {
            if (selectedControl != null)
            {
                TextBox t = (TextBox)selectedControl;
                t.FontWeight = FontWeights.Normal;
            }
        }
        private void fontItalic_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedControl != null)
            {
                TextBox t = (TextBox)selectedControl;
                t.FontStyle = FontStyles.Italic;
            }
        }
        private void fontItalic_Unchecked(object sender, RoutedEventArgs e)
        {
            if (selectedControl != null)
            {
                TextBox t = (TextBox)selectedControl;
                t.FontStyle = FontStyles.Normal;
            }
        }
        private void heightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double v = (int)heightSlider.Value;
            if (selectedControl != null)
            {
                TextBox t = (TextBox)selectedControl;
                t.Height = v;
                heightLabel.Text = "Height : " + v;
            }
        }
        private void widthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double v = (int)widthSlider.Value;
            if (selectedControl != null)
            {
                TextBox t = (TextBox)selectedControl;
                t.Width = v;
                widthLabel.Text = "Width : " + v;
            }
        }
        private void borderCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedControl != null)
            {
                selectedControl.BorderThickness = new Thickness(1);
                borderCheck.Content = "";
            }
        }
        private void borderCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            if (selectedControl != null)
            {
                selectedControl.BorderThickness = new Thickness(0);
                borderCheck.Content = "";
            }
        }
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            switchFileChooser(true);
        }
        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            switchFileChooser(false);
        }
        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            clocks = new List<Clock>();
            FileChooserGrid.Visibility = System.Windows.Visibility.Collapsed;
            switchMenu("CKTMNU");
            circuitGrid.Visibility = System.Windows.Visibility.Visible;
        }
        private void switchFileChooser(bool save)
        {
            this.save = save;
            FileChooserGrid.Visibility = System.Windows.Visibility.Visible;
            if(save)
            {
                okCommand.Content = "";
                fileChooserType.Text = "Save As";
            }
            else
            {
                okCommand.Content = "";
                fileChooserType.Text = "Open";
            }
        }
        private void cancelSave_Click(object sender, RoutedEventArgs e)
        {
            FileChooserGrid.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void folderList_Initialized(object sender, EventArgs e)
        {
            folderList.Children.Clear();
            for (int i = 'A'; i <= 'Z'; i++)
            {
                DirectoryInfo de = new DirectoryInfo((char)i + ":\\");
                if (de.Exists)
                {
                    Button b = new Button() { Content = " " + de.Name, Tag = de.FullName };
                    b.BorderBrush = null;
                    b.Padding = new Thickness(24, 0, 0, 0);
                    b.FontFamily = new FontFamily("Segoe UI Symbol");
                    b.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                    b.BorderThickness = new Thickness(0);
                    b.Height = 24;
                    b.Background = new SolidColorBrush(new Color() { A = 255, R = 245, G = 245, B = 245 });
                    b.Foreground = new SolidColorBrush(Colors.Black);
                    b.Click += b_Click;
                    folderList.Children.Add(b);
                }
            }
            folderPath.Text = " Drives";
        }
        void b_Click(object sender, RoutedEventArgs e)
        {
            Button bi = (Button)sender;
            DirectoryInfo d = new DirectoryInfo((string)bi.Tag);
            DirectoryInfo[] arr = d.GetDirectories();
            FileInfo[] files = d.GetFiles("*.sdf");
            folderList.Children.Clear();
            for (int i = 0; i < arr.Length; i++)
            {
                DirectoryInfo de = arr[i];
                Button b = new Button() { Content = " " + de.Name, Tag = de.FullName };
                b.BorderBrush = null;
                b.Padding = new Thickness(24, 0, 0, 0);
                b.FontFamily = new FontFamily("Segoe UI Symbol");
                b.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                b.BorderThickness = new Thickness(0);
                b.Height = 24;
                b.Background = new SolidColorBrush(new Color() { A = 255, R = 245, G = 245, B = 245 });
                b.Foreground = new SolidColorBrush(Colors.Black);
                b.Click += b_Click;
                folderList.Children.Add(b);
            }
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo de = files[i];
                Button f = new Button() { Content = " " + de.Name, Tag = de.FullName };
                f.BorderBrush = null;
                f.Padding = new Thickness(24, 0, 0, 0);
                f.FontFamily = new FontFamily("Segoe UI Symbol");
                f.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                f.BorderThickness = new Thickness(0);
                f.Height = 24;
                f.Background = new SolidColorBrush(new Color() { A = 255, R = 245, G = 245, B = 245 });
                f.Foreground = new SolidColorBrush(Colors.Black);
                f.Click += f_Click;
                f.GotFocus += f_GotFocus;
                f.LostFocus += f_LostFocus;
                folderList.Children.Add(f);
            }
            folderPath.Text = " Drives  " + (string)bi.Tag;
        }
        void f_LostFocus(object sender, RoutedEventArgs e)
        {
            Button f = (Button)sender;
            f.Background = new SolidColorBrush(new Color() { A = 255, R = 245, G = 245, B = 245 });
        }
        void f_GotFocus(object sender, RoutedEventArgs e)
        {
            Button f = (Button)sender;
            f.Background = new SolidColorBrush(new Color() { A = 255, R = 240, G = 240, B = 240 });
        }
        void f_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string f = (string)b.Content;
            fileName.Text = f.Substring(2);
        }
        private void backSave_Click(object sender, RoutedEventArgs e)
        {
            ShowStatus(folderPath.Text);
            if (!folderPath.Text.Equals(" Drives"))
            {
                string s = folderPath.Text.Substring(folderPath.Text.IndexOf(" Drives  ") + 11);
                if (s.Length == 2)
                {
                    folderList.Children.Clear();
                    for (int i = 'A'; i <= 'Z'; i++)
                    {
                        DirectoryInfo de = new DirectoryInfo((char)i + ":\\");
                        if (de.Exists)
                        {
                            Button b = new Button() { Content = " " + de.Name, Tag = de.FullName };
                            b.BorderBrush = null;
                            b.Padding = new Thickness(24, 0, 0, 0);
                            b.FontFamily = new FontFamily("Segoe UI Symbol");
                            b.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                            b.BorderThickness = new Thickness(0);
                            b.Height = 24;
                            b.Background = new SolidColorBrush(new Color() { A = 255, R = 245, G = 245, B = 245 });
                            b.Foreground = new SolidColorBrush(Colors.Black);
                            b.Click += b_Click;
                            folderList.Children.Add(b);
                        }
                    }
                    folderPath.Text = " Drives";
                } 
                else if (s.Contains("\\"))
                {
                    s = s.Substring(0, s.LastIndexOf("\\"));
                    DirectoryInfo d = new DirectoryInfo(s);
                    DirectoryInfo[] arr = d.GetDirectories();
                    FileInfo[] files = d.GetFiles("*.sdf");
                    folderList.Children.Clear();
                    for (int i = 0; i < arr.Length; i++)
                    {
                        DirectoryInfo de = arr[i];
                        Button b = new Button() { Content = " " + de.Name, Tag = de.FullName };
                        b.BorderBrush = null;
                        b.Padding = new Thickness(24, 0, 0, 0);
                        b.FontFamily = new FontFamily("Segoe UI Symbol");
                        b.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        b.BorderThickness = new Thickness(0);
                        b.Height = 24;
                        b.Background = new SolidColorBrush(new Color() { A = 255, R = 245, G = 245, B = 245 });
                        b.Foreground = new SolidColorBrush(Colors.Black);
                        b.Click += b_Click;
                        folderList.Children.Add(b);
                    }
                    for (int i = 0; i < files.Length; i++)
                    {
                        FileInfo de = files[i];
                        Button f = new Button() { Content = " " + de.Name, Tag = de.FullName };
                        f.BorderBrush = null;
                        f.Padding = new Thickness(24, 0, 0, 0);
                        f.FontFamily = new FontFamily("Segoe UI Symbol");
                        f.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        f.BorderThickness = new Thickness(0);
                        f.Height = 24;
                        f.Background = new SolidColorBrush(new Color() { A = 255, R = 245, G = 245, B = 245 });
                        f.Foreground = new SolidColorBrush(Colors.Black);
                        f.Click += f_Click;
                        f.GotFocus += f_GotFocus;
                        f.LostFocus += f_LostFocus;
                        folderList.Children.Add(f);
                    }
                    folderPath.Text = " Drives  " + s;
                }
            }
        }
        private void saveSerkitFIle_Click(object sender, RoutedEventArgs e)
        {
            if (save)
            {
                string s = folderPath.Text.Substring(folderPath.Text.IndexOf(" Drives  ") + 11);
                TextWriter writer = File.CreateText(s + "\\" + fileName.Text);
                writer.WriteLineAsync(GetCode());
                writer.Close();
                FileChooserGrid.Visibility = System.Windows.Visibility.Collapsed;
                ShowStatus("Saved to " + s + "\\" + fileName.Text);
            }
            else
            {
                FileChooserGrid.Visibility = System.Windows.Visibility.Collapsed;
                ShowStatus("Files cannot be Opened right now!");
            }
        }
        public string GetCode()
        {
            logicTextBuffer = logicTextBuffer.Trim();
            string s = "/**\n * @author=starhash Inc.\n */";
            s += "Serkit " + circuitName + " {\n";
            foreach (object child in canvas.Children)
            {
                string[] text = translateObject(child);
                s += "\n"+text[0];
                logicTextBuffer += "\n" + text[1];
            }
            s += "\nLogic {\n" + logicTextBuffer + "\n}\n";
            s += "\n}";
            return s;
        }
        private string[] translateObject(object child)
        {
            string[] text = { "", "" };
            if(child.GetType() == typeof(AndGateControl))
            {
                AndGateControl gate = (AndGateControl)child;
                text[0] += "And " + gate.Name + " {";
                text[0] += "\n\tX=" + Canvas.GetLeft(gate) + ", Y=" + Canvas.GetTop(gate);
                text[0] += ",\n\tA:Name=" + gate.A.Name + ", B:Name=" + gate.B.Name;
                text[0] += ",\n\tF:Name=" + gate.F.Name;
                text[0] += ",\n\tA:Connection=" + ((gate.A.Connection == null) ? "null" : gate.A.Connection.Name);
                text[0] += ",\n\tB:Connection=" + ((gate.B.Connection == null) ? "null" : gate.B.Connection.Name);
                text[0] += ",\n\tF:Connection=" + ((gate.F.Connection == null) ? "null" : gate.F.Connection.Name);
                text[0] += "\n};";
                return text;
            }
            else if (child.GetType() == typeof(OrGateControl))
            {
                OrGateControl gate = (OrGateControl)child;
                text[0] += "Or " + gate.Name + " {";
                text[0] += "\n\tX=" + Canvas.GetLeft(gate) + ", Y=" + Canvas.GetTop(gate);
                text[0] += ",\n\tA:Name=" + gate.A.Name + ", B:Name=" + gate.B.Name;
                text[0] += ",\n\tF:Name=" + gate.F.Name;
                text[0] += ",\n\tA:Connection=" + ((gate.A.Connection == null) ? "null" : gate.A.Connection.Name);
                text[0] += ",\n\tB:Connection=" + ((gate.B.Connection == null) ? "null" : gate.B.Connection.Name);
                text[0] += ",\n\tF:Connection=" + ((gate.F.Connection == null) ? "null" : gate.F.Connection.Name);
                text[0] += "\n};";
                return text;
            }
            else if (child.GetType() == typeof(NotGateControl))
            {
                NotGateControl gate = (NotGateControl)child;
                text[0] += "Not " + gate.Name + " {";
                text[0] += "\n\tX=" + Canvas.GetLeft(gate) + ", Y=" + Canvas.GetTop(gate);
                text[0] += ",\n\tA:Name=" + gate.A.Name + ", F:Name=" + gate.F.Name;
                text[0] += ",\n\tA:Connection=" + ((gate.A.Connection == null) ? "null" : gate.A.Connection.Name);
                text[0] += ",\n\tF:Connection=" + ((gate.F.Connection == null) ? "null" : gate.F.Connection.Name);
                text[0] += "\n};";
                return text;
            }
            else if (child.GetType() == typeof(NandGateControl))
            {
                NandGateControl gate = (NandGateControl)child;
                text[0] += "Nand " + gate.Name + " {";
                text[0] += "\n\tX=" + Canvas.GetLeft(gate) + ", Y=" + Canvas.GetTop(gate);
                text[0] += ",\n\tA:Name=" + gate.A.Name + ", B:Name=" + gate.B.Name;
                text[0] += ",\n\tF:Name=" + gate.F.Name;
                text[0] += ",\n\tA:Connection=" + ((gate.A.Connection == null) ? "null" : gate.A.Connection.Name);
                text[0] += ",\n\tB:Connection=" + ((gate.B.Connection == null) ? "null" : gate.B.Connection.Name);
                text[0] += ",\n\tF:Connection=" + ((gate.F.Connection == null) ? "null" : gate.F.Connection.Name);
                text[0] += "\n};";
                return text;
            }
            else if (child.GetType() == typeof(NorGateControl))
            {
                NorGateControl gate = (NorGateControl)child;
                text[0] += "And " + gate.Name + " {";
                text[0] += "\n\tX=" + Canvas.GetLeft(gate) + ", Y=" + Canvas.GetTop(gate);
                text[0] += ",\n\tA:Name=" + gate.A.Name + ", B:Name=" + gate.B.Name;
                text[0] += ",\n\tF:Name=" + gate.F.Name;
                text[0] += ",\n\tA:Connection=" + ((gate.A.Connection == null) ? "null" : gate.A.Connection.Name);
                text[0] += ",\n\tB:Connection=" + ((gate.B.Connection == null) ? "null" : gate.B.Connection.Name);
                text[0] += ",\n\tF:Connection=" + ((gate.F.Connection == null) ? "null" : gate.F.Connection.Name);
                text[0] += "\n};";
                return text;
            }
            else if (child.GetType() == typeof(XorGateControl))
            {
                XorGateControl gate = (XorGateControl)child;
                text[0] += "Xor " + gate.Name + " {";
                text[0] += "\n\tX=" + Canvas.GetLeft(gate) + ", Y=" + Canvas.GetTop(gate);
                text[0] += ",\n\tA:Name=" + gate.A.Name + ", B:Name=" + gate.B.Name;
                text[0] += ",\n\tF:Name=" + gate.F.Name;
                text[0] += ",\n\tA:Connection=" + ((gate.A.Connection == null) ? "null" : gate.A.Connection.Name);
                text[0] += ",\n\tB:Connection=" + ((gate.B.Connection == null) ? "null" : gate.B.Connection.Name);
                text[0] += ",\n\tF:Connection=" + ((gate.F.Connection == null) ? "null" : gate.F.Connection.Name);
                text[0] += "\n};";
                return text;
            }
            else if (child.GetType() == typeof(XnorGateControl))
            {
                XnorGateControl gate = (XnorGateControl)child;
                text[0] += "Xnor " + gate.Name + " {";
                text[0] += "\n\tX=" + Canvas.GetLeft(gate) + ", Y=" + Canvas.GetTop(gate);
                text[0] += ",\n\tA:Name=" + gate.A.Name + ", B:Name=" + gate.B.Name;
                text[0] += ",\n\tF:Name=" + gate.F.Name;
                text[0] += ",\n\tA:Connection=" + ((gate.A.Connection == null) ? "null" : gate.A.Connection.Name);
                text[0] += ",\n\tB:Connection=" + ((gate.B.Connection == null) ? "null" : gate.B.Connection.Name);
                text[0] += ",\n\tF:Connection=" + ((gate.F.Connection == null) ? "null" : gate.F.Connection.Name);
                text[0] += "\n};";
                return text;
            }
            else if (child.GetType() == typeof(PinControl))
            {
                PinControl pin = (PinControl)child;
                text[0] += "Pin " + pin.Name + " {";
                text[0] += "\n\tX=" + Canvas.GetLeft(pin) + ", Y=" + Canvas.GetTop(pin); 
                text[0] += ",\n\tConnection=" + ((pin.Connection == null) ? "null" : pin.Connection.Name);
                text[0] += ",\n\tState=" + pin.State;
                text[0] += "\n};";
                return text;
            }
            else if (child.GetType() == typeof(Clock))
            {
                Clock clock = (Clock)child;
                text[0] += "Clock " + clock.Name + " {";
                text[0] += "\n\tX=" + Canvas.GetLeft(clock) + ", Y=" + Canvas.GetTop(clock);
                text[0] += ",\n\tConnection=" + ((clock.clockPin.Connection == null) ? "null" : clock.clockPin.Connection.Name);
                text[0] += ",\n\tState=" + clock.clockPin.State;
                text[0] += "\n};";
                return text;
            }
            else if (child.GetType() == typeof(LED))
            {
                LED led = (LED)child;
                text[0] += "Led " + led.Name + " {";
                text[0] += "\n\tX=" + Canvas.GetLeft(led) + ", Y=" + Canvas.GetTop(led);
                text[0] += ",\n\tConnection=" + ((led.ledPin.Connection == null) ? "null" : led.ledPin.Connection.Name);
                text[0] += ",\n\tState=" + led.ledPin.State;
                text[0] += "\n};";
                return text;
            }
            else if (child.GetType() == typeof(TextBox))
            {
                TextBox textbox = (TextBox)child;
                text[0] += "Text " + textbox.Name + " {";
                text[0] += "\n\tX=" + Canvas.GetLeft(textbox) + ", Y=" + Canvas.GetTop(textbox);
                text[0] += "\n};";
                return text;
            }
            return text;
        }

        //SIDESWITCH
        private void circuitView_Click(object sender, RoutedEventArgs e)
        {
            editorScroll.Visibility = System.Windows.Visibility.Collapsed;
            canvasScroll.Visibility = System.Windows.Visibility.Visible;
        }
        private void codeView_Click(object sender, RoutedEventArgs e)
        {
            editorScroll.Visibility = System.Windows.Visibility.Visible;
            canvasScroll.Visibility = System.Windows.Visibility.Collapsed;
            codeEditor.Text = GetCode();
        }
        private void zoomAmount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (zoomAmount != null && canvasScaleTransform!=null)
            {
                canvasScaleTransform.ScaleX = (double)zoomAmount.Value;
                canvasScaleTransform.ScaleY = (double)zoomAmount.Value;
            }
        }
        private void resetZoom_Click(object sender, RoutedEventArgs e)
        {
            zoomAmount.Value = 1.0;
            canvasScaleTransform.ScaleX = (double)zoomAmount.Value;
            canvasScaleTransform.ScaleY = (double)zoomAmount.Value;
        }

        

        

        

        



    }
}
