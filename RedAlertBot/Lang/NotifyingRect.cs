using RedAlertBot.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedAlertBot.Lang
{
    public class NotifyingRect : Notifier
    {
        #region Properties
        private int x;
        public int X
        {
            get => x;
            set
            {
                if (X == value) return;
                x = value;
                NotifyPropertyChanged();
            }
        }

        private int y;
        public int Y
        {
            get => y;
            set
            {
                if (Y == value) return;
                y = value;
                NotifyPropertyChanged();
            }
        }

        private int width;
        public int Width
        {
            get => width;
            set
            {
                if (Width == value) return;
                width = value;
                NotifyPropertyChanged();
            }
        }

        private int height;
        public int Height
        {
            get => height;
            set
            {
                if (Height == value) return;
                height = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public NotifyingRect() { }
        public NotifyingRect(in RECT rect)
        {
            X = rect.left;
            Y = rect.top;
            Width = rect.right - rect.left;
            Height = rect.bottom - rect.top;
        }
    }
}
