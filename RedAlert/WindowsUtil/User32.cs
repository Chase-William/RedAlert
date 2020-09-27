using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using HWND = System.IntPtr;

namespace RedAlertUI.WindowsUtil
{
    public static class User32
    {
        const string USER32_DLL = "user32.dll";              

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-findwindowa
        /// </summary>
        [DllImport(USER32_DLL)]
        public static extern HWND FindWindowA(string className, string windowName);

        [DllImport(USER32_DLL)]
        public static extern bool GetWindowRect(HWND hWnd, out RECT lpRect);

        [DllImport(USER32_DLL)]
        public static extern bool PrintWindow(HWND hWnd, HWND hdcBlt, int nFlags);
        
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
}
