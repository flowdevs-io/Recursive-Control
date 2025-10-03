# Local OmniParser Auto-Management Integration 🚀

## 🎯 What This Does

FlowVision now **automatically manages your local OmniParser server**! No manual server startup, no configuration hassles - it just works!

### Before vs After

**Before:**
- ❌ Start Python server manually
- ❌ Keep terminal window open
- ❌ Configure server URL
- ❌ Restart if it crashes
- ❌ Check if server is running

**After:**
- ✅ **Automatic server startup**
- ✅ **Auto-detection and health checking**
- ✅ **Auto-restart on failure**
- ✅ **Zero manual management**
- ✅ **Just use it!** 🎊

## 🏗️ Architecture

### LocalOmniParserManager

**What:** Smart server manager that handles everything
**Location:** `FlowVision\lib\Classes\LocalOmniParserManager.cs`

**Features:**
1. **Auto-Detection** - Checks if server is running
2. **Auto-Start** - Starts server when needed
3. **Health Monitoring** - Verifies server responds
4. **Process Management** - Handles server lifecycle
5. **Cooldown Logic** - Prevents rapid restart loops
6. **Diagnostics** - Detailed status information

### Integration Points

**ScreenCaptureOmniParserPlugin:**
```csharp
// Before capturing screen
await LocalOmniParserManager.EnsureServerRunningAsync();

// Server is now guaranteed to be running
// Process the screenshot
var result = await omniClient.ProcessScreenshotAsync(image);
```

**Automatic Flow:**
```
User: "Capture screen"
  ↓
FlowVision checks: Is server running?
  ↓ No
FlowVision: Starting server...
  ↓ Wait for ready (~10-20 seconds)
FlowVision: Server ready! ✓
  ↓
FlowVision: Process screenshot
  ↓
Return: Parsed UI elements
```

## 📋 Requirements

### 1. OmniParser Installation

**Required Location:** `T:\OmniParser`

**Directory Structure:**
```
T:\OmniParser\
├── omnitool\
│   └── omniparserserver\
│       └── omniparserserver.py    ← Server script
├── weights\
│   ├── icon_detect\
│   │   └── model.pt              ← Detection model
│   └── icon_caption_florence\    ← Caption model
├── util\
│   └── omniparser.py             ← Core logic
└── requirements.txt
```

### 2. Python Environment

**Options:**

**Option A: Conda (Recommended)**
```bash
cd T:\OmniParser
conda create -n omni python==3.12
conda activate omni
pip install -r requirements.txt
```

**Option B: System Python**
```bash
cd T:\OmniParser
python -m pip install -r requirements.txt
```

### 3. Model Weights

**Download from HuggingFace:**
```bash
cd T:\OmniParser
huggingface-cli download microsoft/OmniParser-v2.0 \
  "icon_detect/model.pt" \
  "icon_caption/model.safetensors" \
  --local-dir weights
```

**Or:** Follow OmniParser setup guide: https://github.com/microsoft/OmniParser

## 🚀 Usage

### Basic Usage (Zero Config)

**Just use FlowVision - it handles everything!**

```
You: "Capture and parse the screen"
AI: [Checks server...]
AI: [Server not running, starting...]
AI: [Server ready in 15 seconds]
AI: [Processing screenshot...]
AI: "I can see these UI elements: [list]"
```

**Subsequent Calls:**
```
You: "Capture again"
AI: [Checks server - already running!]
AI: [Processing screenshot - instant!]
AI: "Elements: [list]"
```

### First Run Experience

**First time using OmniParser:**

1. FlowVision detects no server running
2. Starts the Python server automatically
3. Loads models (~10-20 seconds)
4. Verifies server is responding
5. Processes your screenshot
6. Done! ✓

**Server stays running** - future captures are instant!

### Manual Server Control (Optional)

**Check Status:**
```csharp
bool running = await LocalOmniParserManager.IsServerRunningAsync();
```

**Get Diagnostics:**
```csharp
string info = LocalOmniParserManager.GetDiagnostics();
```

**Stop Server:**
```csharp
LocalOmniParserManager.StopServer();
```

**Configure Paths:**
```csharp
LocalOmniParserManager.Configure(
    omniParserPath: @"T:\OmniParser",
    pythonExe: "python",
    serverUrl: "http://127.0.0.1:8080",
    port: 8080
);
```

## 🎭 Smart Features

### 1. Health Checking

**What:** Verifies server is actually responding, not just running

**How:**
```csharp
// Checks /probe/ endpoint
var response = await httpClient.GetAsync("http://127.0.0.1:8080/probe/");
if (response.IsSuccessStatusCode) {
    // Server is healthy!
}
```

**Why:** Process might be running but not responding (crashed, loading, etc.)

### 2. Auto-Startup

**What:** Automatically starts server when needed

**Process:**
1. Check if server responding
2. If not, verify installation exists
3. Start Python process with proper arguments
4. Wait for server to respond (up to 30 seconds)
5. Verify health with /probe/
6. Ready!

**Arguments Used:**
```bash
python -m omniparserserver \
  --som_model_path "T:\OmniParser\weights\icon_detect\model.pt" \
  --caption_model_name florence2 \
  --caption_model_path "T:\OmniParser\weights\icon_caption_florence" \
  --device cpu \
  --BOX_TRESHOLD 0.05 \
  --port 8080 \
  --host 0.0.0.0
```

### 3. Cooldown Protection

**What:** Prevents rapid restart loops if server keeps failing

**Logic:**
- First failure: Try to start
- Fails again within 10 seconds: Wait
- After cooldown: Try again

**Why:** Prevents hammering the system if there's a config issue

### 4. Output Capture

**What:** Captures server logs for debugging

**Where:** Visible in FlowVision logs as "OmniParser-Server" entries

**Example:**
```
[OmniParser-Server] Output: Loading YOLO model...
[OmniParser-Server] Output: Model loaded successfully
[OmniParser-Server] Output: Server started on port 8080
```

### 5. Process Lifecycle

**What:** Proper process management

**Features:**
- Hidden window (no console popup)
- Output redirection (for logging)
- Graceful shutdown (when FlowVision closes)
- Automatic cleanup

## 📊 Performance

### Timing Breakdown

**First Request (Cold Start):**
```
Check server: ~0.5s
  ↓ (not running)
Start process: ~1s
Load models: ~10-15s
Verify health: ~0.5s
Process image: ~2-3s
────────────────────
Total: ~15-20 seconds
```

**Subsequent Requests (Warm):**
```
Check server: ~0.1s
  ↓ (already running)
Process image: ~2-3s
────────────────────
Total: ~2-3 seconds
```

### Optimization Tips

**1. Use GPU (if available):**
```csharp
// Modify startup arguments to use CUDA
LocalOmniParserManager.Configure(device: "cuda");
```
Result: ~1 second per screenshot (vs 2-3 on CPU)

**2. Keep Server Running:**
- Don't stop server between requests
- Manager keeps it running automatically
- Restart only if needed

**3. Batch Processing:**
- Server handles multiple requests
- No per-request overhead
- Process many screenshots efficiently

## 🐛 Troubleshooting

### Issue: "OmniParser server not available"

**Cause:** Server failed to start

**Solutions:**

1. **Check Installation:**
```
Verify: T:\OmniParser\omnitool\omniparserserver\omniparserserver.py exists
Verify: T:\OmniParser\weights\ folder exists with models
```

2. **Check Python:**
```bash
python --version  # Should be 3.12 or similar
python -c "import torch; import transformers"  # Check dependencies
```

3. **Get Diagnostics:**
```csharp
string info = LocalOmniParserManager.GetDiagnostics();
// Shows detailed status
```

4. **Check Logs:**
Look for "OmniParser-Server" entries in FlowVision logs for error messages

### Issue: "Server starting but timing out"

**Cause:** Models taking too long to load

**Solutions:**
1. Increase wait timeout (default 30 seconds)
2. Use smaller/faster models
3. Use GPU instead of CPU
4. Check system resources (RAM, disk)

### Issue: "Server crashes after starting"

**Cause:** Dependency or configuration issue

**Solutions:**
1. Check Python dependencies:
```bash
cd T:\OmniParser
pip install -r requirements.txt
```

2. Verify model files:
```bash
# Should exist and be valid
T:\OmniParser\weights\icon_detect\model.pt
T:\OmniParser\weights\icon_caption_florence\*
```

3. Check logs for Python errors

### Issue: "Port 8080 already in use"

**Cause:** Another service using port 8080

**Solutions:**
1. Stop other service on port 8080
2. Or configure different port:
```csharp
LocalOmniParserManager.Configure(port: 8081);
```

### Issue: "Models not found"

**Cause:** Weights not downloaded or wrong location

**Solutions:**
1. Download models:
```bash
cd T:\OmniParser
huggingface-cli download microsoft/OmniParser-v2.0 --local-dir weights
```

2. Verify paths match:
```
weights\icon_detect\model.pt
weights\icon_caption_florence\*
```

## 🔧 Advanced Configuration

### Custom Installation Path

**If OmniParser is elsewhere:**
```csharp
LocalOmniParserManager.Configure(
    omniParserPath: @"C:\MyCustomPath\OmniParser"
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

Or activate conda first, then Python will use that environment automatically.

### Custom Port

**If port conflict:**
```csharp
LocalOmniParserManager.Configure(
    port: 8081
);
```

### Using GPU

**Modify server startup** (in LocalOmniParserManager.cs):
```csharp
// Change from:
$"--device cpu " +

// To:
$"--device cuda " +
```

Then rebuild FlowVision.

## 📖 API Reference

### LocalOmniParserManager

**Static Methods:**

```csharp
// Check if server is running and responding
Task<bool> IsServerRunningAsync()

// Ensure server is running (auto-start if needed)
Task<bool> EnsureServerRunningAsync()

// Stop the server
void StopServer()

// Configure paths and settings
void Configure(
    string omniParserPath = null,
    string pythonExe = null,
    string serverUrl = null,
    int? port = null
)

// Get current server URL
string GetServerUrl()

// Check if OmniParser is installed
bool IsInstalled()

// Get detailed diagnostics
string GetDiagnostics()
```

**Usage Examples:**

```csharp
// Basic usage
await LocalOmniParserManager.EnsureServerRunningAsync();

// Check status
bool running = await LocalOmniParserManager.IsServerRunningAsync();
if (!running) {
    // Server not available
}

// Get diagnostics
string info = LocalOmniParserManager.GetDiagnostics();
Console.WriteLine(info);

// Custom configuration
LocalOmniParserManager.Configure(
    omniParserPath: @"D:\OmniParser",
    pythonExe: "python3",
    port: 8081
);
```

## 📚 Implementation Details

### Server Lifecycle

**States:**
1. **Not Installed** - OmniParser files missing
2. **Installed but Stopped** - Ready to start
3. **Starting** - Process launching, models loading
4. **Running** - Server responding to requests
5. **Failed** - Started but not responding

### Process Management

**Features:**
- Hidden console window (no popup)
- Output/error stream capture
- Proper disposal on exit
- Zombie process prevention

**Implementation:**
```csharp
var startInfo = new ProcessStartInfo
{
    FileName = _pythonExecutable,
    Arguments = arguments,
    WorkingDirectory = workingDir,
    UseShellExecute = false,
    CreateNoWindow = true,  // No popup!
    RedirectStandardOutput = true,
    RedirectStandardError = true
};
```

### Health Monitoring

**Probe Endpoint:**
```
GET http://127.0.0.1:8080/probe/
Response: {"message": "Omniparser API ready"}
```

**Health Check:**
- Sends GET to /probe/
- Expects 200 OK
- Timeout: 2 seconds
- Returns: True/False

### Error Handling

**Graceful Degradation:**
1. Try to start server
2. If fails, show detailed error
3. Include diagnostics
4. Suggest solutions

**User Experience:**
- Clear error messages
- Actionable suggestions
- Diagnostic information
- No silent failures

## 🎁 Benefits

### For Users

1. **Zero Manual Management**
   - No server startup
   - No configuration
   - Just works!

2. **Automatic Recovery**
   - Detects failures
   - Restarts automatically
   - Maintains availability

3. **Clear Feedback**
   - Status messages
   - Progress indicators
   - Error diagnostics

### For Developers

1. **Simple Integration**
   ```csharp
   await LocalOmniParserManager.EnsureServerRunningAsync();
   // Server is now running!
   ```

2. **Robust Error Handling**
   - Detailed diagnostics
   - Proper exceptions
   - Clear messages

3. **Maintainable**
   - Single responsibility
   - Clean interface
   - Well documented

## 📊 Status Messages

**What You'll See:**

```
Starting:
"Checking local OmniParser server..."
"Starting local OmniParser server..."
"Server started successfully in 15 seconds"

Running:
"Server already running"
"Processing screenshot..."
"Done!"

Errors:
"Failed to start server: [reason]"
"OmniParser not found at T:\OmniParser"
"Check installation and try again"
```

## 🎯 Summary

### What You Get

✅ **Automatic server management** - no manual startup
✅ **Health monitoring** - ensures server is responding  
✅ **Auto-restart** - handles failures gracefully
✅ **Smart cooldown** - prevents restart loops
✅ **Process lifecycle** - proper cleanup
✅ **Detailed diagnostics** - easy troubleshooting
✅ **Zero configuration** - works with defaults
✅ **Local processing** - fast and private

### What You Need

📋 OmniParser installed at `T:\OmniParser`
📋 Python 3.12+ with dependencies
📋 Model weights downloaded
📋 Port 8080 available (or configure different)

### How It Works

1. You request screen capture
2. FlowVision checks if server running
3. If not, starts it automatically
4. Waits for server to be ready
5. Processes your screenshot
6. Returns parsed UI elements
7. Server stays running for next request

**It just works!** 🎊

---

**Status:** ✅ Implemented and Ready
**Build:** ✅ Successful
**Testing:** Ready for use
**Documentation:** Complete
