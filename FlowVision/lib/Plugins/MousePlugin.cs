using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SemanticKernel;

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

        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;

        [KernelFunction, Description("Clicks at the specified normalized bounding box coordinates on a specific window handle.")]
        public async Task<bool> ClickOnWindow(string windowHandleString, double[] bBox, bool leftClick, int clickTimes)
        {
            AppendLog("== MousePlugin ClickOnWindow CALL START ==");
            IntPtr windowHandle = new IntPtr(Convert.ToInt32(windowHandleString));

            if (!GetWindowRect(windowHandle, out RECT rc))
            {
                AppendLog("Failed to get window rectangle.");
                throw new InvalidOperationException("Failed to get window rectangle.");
            }

            int windowWidth = rc.Right - rc.Left;
            int windowHeight = rc.Bottom - rc.Top;

            // Calculate absolute position based on bounding box (normalized)
            int x = rc.Left + (int)((bBox[0] + bBox[2]) / 2 * windowWidth);
            int y = rc.Top + (int)((bBox[1] + bBox[3]) / 2 * windowHeight);

            AppendLog($"Calculated click coordinates: ({x}, {y})");

            if (!SetCursorPos(x, y))
            {
                AppendLog("Failed to set cursor position.");
                throw new InvalidOperationException("Failed to set cursor position.");
            }

            await Task.Delay(100);

            for (int i = 0; i < clickTimes; i++)
            {
                SimulateClick(x, y, leftClick);
                AppendLog($"Simulated {(leftClick ? "left" : "right")} click {i + 1} time(s).");
                await Task.Delay(50);
            }

            AppendLog("== MousePlugin ClickOnWindow CALL END ==");
            return true;
        }

        private void SimulateClick(int x, int y, bool leftClick)
        {
            uint down = leftClick ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_RIGHTDOWN;
            uint up = leftClick ? MOUSEEVENTF_LEFTUP : MOUSEEVENTF_RIGHTUP;

            mouse_event(down, (uint)x, (uint)y, 0, UIntPtr.Zero);
            mouse_event(up, (uint)x, (uint)y, 0, UIntPtr.Zero);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        /// <summary>
        /// Appends a message to the UI control safely.
        /// </summary>
        /// <param name="message">Message to log.</param>
        private void AppendLog(string message)
        {
        }
    }
}