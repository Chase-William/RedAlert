using RedAlertBot.Lang;
using RedAlertBot.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;
using HWND = System.IntPtr;

namespace RedAlertBot
{
    /// <summary>
    /// Manages the recording of the target window's screen.
    /// </summary>
    public class ImageRecorder : Notifier
    {
        #region Constants
        private const string ARK_WINDOW_TITLE = "ARK: Survival Evolved";
        public const int DEFAULT_TIMER_INTERVAL = 2000;
        #endregion        

        #region Events
        public event EventHandler<ImageRecordingEventArgs> RecordingStateChanged;                        // Recording of target window has changed
        public event EventHandler<TargetWindowDiscoveredChangedArgs> TargetWindowDiscoveredChanged; // Target window value has changed
        #endregion

        #region Fields
        private RedAlertBot bot;
        #endregion

        #region Properties
        public Timer RecordingTimer = new Timer(DEFAULT_TIMER_INTERVAL);

        public ImagePredictor Predictor { get; set; } = new ImagePredictor();

        private HWND targetWindowHWND;
        /// <summary>
        /// A HWND to the Active Window.
        /// </summary>
        public HWND TargetWindowHWND
        {
            get => targetWindowHWND;
            set
            {
                if (targetWindowHWND == value) return;
                targetWindowHWND = value;
                TargetWindowDiscoveredChanged?.Invoke(this, new TargetWindowDiscoveredChangedArgs(TargetWindowHWND));

                // If the bot is disabled the recording cannot start
                if (bot.IsBotEnabled)
                    if (targetWindowHWND == HWND.Zero)
                        IsRecording = false;                    
                    else
                        IsRecording = true;                    
            }
        }

        private bool isRecording;
        public bool IsRecording
        {
            get => isRecording;
            private set
            {
                if (IsRecording == value) return;
                isRecording = value;
                RecordingStateChanged?.Invoke(this, new ImageRecordingEventArgs(isRecording));
                NotifyPropertyChanged();
            }
        }

        public NotifyingRect RecordingAreaOffset { get; set; }
        public NotifyingRect RecordingAreaOrigin { get; private set; }
        #endregion

        public ImageRecorder(RedAlertBot _bot)
        {
            bot = _bot;
            // If the bot has been disabled, notify the IsRecording state variable
            bot.BotIsEnabledChanged += (sender, args) =>
            {
                // If the bot is disabled, turn off the recording
                if (args.IsEnabled)
                    IsRecording = true;
                else
                    IsRecording = false;
            };
            RecordingTimer.Elapsed += RecordingTimer_Elapsed;
            // Toggle the recording when the recording state changes
            RecordingStateChanged += (sender, args) =>
            {
                if (args.IsRecording)
                    RecordingTimer.Start();
                else                
                    RecordingTimer.Stop();                
            };
            // Update the recording area origin rect when the target window is discovered or lost
            TargetWindowDiscoveredChanged += delegate
            {
                _ = WindowsUtil.GetWindowRect(TargetWindowHWND, out RECT sRect);
                RecordingAreaOrigin = new NotifyingRect(sRect);
            };
        }

        private void RecordingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SaveBitmap(CaptureWindowBitmap());

#if DEBUG
            Predictor.PredictImage("../../ss.jpeg");            
#else
            throw new NotImplementedException();
#endif

        }

        private Bitmap CaptureWindowBitmap()
        {
            _ = WindowsUtil.GetWindowRect(TargetWindowHWND, out RECT sRect);
            RecordingAreaOrigin = new NotifyingRect(sRect);

            Bitmap bmp = new Bitmap(sRect.right - sRect.left, sRect.bottom - sRect.top, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            HWND hdcBitmap = gfxBmp.GetHdc();

            WindowsUtil.PrintWindow(TargetWindowHWND, hdcBitmap, 0);
            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();
            return bmp;
        }

        private void SaveBitmap(Bitmap bitmap)
        {
            bitmap.Save("ss.jpeg");
        }
        

        /// <summary>
        /// Tries to retrieve the HWND for the Ark Survival Evolved window.
        /// </summary>
        public void ScanForArkWindow() => TargetWindowHWND = WindowsUtil.FindWindowA(null, ARK_WINDOW_TITLE);
    }

    public class ImageRecordingEventArgs : EventArgs
    {
        public bool IsRecording { get; set; }

        public ImageRecordingEventArgs(bool _isRecording) => IsRecording = _isRecording;
    }

    public class TargetWindowDiscoveredChangedArgs : EventArgs
    {
        public HWND WindowHWND { get; set; }

        public TargetWindowDiscoveredChangedArgs(HWND _windowHWND) => WindowHWND = _windowHWND;
    }
}
