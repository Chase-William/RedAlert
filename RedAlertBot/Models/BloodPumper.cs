using RedAlertBot.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Timers;
using System.Security.Cryptography.X509Certificates;
using RedLearner;

namespace RedAlertBot.Models
{
    /// <summary>
    /// 1. Search for bed by name<br/>
    /// 2. Click on bed at location to select<br/>
    /// 3. Click on respawn btn
    /// </summary>
    public class BloodPumper : Notifier
    {
        private const string FILE_NAME = "bloodpumper.json";
        private const int OPEN_INVENT_TIMER_INTERVAL = 2000;
        private const int SCAN_WHILE_TURNING_TIMER_INTERVAL = 250;
        private const float THRESHOLD = 0.90f;

        private System.Timers.Timer timer = new System.Timers.Timer(200);
        private int Counter { get; set; }

        private Point respawnSearchBarLocation;
        public Point RespawnSearchBarLocation
        {
            get => respawnSearchBarLocation;
            set
            {
                if (RespawnBtnLocation == value) return;
                respawnSearchBarLocation = value;
                NotifyPropertyChanged();
            }
        }

        private string bedName;
        public string BedName
        {
            get => bedName;
            set
            {
                if (BedName == value) return;
                bedName = value;
                NotifyPropertyChanged();
            }
        }

        private Point bedLocation;
        public Point BedLocation
        {
            get => bedLocation;
            set
            {
                if (BedLocation == value) return;
                bedLocation = value;
                NotifyPropertyChanged();
            }
        }

        private Point respawnBtnLocation;
        public Point RespawnBtnLocation
        {
            get => respawnBtnLocation;
            set
            {
                if (RespawnBtnLocation == value) return;
                respawnBtnLocation = value;
                NotifyPropertyChanged();
            }
        }

        private Point inventFirstSlotLocation;
        public Point InventFirstSlotLocation
        {
            get => inventFirstSlotLocation;
            set
            {
                if (InventFirstSlotLocation == value) return;
                inventFirstSlotLocation = value;
                NotifyPropertyChanged();
            }
        }

        private Point inventSearchbarLocation;
        public Point InventSearchbarLocation
        {
            get => inventSearchbarLocation;
            set
            {
                if (InventSearchbarLocation == value) return;
                inventSearchbarLocation = value;
                NotifyPropertyChanged();
            }
        }       

        private Point otherInventFirstSlotLocation;
        public Point OtherInventFirstSlotLocation
        {
            get => otherInventFirstSlotLocation;
            set
            {
                if (OtherInventFirstSlotLocation == value) return;
                otherInventFirstSlotLocation = value;
                NotifyPropertyChanged();
            }
        }

        private Point transferAllBtnLocation;
        public Point TransferAllBtnLocation
        {
            get => transferAllBtnLocation;
            set
            {
                if (TransferAllBtnLocation == value) return;
                transferAllBtnLocation = value;
                NotifyPropertyChanged();
            }
        }

        private Point deathBedsLocation;
        public Point DeathBedsLocation
        {
            get => deathBedsLocation;
            set
            {
                if (DeathBedsLocation == value) return;
                deathBedsLocation = value;
                NotifyPropertyChanged();
            }
        }

        private int usesBeforeDeath;
        public int UsesBeforeDeath
        {
            get => usesBeforeDeath;
            set
            {
                if (UsesBeforeDeath == value) return;
                usesBeforeDeath = value;
                NotifyPropertyChanged();
            }
        }
        

        public BloodPumper()
        {
            //timer.Elapsed += delegate
            //{
            //    WindowsUtil.PerformKeyboardClick(KeyboardVKs.E);
            //    Counter++;

            //    // Stop sucking blood
            //    if (Counter == UsesBeforeDeath)
            //    {
            //        Counter = 1;
            //        timer.Stop();
            //        Finish();
            //    }
            //};
        }        

        /// <summary>
        /// 1. Move cursor to respawn searchbar
        /// 2. Click on search bar
        /// 3. Type bed name in searchbar
        /// 4. Move mouse to the bed location on screen<br/>
        /// 5. Click on the bed<br/>
        /// 6. Move mouse to respawn button locations<br/>
        /// 7. Click Respawn button<br/>
        /// 8. Open Vault Inventory<br/>
        /// 9. Move mouse over first item in vault inventory<br/>
        /// 9. Take item once from vault<br/>
        /// 10. Move cursor to user searchbar<br/>
        /// 11. Enter syringe into searchbar<br/>
        /// 12. Move cursor over sryinge<br/>
        /// 13. Select syringe<br/>
        /// 14. Press e to use the syringe<br/>
        /// 15. Start a timer to suck blood every 5 seconds<br/>
        /// </summary>
        public async void Begin()
        {
            await Task.Run(() =>
            {
                try
                {
                    //Task.Delay(1000).Wait();
                    //WindowsUtil.PerformMouseClick(RespawnSearchBarLocation);
                    //Task.Delay(100).Wait();
                    //WindowsUtil.PerformKeyboardClicks(BedName);
                    //Task.Delay(200).Wait();
                    //WindowsUtil.PerformMouseClick(BedLocation);
                    //Task.Delay(100).Wait();
                    //WindowsUtil.PerformMouseClick(RespawnBtnLocation);

                    RespawnUsingBed(BedName, BedLocation);

                    // Open Vault UI
                    BeginOpenVaultProcess();
                    //WindowsUtil.PerformKeyboardClick(KeyboardVKs.F);
                    //Task.Delay(2000).Wait();
                    //WindowsUtil.SetCursorPos(OtherInventFirstSlotLocation.X, OtherInventFirstSlotLocation.Y);
                    //Task.Delay(200).Wait();
                    //WindowsUtil.PerformKeyboardClick(KeyboardVKs.T);
                    //Task.Delay(200).Wait();
                    //WindowsUtil.SetCursorPos(InventSearchbarLocation.X, InventSearchbarLocation.Y);
                    //WindowsUtil.PerformMouseClick();
                    //Task.Delay(200).Wait();
                    //WindowsUtil.PerformKeyboardClicks("SYRINGE");
                    //Task.Delay(200).Wait();
                    //WindowsUtil.SetCursorPos(InventFirstSlotLocation.X, InventFirstSlotLocation.Y);
                    //WindowsUtil.PerformMouseClick();
                    //Task.Delay(200).Wait();
                    //WindowsUtil.PerformKeyboardClick(KeyboardVKs.E);
                    //timer.Interval = 5500;
                    //timer.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }                
            });            
        }

        /// <summary>
        /// Begins the process of intelligently opening the vault's inventory.
        /// </summary>
        public void BeginOpenVaultProcess()
        {
            PrintAction(nameof(BeginOpenVaultProcess));
            RedAlertBot.Bot.IsBotEnabled = true;
            RedAlertBot.Bot.Recorder.RecordingTimer.Interval = OPEN_INVENT_TIMER_INTERVAL;
            RedAlertBot.Bot.Recorder.Predictor.UpdateTick += OnTryToOpenVault;

            void OnTryToOpenVault(object sender, UpdateTickEventArgs e)
            {
                PrintAction(nameof(OnTryToOpenVault));
                if (e.Value.Class == ImgClasses.Vault)
                {
                    Console.WriteLine("Identifed a vault.");
                    WindowsUtil.PerformKeyboardClick(KeyboardVKs.F);
                }
                else if (e.Value.Class == ImgClasses.Vault_UI)
                {
                    Console.WriteLine("Identified vault inventory.");
                    RedAlertBot.Bot.Recorder.Predictor.UpdateTick -= OnTryToOpenVault;
                    RedAlertBot.Bot.IsBotEnabled = false;
                    UseSyringeToThreshold();
                }
            }
        }

        /// <summary>
        /// Sucks blood from the player using syringe until the player is one suck away from death. Uses <see cref="UsesBeforeDeath"/> to know how many times it can suck blood.
        /// </summary>
        public async void UseSyringeToThreshold()
        {
            PrintAction(nameof(UseSyringeToThreshold));
            // Take the first slot in the other inventory
            WindowsUtil.SetCursorPos(OtherInventFirstSlotLocation.X, OtherInventFirstSlotLocation.Y);
            await Task.Delay(200);
            WindowsUtil.PerformKeyboardClick(KeyboardVKs.T);
            await Task.Delay(200);

            WindowsUtil.PerformMouseClick(InventSearchbarLocation);
            await Task.Delay(200);
            WindowsUtil.PerformKeyboardClicks("SYRINGE");

            await Task.Delay(200);
            WindowsUtil.PerformMouseClick(InventFirstSlotLocation);
            await Task.Delay(200);
            WindowsUtil.PerformKeyboardClick(KeyboardVKs.E);
            timer.Interval = 5500;
            timer.Elapsed += OnTimerElasped_UseSyringe;
            timer.Start();
        }

        /// <summary>
        /// Fires everytime the bot or user should use the syringe to extract blood.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerElasped_UseSyringe(object sender, ElapsedEventArgs e)
        {
            PrintAction(nameof(OnTimerElasped_UseSyringe));
            if (Counter == UsesBeforeDeath)
            {
                timer.Stop();
                timer.Elapsed -= OnTimerElasped_UseSyringe;
                Counter = 0;
                // Depo the syringe quickly
                Task.Delay(2000).Wait();
                WindowsUtil.PerformMouseClick(TransferAllBtnLocation);
                Task.Delay(200).Wait();
                WindowsUtil.PerformKeyboardClick(KeyboardVKs.F);
                // Starting the opening fridge process
                BeginOpenFridgeProcess();
                return;
            }
            else
            {
                Counter++;
                WindowsUtil.PerformKeyboardClick(KeyboardVKs.E);
            }
        }

        /// <summary>
        /// Begins the process of intelligently trying to open the fridge's inventory.
        /// </summary>
        public void BeginOpenFridgeProcess()
        {
            bool isInventoryOpen = false;

            PrintAction(nameof(BeginOpenFridgeProcess));
            RedAlertBot.Bot.IsBotEnabled = true;
            RedAlertBot.Bot.Recorder.RecordingTimer.Interval = SCAN_WHILE_TURNING_TIMER_INTERVAL;
            RedAlertBot.Bot.Recorder.Predictor.UpdateTick += OnTryToOpenFridge;

            //WindowsUtil.keybd_event((byte)KeyboardVKs.LEFT_ARROW, 0, WindowsUtil.KEYEVENTF_EXTENDEDKEY, 0);
            ////WindowsUtil.SendMessage(WindowsUtil.TargetWindow, WindowsUtil.KEYEVENTF_KEYDOWN, (uint)KeyboardVKs.LEFT_ARROW, 0);
            //Task.Delay(920).Wait();
            //WindowsUtil.keybd_event((byte)KeyboardVKs.LEFT_ARROW, 0, WindowsUtil.KEYEVENTF_EXTENDEDKEY | WindowsUtil.KEYEVENTF_KEYUP, 0);
            ////WindowsUtil.SendMessage(WindowsUtil.TargetWindow, WindowsUtil.KEYEVENTF_KEYUP, (uint)KeyboardVKs.LEFT_ARROW, 0);
            //Task.Delay(200).Wait();
            //WindowsUtil.PerformKeyboardClick(KeyboardVKs.F);            

            void OnTryToOpenFridge(object sender, UpdateTickEventArgs e)
            {
                PrintAction(nameof(OnTryToOpenFridge));

                // Keep rotating the character for as long as we don't see the fridge and the fridge isnt open
                WindowsUtil.keybd_event((byte)KeyboardVKs.LEFT_ARROW, 0, WindowsUtil.KEYEVENTF_EXTENDEDKEY, 0);
                Console.WriteLine("Searching for fridge.");                

                if (e.Value.Class == ImgClasses.Fridge && e.Value.Confidence >= THRESHOLD)
                {
                    Console.WriteLine("Identified a fridge.");
                    RedAlertBot.Bot.Recorder.RecordingTimer.Interval = OPEN_INVENT_TIMER_INTERVAL;
                    WindowsUtil.keybd_event((byte)KeyboardVKs.LEFT_ARROW, 0, WindowsUtil.KEYEVENTF_EXTENDEDKEY | WindowsUtil.KEYEVENTF_KEYUP, 0);
                    WindowsUtil.PerformKeyboardClick(KeyboardVKs.F);
                    isInventoryOpen = true;
                }
                else if (e.Value.Class == ImgClasses.Fridge_UI && e.Value.Confidence >= THRESHOLD)
                {
                    Console.WriteLine("Identified fridge inventory.");
                    // Once the FridgeUI is open we can unsub
                    RedAlertBot.Bot.Recorder.Predictor.UpdateTick -= OnTryToOpenFridge;
                    RedAlertBot.Bot.IsBotEnabled = false;
                    DepositBloodBags();
                }
                else if (e.Value.Class == ImgClasses.Vault_UI)
                {
                    if (!isInventoryOpen)
                    {
                        WindowsUtil.PerformKeyboardClick(KeyboardVKs.F);
                        isInventoryOpen = false;
                    }                    
                }
            }
        }

        /// <summary>
        /// Deposits the blood bags.
        /// </summary>
        public void DepositBloodBags()
        {
            PrintAction(nameof(DepositBloodBags));
            Task.Delay(200).Wait();
            WindowsUtil.PerformMouseClick(TransferAllBtnLocation);
            Task.Delay(200).Wait();
            WindowsUtil.PerformKeyboardClick(KeyboardVKs.F);
            BeginTransferUsingnBedProcess();
        }       

        public void BeginTransferUsingnBedProcess()
        {
            bool isUIOpen = false;
            int counter = 0;

            PrintAction(nameof(BeginTransferUsingnBedProcess));
            RedAlertBot.Bot.IsBotEnabled = true;
            RedAlertBot.Bot.Recorder.RecordingTimer.Interval = SCAN_WHILE_TURNING_TIMER_INTERVAL;
            RedAlertBot.Bot.Recorder.Predictor.UpdateTick += LookDownAtBed;            
            
            void LookDownAtBed(object sender, UpdateTickEventArgs e)
            {
                if (counter < 5)
                {
                    Console.WriteLine("Searching for bed.");
                    WindowsUtil.keybd_event((byte)KeyboardVKs.DOWN_ARROW, 0, WindowsUtil.KEYEVENTF_EXTENDEDKEY, 0);
                    counter++;
                }
                else
                {
                    RedAlertBot.Bot.Recorder.Predictor.UpdateTick -= LookDownAtBed;
                    Console.WriteLine("Identified a bed.");
                    WindowsUtil.keybd_event((byte)KeyboardVKs.DOWN_ARROW, 0, WindowsUtil.KEYEVENTF_EXTENDEDKEY | WindowsUtil.KEYEVENTF_KEYUP, 0);
                    WindowsUtil.PerformKeyboardClick(KeyboardVKs.E);
                    isUIOpen = true;
                    RedAlertBot.Bot.Recorder.Predictor.UpdateTick += OnTryToTransferUsingBed;
                }
            }

            void OnTryToTransferUsingBed(object sender, UpdateTickEventArgs e)
            {
                // If we are not looking at a fridge or in its ui, continue panning down
                
                //Console.WriteLine("Searching for bed.");
                //WindowsUtil.keybd_event((byte)KeyboardVKs.DOWN_ARROW, 0, WindowsUtil.KEYEVENTF_EXTENDEDKEY, 0);
                //if (e.Value.Class == ImgClasses.Bed && e.Value.Confidence >= THRESHOLD)
                //{
                //    Console.WriteLine("Identified a bed.");
                //    WindowsUtil.keybd_event((byte)KeyboardVKs.DOWN_ARROW, 0, WindowsUtil.KEYEVENTF_EXTENDEDKEY | WindowsUtil.KEYEVENTF_KEYUP, 0);
                //    WindowsUtil.PerformKeyboardClick(KeyboardVKs.E);
                //    isUIOpen = true;
                //}
                if (e.Value.Class == ImgClasses.Bed_UI)
                {
                    Console.WriteLine("Identified bed UI.");
                    RedAlertBot.Bot.Recorder.Predictor.UpdateTick -= OnTryToTransferUsingBed;
                    RedAlertBot.Bot.IsBotEnabled = false;
                    RespawnUsingBed("BDEATH", DeathBedsLocation);
                    Task.Delay(15000).Wait();
                    BeginRespawnProcess();
                }
                else if (e.Value.Class == ImgClasses.Fridge_UI || e.Value.Class == ImgClasses.Vault_UI)
                {
                    if (isUIOpen)
                    {
                        WindowsUtil.PerformKeyboardClick(KeyboardVKs.F);
                        isUIOpen = false;
                    }                    
                }
            }

            //Task.Delay(200).Wait();
            //WindowsUtil.keybd_event((byte)KeyboardVKs.DOWN_ARROW, 0, WindowsUtil.KEYEVENTF_EXTENDEDKEY, 0);
            //Task.Delay(1800).Wait();
            //WindowsUtil.keybd_event((byte)KeyboardVKs.DOWN_ARROW, 0, WindowsUtil.KEYEVENTF_EXTENDEDKEY | WindowsUtil.KEYEVENTF_KEYUP, 0);
            //Task.Delay(200).Wait();
            //WindowsUtil.PerformKeyboardClick(KeyboardVKs.E);
            //Task.Delay(1500).Wait();
            //WindowsUtil.PerformMouseClick(RespawnSearchBarLocation);
            //Task.Delay(200).Wait();
            //WindowsUtil.PerformKeyboardClicks("BDEATH");
            //Task.Delay(1000).Wait();
            //WindowsUtil.PerformMouseClick(DeathBedsLocation);
            //Task.Delay(1000).Wait();
            //WindowsUtil.PerformMouseClick(RespawnBtnLocation);
        }

        public void BeginRespawnProcess()
        {
            PrintAction(nameof(BeginRespawnProcess));
            RedAlertBot.Bot.IsBotEnabled = true;
            RedAlertBot.Bot.Recorder.RecordingTimer.Interval = SCAN_WHILE_TURNING_TIMER_INTERVAL;
            RedAlertBot.Bot.Recorder.Predictor.UpdateTick += OnTryToRespawnUsingBed;

            void OnTryToRespawnUsingBed(object sender, UpdateTickEventArgs e)
            {
                PrintAction(nameof(OnTryToRespawnUsingBed));
                if (e.Value.Class == ImgClasses.Bed_UI)
                {                    
                    RedAlertBot.Bot.Recorder.Predictor.UpdateTick -= OnTryToRespawnUsingBed;
                    RedAlertBot.Bot.IsBotEnabled = false;
                    RespawnUsingBed("BLOOD", BedLocation);
                    BeginOpenVaultProcess();
                }
            }
        }

        private void RespawnUsingBed(string bedName, Point bedLoc)
        {
            Console.WriteLine("Identified bed UI.");
            WindowsUtil.PerformMouseClick(RespawnSearchBarLocation);
            Task.Delay(200).Wait();
            WindowsUtil.PerformKeyboardClicks(bedName);
            Task.Delay(100).Wait();
            WindowsUtil.PerformMouseClick(bedLoc);
            Task.Delay(1000).Wait();
            WindowsUtil.PerformMouseClick(RespawnBtnLocation);
        }

        /// <summary>
        /// 1. Close inventory
        /// </summary>
        //private async void Finish()
        //{
        //    await Task.Run(() => {
        //        try
        //        {
        //            // depo syringe
        //            //Task.Delay(200).Wait();
        //            //WindowsUtil.PerformMouseClick(TransferAllBtnLocation);
        //            //Task.Delay(200).Wait();
        //            //WindowsUtil.PerformKeyboardClick(KeyboardVKs.F);

        //            // Closing self inventory and depo blood in vault
        //            //Task.Delay(200).Wait();
        //            //WindowsUtil.PerformKeyboardClick(KeyboardVKs.F);
        //            //Task.Delay(200).Wait();

        //            //BeginOpenFridgeProcess();

        //            //WindowsUtil.keybd_event((byte)KeyboardVKs.LEFT_ARROW, 0, WindowsUtil.KEYEVENTF_EXTENDEDKEY, 0);
        //            ////WindowsUtil.SendMessage(WindowsUtil.TargetWindow, WindowsUtil.KEYEVENTF_KEYDOWN, (uint)KeyboardVKs.LEFT_ARROW, 0);
        //            //Task.Delay(920).Wait();
        //            //WindowsUtil.keybd_event((byte)KeyboardVKs.LEFT_ARROW, 0, WindowsUtil.KEYEVENTF_EXTENDEDKEY | WindowsUtil.KEYEVENTF_KEYUP, 0);
        //            ////WindowsUtil.SendMessage(WindowsUtil.TargetWindow, WindowsUtil.KEYEVENTF_KEYUP, (uint)KeyboardVKs.LEFT_ARROW, 0);
        //            //Task.Delay(200).Wait();
        //            //WindowsUtil.PerformKeyboardClick(KeyboardVKs.F);
        //            //Task.Delay(2000).Wait();
        //            //WindowsUtil.PerformMouseClick(TransferAllBtnLocation);
        //            //Task.Delay(200).Wait();
        //            //WindowsUtil.PerformKeyboardClick(KeyboardVKs.F);

        //            // Look down to bed and respawn
        //            //Task.Delay(200).Wait();
        //            //WindowsUtil.keybd_event((byte)KeyboardVKs.DOWN_ARROW, 0, WindowsUtil.KEYEVENTF_EXTENDEDKEY, 0);
        //            //Task.Delay(1800).Wait();
        //            //WindowsUtil.keybd_event((byte)KeyboardVKs.DOWN_ARROW, 0, WindowsUtil.KEYEVENTF_EXTENDEDKEY | WindowsUtil.KEYEVENTF_KEYUP, 0);
        //            //Task.Delay(200).Wait();
        //            //WindowsUtil.PerformKeyboardClick(KeyboardVKs.E);
        //            //Task.Delay(1500).Wait();
        //            //WindowsUtil.PerformMouseClick(RespawnSearchBarLocation);
        //            //Task.Delay(200).Wait();
        //            //WindowsUtil.PerformKeyboardClicks("BDEATH");
        //            //Task.Delay(1000).Wait();
        //            //WindowsUtil.PerformMouseClick(DeathBedsLocation);
        //            //Task.Delay(1000).Wait();
        //            //WindowsUtil.PerformMouseClick(RespawnBtnLocation);
        //            Task.Delay(15000).Wait();
        //            Begin();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    });
        //}

        public void Stop()
        {

        }

        /// <summary>
        /// Saves the user's specification to a json file to be loaded and used on subsquent launches.
        /// </summary>
        public void Save()
        {
            File.WriteAllText(FILE_NAME, JsonConvert.SerializeObject(this));
        }

        public static BloodPumper Load()
        {
            if (File.Exists(FILE_NAME))
                return JsonConvert.DeserializeObject<BloodPumper>(File.ReadAllText(FILE_NAME));
            else
                return new BloodPumper();
        }

        public void SetRespawnSearchBar(Point p, Rectangle rect)
        {
            RespawnSearchBarLocation = p;
            WindowsUtil.MouseCoordsPolled = null;
        }

        public void SetBedLocation(Point p, Rectangle rect)
        {
            BedLocation = p;
            WindowsUtil.MouseCoordsPolled = null;
        }

        public void SetRespawnBtnLocation(Point p, Rectangle rect)
        {
            RespawnBtnLocation = p;
            WindowsUtil.MouseCoordsPolled = null;
        }

        public void SetOtherInventFirstSlotLocation(Point p, Rectangle rect)
        {
            OtherInventFirstSlotLocation = p;
            WindowsUtil.MouseCoordsPolled = null;
        }

        public void SetInventSearchbarLocation(Point p, Rectangle rect)
        {
            InventSearchbarLocation = p;
            WindowsUtil.MouseCoordsPolled = null;
        }

        public void SetInventFirstSlotLocation(Point p, Rectangle rect)
        {
            InventFirstSlotLocation = p;
            WindowsUtil.MouseCoordsPolled = null;
        }

        public void SetTransferAllBtnLocatin(Point p, Rectangle rect)
        {
            TransferAllBtnLocation = p;
            WindowsUtil.MouseCoordsPolled = null;
        }

        public void SetDeathBedLocation(Point p, Rectangle rect)
        {
            DeathBedsLocation = p;
            WindowsUtil.MouseCoordsPolled = null;
        }

        private static void PrintAction(string actionName) => Console.WriteLine("Action: " + actionName);
    }
}
