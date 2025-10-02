using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                try
                {
                    if (p.MainWindowHandle == IntPtr.Zero)
                        continue;
                    string item = $"Handle: {p.MainWindowHandle}, Title: {p.MainWindowTitle}, Process: {p.ProcessName}";
                    windowList.Add(item);
                }
                catch
                {
                    continue;
                }
            }
            return string.Join("\n", windowList);
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
