using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using RedAlertUI.ViewModels;
using RedAlertUI.WindowsUtil;

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

#if DEBUG
            User32.InitConsole();
#endif

            InitializeComponent();

            //VM.Recorder.RecordingStarted += delegate
            //{
                
            //};

            //VM.Recorder.RecordingStopped += delegate
            //{

            //};

            // Try to get the ark window apon launch
            VM.Recorder.ScanForArkWindow();            
        }
    }
}
