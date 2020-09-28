using NumSharp.Extensions;
using RedAlertBot.Lang;
using RedAlertBot.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
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

        private float TRIBELOG_BOX_X_PERCENTAGE = 0.693f;
        private float TRIBELOG_BOX_Y_PERCENTAGE = 0.2f;
        #endregion

        #region Events
        public event EventHandler<ImageRecordingEventArgs> RecordingStateChanged;                   // Recording of target window has changed
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

                if (value == true && TargetWindowHWND != HWND.Zero)
                {
                    isRecording = value;
                    RecordingTimer.Start();
                    NotifyPropertyChanged();
                }                
                else
                {
                    isRecording = false;
                    RecordingTimer.Stop();
                    NotifyPropertyChanged();
                }
                RecordingStateChanged?.Invoke(this, new ImageRecordingEventArgs(IsRecording));
            }
        }

        public Rectangle TargetWindowRect { get; set; }

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
            TargetWindowDiscoveredChanged += delegate
            {
                _ = WindowsUtil.GetWindowRect(TargetWindowHWND, out RECT sRect);
                TargetWindowRect = new Rectangle(sRect.left, sRect.top, sRect.right - sRect.left, sRect.bottom - sRect.top);
            };
        }

        private void RecordingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CaptureWindowBitmap();
            //SaveBitmap(CaptureWindowBitmap());

#if DEBUG
            //Predictor.PredictImage("../../ss.jpeg");            
#else
            throw new NotImplementedException();
#endif

        }

        //private Bitmap CaptureWindowBitmap()
        //{
        //    ActiveImg = null;
        //    Bitmap bmp;

        //    _ = WindowsUtil.GetWindowRect(TargetWindowHWND, out RECT sRect);

        //    int width = (int)((sRect.right - sRect.left) * WidthPercentage);
        //    int height = (int)((sRect.bottom - sRect.top) * HeightPercentage);

        //    bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        //    Graphics gfxBmp = Graphics.FromImage(bmp);
        //    gfxBmp.CopyFromScreen(sRect.left + 400, sRect.top + 400, 0, 0, new Size(width, height));
        //    //HWND hdcBitmap = gfxBmp.GetHdc();

        //    //WindowsUtil.PrintWindow(TargetWindowHWND, hdcBitmap, 0);
        //    //gfxBmp.ReleaseHdc(hdcBitmap);
        //    //gfxBmp.Dispose();

        //    try
        //    {
        //        // Comment this line below to have the program work.
        //        bmp = bmp.Clone(new Rectangle(100, 100, 200, 200), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    finally
        //    {
        //        bmp.Save("test.jpeg");
        //        bmp.Dispose();
        //        ActiveImg = new Uri(@"C:\Dev\RedAlert\RedAlert\bin\Debug\netcoreapp3.1\test.jpeg");
        //        NotifyPropertyChanged(nameof(ActiveImg));
        //    }
        //    return bmp;
        //}
        
        private void CaptureWindowBitmap()
        {
            Bitmap bmp;
            Bitmap copyBmp;
            _ = WindowsUtil.GetWindowRect(TargetWindowHWND, out RECT sRect);

            int width = sRect.right - sRect.left;
            int height = sRect.bottom - sRect.top;

            bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Graphics gfxBmp = Graphics.FromImage(bmp);
            HWND hdcBitmap = gfxBmp.GetHdc();

            WindowsUtil.PrintWindow(TargetWindowHWND, hdcBitmap, 0);            
            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();
            
            try
            {                
                Rectangle rect = new Rectangle(0, 0, 146, 269);
                // Comment this line below to have the program work.
                copyBmp = bmp.Clone(rect, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                                
                copyBmp.Save("copyBmp.jpeg");
                copyBmp.Dispose();
            }
            catch (Exception ex)
            {                
            }
            finally
            {
                bmp.Save("test.jpeg");
                bmp.Dispose();
            }           
        }

        private void SaveBitmap(Bitmap bitmap)
        {
            bitmap.Save("ss.jpeg");
            bitmap.Dispose();
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
