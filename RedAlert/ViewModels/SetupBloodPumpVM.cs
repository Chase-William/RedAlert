using RedAlert.Lang;
using RedAlertBot.Models;
using RedAlertBot.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Input;

namespace RedAlert.ViewModels
{
    public class SetupBloodPumpVM : Notifier
    {
        public const string RESPAWN_SEARCH_BAR_LOC = "rsbl";
        public const string BED_LOC = "bl";
        public const string RESPAWN_BTN_LOC = "rbl";
        public const string INVENT_FIRST_SLOT_LOC = "ifsl";
        public const string INVENT_SEARCH_BAR_LOC = "isbl";
        public const string OTHER_INVENT_FIRST_SLOT_LOC = "oifsl";
        public const string INVENT_TRANSFER_ALL_BTN_LOC = "itabl";
        public const string DEATH_BED_LOCATION = "dbl";

        private bool isEnabled;
        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (IsEnabled == value) return;
                isEnabled = value;                
                NotifyPropertyChanged();                

                // Toggle the bloodpumper
                if (IsEnabled) BloodPumper.Begin();
                else BloodPumper.Stop();
            }
        }

        public BloodPumper BloodPumper { get; set; }

        public ICommand SetLocationCommand => new Command((code) =>
        {
            switch ((string)code)
            {
                case RESPAWN_SEARCH_BAR_LOC:
                    WindowsUtil.MouseCoordsPolled = BloodPumper.SetRespawnSearchBar;                   
                    break;

                case BED_LOC:            
                    WindowsUtil.MouseCoordsPolled = BloodPumper.SetBedLocation;
                    break;

                case RESPAWN_BTN_LOC:
                    WindowsUtil.MouseCoordsPolled = BloodPumper.SetRespawnBtnLocation;
                    break;

                case INVENT_SEARCH_BAR_LOC:
                    WindowsUtil.MouseCoordsPolled = BloodPumper.SetInventSearchbarLocation;
                    break;

                case INVENT_FIRST_SLOT_LOC:
                    WindowsUtil.MouseCoordsPolled = BloodPumper.SetInventFirstSlotLocation;
                    break;

                case OTHER_INVENT_FIRST_SLOT_LOC:
                    WindowsUtil.MouseCoordsPolled = BloodPumper.SetOtherInventFirstSlotLocation;
                    break;

                case INVENT_TRANSFER_ALL_BTN_LOC:
                    WindowsUtil.MouseCoordsPolled = BloodPumper.SetTransferAllBtnLocatin;
                    break;

                case DEATH_BED_LOCATION:
                    WindowsUtil.MouseCoordsPolled = BloodPumper.SetDeathBedLocation;
                    break;
                default:
                    break;
            }
        });

        public ICommand ToggleCommand => new Command(() => IsEnabled = !IsEnabled);

        public ICommand SaveCommand => new Command(BloodPumper.Save);

        public SetupBloodPumpVM()
        {
            BloodPumper = BloodPumper.Load();
        }       
    }
}
