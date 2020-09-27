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
using RedAlertBot;
using RedAlertBot.Util;

namespace RedAlert.ViewModels
{
    public class MainWindowVM : Notifier
    {
        public readonly string[] MODES = { "Tribe Logging" };

        #region Properties
        public RedAlertBot.RedAlertBot Bot { get; set; } = RedAlertBot.RedAlertBot.Bot;
        

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
        public ICommand ToggleBotCommand { get; set; }
        #endregion

        public MainWindowVM()
        {            
            ActiveWindowTxt = "NULL";            
            SetRecordingWindowCommand = new Command(Bot.Recorder.ScanForArkWindow);
            Bot.Recorder.TargetWindowDiscoveredChanged += (sender, args) =>
            {
                ActiveWindowTxt = args.WindowHWND.ToString();
            };
        }                
    }    
}