using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using RedAlert.ViewModels;
using RedAlertBot;

namespace RedAlert
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowVM VM => (MainWindowVM)DataContext;

        //public RedAlertBot.Util.WindowsUtil WindowsUtil { get; private set; }                

        public MainWindow()
        {
            InitializeComponent();            
            Content = new SetupBloodPumperPage();

            //VM.Recorder.RecordingStarted += delegate
            //{
                
            //};

            //VM.Recorder.RecordingStopped += delegate
            //{

            //};

            // Try to get the ark window apon launch
            //VM.Bot.Recorder.ScanForArkWindow();            
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            //WindowsUtil = new RedAlertBot.Util.WindowsUtil(new WindowInteropHelper(this).Handle);

            // Init our bot
            if (!RedAlertBot.RedAlertBot.Bot.Init(new WindowInteropHelper(this).Handle))
            {
                Console.WriteLine("FAILURE: Failed to initialize.");                
            }

            App.Source = HwndSource.FromHwnd(RedAlertBot.Util.WindowsUtil.SourceWindow);
            App.Source.AddHook(RedAlertBot.Util.WindowsUtil.ToggleHooks);            
        }        
    }
}
