//using Microsoft.ML.Transforms.Text;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HWND = System.IntPtr;

namespace RedAlertBot.Util
{
    public class WindowsUtil
    {               
        const string USER32_DLL = "user32.dll";

        public const int CLEINT_ONLY_AREA_FLAG = 0x1;

        public static Action<Point, Rectangle> MouseCoordsPolled { get; set; }

        /// <summary>
        /// Window handle to the window that is our application.
        /// </summary>
        public static HWND SourceWindow { get; private set; }
        /// <summary>
        /// Window handle to the window that our application is targeting.
        /// </summary>
        //public static HWND TargetWindow { get; private set; }

        #region Mouse Consts

        private const int MOUSEEVENTF_LEFT = 0x01;
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        #endregion Mouse Consts

        #region Keyboard Consts

        public const int HOTKEY_ID = 9000;     // Used to register a hotkey
        public const int MW_HOTKEY = 0x0312;
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const ushort KEYEVENTF_KEYUP = 0x0002;

        public const uint MOD_NONE = 0x0000;

        #endregion Keyboard Consts  

        public WindowsUtil(HWND hwnd)
        {
            SourceWindow = hwnd;
            //TargetWindow = WindowsUtil.FindWindowA(null, "ARK: Survival Evolved");
            RegisterHotKey(SourceWindow, HOTKEY_ID, MOD_NONE, (uint)KeyboardVKs.F8);             
        }                
        
        ~WindowsUtil()
        {
            UnregisterHotKey(SourceWindow, HOTKEY_ID);
        }

        /// <summary>
        ///     Executes a click using the win32 dll
        ///     <br/><br/>
        ///     Param Options:<br/>
        ///     No arguments == perform click at current mouse location<br/>
        ///     Yes arguments == perform click at provided point location
        /// </summary>
        public static void PerformMouseClick(Point _location = default)
        {
            // No location was passed so we will click at the current mouses location
            if (_location.IsEmpty)
            {
                GetCursorPos(out Point loc);
                ExecuteMouseEvent(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, loc.X, loc.Y, 0, 0);
            }
            // A location was passed so we will move the cursor there and click
            else
            {
                SetCursorPosition(_location.X, _location.Y);
                ExecuteMouseEvent(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, _location.X, _location.Y, 0, 0);
            }
        }

        public static void PerformKeyboardClicks(string statement)
        {
            SetActiveWindow(RedAlertBot.Bot.Recorder.TargetWindowHWND);
            for (int i = 0; i < statement.Length; i++)
            {
                byte key = (byte)statement[i];

                //SendMessage(TargetWindow, KEYEVENTF_KEYDOWN, key, 0);
                //SendMessage(TargetWindow, KEYEVENTF_KEYDOWN | KEYEVENTF_KEYUP, key, 0);

                keybd_event(key, 0, KEYEVENTF_EXTENDEDKEY, 0);
                keybd_event(key, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            }
        }

        /// <summary>
        ///     Presses a specified key using the win32 dll
        /// </summary>
        public static void PerformKeyboardClick(KeyboardVKs _key)
        {
            byte key = (byte)_key;

            keybd_event(key, 0, KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(key, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        /// <summary>
        ///     Called when a hook (hotkey) is clicked
        ///     
        ///     IMPORTANT: 
        ///             This function needs to know the calling hook's object or the sender. The instance that goes with the called hook in our program.
        ///             In order to find this out we need to make sure we are able to search our protocols keys using the hook. This way we will be able
        ///             to find the original sender instance.
        /// </summary>
        public static HWND ToggleHooks(HWND hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == MW_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                uint clickedKey = ((uint)lParam >> 16) & 0xFFFF;
                try
                {
                    switch ((KeyboardVKs)clickedKey)
                    {
                        case KeyboardVKs.F8:
                            {
                                GetCursorPos(out Point p);
                                GetWindowRect(RedAlertBot.Bot.Recorder.TargetWindowHWND, out RECT rect);
                                MouseCoordsPolled?.Invoke(p, new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top));
                            }                            
                            break;
                        default:
                            break;
                    }
                    Console.WriteLine(Enum.GetName(typeof(KeyboardVKs), (KeyboardVKs)clickedKey));                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                handled = true;
            }
            return IntPtr.Zero;
        }
        [DllImport("user32.dll")] //Set the active window

        public static extern HWND SetActiveWindow(HWND hWnd);

        [DllImport(USER32_DLL)] //sends a windows message to the specified window
        public static extern int SendMessage(HWND hWnd, int Msg, uint wParam, int lParam);

        [DllImport(USER32_DLL)]
        public static extern bool GetCursorPos(out Point point);

        [DllImport(USER32_DLL)]
        public static extern bool SetCursorPos(int x, int y);

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-findwindowa
        /// </summary>
        [DllImport(USER32_DLL)]
        public static extern HWND FindWindowA(string className, string windowName);

        [DllImport(USER32_DLL)]
        public static extern bool GetWindowRect(HWND hWnd, out RECT lpRect);

        [DllImport(USER32_DLL)]
        public static extern bool PrintWindow(HWND hWnd, HWND hdcBlt, int nFlags);

        [DllImport(USER32_DLL, EntryPoint = "SetCursorPos")]
        static extern bool SetCursorPosition(int x, int y);

        [DllImport(USER32_DLL, EntryPoint = "mouse_event")]
        static extern void ExecuteMouseEvent(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport(USER32_DLL)]
        private static extern void RegisterHotKey(HWND hWnd, int id, uint fsModifiers, uint vk);

        [DllImport(USER32_DLL)]
        private static extern bool UnregisterHotKey(HWND hWnd, int id);

        [DllImport(USER32_DLL)]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

#if DEBUG
        const string KERNAL_DLL = "kernel32.dll";

        public static bool HasConsole { get; private set; }

        [DllImport(KERNAL_DLL)]
        public static extern void AllocConsole();

        [DllImport(KERNAL_DLL)]
        public static extern void FreeConsole();

        [DllImport(KERNAL_DLL)]
        static extern bool AttachConsole(in uint dwProcessId);

        [STAThread]
        public static void InitConsole()
        {
            try
            {
                if (!AttachToConsole())
                {
                    AllocConsole();
                    HasConsole = true;
                    Console.WriteLine("Console Initialized.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void CloseConsole()
        {
            if (HasConsole)
            {
                HasConsole = false;
                FreeConsole();
            }
        }

        public static bool AttachToConsole()
        {
            const uint ParentProcess = 0xFFFFFFFF;
            if (!AttachConsole(ParentProcess))
                return false;

            Console.Clear();
            Console.WriteLine("Attached to console!");
            return true;
        }
#endif                
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    public enum KeyboardVKs
    {
        LEFT_ARROW = 0x25,        
        UP_ARROW = 0x26,
        RIGHT_ARROW = 0x27,
        DOWN_ARROW = 0x28,

        A = 0x41,
        B = 0x42, 
        C = 0x43,
        D = 0x44, 
        E = 0x45, 
        F = 0x46, 
        G = 0x47, 
        H = 0x48, 
        I = 0x49,  
        J = 0x4A,        
        K = 0x4B,        
        L = 0x4C,        
        M = 0x4D,        
        N = 0x4E,        
        O = 0x4F,        
        P = 0x50,        
        Q = 0x51,        
        R = 0x52,       
        S = 0x53,         
        T = 0x54,    
        U = 0x55,        
        V = 0x56,        
        W = 0x57,        
        X = 0x58,        
        Y = 0x59,        
        Z = 0x5A,

        //a = 0x41,
        //b = 0x42,
        //c = 0x43,
        //d = 0x44,
        //e = 0x45,
        //f = 0x46,
        //g = 0x47,
        //h = 0x48,
        //i = 0x49,
        //j = 0x4A,
        //k = 0x4B,
        //l = 0x4C,
        //m = 0x4D,
        //n = 0x4E,
        //o = 0x4F,
        //p = 0x50,
        //q = 0x51,
        //r = 0x52,
        //s = 0x53,
        //t = 0x54,
        //u = 0x55,
        //v = 0x56,
        //w = 0x57,
        //x = 0x58,
        //y = 0x59,
        //z = 0x5A,

        // Function Keys
        F1 = 0x70,
        F2 = 0x71,
        F3 = 0x72,
        F4 = 0x73,
        F5 = 0x74,
        F6 = 0x75,
        F7 = 0x76,
        F8 = 0x77,
        F9 = 0x78,
        F10 = 0x79,
        F11 = 0x7A,
        F12 = 0x7A,        
    }            
}
