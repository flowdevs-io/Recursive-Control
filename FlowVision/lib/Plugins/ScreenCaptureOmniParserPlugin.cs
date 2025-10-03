using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Classes;

namespace FlowVision.lib.Plugins
{
    /// <summary>
    /// KISS Screen Capture Plugin with OmniParser
    /// Simple, Fast, No external dependencies
    /// </summary>
    internal class ScreenCaptureOmniParserPlugin
    {
        private readonly WindowSelectionPlugin _windowSelector;
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out WindowSelectionPlugin.RECT lpRect);

        public ScreenCaptureOmniParserPlugin()
        {
            _windowSelector = new WindowSelectionPlugin();
        }

        [Description("Used to capture the Screen and return Parsed Content")]
        public async Task<List<ParsedContent>> CaptureScreen(string handleString)
        {
            PluginLogger.LogPluginUsage("ScreenCaptureOmniParserPlugin", "CaptureScreen");

            using (Bitmap screenshot = CaptureWindowBitmap(handleString))
            {
                return await ProcessWithOmniParser(screenshot);
            }
        }

        [Description("Used to capture the whole screen")]
        public async Task<List<ParsedContent>> CaptureWholeScreen()
        {
            PluginLogger.LogPluginUsage("ScreenCaptureOmniParserPlugin", "CaptureWholeScreen");

            // Capture all screens
            using (Bitmap screenshot = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height))
            {
                using (Graphics gfx = Graphics.FromImage(screenshot))
                {
                    gfx.CopyFromScreen(
                        SystemInformation.VirtualScreen.X, 
                        SystemInformation.VirtualScreen.Y, 
                        0, 0, 
                        SystemInformation.VirtualScreen.Size, 
                        CopyPixelOperation.SourceCopy);
                }

                return await ProcessWithOmniParser(screenshot);
            }
        }

        /// <summary>
        /// Process screenshot with Simple OmniParser (KISS implementation)
        /// </summary>
        private async Task<List<ParsedContent>> ProcessWithOmniParser(Bitmap screenshot)
        {
            return await Task.Run(() =>
            {
                try
                {
                    PluginLogger.NotifyTaskStart("OmniParser", "Processing with native ONNX engine...");
                    
                    // Use singleton instance
                    var elements = SimpleOmniParser.Instance.ParseScreenshot(screenshot);
                    var parsedContent = ConvertToLegacyFormat(elements);
                    
                    PluginLogger.NotifyTaskComplete("OmniParser");
                    PluginLogger.LogInfo("ScreenCaptureOmniParserPlugin", "ProcessWithOmniParser", 
                        $"✓ Found {parsedContent.Count} UI elements");
                    
                    return parsedContent;
                }
                catch (Exception ex)
                {
                    PluginLogger.LogError("ScreenCaptureOmniParserPlugin", "ProcessWithOmniParser", 
                        $"Error processing screenshot: {ex.Message}");
                    throw;
                }
            });
        }

        /// <summary>
        /// Convert simple UIElement to legacy ParsedContent format
        /// </summary>
        private List<ParsedContent> ConvertToLegacyFormat(List<UIElement> elements)
        {
            var parsedContent = new List<ParsedContent>();

            foreach (var element in elements)
            {
                parsedContent.Add(new ParsedContent
                {
                    Type = "ui_element",
                    BBox = new double[] 
                    { 
                        element.X,
                        element.Y,
                        element.X + element.Width,
                        element.Y + element.Height
                    },
                    Content = $"UI Element #{element.ElementId} at ({element.X},{element.Y}) " +
                             $"[size: {element.Width}x{element.Height}] " +
                             $"[confidence: {element.Confidence:P1}]",
                    Interactivity = true,
                    Source = "simple_onnx"
                });
            }

            return parsedContent;
        }

        /// <summary>
        /// Capture a specific window as a bitmap
        /// </summary>
        private Bitmap CaptureWindowBitmap(string handleString)
        {
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

            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                gfx.CopyFromScreen(rc.Left, rc.Top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            }

            return bitmap;
        }
    }
}
