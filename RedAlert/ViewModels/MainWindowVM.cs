using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using RedAlert.Lang;
using RedAlertUI.Lang;
using RedAlertUI.WindowsUtil;
using HWND = System.IntPtr;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace RedAlertUI.ViewModels
{
    public class MainWindowVM : Notifier
    {
        public ImageRecorder Recorder { get; set; }

        private string activeWindowTxt;
        /// <summary>
        /// Text to be displayed before/during/after setting the active window to record from.
        /// </summary>
        public string ActiveWindowTxt
        {
            get => activeWindowTxt;
            set
            {
                if (ActiveWindowTxt == value) return;

                activeWindowTxt = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand SetRecordingWindowCommand { get; set; }     

        public MainWindowVM()
        {
            Recorder = new ImageRecorder();            
            SetRecordingWindowCommand = new Command(Recorder.ScanForArkWindow);
        }                
    }    
}