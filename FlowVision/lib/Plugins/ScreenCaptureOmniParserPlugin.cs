using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Classes;
using Microsoft.SemanticKernel;

namespace FlowVision.lib.Plugins
{
    internal class ScreenCaptureOmniParserPlugin
    {
        private readonly string capPath = "c:\\temp\\cap.png";
        private readonly string prosPath = "c:\\temp\\pros.png";
        private readonly WindowSelectionPlugin _windowSelector;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out WindowSelectionPlugin.RECT lpRect);

        public ScreenCaptureOmniParserPlugin()
        {
            _windowSelector = new WindowSelectionPlugin();
        }

        [KernelFunction, Description("Used to capture the Screen and return Parsed Content")]
        public async Task<List<ParsedContent>> CaptureScreen(string handleString)
        {
            PluginLogger.LogPluginUsage("ScreenCaptureOmniParserPlugin", "CaptureScreen");

            OmniparserResponse omniResult;
            var capBase64 = CaptureWindow(handleString);

            using (HttpClient httpClient = new HttpClient())
            {
                OmniParserClient omniClient = new OmniParserClient(httpClient);
                // ProcessScreenshotAsync now returns a custom object.
                omniResult = await omniClient.ProcessScreenshotAsync(capBase64);
                SaveSomImage(omniResult.SomImageBase64);
            }

            return omniResult.ParsedContentList;
        }

        //capture the whole screen
        [KernelFunction, Description("Used to capture the whole screen")]
        public async Task<List<ParsedContent>> CaptureWholeScreen()
        {
            FlowVision.lib.Classes.PluginLogger.LogPluginUsage("ScreenCaptureOmniParserPlugin", "CaptureWholeScreen");
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

            return omniResult.ParsedContentList;
        }

        /// <summary>
        /// Save the image to the file system.
        /// </summary>
        /// <param name="somImageBase64"></param>
        private void SaveSomImage(string somImageBase64)
        {
            FlowVision.lib.Classes.PluginLogger.LogPluginUsage("ScreenCaptureOmniParserPlugin", "SaveSomImage");
            byte[] imageBytes = Convert.FromBase64String(somImageBase64);
            using (var ms = new System.IO.MemoryStream(imageBytes))
            {
                Image image = Image.FromStream(ms);
                image.Save(prosPath, ImageFormat.Png);
            }
        }

        public string CaptureWindow(string handleString)
        {
            FlowVision.lib.Classes.PluginLogger.LogPluginUsage("ScreenCaptureOmniParserPlugin", "CaptureWindow");
            IntPtr windowHandle = new IntPtr(Convert.ToInt32(handleString));

            if (!_windowSelector.IsWindowHandleValid(windowHandle))
            {
                throw new ArgumentException("Provided window handle is invalid.");
            }

            if (!SetForegroundWindow(windowHandle))
            {
                throw new InvalidOperationException("Failed to bring the window to the foreground.");
            }

            Thread.Sleep(200);

            if (!GetWindowRect(windowHandle, out WindowSelectionPlugin.RECT rc))
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
    }
}
