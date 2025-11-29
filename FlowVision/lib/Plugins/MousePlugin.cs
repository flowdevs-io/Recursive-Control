using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Classes;

namespace FlowVision.lib.Plugins
{
    internal class MousePlugin
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

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

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        private const int SW_RESTORE = 9;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;

        [Description("Clicks at the specified normalized bounding box coordinates on a specific window handle. Box is [x1, y1, x2, y2].")]
        public async Task<bool> ClickOnWindow(string windowHandleString, double x1, double y1, double x2, double y2, bool leftClick, int clickTimes)
        {
            // Log the plugin usage
            PluginLogger.LogPluginUsage("MousePlugin", "ClickOnWindow", 
                $"window={windowHandleString}, box=[{x1},{y1},{x2},{y2}], leftClick={leftClick}");

            IntPtr windowHandle = new IntPtr(Convert.ToInt32(windowHandleString));

            // Ensure window is visible and focused
            if (!BringWindowToForegroundWithFocus(windowHandle))
            {
                PluginLogger.LogError("MousePlugin", "ClickOnWindow", "Failed to focus window");
                return false;
            }

            if (!GetWindowRect(windowHandle, out RECT rc))
            {
                throw new InvalidOperationException("Failed to get window rectangle.");
            }

            int windowWidth = rc.Right - rc.Left;
            int windowHeight = rc.Bottom - rc.Top;

            // Calculate center of bounding box
            double centerX = (x1 + x2) / 2;
            double centerY = (y1 + y2) / 2;
            
            int x, y;
            
            // Check if coordinates are normalized (0-1 range)
            bool isNormalized = (x1 >= 0 && x1 <= 1.0 && y1 >= 0 && y1 <= 1.0 && 
                                 x2 >= 0 && x2 <= 1.0 && y2 >= 0 && y2 <= 1.0);
            
            if (isNormalized)
            {
                // Normalized: multiply by window size
                x = rc.Left + (int)(centerX * windowWidth);
                y = rc.Top + (int)(centerY * windowHeight);
            }
            else
            {
                // Window-relative pixel coordinates: add window origin
                x = rc.Left + (int)centerX;
                y = rc.Top + (int)centerY;
            }
            
            PluginLogger.LogInfo("MousePlugin", "ClickOnWindow", 
                $"bbox center ({centerX:F0}, {centerY:F0}) + window ({rc.Left}, {rc.Top}) -> screen ({x}, {y})");

            if (!SetCursorPos(x, y))
            {
                throw new InvalidOperationException("Failed to set cursor position.");
            }

            // Increased delay to allow UI to register hover state
            await Task.Delay(200);

            for (int i = 0; i < clickTimes; i++)
            {
                SimulateClick(x, y, leftClick);
                await Task.Delay(50);
            }

            return true;
        }

        [Description("Clicks at screen coordinates. Use with GetPageElements() to find elements.")]
        public async Task<bool> ClickAtScreenCoordinates(double x1, double y1, double x2, double y2, bool leftClick, int clickTimes)
        {
            int x = (int)((x1 + x2) / 2);
            int y = (int)((y1 + y2) / 2);
            
            PluginLogger.LogPluginUsage("MousePlugin", "ClickAtScreenCoordinates", 
                $"box=[{x1},{y1},{x2},{y2}], center=({x},{y}), leftClick={leftClick}");

            if (!SetCursorPos(x, y))
            {
                throw new InvalidOperationException("Failed to set cursor position.");
            }

            await Task.Delay(200);

            for (int i = 0; i < clickTimes; i++)
            {
                SimulateClick(x, y, leftClick);
                await Task.Delay(50);
            }

            return true;
        }

        [Description("uses scroll wheel on a specific window handle.")]
        public async Task<bool> ScrollOnWindow(string windowHandleString, int scrollAmount)
        {
            // Log the plugin usage
            PluginLogger.LogPluginUsage("MousePlugin", "ScrollOnWindow", 
                $"window={windowHandleString}, amount={scrollAmount}");
            
            IntPtr windowHandle = new IntPtr(Convert.ToInt32(windowHandleString));
            
            if (!BringWindowToForegroundWithFocus(windowHandle))
            {
                return false;
            }

            if (!GetWindowRect(windowHandle, out RECT rc))
            {
                throw new InvalidOperationException("Failed to get window rectangle.");
            }
            int x = (rc.Left + rc.Right) / 2;
            int y = (rc.Top + rc.Bottom) / 2;
            if (!SetCursorPos(x, y))
            {
                throw new InvalidOperationException("Failed to set cursor position.");
            }
            await Task.Delay(200);
            mouse_event(0x0800, 0, 0, (uint)scrollAmount, UIntPtr.Zero);
            return true;
        }

        private void SimulateClick(int x, int y, bool leftClick)
        {
            uint down = leftClick ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_RIGHTDOWN;
            uint up = leftClick ? MOUSEEVENTF_LEFTUP : MOUSEEVENTF_RIGHTUP;

            mouse_event(down, (uint)x, (uint)y, 0, UIntPtr.Zero);
            mouse_event(up, (uint)x, (uint)y, 0, UIntPtr.Zero);
        }

        /// <summary>
        /// Brings a window to the foreground and ensures it has focus using multiple techniques
        /// </summary>
        private bool BringWindowToForegroundWithFocus(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return false;

            try
            {
                ShowWindow(hWnd, SW_RESTORE);
                IntPtr currentForeground = GetForegroundWindow();
                if (currentForeground == hWnd) return true;

                uint currentThreadId = GetCurrentThreadId();
                uint targetThreadId = GetWindowThreadProcessId(hWnd, out _);
                uint foregroundThreadId = GetWindowThreadProcessId(currentForeground, out _);

                bool needsDetach = false;
                if (currentThreadId != foregroundThreadId)
                {
                    AttachThreadInput(currentThreadId, foregroundThreadId, true);
                    needsDetach = true;
                }
                if (targetThreadId != currentThreadId && targetThreadId != foregroundThreadId)
                {
                     AttachThreadInput(currentThreadId, targetThreadId, true);
                }

                bool success = SetForegroundWindow(hWnd);
                SetFocus(hWnd);

                if (needsDetach) AttachThreadInput(currentThreadId, foregroundThreadId, false);
                if (targetThreadId != currentThreadId && targetThreadId != foregroundThreadId)
                     AttachThreadInput(currentThreadId, targetThreadId, false);

                System.Threading.Thread.Sleep(100);
                return GetForegroundWindow() == hWnd;
            }
            catch (Exception ex)
            {
                PluginLogger.LogError("MousePlugin", "BringWindowToForegroundWithFocus", $"Error: {ex.Message}");
                return false;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}