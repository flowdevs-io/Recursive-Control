# ONNX OmniParser Quick Start Guide

## 🚀 Getting Started in 3 Steps

### Step 1: Verify ONNX Model Exists

Check if the ONNX model file exists:

```
T:\OmniParser\weights\icon_detect\model.onnx
```

If it doesn't exist, convert the PyTorch model:

```bash
cd T:\OmniParser\weights\icon_detect
# Using Python with ultralytics installed
python -c "from ultralytics import YOLO; YOLO('model.pt').export(format='onnx', imgsz=640, simplify=True, opset=12)"
```

### Step 2: Build and Run

```bash
cd T:\Recursive-Control
msbuild FlowVision.sln /t:Build /p:Configuration=Debug
```

Or use Visual Studio: **Build > Build Solution** (Ctrl+Shift+B)

### Step 3: Use OmniParser

The application now automatically uses ONNX mode - no Python server required!

```csharp
// In your code - it just works!
var plugin = new ScreenCaptureOmniParserPlugin();
var results = await plugin.CaptureWholeScreen();

// Results contain detected UI elements with bounding boxes
foreach (var element in results)
{
    Console.WriteLine($"Found {element.Type} at [{element.BBox[0]}, {element.BBox[1]}]");
}
```

## 🎯 Key Benefits

### Before (Python Server Mode)
- ❌ Requires Python installation
- ❌ 15-30 second startup time
- ❌ Needs to manage server process
- ❌ HTTP overhead for each request

### Now (ONNX Mode)  
- ✅ **Pure .NET** - no Python required!
- ✅ **< 1 second** startup time
- ✅ Direct in-process inference
- ✅ Lower memory usage

## 💡 Example Usage

### Basic Usage
```csharp
// Capture and parse the whole screen
var plugin = new ScreenCaptureOmniParserPlugin();
var elements = await plugin.CaptureWholeScreen();

Console.WriteLine($"Detected {elements.Count} UI elements");
```

### Advanced Usage
```csharp
// Use ONNX engine directly for more control
using var engine = new OnnxOmniParserEngine(
    modelPath: @"T:\OmniParser\weights\icon_detect\model.onnx",
    confidenceThreshold: 0.05f
);

// Load an image
using var bitmap = new Bitmap("screenshot.png");

// Detect UI elements
var result = engine.ParseImage(bitmap);

// Visualize detections
using var annotated = engine.DrawDetections(bitmap, result);
annotated.Save("annotated.png");

Console.WriteLine($"Found {result.Detections.Count} elements:");
foreach (var detection in result.Detections)
{
    Console.WriteLine($"  - {detection.ElementType} at " +
        $"({detection.BoundingBox.X}, {detection.BoundingBox.Y}) " +
        $"with confidence {detection.Confidence:F2}");
}
```

### Window-Specific Capture
```csharp
var plugin = new ScreenCaptureOmniParserPlugin();

// Get window handle (example)
IntPtr windowHandle = FindWindow("Notepad");

// Capture and parse specific window
var elements = await plugin.CaptureScreen(windowHandle.ToString());
```

## ⚙️ Configuration

### Switch Between Modes

```csharp
// Use ONNX mode (default)
ScreenCaptureOmniParserPlugin.ConfigureMode(useOnnx: true);

// Fall back to HTTP server mode if needed
ScreenCaptureOmniParserPlugin.ConfigureMode(useOnnx: false);
```

### Adjust Detection Sensitivity

```csharp
// Lower threshold = more detections (but more false positives)
var engine = new OnnxOmniParserEngine(confidenceThreshold: 0.03f);

// Higher threshold = fewer, more confident detections
var engine = new OnnxOmniParserEngine(confidenceThreshold: 0.15f);
```

## 📊 Performance Tips

1. **First Run**: Initial model load takes ~1 second
2. **Subsequent Runs**: Inference is 2-5 seconds per image
3. **Memory**: Keep ~500 MB RAM available
4. **GPU**: Enable GPU mode for 5-10x faster inference (requires CUDA)

## 🐛 Troubleshooting

### Issue: "ONNX model not found"

**Solution**: Convert the model:
```bash
pip install ultralytics
python -c "from ultralytics import YOLO; YOLO('T:/OmniParser/weights/icon_detect/model.pt').export(format='onnx')"
```

### Issue: "Unable to load DLL 'onnxruntime'"

**Solution**: Rebuild the project - native DLLs are automatically copied to the output directory.

### Issue: Poor detection results

**Solutions**:
- Lower confidence threshold
- Ensure good screenshot quality
- Check for adequate contrast

## 📚 Additional Resources

- Full Documentation: [ONNX_OMNIPARSER_INTEGRATION.md](ONNX_OMNIPARSER_INTEGRATION.md)
- OmniParser GitHub: https://github.com/microsoft/OmniParser
- ONNX Runtime: https://onnxruntime.ai/

## ✨ What's Next?

The integration supports:
- ✅ UI element detection with YOLO
- ⏳ Caption generation (planned)
- ⏳ OCR integration (planned)
- ⏳ Custom model training (planned)

---

**Ready to go!** The application now runs OmniParser natively in .NET with no Python dependencies. Just build and run! 🎉
