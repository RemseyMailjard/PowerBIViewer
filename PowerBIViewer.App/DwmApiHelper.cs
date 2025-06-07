using System.Runtime.InteropServices;
using System.Windows.Interop;
using System;
using System.Windows;

namespace PowerBIViewer.App.Helpers
{
    public static class DwmApiHelper
    {
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        public static void SetTitleBarTheme(Window window, bool isDarkMode)
        {
            try
            {
                var hwnd = new WindowInteropHelper(window).EnsureHandle();
                int DwmImmersiveDark = isDarkMode ? 1 : 0;
                DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref DwmImmersiveDark, sizeof(int));
            }
            catch (Exception)
            {
                // Negeer fouten, dit werkt mogelijk niet op oudere OS-versies.
            }
        }
    }
}