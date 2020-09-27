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
