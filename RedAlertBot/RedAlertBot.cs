using RedAlertBot.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedAlertBot
{
    public class RedAlertBot : Notifier
    {
        #region Instance Getter
        private static readonly RedAlertBot bot = new RedAlertBot();
        public static RedAlertBot Bot => bot;
        #endregion

        #region Properties
        private bool isBotEnabled;
        /// <summary>
        /// Toggles the bot on and off.
        /// </summary>
        public bool IsBotEnabled
        {
            get => isBotEnabled;
            set
            {
                if (IsBotEnabled == value) return;
                isBotEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public ImageRecorder Recorder { get; set; } = new ImageRecorder();
        #endregion

        private RedAlertBot()
        {
#if DEBUG
            WindowsUtil.InitConsole();
#endif
        }

        ~RedAlertBot()
        {
#if DEBUG
            WindowsUtil.CloseConsole();
#endif
        }
    }
}
