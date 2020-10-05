using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using MahApps.Metro.Controls;
using RedAlert.ViewModels;
namespace RedAlert
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        MainWindowVM VM => (MainWindowVM)DataContext;

        public MainWindow()
        {
            InitializeComponent();

            //VM.Recorder.RecordingStarted += delegate
            //{
                
            //};

            //VM.Recorder.RecordingStopped += delegate
            //{

            //};

            // Try to get the ark window apon launch
            VM.Bot.Recorder.ScanForArkWindow();            
        }
    }
}
