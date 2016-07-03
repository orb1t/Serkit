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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Serkit
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        public Splash()
        {
            InitializeComponent();
            Loader l = new Loader(this);
            l.Interval = new TimeSpan(1000);
            l.MaximumCount = 1000;
            l.Start();
            l.Tick += l_Tick;
        }

        void l_Tick(object sender, EventArgs e)
        {
            Loader l = (Loader)sender;
            if(l.Count >= 500)
            {
                if (progress.IsIndeterminate)
                    progress.IsIndeterminate = false;
                Console.WriteLine(l.Count);
                PaintProgress(l.Count - 500, 500);
            }
        }

        void PaintProgress(long value, long over)
        {
            progress.Maximum = over;
            progress.Minimum = 0;
            progress.Value = value;
        }
    }

    public class Loader : DispatcherTimer
    {
        private long count;
        private long max = 0;
        Splash s;
        public long Count { get { return count; } set { count = value; } }
        public long MaximumCount { get { return max; } set { max = value; } }

        public Loader(Splash s)
        {
            this.s = s;
            this.Tick += Loader_Tick;
        }

        void Loader_Tick(object sender, EventArgs e)
        {
            count++;
            Console.WriteLine(count);
            if (count == max)
            {
                MainWindow m = new MainWindow();
                m.Visibility = Visibility.Visible;
                m.Loaded += m_Loaded;
                this.Stop();
            }
        }

        void m_Loaded(object sender, RoutedEventArgs e)
        {
            s.Visibility = Visibility.Collapsed;
        }
    }
}
