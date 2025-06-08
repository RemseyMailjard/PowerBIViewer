// FILE: Helpers/DwmApiHelper.cs
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace PowerBIViewer.App.Helpers
{
    public static partial class DwmApiHelper
    {
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        // ✨ TERUGGEZET: We gebruiken weer DllImport, wat stabiel en betrouwbaar is voor deze API-call.
        [LibraryImport("dwmapi.dll")]
        private static partial int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        public static void SetTitleBarTheme(Window window, bool isDarkMode)
        {
            // Een check op het OS blijft een goede praktijk.
            if (!IsWindows10OrGreater(10, 0, 19041))
            {
                return; // Doe niets op oudere systemen.
            }

            try
            {
                var hwnd = new WindowInteropHelper(window).EnsureHandle();
                int dwmImmersiveDark = isDarkMode ? 1 : 0;

                // We vangen nog steeds het resultaat op om de CA1806 waarschuwing te voorkomen.
                int result = DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dwmImmersiveDark, sizeof(int));

                if (result != 0)
                {
                    Debug.WriteLine($"DwmSetWindowAttribute failed with HRESULT: {result}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to set title bar theme: {ex.Message}");
            }
        }

        private static bool IsWindows10OrGreater(int major, int minor, int build)
        {
            // Environment.OSVersion.Version is voldoende voor deze check.
            return Environment.OSVersion.Version.Major >= major &&
                   Environment.OSVersion.Version.Minor >= minor &&
                   Environment.OSVersion.Version.Build >= build;
        }
    }
}