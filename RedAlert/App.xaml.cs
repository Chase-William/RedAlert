using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace RedAlert
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {       
        // Class that is a wrapper for the win32 window 
        public static HwndSource Source;

        public App()
        {            
        }
    }
}
