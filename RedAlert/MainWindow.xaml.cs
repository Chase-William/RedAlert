using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using RedAlert.ViewModels;
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

            //VM.Recorder.RecordingStarted += delegate
            //{
                
            //};

            //VM.Recorder.RecordingStopped += delegate
            //{

            //};

            // Try to get the ark window apon launch
            VM.Bot.Recorder.ScanForArkWindow();            
        }

        private void OnSetTribeLogRect_BtnClicked(object sender, RoutedEventArgs e)
        {
            CreateSnippingWindow();
        }

        private void CreateSnippingWindow()
        {
            const int WINDOW_BORDER_WIDTH = 6;
            
            Rectangle rect = VM.Bot.Recorder.TargetWindowRect;

            Window window = new Window
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Opacity = 0.4f,
                // We need to do this offseting because when we get the window it has some thick borders.
                Left = rect.Left + WINDOW_BORDER_WIDTH,
                Top = rect.Top,
                Height = rect.Height - WINDOW_BORDER_WIDTH,
                Width = rect.Width - WINDOW_BORDER_WIDTH * 2,
                Topmost = true                
            };

            window.MouseDown += (sender, args) =>
            {
               
            };

            window.MouseUp += (sender, args) =>
            {

                window.Close();               
            };


            window.Show();
        }
    }
}
