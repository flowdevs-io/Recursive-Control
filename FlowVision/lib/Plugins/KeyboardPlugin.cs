using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Classes;

namespace FlowVision.lib.Plugins
{
    internal class KeyboardPlugin
    {
        // Constants for keybd_event
        private const byte VK_LWIN = 0x5B;
        private const byte VK_R = 0x52; // 'R' key
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        // Import keybd_event from user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [Description("Used to interact with the keyboard")]
        public async Task<bool> SendKey(string keyCombo)
        {
            PluginLogger.LogPluginUsage("KeyboardPlugin", "SendKey", keyCombo);
            try
            {
                SendKeys.SendWait(keyCombo);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [Description("Send keyboard input to a specific window by handle")]
        public async Task<bool> SendKeyToWindow(string windowHandleString, string keyCombo)
        {
            PluginLogger.LogPluginUsage("KeyboardPlugin", "SendKeyToWindow", 
                $"window={windowHandleString}, keys={keyCombo}");
            
            try
            {
                IntPtr windowHandle = new IntPtr(Convert.ToInt32(windowHandleString));
                
                // Bring window to foreground with proper focus
                if (!BringWindowToForegroundWithFocus(windowHandle))
                {
                    PluginLogger.LogError("KeyboardPlugin", "SendKeyToWindow", 
                        "Failed to bring window to foreground");
                    return false;
                }

                // Wait a bit for the window to become active
                await Task.Delay(200);

                // Send the keys
                SendKeys.SendWait(keyCombo);
                
                PluginLogger.LogInfo("KeyboardPlugin", "SendKeyToWindow", 
                    $"✓ Successfully sent keys to window {windowHandleString}");
                
                return true;
            }
            catch (Exception ex)
            {
                PluginLogger.LogError("KeyboardPlugin", "SendKeyToWindow", 
                    $"Error: {ex.Message}");
                return false;
            }
        }

        [Description("Enter Key")]
        public async Task<bool> EnterKey()
        {
            PluginLogger.LogPluginUsage("KeyboardPlugin", "EnterKey", "ENTER");
            try
            {
                SendKeys.SendWait("{ENTER}");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [Description("Send Enter key to a specific window by handle")]
        public async Task<bool> EnterKeyToWindow(string windowHandleString)
        {
            return await SendKeyToWindow(windowHandleString, "{ENTER}");
        }

        [Description("Ctrl + Letter")]
        public async Task<bool> CtrlKey(string letter)
        {
            PluginLogger.LogPluginUsage("KeyboardPlugin", "CtrlKey", letter);
            try
            {
                SendKeys.SendWait($"^({letter})");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [Description("Send Ctrl+Letter combination to a specific window by handle")]
        public async Task<bool> CtrlKeyToWindow(string windowHandleString, string letter)
        {
            return await SendKeyToWindow(windowHandleString, $"^({letter})");
        }

        /// <summary>
        /// Brings a window to the foreground and ensures it has focus using multiple techniques
        /// </summary>
        private bool BringWindowToForegroundWithFocus(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                return false;

            try
            {
                // Get the current foreground window
                IntPtr currentForeground = GetForegroundWindow();

                // If it's already in foreground, we're done
                if (currentForeground == hWnd)
                    return true;

                // Get thread IDs
                uint currentThreadId = GetCurrentThreadId();
                uint targetThreadId = GetWindowThreadProcessId(hWnd, out _);
                uint foregroundThreadId = GetWindowThreadProcessId(currentForeground, out _);

                // Attach to the foreground thread to bypass restrictions
                bool needsDetach = false;
                if (currentThreadId != foregroundThreadId)
                {
                    AttachThreadInput(currentThreadId, foregroundThreadId, true);
                    needsDetach = true;
                }

                // Try to set foreground window
                bool success = SetForegroundWindow(hWnd);

                // Detach if we attached
                if (needsDetach)
                {
                    AttachThreadInput(currentThreadId, foregroundThreadId, false);
                }

                // Give it a moment to process
                Thread.Sleep(50);

                // Verify the window is now in foreground
                IntPtr newForeground = GetForegroundWindow();
                return newForeground == hWnd;
            }
            catch (Exception ex)
            {
                PluginLogger.LogError("KeyboardPlugin", "BringWindowToForegroundWithFocus", 
                    $"Error: {ex.Message}");
                return false;
            }
        }

        /*
         *
         *[Description("Alt + F4")]
        public async Task<bool> AltFFour()
        {
            FlowVision.lib.Classes.PluginLogger.LogPluginUsage("KeyboardPlugin", "AltFFour", "Alt + F4");
            AppendLog("Sending Key Command: Alt + F4");
            try
            {
                SendKeys.SendWait("%{F4}");
                return true;
            }
            catch (Exception ex)
            {
                AppendLog($"Error Sending Key: {ex.Message}");
                return false;
            }
        }
         */

        /*
        [Description("Opens the Run dialog (Windows+R)")]
        public async Task<bool> OpenRunDialog()
        {
            FlowVision.lib.Classes.PluginLogger.LogPluginUsage("KeyboardPlugin", "OpenRunDialog", "Windows+R");
            AppendLog("Opening Run dialog (Windows+R)");
            try
            {
                // Simulate Windows key down
                keybd_event(VK_LWIN, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
                // Simulate 'R' key down
                keybd_event(VK_R, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
                // Simulate 'R' key up
                keybd_event(VK_R, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
                // Simulate Windows key up
                keybd_event(VK_LWIN, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);

                return true;
            }
            catch (Exception ex)
            {
                AppendLog($"Error opening Run dialog: {ex.Message}");
                return false;
            }
        }
        */
    }
}
