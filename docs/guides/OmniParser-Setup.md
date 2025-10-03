# OmniParser Quick Start Guide 🚀

## TL;DR - Just Use It! ✨

**OmniParser is now fully automatic!** No server management needed.

```
You: "Capture and parse the screen"
FlowVision: [Auto-starts server if needed]
FlowVision: [Returns parsed UI elements]
```

**That's it!** 🎊

---

## What Changed

### Before ❌
```bash
# Terminal 1
cd T:\OmniParser\omnitool\omniparserserver
python -m omniparserserver --som_model_path ...
[Keep this terminal open forever]

# Terminal 2 - FlowVision
[Configure server URL]
[Use FlowVision]
```

### After ✅
```bash
# Just use FlowVision - it handles everything!
[Launch FlowVision]
[Use screen capture]
[Done!]
```

---

## First Time Setup

### One-Time: Install OmniParser

**1. Clone OmniParser (if not already installed):**
```bash
cd T:\
git clone https://github.com/microsoft/OmniParser.git
```

**2. Install Python Dependencies:**
```bash
cd T:\OmniParser
pip install -r requirements.txt
```

**3. Download Model Weights:**
```bash
huggingface-cli download microsoft/OmniParser-v2.0 --local-dir weights
```

**Done!** ✓

### Verify Installation

Check that these exist:
- `T:\OmniParser\omnitool\omniparserserver\omniparserserver.py` ✓
- `T:\OmniParser\weights\icon_detect\model.pt` ✓
- `T:\OmniParser\weights\icon_caption_florence\` ✓

---

## Usage

### Basic Usage (Most Common)

**Just use screen capture - FlowVision does the rest!**

```
You: "Capture the current screen and tell me what you see"
```

**First Time:**
- Takes ~15-20 seconds (server startup + model loading)
- Shows status: "Starting local OmniParser server..."

**After First Time:**
- Takes ~2-3 seconds (server already running)
- Instant processing!

### What Happens Automatically

```
1. You request screen capture
   ↓
2. FlowVision checks: Server running?
   ↓ No
3. FlowVision starts Python server
   ↓
4. Server loads models (~15 seconds)
   ↓
5. FlowVision verifies server ready
   ↓
6. Processes your screenshot
   ↓
7. Returns parsed UI elements
   ↓
8. Server stays running for next time!
```

---

## Performance

### Timing

**First Request (Cold Start):**
```
Server startup:  ~15 seconds
Processing:      ~3 seconds
──────────────────────────────
Total:          ~18 seconds
```

**Subsequent Requests:**
```
Processing:      ~3 seconds  ✓
```

**With GPU (if configured):**
```
Processing:      ~1 second   ✓✓
```

### Making It Faster

**Option 1: Keep FlowVision Running**
- Server stays active
- All captures are fast (~3 seconds)

**Option 2: Use GPU**
1. Edit `LocalOmniParserManager.cs`
2. Change `--device cpu` to `--device cuda`
3. Rebuild FlowVision
4. Result: ~1 second per screenshot!

---

## Troubleshooting

### "OmniParser server not available"

**Check installation:**
```bash
# Verify these exist:
dir T:\OmniParser\omnitool\omniparserserver\omniparserserver.py
dir T:\OmniParser\weights\icon_detect\model.pt
dir T:\OmniParser\weights\icon_caption_florence
```

**Check Python:**
```bash
python --version  # Should be 3.12+
pip list | findstr torch  # Should show PyTorch
```

### "Server starting but timing out"

**First time takes longer** - model loading can take 20-30 seconds.
- Wait patiently
- Check FlowVision logs for "OmniParser-Server" messages
- Verify models downloaded correctly

### "Port 8080 already in use"

**Option 1: Stop other service**
```bash
netstat -ano | findstr :8080
taskkill /PID <pid> /F
```

**Option 2: Use different port**
```csharp
// In your code
LocalOmniParserManager.Configure(port: 8081);
```

### Get Detailed Diagnostics

```csharp
string info = LocalOmniParserManager.GetDiagnostics();
Console.WriteLine(info);
```

Shows:
- Installation path
- Server script status
- Weights folder status
- Server running status
- Process status

---

## Advanced Configuration

### Custom OmniParser Location

**If not at T:\OmniParser:**
```csharp
LocalOmniParserManager.Configure(
    omniParserPath: @"C:\MyPath\OmniParser"
);
```

### Custom Python Executable

**If using specific Python:**
```csharp
LocalOmniParserManager.Configure(
    pythonExe: @"C:\Python312\python.exe"
);
```

### Using Conda Environment

**If using conda:**
```csharp
LocalOmniParserManager.Configure(
    pythonExe: @"C:\Users\YourName\miniconda3\envs\omni\python.exe"
);
```

### Custom Port

**If port conflict:**
```csharp
LocalOmniParserManager.Configure(
    port: 8081
);
```

---

## API Reference

### Check Server Status

```csharp
bool isRunning = await LocalOmniParserManager.IsServerRunningAsync();
if (isRunning) {
    Console.WriteLine("Server is ready!");
}
```

### Ensure Server Running

```csharp
bool started = await LocalOmniParserManager.EnsureServerRunningAsync();
if (started) {
    // Server is now running, proceed with capture
}
```

### Stop Server

```csharp
LocalOmniParserManager.StopServer();
// Server stopped (will auto-restart on next capture)
```

### Get Diagnostics

```csharp
string diagnostics = LocalOmniParserManager.GetDiagnostics();
Console.WriteLine(diagnostics);
```

Output:
```
OmniParser Diagnostics:
  Installation Path: T:\OmniParser
  Server Script: ✓ Found
  Weights Folder: ✓ Found
  Python Executable: python
  Server URL: http://127.0.0.1:8080
  Server Running: ✓ Yes
  Process Active: ✓ Yes
```

---

## Tips & Tricks

### 1. First Use is Slow
- Normal! Server startup + model loading
- Takes ~15-20 seconds first time
- Subsequent captures are fast (~2-3 seconds)

### 2. Keep Server Running
- Server stays active between captures
- No need to restart
- Much faster for multiple captures

### 3. Check Logs
- FlowVision logs show server output
- Look for "OmniParser-Server" entries
- Helps debug issues

### 4. GPU Acceleration
- Edit LocalOmniParserManager.cs
- Change `--device cpu` to `--device cuda`
- Requires CUDA-capable GPU
- ~1 second per screenshot!

### 5. Batch Processing
- Server handles multiple requests
- Process many screenshots efficiently
- No per-request overhead

---

## Common Workflows

### Single Screenshot

```
You: "Capture and analyze the screen"
FlowVision: [Ensures server running]
FlowVision: [Captures and processes]
FlowVision: "I see: [UI elements]"
```

### Multiple Screenshots

```
You: "Capture the screen"
FlowVision: [First: ~15s, starts server]

You: "Capture again"
FlowVision: [Fast: ~3s, server running]

You: "And again"
FlowVision: [Fast: ~3s, server running]
```

### Different Applications

```
You: "Capture Chrome window"
FlowVision: [Processes Chrome UI]

You: "Now capture VS Code"
FlowVision: [Processes VS Code UI]

You: "Back to Chrome"
FlowVision: [Processes Chrome UI]
```

All fast after first one!

---

## Status Messages

### You'll See:

**Starting:**
```
"Checking local OmniParser server..."
"Starting local OmniParser server..."
"Server started successfully in 15 seconds"
```

**Running:**
```
"Server already running"
"Processing screenshot..."
"Done!"
```

**Errors:**
```
"Failed to start server: [reason]"
"OmniParser not found at T:\OmniParser"
"Check installation and try again"
```

---

## FAQ

### Q: Do I need to start the server manually?
**A:** No! FlowVision starts it automatically.

### Q: Will it restart if it crashes?
**A:** Yes! Next capture will restart it automatically.

### Q: Can I use it offline?
**A:** Yes! Everything runs locally.

### Q: Does it use GPU?
**A:** CPU by default. Edit code for GPU.

### Q: Can I change the port?
**A:** Yes! Use `LocalOmniParserManager.Configure(port: 8081)`

### Q: What if OmniParser is elsewhere?
**A:** Configure path: `LocalOmniParserManager.Configure(omniParserPath: "C:\MyPath")`

### Q: How do I know if it's working?
**A:** Check diagnostics: `LocalOmniParserManager.GetDiagnostics()`

---

## Summary

### What You Get ✨

✅ **Automatic** - Server starts when needed
✅ **Fast** - ~3 seconds after first request
✅ **Reliable** - Auto-restart on failure
✅ **Local** - No cloud, works offline
✅ **Private** - All processing on your machine
✅ **Simple** - No manual management

### What You Need 📋

✅ OmniParser at `T:\OmniParser`
✅ Python 3.12+ with dependencies
✅ Model weights downloaded
✅ Port 8080 available (or configure different)

### How to Use 🚀

1. Use FlowVision screen capture
2. That's it! ✓

**It just works!** 🎊

---

**Status:** ✅ Ready to use
**Configuration:** ❌ None needed (optional customization available)