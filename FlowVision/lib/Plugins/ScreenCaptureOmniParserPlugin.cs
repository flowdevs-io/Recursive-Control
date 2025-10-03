using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Classes;

namespace FlowVision.lib.Plugins
{
    internal class ScreenCaptureOmniParserPlugin
    {
        private readonly string prosPath = Path.Combine(Path.GetTempPath(), "pros.png");
        private readonly WindowSelectionPlugin _windowSelector;
        private static OnnxOmniParserEngine _onnxEngine;
        private static bool _useOnnxMode = true; // Default to ONNX mode (no Python server required!)
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out WindowSelectionPlugin.RECT lpRect);

        public ScreenCaptureOmniParserPlugin()
        {
            _windowSelector = new WindowSelectionPlugin();
            
            // Initialize ONNX engine at startup to keep YOLO model ready
            if (_useOnnxMode && _onnxEngine == null)
            {
                ConfigureMode(true);
            }
        }

        /// <summary>
        /// Configure OmniParser to use ONNX (native .NET) or HTTP Server (Python) mode
        /// </summary>
        public static void ConfigureMode(bool useOnnx = true, string onnxModelPath = null)
        {
            _useOnnxMode = useOnnx;
            
            if (_useOnnxMode && _onnxEngine == null)
            {
                try
                {
                    PluginLogger.LogInfo("ScreenCaptureOmniParserPlugin", "ConfigureMode", 
                        "Initializing native ONNX OmniParser (no Python server required)");
                    _onnxEngine = new OnnxOmniParserEngine(onnxModelPath);
                    PluginLogger.LogInfo("ScreenCaptureOmniParserPlugin", "ConfigureMode", 
                        "✓ ONNX OmniParser initialized successfully");
                }
                catch (Exception ex)
                {
                    PluginLogger.LogError("ScreenCaptureOmniParserPlugin", "ConfigureMode", 
                        $"Failed to initialize ONNX engine: {ex.Message}. Falling back to HTTP server mode.");
                    _useOnnxMode = false;
                }
            }
        }

        [Description("Used to capture the Screen and return Parsed Content")]
        public async Task<List<ParsedContent>> CaptureScreen(string handleString)
        {
            PluginLogger.LogPluginUsage("ScreenCaptureOmniParserPlugin", "CaptureScreen");

            var capBase64 = CaptureWindow(handleString);
            return await ProcessWithOmniParser(capBase64);
        }

        [Description("Used to capture the whole screen")]
        public async Task<List<ParsedContent>> CaptureWholeScreen()
        {
            PluginLogger.LogPluginUsage("ScreenCaptureOmniParserPlugin", "CaptureWholeScreen");
            var capBase64 = "";

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

            return await ProcessWithOmniParser(capBase64);
        }

        /// <summary>
        /// Process screenshot with OmniParser - automatically chooses ONNX or HTTP mode
        /// </summary>
        private async Task<List<ParsedContent>> ProcessWithOmniParser(string base64Image)
        {
            // Try ONNX mode first (native .NET, no Python required)
            if (_useOnnxMode)
            {
                try
                {
                    if (_onnxEngine == null)
                    {
                        ConfigureMode(true);
                    }

                    if (_onnxEngine != null)
                    {
                        PluginLogger.NotifyTaskStart("OmniParser", "Processing with native ONNX engine...");
                        
                        var result = _onnxEngine.ParseImageBase64(base64Image);
                        var parsedContent = ConvertOnnxResultToParsedContent(result);
                        
                        PluginLogger.NotifyTaskComplete("OmniParser");
                        PluginLogger.LogInfo("ScreenCaptureOmniParserPlugin", "ProcessWithOmniParser", 
                            $"✓ Found {parsedContent.Count} UI elements (ONNX mode)");
                        
                        return parsedContent;
                    }
                }
                catch (Exception ex)
                {
                    PluginLogger.LogError("ScreenCaptureOmniParserPlugin", "ProcessWithOmniParser", 
                        $"ONNX mode failed: {ex.Message}. Falling back to HTTP server mode.");
                    _useOnnxMode = false;
                }
            }

            // Fall back to HTTP server mode
            return await ProcessWithHttpServer(base64Image);
        }

        /// <summary>
        /// Process with Python HTTP server (legacy mode)
        /// </summary>
        private async Task<List<ParsedContent>> ProcessWithHttpServer(string base64Image)
        {
            PluginLogger.NotifyTaskStart("OmniParser", "Checking local OmniParser server...");
            bool serverReady = await LocalOmniParserManager.EnsureServerRunningAsync();
            
            if (!serverReady)
            {
                PluginLogger.LogError("ScreenCaptureOmniParserPlugin", "ProcessWithHttpServer", 
                    "Failed to start local OmniParser server. Check installation at T:\\OmniParser");
                throw new InvalidOperationException(
                    "OmniParser server not available. Please ensure OmniParser is installed at T:\\OmniParser\n" +
                    LocalOmniParserManager.GetDiagnostics());
            }

            OmniparserResponse omniResult;
            using (HttpClient httpClient = new HttpClient())
            {
                OmniParserClient omniClient = new OmniParserClient(httpClient);
                omniResult = await omniClient.ProcessScreenshotAsync(base64Image);
            }

            PluginLogger.NotifyTaskComplete("OmniParser");
            return omniResult.ParsedContentList;
        }

        /// <summary>
        /// Convert ONNX detection result to ParsedContent format
        /// </summary>
        private List<ParsedContent> ConvertOnnxResultToParsedContent(OmniParserResult onnxResult)
        {
            var parsedContent = new List<ParsedContent>();
            int labelIndex = 1;

            foreach (var detection in onnxResult.Detections)
            {
                // Create a more descriptive label including position information
                string positionDesc = $"at ({(int)detection.BoundingBox.X},{(int)detection.BoundingBox.Y})";
                string contentLabel = detection.Caption;
                
                // If no OCR text was extracted, create a descriptive label
                if (string.IsNullOrWhiteSpace(contentLabel))
                {
                    contentLabel = $"UI Element #{labelIndex} {positionDesc} [size: {(int)detection.BoundingBox.Width}x{(int)detection.BoundingBox.Height}]";
                }

                parsedContent.Add(new ParsedContent
                {
                    Type = detection.ElementType ?? "ui_element",
                    BBox = new double[] 
                    { 
                        detection.BoundingBox.Left,
                        detection.BoundingBox.Top,
                        detection.BoundingBox.Right,
                        detection.BoundingBox.Bottom
                    },
                    Content = contentLabel,
                    Interactivity = true,
                    Source = "onnx"
                });
                labelIndex++;
            }

            return parsedContent;
        }

        public string CaptureWindow(string handleString)
        {
            PluginLogger.LogPluginUsage("ScreenCaptureOmniParserPlugin", "CaptureWindow");
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
