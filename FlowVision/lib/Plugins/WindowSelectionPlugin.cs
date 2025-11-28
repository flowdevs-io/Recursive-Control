using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FlowVision.lib.Classes;

namespace FlowVision.lib.Plugins
{
    internal class WindowSelectionPlugin
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        // --- New P/Invoke Definitions for Safe Enumeration ---
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(
            IntPtr hWnd, 
            uint Msg, 
            IntPtr wParam, 
            IntPtr lParam, 
            uint fuFlags, 
            uint uTimeout, 
            out IntPtr lpdwResult);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        private const uint WM_GETTEXT = 0x000D;
        private const uint WM_GETTEXTLENGTH = 0x000E;
        private const uint SMTO_ABORTIFHUNG = 0x0002;
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        // ------------------------------------------------------

        [Description("Gets the handle and title of the currently active foreground window.")]
        public string GetForegroundWindowInfo()
        {
            IntPtr hWnd = GetForegroundWindow();
            if (hWnd == IntPtr.Zero) return "No active window";

            string title = GetWindowTitleSafe(hWnd);
            return $"Handle: {hWnd}, Title: {title}";
        }

        [Description("Used to set current handle as foreground")]
        public async Task<bool> ForegroundSelect(string handleString)
        {
            // Log the plugin usage
            PluginLogger.LogPluginUsage("WindowSelectionPlugin", "ForegroundSelect");

            IntPtr windowHandle = new IntPtr(Convert.ToInt32(handleString));
            if (!SetForegroundWindow(windowHandle))
            {
                throw new InvalidOperationException("Failed to bring the window to the foreground.");
            }
            return true;
        }

        [Description("Returns a list of available window handles, titles, and process names.")]
        public string ListWindowHandles()
        {
            // Log the plugin usage
            PluginLogger.LogPluginUsage("WindowSelectionPlugin", "ListWindowHandles");

            var windowList = new List<string>();

            EnumWindows((hWnd, lParam) =>
            {
                // Filter invisible windows to reduce noise and hangs
                if (!IsWindowVisible(hWnd))
                    return true; // Continue enumeration

                // Safely get window title with timeout
                string title = GetWindowTitleSafe(hWnd);
                
                // Skip untitled windows (often hidden helper windows)
                if (string.IsNullOrWhiteSpace(title))
                    return true;

                // Get process name
                string processName = "Unknown";
                try
                {
                    GetWindowThreadProcessId(hWnd, out uint processId);
                    using (var p = Process.GetProcessById((int)processId))
                    {
                        processName = p.ProcessName;
                    }
                }
                catch { /* Process might have exited or access denied */ }

                string item = $"Handle: {hWnd}, Title: {title}, Process: {processName}";
                windowList.Add(item);

                return true; // Continue enumeration
            }, IntPtr.Zero);

            return string.Join("\n", windowList);
        }

        /// <summary>
        /// Safely retrieves window title with a timeout to prevent hanging on unresponsive windows.
        /// </summary>
        private string GetWindowTitleSafe(IntPtr hWnd)
        {
            const int timeoutMs = 100; // Short timeout to ensure responsiveness

            // 1. Get text length with timeout
            IntPtr result;
            IntPtr ret = SendMessageTimeout(
                hWnd, 
                WM_GETTEXTLENGTH, 
                IntPtr.Zero, 
                IntPtr.Zero, 
                SMTO_ABORTIFHUNG, 
                timeoutMs, 
                out result);

            if (ret == IntPtr.Zero) return string.Empty; // Failed or timed out

            int length = (int)result;
            if (length == 0) return string.Empty;

            // 2. Get actual text with timeout
            // Allocate unmanaged memory for the string buffer
            // Length + 1 for null terminator, * 2 for Unicode characters
            int bufferSize = (length + 1) * 2;
            IntPtr buffer = Marshal.AllocHGlobal(bufferSize);

            try
            {
                ret = SendMessageTimeout(
                    hWnd,
                    WM_GETTEXT,
                    new IntPtr(length + 1),
                    buffer,
                    SMTO_ABORTIFHUNG,
                    timeoutMs,
                    out result);

                if (ret == IntPtr.Zero) return string.Empty;

                return Marshal.PtrToStringAuto(buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// Checks if the provided window handle is valid.
        /// </summary>
        public bool IsWindowHandleValid(IntPtr hWnd)
        {
            // Log the plugin usage
            PluginLogger.LogPluginUsage("WindowSelectionPlugin", "IsWindowHandleValid");

            return hWnd != IntPtr.Zero && GetWindowRect(hWnd, out _);
        }

        /// <summary>
        /// Appends a message to the UI control safely.
        /// </summary>
        /// <param name="message">The message to append.</param>
        private void AppendLog(string message)
        {
            // Log the plugin usage
            PluginLogger.LogPluginUsage("WindowSelectionPlugin", "AppendLog");
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
