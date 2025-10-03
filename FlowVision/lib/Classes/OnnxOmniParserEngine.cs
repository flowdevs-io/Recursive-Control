using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Numerics.Tensors;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Native .NET implementation of OmniParser using ONNX Runtime
    /// No Python server required - runs YOLO model directly in .NET
    /// </summary>
    public class OnnxOmniParserEngine : IDisposable
    {
        private InferenceSession _session;
        private readonly string _modelPath;
        private readonly float _confidenceThreshold;
        private readonly int _inputSize;
        private bool _disposed;

        public OnnxOmniParserEngine(string modelPath = null, float confidenceThreshold = 0.05f, int inputSize = 640)
        {
            _modelPath = modelPath ?? Path.Combine(@"T:\OmniParser\weights\icon_detect", "model.onnx");
            _confidenceThreshold = confidenceThreshold;
            _inputSize = inputSize;

            if (!File.Exists(_modelPath))
            {
                throw new FileNotFoundException($"ONNX model not found at: {_modelPath}");
            }

            InitializeModel();
        }

        private void InitializeModel()
        {
            try
            {
                PluginLogger.LogInfo("OnnxOmniParser", "Initialize", $"Loading ONNX model from: {_modelPath}");
                
                var sessionOptions = new SessionOptions();
                sessionOptions.EnableCpuMemArena = true;
                sessionOptions.EnableMemoryPattern = true;
                sessionOptions.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;
                
                // Use CPU execution provider by default (can be changed to GPU if available)
                // sessionOptions.AppendExecutionProvider_CUDA(0); // Uncomment for GPU support
                
                _session = new InferenceSession(_modelPath, sessionOptions);
                
                PluginLogger.LogInfo("OnnxOmniParser", "Initialize", "ONNX model loaded successfully");
            }
            catch (Exception ex)
            {
                PluginLogger.LogError("OnnxOmniParser", "Initialize", $"Failed to load model: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Parse an image and detect UI elements with OCR
        /// </summary>
        public OmniParserResult ParseImage(Bitmap image)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(OnnxOmniParserEngine));

            try
            {
                PluginLogger.LogInfo("OnnxOmniParser", "ParseImage", $"Processing image {image.Width}x{image.Height}");

                // Preprocess image
                var inputTensor = PreprocessImage(image);

                // Run inference
                var outputs = RunInference(inputTensor);

                // Post-process outputs
                var result = PostProcessOutputs(outputs, image.Width, image.Height);

                PluginLogger.LogInfo("OnnxOmniParser", "ParseImage", $"Detected {result.Detections.Count} UI elements");

                // Extract text from detected regions using OCR
                ExtractTextFromDetections(image, result).Wait();

                return result;
            }
            catch (Exception ex)
            {
                PluginLogger.LogError("OnnxOmniParser", "ParseImage", $"Error parsing image: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Parse an image from base64 string
        /// </summary>
        public OmniParserResult ParseImageBase64(string base64Image)
        {
            byte[] imageBytes = Convert.FromBase64String(base64Image);
            using (var ms = new MemoryStream(imageBytes))
            using (var bitmap = new Bitmap(ms))
            {
                return ParseImage(bitmap);
            }
        }

        /// <summary>
        /// Extract text from detected UI elements using OCR
        /// </summary>
        private async Task ExtractTextFromDetections(Bitmap sourceImage, OmniParserResult result)
        {
            if (!OcrHelper.IsAvailable)
            {
                PluginLogger.LogInfo("OnnxOmniParser", "ExtractTextFromDetections", 
                    "OCR not available - text extraction skipped");
                return;
            }

            PluginLogger.LogInfo("OnnxOmniParser", "ExtractTextFromDetections", 
                $"Extracting text from {result.Detections.Count} detected elements");

            int processedCount = 0;
            int textFoundCount = 0;

            foreach (var detection in result.Detections)
            {
                try
                {
                    string text = await OcrHelper.ExtractTextFromRegionAsync(sourceImage, detection.BoundingBox);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        detection.Caption = text;
                        textFoundCount++;
                    }
                    processedCount++;
                }
                catch (Exception ex)
                {
                    PluginLogger.LogError("OnnxOmniParser", "ExtractTextFromDetections", 
                        $"Failed to extract text from detection: {ex.Message}");
                }
            }

            PluginLogger.LogInfo("OnnxOmniParser", "ExtractTextFromDetections", 
                $"OCR complete: {textFoundCount} elements with text out of {processedCount} processed");
        }

        /// <summary>
        /// Preprocess image for YOLO model input
        /// </summary>
        private DenseTensor<float> PreprocessImage(Bitmap image)
        {
            // Resize image to model input size while maintaining aspect ratio
            var resized = ResizeImage(image, _inputSize, _inputSize);
            
            // Create tensor with shape [1, 3, height, width] (NCHW format)
            var tensor = new DenseTensor<float>(new[] { 1, 3, _inputSize, _inputSize });
            
            // Convert bitmap to tensor (normalize to 0-1 range)
            for (int y = 0; y < _inputSize; y++)
            {
                for (int x = 0; x < _inputSize; x++)
                {
                    Color pixel = resized.GetPixel(x, y);
                    
                    // YOLO expects RGB normalized to [0, 1]
                    tensor[0, 0, y, x] = pixel.R / 255.0f; // R channel
                    tensor[0, 1, y, x] = pixel.G / 255.0f; // G channel
                    tensor[0, 2, y, x] = pixel.B / 255.0f; // B channel
                }
            }
            
            resized.Dispose();
            return tensor;
        }

        /// <summary>
        /// Run ONNX inference
        /// </summary>
        private IDisposableReadOnlyCollection<DisposableNamedOnnxValue> RunInference(DenseTensor<float> inputTensor)
        {
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(_session.InputMetadata.Keys.First(), inputTensor)
            };

            return _session.Run(inputs);
        }

        /// <summary>
        /// Post-process YOLO outputs
        /// </summary>
        private OmniParserResult PostProcessOutputs(IDisposableReadOnlyCollection<DisposableNamedOnnxValue> outputs, 
            int originalWidth, int originalHeight)
        {
            var result = new OmniParserResult
            {
                Detections = new List<UIElementDetection>()
            };

            // YOLO output format: [1, 5, 8400] where 5 = [x, y, w, h, confidence]
            var output = outputs.First().AsTensor<float>();
            
            // Get dimensions
            int numDetections = output.Dimensions[2]; // 8400
            
            // Scale factors for converting back to original image size
            float scaleX = (float)originalWidth / _inputSize;
            float scaleY = (float)originalHeight / _inputSize;

            // Parse detections
            for (int i = 0; i < numDetections; i++)
            {
                float confidence = output[0, 4, i]; // Confidence is at index 4
                
                if (confidence < _confidenceThreshold)
                    continue;

                // Extract box coordinates (center x, center y, width, height)
                float centerX = output[0, 0, i];
                float centerY = output[0, 1, i];
                float width = output[0, 2, i];
                float height = output[0, 3, i];

                // Convert from center coordinates to corner coordinates
                float x1 = (centerX - width / 2) * scaleX;
                float y1 = (centerY - height / 2) * scaleY;
                float x2 = (centerX + width / 2) * scaleX;
                float y2 = (centerY + height / 2) * scaleY;

                // Clamp to image bounds
                x1 = Math.Max(0, Math.Min(x1, originalWidth));
                y1 = Math.Max(0, Math.Min(y1, originalHeight));
                x2 = Math.Max(0, Math.Min(x2, originalWidth));
                y2 = Math.Max(0, Math.Min(y2, originalHeight));

                result.Detections.Add(new UIElementDetection
                {
                    BoundingBox = new RectangleF(x1, y1, x2 - x1, y2 - y1),
                    Confidence = confidence,
                    Label = $"element_{i}",
                    ElementType = "ui_element"
                });
            }

            // Apply Non-Maximum Suppression (NMS) to remove overlapping boxes
            result.Detections = ApplyNMS(result.Detections, 0.45f);

            return result;
        }

        /// <summary>
        /// Apply Non-Maximum Suppression to remove overlapping detections
        /// </summary>
        private List<UIElementDetection> ApplyNMS(List<UIElementDetection> detections, float iouThreshold)
        {
            if (detections.Count == 0)
                return detections;

            // Sort by confidence descending
            var sorted = detections.OrderByDescending(d => d.Confidence).ToList();
            var result = new List<UIElementDetection>();

            while (sorted.Count > 0)
            {
                var best = sorted[0];
                result.Add(best);
                sorted.RemoveAt(0);

                // Remove overlapping boxes
                sorted = sorted.Where(d => CalculateIoU(best.BoundingBox, d.BoundingBox) < iouThreshold).ToList();
            }

            return result;
        }

        /// <summary>
        /// Calculate Intersection over Union (IoU) between two boxes
        /// </summary>
        private float CalculateIoU(RectangleF box1, RectangleF box2)
        {
            float x1 = Math.Max(box1.Left, box2.Left);
            float y1 = Math.Max(box1.Top, box2.Top);
            float x2 = Math.Min(box1.Right, box2.Right);
            float y2 = Math.Min(box1.Bottom, box2.Bottom);

            float intersectionArea = Math.Max(0, x2 - x1) * Math.Max(0, y2 - y1);
            float box1Area = box1.Width * box1.Height;
            float box2Area = box2.Width * box2.Height;
            float unionArea = box1Area + box2Area - intersectionArea;

            return unionArea > 0 ? intersectionArea / unionArea : 0;
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

        /// <summary>
        /// Draw bounding boxes on image for visualization
        /// </summary>
        public Bitmap DrawDetections(Bitmap image, OmniParserResult result)
        {
            var output = new Bitmap(image);
            using (var graphics = Graphics.FromImage(output))
            {
                var pen = new Pen(Color.Red, 2);
                var brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0));
                var font = new Font("Arial", 10);
                var textBrush = new SolidBrush(Color.White);

                int labelIndex = 1;
                foreach (var detection in result.Detections)
                {
                    // Draw bounding box
                    graphics.DrawRectangle(pen, detection.BoundingBox.X, detection.BoundingBox.Y,
                        detection.BoundingBox.Width, detection.BoundingBox.Height);

                    // Draw label
                    string label = $"{labelIndex}: {detection.Confidence:F2}";
                    var labelSize = graphics.MeasureString(label, font);
                    var labelRect = new RectangleF(
                        detection.BoundingBox.X,
                        detection.BoundingBox.Y - labelSize.Height,
                        labelSize.Width,
                        labelSize.Height);

                    graphics.FillRectangle(brush, labelRect);
                    graphics.DrawString(label, font, textBrush, labelRect.Location);

                    labelIndex++;
                }

                pen.Dispose();
                brush.Dispose();
                font.Dispose();
                textBrush.Dispose();
            }

            return output;
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
    /// Result of OmniParser detection
    /// </summary>
    public class OmniParserResult
    {
        public List<UIElementDetection> Detections { get; set; }
    }

    /// <summary>
    /// Single UI element detection
    /// </summary>
    public class UIElementDetection
    {
        public RectangleF BoundingBox { get; set; }
        public float Confidence { get; set; }
        public string Label { get; set; }
        public string ElementType { get; set; }
        public string Caption { get; set; }
    }
}
