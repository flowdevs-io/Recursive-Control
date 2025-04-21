using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SemanticKernel;

namespace FlowVision.lib.Plugins
{
    internal class ScreenCapturePlugin
    {
        private readonly string capPath = "c:\\temp\\cap.png";
        private readonly string prosPath = "c:\\temp\\pros.png";


        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [KernelFunction, Description("Used to capture the Screen and return Parsed Content")]
        public async Task<List<ParsedContent>> CaptureScreen(string handleString)
        {
            AppendLog("== ScreenCapturePlugin CALL START ==");
            AppendLog("Screen Capture");

            OmniparserResponse omniResult;
            var capBase64 = CaptureWindow(handleString);

            using (HttpClient httpClient = new HttpClient())
            {
                OmniParserClient omniClient = new OmniParserClient(httpClient);
                // ProcessScreenshotAsync now returns a custom object.
                omniResult = await omniClient.ProcessScreenshotAsync(capBase64);
                SaveSomImage(omniResult.SomImageBase64);
            }

            AppendLog("== ScreenCapturePlugin CALL END ==");
            return omniResult.ParsedContentList;
        }

        //capture the whole screen
        [KernelFunction, Description("Used to capture the whole screen")]
        public async Task<List<ParsedContent>> CapturewholeScreen()
        {
            var capBase64 = "";
            OmniparserResponse omniResult;

            //take capture all screens
            using (Bitmap bmp = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height))
            {
                using (Graphics gfxBmp = Graphics.FromImage(bmp))
                {
                    gfxBmp.CopyFromScreen(SystemInformation.VirtualScreen.X, SystemInformation.VirtualScreen.Y, 0, 0, SystemInformation.VirtualScreen.Size, CopyPixelOperation.SourceCopy);
                }
                using (var ms = new System.IO.MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    capBase64 = Convert.ToBase64String(ms.ToArray());
                }
            }

            using (HttpClient httpClient = new HttpClient())
            {
                OmniParserClient omniClient = new OmniParserClient(httpClient);
                // ProcessScreenshotAsync now returns a custom object.
                omniResult = await omniClient.ProcessScreenshotAsync(capBase64);
                SaveSomImage(omniResult.SomImageBase64);
            }

            AppendLog("== ScreenCapturePlugin CALL END ==");
            return omniResult.ParsedContentList;
        }

        [KernelFunction, Description("Used to set current handle as foreground")]
        public async Task<bool> ForegroundSelect(string handleString)
        {
            IntPtr windowHandle = new IntPtr(Convert.ToInt32(handleString));
            if (!SetForegroundWindow(windowHandle))
            {
                throw new InvalidOperationException("Failed to bring the window to the foreground.");
            }
            return true;
        }

        /// <summary>
        /// Save the image to the file system.
        /// </summary>
        /// <param name="somImageBase64"></param>
        private void SaveSomImage(string somImageBase64)
        {
            byte[] imageBytes = Convert.FromBase64String(somImageBase64);
            using (var ms = new System.IO.MemoryStream(imageBytes))
            {
                Image image = Image.FromStream(ms);
                image.Save(prosPath, ImageFormat.Png);
            }
        }

        public string CaptureWindow(string handleString)
        {
            IntPtr windowHandle = new IntPtr(Convert.ToInt32(handleString));

            if (!IsWindowHandleValid(windowHandle))
            {
                throw new ArgumentException("Provided window handle is invalid.");
            }

            if (!SetForegroundWindow(windowHandle))
            {
                throw new InvalidOperationException("Failed to bring the window to the foreground.");
            }

            Thread.Sleep(200);

            if (!GetWindowRect(windowHandle, out RECT rc))
            {
                throw new InvalidOperationException("Failed to get the window rectangle.");
            }

            int width = rc.Right - rc.Left;
            int height = rc.Bottom - rc.Top;

            using (Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                using (Graphics gfxBmp = Graphics.FromImage(bmp))
                {
                    gfxBmp.CopyFromScreen(rc.Left, rc.Top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
                }

                using (var ms = new System.IO.MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        [KernelFunction, Description("Returns a list of available window handles, titles, and process names.")]
        public string ListWindowHandles()
        {
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

        private bool IsWindowHandleValid(IntPtr hWnd)
        {
            return hWnd != IntPtr.Zero && GetWindowRect(hWnd, out _);
        }

        /// <summary>
        /// Appends a message to the UI control safely.
        /// </summary>
        /// <param name="message">The message to append.</param>
        private void AppendLog(string message)
        {
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
