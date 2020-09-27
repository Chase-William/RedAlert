using RedAlertBot.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;
using HWND = System.IntPtr;

namespace RedAlertBot
{
    public class ImageRecorder : Notifier
    {
        #region Constants
        private const string ARK_WINDOW_TITLE = "ARK: Survival Evolved";
        private const string ARK_WINDOW_NOT_FOUND = "Ark Window Not Found";
        public const int DEFAULT_TIMER_INTERVAL = 2000;
        #endregion

        /// <summary>
        /// Learner that classifies images.
        /// </summary>
        public static RedLearner.RedLearner Learner { get; private set; } = new RedLearner.RedLearner();

        #region Events
        public event EventHandler<ImageRecordingEventArgs> RecordingStarted;                        // Recording of target window has started
        public event EventHandler<ImageRecordingEventArgs> RecordingStopped;                        // Recording of target window has stopped
        public event EventHandler<TargetWindowDiscoveredChangedArgs> TargetWindowDiscoveredChanged; // Target window value has changed
        #endregion

        #region Properties
        public Timer RecordingTimer = new Timer(DEFAULT_TIMER_INTERVAL);

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

                if (targetWindowHWND == HWND.Zero)
                {
                    IsRecording = false;
                    RecordingStopped?.Invoke(this, new ImageRecordingEventArgs(isRecording));
                }
                else
                {
                    IsRecording = true;
                    RecordingStarted?.Invoke(this, new ImageRecordingEventArgs(isRecording));
                }
            }
        }

        private bool isRecording;
        public bool IsRecording
        {
            get => isRecording;
            set
            {
                if (IsRecording == value) return;
                isRecording = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public ImageRecorder()
        {
            RecordingTimer.Elapsed += RecordingTimer_Elapsed;
            RecordingStarted += delegate { RecordingTimer.Start(); };
            RecordingStopped += delegate { RecordingTimer.Stop(); };
        }

        private void RecordingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SaveBitmap(CaptureWindowBitmap());

#if DEBUG
            Console.WriteLine(Learner.ClassifySingleImage("../../ss.jpeg"));
#else
            Learner.ClassifySingleImage("../../ss.jpeg");
#endif

        }

        private Bitmap CaptureWindowBitmap()
        {
            _ = WindowsUtil.GetWindowRect(TargetWindowHWND, out RECT sRect);

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
