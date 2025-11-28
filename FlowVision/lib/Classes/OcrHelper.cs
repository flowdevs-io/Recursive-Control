using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Helper class for performing OCR on images using Tesseract
    /// </summary>
    public static class OcrHelper
    {
        private static bool _initialized = false;
        private static bool _isAvailable = false;
        private static TesseractEngine _engine;
        private static readonly object _lock = new object();
        private static string _tessdataPath;

        static OcrHelper()
        {
            Initialize();
        }

        private static void Initialize()
        {
            if (_initialized)
                return;

            lock (_lock)
            {
                if (_initialized)
                    return;

                _initialized = true;

                try
                {
                    // Try to find tessdata directory
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    _tessdataPath = Path.Combine(baseDir, "tessdata");

                    if (!Directory.Exists(_tessdataPath))
                    {
                        PluginLogger.LogError("OcrHelper", "Initialize", 
                            $"tessdata directory not found at: {_tessdataPath}");
                        _isAvailable = false;
                        return;
                    }

                    string engDataFile = Path.Combine(_tessdataPath, "eng.traineddata");
                    if (!File.Exists(engDataFile))
                    {
                        PluginLogger.LogError("OcrHelper", "Initialize", 
                            $"English language data not found at: {engDataFile}");
                        _isAvailable = false;
                        return;
                    }

                    // Initialize Tesseract engine
                    _engine = new TesseractEngine(_tessdataPath, "eng", EngineMode.Default);
                    
                    // Configure for better UI text recognition
                    _engine.SetVariable("tessedit_char_whitelist", 
                        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 .-_:@/\\()[]{}!?&+=#$%");
                    _engine.SetVariable("preserve_interword_spaces", "1");
                    
                    _isAvailable = true;
                    PluginLogger.LogInfo("OcrHelper", "Initialize", 
                        "✓ Tesseract OCR initialized successfully. Text extraction is now enabled.");
                }
                catch (Exception ex)
                {
                    PluginLogger.LogError("OcrHelper", "Initialize", 
                        $"Failed to initialize Tesseract: {ex.Message}");
                    _isAvailable = false;
                    _engine?.Dispose();
                    _engine = null;
                }
            }
        }

        /// <summary>
        /// Search for specific text in an image and return its bounding box.
        /// Returns the first match found.
        /// </summary>
        public static async Task<Rectangle?> FindTextLocationAsync(Bitmap image, string searchText)
        {
            if (!_isAvailable || _engine == null || string.IsNullOrWhiteSpace(searchText))
                return null;

            return await Task.Run(() =>
            {
                try
                {
                    lock (_lock)
                    {
                        using (var pix = PixConverter.ToPix(image))
                        using (var page = _engine.Process(pix))
                        using (var iter = page.GetIterator())
                        {
                            iter.Begin();
                            do
                            {
                                // Get text at current iterator level (Word)
                                string currentText = iter.GetText(PageIteratorLevel.Word)?.Trim();
                                
                                // Simple case-insensitive match
                                if (!string.IsNullOrWhiteSpace(currentText) && 
                                    currentText.Equals(searchText, StringComparison.OrdinalIgnoreCase))
                                {
                                    // Found exact match! Get bounding box.
                                    if (iter.TryGetBoundingBox(PageIteratorLevel.Word, out var rect))
                                    {
                                        return (Rectangle?)new Rectangle(rect.X1, rect.Y1, rect.Width, rect.Height);
                                    }
                                }
                            } while (iter.Next(PageIteratorLevel.Word));
                        }
                    }
                    return (Rectangle?)null;
                }
                catch (Exception ex)
                {
                    PluginLogger.LogError("OcrHelper", "FindTextLocationAsync", $"OCR search failed: {ex.Message}");
                    return (Rectangle?)null;
                }
            });
        }

        /// <summary>
        /// Extract text from a bitmap image
        /// </summary>
        public static async Task<string> ExtractTextAsync(Bitmap image)
        {
            if (!_isAvailable || _engine == null)
                return string.Empty;

            return await Task.Run(() =>
            {
                try
                {
                    lock (_lock)
                    {
                        using (var pix = PixConverter.ToPix(image))
                        using (var page = _engine.Process(pix))
                        {
                            string text = page.GetText()?.Trim();
                            return text ?? string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    PluginLogger.LogError("OcrHelper", "ExtractTextAsync", 
                        $"OCR failed: {ex.Message}");
                    return string.Empty;
                }
            });
        }

        /// <summary>
        /// Extract text from a specific region of an image
        /// </summary>
        public static async Task<string> ExtractTextFromRegionAsync(Bitmap sourceImage, RectangleF region)
        {
            if (!_isAvailable || _engine == null)
                return string.Empty;

            return await Task.Run(() =>
            {
                try
                {
                    // Validate and adjust region bounds
                    int x = Math.Max(0, (int)region.X);
                    int y = Math.Max(0, (int)region.Y);
                    int width = Math.Min((int)region.Width, sourceImage.Width - x);
                    int height = Math.Min((int)region.Height, sourceImage.Height - y);

                    // Skip very small regions (likely not text)
                    if (width < 10 || height < 10)
                        return string.Empty;

                    // Crop the region
                    Rectangle cropRect = new Rectangle(x, y, width, height);
                    using (Bitmap croppedImage = sourceImage.Clone(cropRect, sourceImage.PixelFormat))
                    {
                        lock (_lock)
                        {
                            using (var pix = PixConverter.ToPix(croppedImage))
                            using (var page = _engine.Process(pix))
                            {
                                string text = page.GetText()?.Trim();
                                
                                // Only return if we found meaningful text (more than just whitespace)
                                if (!string.IsNullOrWhiteSpace(text) && text.Length > 1)
                                {
                                    return text;
                                }
                                return string.Empty;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    PluginLogger.LogError("OcrHelper", "ExtractTextFromRegionAsync", 
                        $"OCR failed for region: {ex.Message}");
                    return string.Empty;
                }
            });
        }

        /// <summary>
        /// Check if OCR is available
        /// </summary>
        public static bool IsAvailable => _isAvailable;

        /// <summary>
        /// Cleanup resources
        /// </summary>
        public static void Dispose()
        {
            lock (_lock)
            {
                if (_engine != null)
                {
                    _engine.Dispose();
                    _engine = null;
                    _isAvailable = false;
                }
            }
        }
    }
}
