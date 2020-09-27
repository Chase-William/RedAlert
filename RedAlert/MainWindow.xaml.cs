using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using HWND = System.IntPtr; // HWND == handle to a window C++ win32.dll
using HWINEVENTHOOK = System.IntPtr;
using HMODULE = System.IntPtr;
using WINEVENTPROC = System.IntPtr;
using Point = System.Windows.Point;
using RedAlertUI.ViewModels;

namespace RedAlert
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowVM VM => (MainWindowVM)DataContext;

        public MainWindow()
        {
            InitializeComponent();

            // Try to get the ark window apon launch
            VM.ScanForArkWindow();
        }
    }
}
