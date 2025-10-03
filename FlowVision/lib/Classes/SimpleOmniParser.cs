using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// KISS Implementation of OmniParser - Simple, Fast, Portable
    /// Uses embedded ONNX model for UI element detection
    /// No Python server, no external dependencies, just pure .NET
    /// </summary>
    public class SimpleOmniParser : IDisposable
    {
        private static SimpleOmniParser _instance;
        private static readonly object _lock = new object();
        private InferenceSession _session;
        private bool _disposed;

        // YOLO model configuration
        private const int INPUT_SIZE = 640;
        private const float CONFIDENCE_THRESHOLD = 0.05f;
        private const float NMS_THRESHOLD = 0.45f;

        /// <summary>
        /// Get singleton instance (lazy initialization)
        /// </summary>
        public static SimpleOmniParser Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SimpleOmniParser();
                        }
                    }
                }
                return _instance;
            }
        }

        private SimpleOmniParser()
        {
            InitializeModel();
        }

        /// <summary>
        /// Initialize ONNX model from embedded resource or file
        /// </summary>
        private void InitializeModel()
        {
            try
            {
                PluginLogger.LogInfo("SimpleOmniParser", "Initialize", "Loading ONNX model...");

                // Try to load from embedded resource first
                byte[] modelBytes = LoadModelFromResource();
                
                if (modelBytes == null)
                {
                    // Fallback: try to load from models directory
                    string modelsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models");
                    string modelPath = Path.Combine(modelsDir, "icon_detect.onnx");
                    
                    if (File.Exists(modelPath))
                    {
                        modelBytes = File.ReadAllBytes(modelPath);
                        PluginLogger.LogInfo("SimpleOmniParser", "Initialize", $"Loaded model from: {modelPath}");
                    }
                    else
                    {
                        throw new FileNotFoundException(
                            "OmniParser model not found. Please download icon_detect/model.onnx from " +
                            "https://huggingface.co/microsoft/OmniParser-v2.0/tree/main " +
                            $"and place it at: {modelPath}");
                    }
                }
                else
                {
                    PluginLogger.LogInfo("SimpleOmniParser", "Initialize", "Loaded model from embedded resource");
                }

                // Create session
                var sessionOptions = new SessionOptions
                {
                    EnableCpuMemArena = true,
                    EnableMemoryPattern = true,
                    GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL
                };

                _session = new InferenceSession(modelBytes, sessionOptions);
                PluginLogger.LogInfo("SimpleOmniParser", "Initialize", "✓ Model loaded successfully");
            }
            catch (Exception ex)
            {
                PluginLogger.LogError("SimpleOmniParser", "Initialize", $"Failed to load model: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Load model from embedded resource
        /// </summary>
        private byte[] LoadModelFromResource()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith("icon_detect.onnx"));

                if (resourceName != null)
                {
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLogger.LogInfo("SimpleOmniParser", "LoadModelFromResource", 
                    $"Could not load embedded resource: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Parse screenshot and detect UI elements
        /// </summary>
        public List<UIElement> ParseScreenshot(Bitmap screenshot)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SimpleOmniParser));

            if (screenshot == null)
                throw new ArgumentNullException(nameof(screenshot));

            try
            {
                // Preprocess image
                var inputTensor = PreprocessImage(screenshot);

                // Run inference
                var outputs = RunInference(inputTensor);

                // Post-process and return results
                var elements = PostProcess(outputs, screenshot.Width, screenshot.Height);

                return elements;
            }
            catch (Exception ex)
            {
                PluginLogger.LogError("SimpleOmniParser", "ParseScreenshot", 
                    $"Error parsing screenshot: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Preprocess image for YOLO model
        /// </summary>
        private DenseTensor<float> PreprocessImage(Bitmap image)
        {
            // Resize to 640x640 maintaining aspect ratio
            using (var resized = ResizeImage(image, INPUT_SIZE, INPUT_SIZE))
            {
                // Create tensor [1, 3, 640, 640]
                var tensor = new DenseTensor<float>(new[] { 1, 3, INPUT_SIZE, INPUT_SIZE });

                // Convert to RGB and normalize to [0, 1]
                for (int y = 0; y < INPUT_SIZE; y++)
                {
                    for (int x = 0; x < INPUT_SIZE; x++)
                    {
                        Color pixel = resized.GetPixel(x, y);
                        tensor[0, 0, y, x] = pixel.R / 255.0f;
                        tensor[0, 1, y, x] = pixel.G / 255.0f;
                        tensor[0, 2, y, x] = pixel.B / 255.0f;
                    }
                }

                return tensor;
            }
        }

        /// <summary>
        /// Run ONNX inference
        /// </summary>
        private IDisposableReadOnlyCollection<DisposableNamedOnnxValue> RunInference(DenseTensor<float> input)
        {
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(_session.InputMetadata.Keys.First(), input)
            };

            return _session.Run(inputs);
        }

        /// <summary>
        /// Post-process YOLO outputs
        /// </summary>
        private List<UIElement> PostProcess(IDisposableReadOnlyCollection<DisposableNamedOnnxValue> outputs, 
            int originalWidth, int originalHeight)
        {
            var detections = new List<UIElement>();

            // YOLO output: [1, 5, N] where N is number of detections
            var output = outputs.First().AsTensor<float>();
            int numDetections = output.Dimensions[2];

            // Scale factors
            float scaleX = (float)originalWidth / INPUT_SIZE;
            float scaleY = (float)originalHeight / INPUT_SIZE;

            // Extract detections
            for (int i = 0; i < numDetections; i++)
            {
                float confidence = output[0, 4, i];
                
                if (confidence < CONFIDENCE_THRESHOLD)
                    continue;

                // Get box coordinates (center format)
                float cx = output[0, 0, i];
                float cy = output[0, 1, i];
                float w = output[0, 2, i];
                float h = output[0, 3, i];

                // Convert to corner format and scale to original size
                float x1 = Math.Max(0, (cx - w / 2) * scaleX);
                float y1 = Math.Max(0, (cy - h / 2) * scaleY);
                float x2 = Math.Min(originalWidth, (cx + w / 2) * scaleX);
                float y2 = Math.Min(originalHeight, (cy + h / 2) * scaleY);

                detections.Add(new UIElement
                {
                    X = (int)x1,
                    Y = (int)y1,
                    Width = (int)(x2 - x1),
                    Height = (int)(y2 - y1),
                    Confidence = confidence,
                    ElementId = detections.Count + 1
                });
            }

            // Apply NMS to remove overlapping boxes
            detections = ApplyNMS(detections);

            PluginLogger.LogInfo("SimpleOmniParser", "PostProcess", 
                $"Detected {detections.Count} UI elements");

            return detections;
        }

        /// <summary>
        /// Apply Non-Maximum Suppression
        /// </summary>
        private List<UIElement> ApplyNMS(List<UIElement> detections)
        {
            if (detections.Count == 0)
                return detections;

            var sorted = detections.OrderByDescending(d => d.Confidence).ToList();
            var result = new List<UIElement>();

            while (sorted.Count > 0)
            {
                var best = sorted[0];
                result.Add(best);
                sorted.RemoveAt(0);

                // Remove overlapping boxes
                sorted = sorted.Where(d => CalculateIoU(best, d) < NMS_THRESHOLD).ToList();
            }

            // Re-assign element IDs
            for (int i = 0; i < result.Count; i++)
            {
                result[i].ElementId = i + 1;
            }

            return result;
        }

        /// <summary>
        /// Calculate Intersection over Union
        /// </summary>
        private float CalculateIoU(UIElement a, UIElement b)
        {
            float x1 = Math.Max(a.X, b.X);
            float y1 = Math.Max(a.Y, b.Y);
            float x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            float y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            float intersection = Math.Max(0, x2 - x1) * Math.Max(0, y2 - y1);
            float areaA = a.Width * a.Height;
            float areaB = b.Width * b.Height;
            float union = areaA + areaB - intersection;

            return union > 0 ? intersection / union : 0;
        }

        /// <summary>
        /// Resize image maintaining aspect ratio
        /// </summary>
        private Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _session?.Dispose();
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Simple UI element representation
    /// </summary>
    public class UIElement
    {
        public int ElementId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Confidence { get; set; }

        public Rectangle GetBounds() => new Rectangle(X, Y, Width, Height);

        public override string ToString()
        {
            return $"Element #{ElementId}: ({X},{Y}) {Width}x{Height} [{Confidence:P1}]";
        }
    }
}
