using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Logic.Circuit.Controls;

namespace Serkit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static PinControl SelectedPin;
        public static PinControl ActiveMouseOverPin;
        public static bool MouseOverPin;
        public static bool PinSelected;

    }
}
