using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using RedAlertUI.Lang;
using RedAlertUI.WindowsUtil;
using HWND = System.IntPtr;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace RedAlertUI.ViewModels
{
    public class MainWindowVM : Notifier
    {
        #region Constants
        private const string ARK_WINDOW_TITLE = "ARK: Survival Evolved";
        private const string ARK_WINDOW_NOT_FOUND = "Ark Window Not Found";
        #endregion

        //#region Events
        ///// <summary>
        ///// Notifies subscribers that we are ready to begin capturing bitmaps of the target window.
        ///// </summary>
        //public event EventHandler ReadyToCapture;
        //#endregion

        #region Properties
        private HWND activeWindowHWND;
        /// <summary>
        /// A HWND to the Active Window.
        /// </summary>
        public HWND TargetWindowHWND
        {
            get => activeWindowHWND;
            set
            {
                activeWindowHWND = value;
                if (TargetWindowHWND == HWND.Zero)
                    ActiveWindowTxt = ARK_WINDOW_NOT_FOUND;
                else
                {
                    ActiveWindowTxt = ARK_WINDOW_TITLE;
                    SaveBitmap(CaptureWindowBitmap());
                }
            }
        }

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

        public ICommand SetRecordingWindowCommand => new Command(ScanForArkWindow);
        #endregion

        public MainWindowVM()
        {

        }

        /// <summary>
        /// Tries to retrieve the HWND for the Ark Survival Evolved window.
        /// </summary>
        public void ScanForArkWindow() => TargetWindowHWND = User32.FindWindowA(null, ARK_WINDOW_TITLE);

        private Bitmap CaptureWindowBitmap()
        {
            _ = User32.GetWindowRect(TargetWindowHWND, out RECT sRect);
           
            Bitmap bmp = new Bitmap(sRect.right - sRect.left, sRect.bottom - sRect.top, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            HWND hdcBitmap = gfxBmp.GetHdc();

            User32.PrintWindow(TargetWindowHWND, hdcBitmap, 0);
            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();
            return bmp;
        }

        private void SaveBitmap(Bitmap bitmap)
        {
            bitmap.Save("ark_test.jpeg");           
        }
    }    
}