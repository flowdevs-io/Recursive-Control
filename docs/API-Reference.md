---
layout: default
title: API Reference
---

# API Reference

Developer documentation for extending and integrating with Recursive Control.

## Plugin Development

### Creating a Custom Plugin

Plugins extend Recursive Control's capabilities. Here's how to create one:

```csharp
using Microsoft.SemanticKernel;
using System.ComponentModel;

public class MyCustomPlugin
{
    [KernelFunction]
    [Description("Does something useful")]
    public string MyFunction(
        [Description("Input parameter")] string input)
    {
        // Your implementation here
        return $"Processed: {input}";
    }
}
```

### Plugin Interface Requirements

All plugins must:
1. Use `[KernelFunction]` attribute for exposed methods
2. Include `[Description]` for functions and parameters
3. Return serializable types (string, int, bool, etc.)
4. Handle exceptions gracefully

### Registering Your Plugin

```csharp
// In your initialization code
kernel.ImportPluginFromType<MyCustomPlugin>();
```

## Built-in Plugins API

### CMDPlugin

Execute command line instructions.

```csharp
[KernelFunction]
[Description("Execute a Windows command")]
string ExecuteCommand(
    [Description("Command to execute")] string command,
    [Description("Working directory")] string workingDirectory = null)
```

**Example Usage**: "Execute dir command in C:\\Users"

### PowerShellPlugin

Run PowerShell scripts and commands.

```csharp
[KernelFunction]
[Description("Execute PowerShell command")]
string ExecutePowerShell(
    [Description("PowerShell script")] string script)
```

**Example Usage**: "Run PowerShell to get running processes"

### KeyboardPlugin

Automate keyboard input.

```csharp
[KernelFunction]
[Description("Type text using keyboard")]
void TypeText(
    [Description("Text to type")] string text)

[KernelFunction]
[Description("Press a key combination")]
void PressKeys(
    [Description("Keys to press")] string keys)
```

**Example Usage**: "Type Hello World" or "Press Ctrl+C"

### MousePlugin

Automate mouse actions.

```csharp
[KernelFunction]
[Description("Click at coordinates")]
void Click(
    [Description("X coordinate")] int x,
    [Description("Y coordinate")] int y,
    [Description("Button (left/right/middle)")] string button = "left")

[KernelFunction]
[Description("Move mouse to position")]
void MoveTo(
    [Description("X coordinate")] int x,
    [Description("Y coordinate")] int y)
```

**Example Usage**: "Click at position 500, 300"

### ScreenCapturePlugin

Capture and analyze screenshots.

```csharp
[KernelFunction]
[Description("Capture screenshot")]
string CaptureScreen(
    [Description("Capture full screen or window")] string mode = "fullscreen")

[KernelFunction]
[Description("Get screen dimensions")]
string GetScreenSize()
```

**Example Usage**: "Take a screenshot of the current window"

### WindowSelectionPlugin

Manage application windows.

```csharp
[KernelFunction]
[Description("List all open windows")]
string ListWindows()

[KernelFunction]
[Description("Bring window to front")]
void FocusWindow(
    [Description("Window title or handle")] string identifier)

[KernelFunction]
[Description("Close a window")]
void CloseWindow(
    [Description("Window identifier")] string identifier)
```

**Example Usage**: "List all open windows" or "Focus Chrome window"

### PlaywrightPlugin

Automate web browsers.

```csharp
[KernelFunction]
[Description("Launch browser")]
Task<string> LaunchBrowser(
    [Description("Browser type")] string browser = "chromium")

[KernelFunction]
[Description("Navigate to URL")]
Task<string> NavigateTo(
    [Description("URL to navigate")] string url)

[KernelFunction]
[Description("Execute JavaScript")]
Task<string> ExecuteScript(
    [Description("JavaScript code")] string script)

[KernelFunction]
[Description("Close browser")]
Task CloseBrowser()
```

**Example Usage**: "Open browser and go to github.com"

### RemoteControlPlugin

HTTP API for remote command execution.

```csharp
[KernelFunction]
[Description("Start HTTP server")]
void StartServer(
    [Description("Port number")] int port = 8080)

[KernelFunction]
[Description("Stop HTTP server")]
void StopServer()
```

**HTTP API Endpoint**:
```bash
POST http://localhost:8080/command
Content-Type: application/json

{
  "command": "Your natural language command here"
}
```

## Configuration API

### ToolConfig

Main configuration for plugins and features.

```csharp
public class ToolConfig
{
    // Plugin toggles
    public bool EnableKeyboard { get; set; }
    public bool EnableMouse { get; set; }
    public bool EnableScreenCapture { get; set; }
    public bool EnableCMD { get; set; }
    public bool EnablePowerShell { get; set; }
    public bool EnablePlaywright { get; set; }
    public bool EnableRemoteControl { get; set; }
    
    // System prompts
    public string CoordinatorPrompt { get; set; }
    public string PlannerPrompt { get; set; }
    public string ExecutorPrompt { get; set; }
    
    // Other settings
    public int RemoteControlPort { get; set; }
    public bool VerboseLogging { get; set; }
}
```

**Config Location**: `%APPDATA%\FlowVision\toolconfig.json`

### APIConfig

AI provider configuration.

```csharp
public class APIConfig
{
    public string Provider { get; set; } // "OpenAI", "Azure", "Anthropic", etc.
    public string ApiKey { get; set; }
    public string Endpoint { get; set; }
    public string ModelName { get; set; }
    public string DeploymentName { get; set; }
    public int MaxTokens { get; set; }
    public double Temperature { get; set; }
}
```

**Config Location**: `%APPDATA%\FlowVision\apiconfig.json`

## Multi-Agent Architecture

### Agent Roles

```csharp
// Coordinator: Routes requests to appropriate agent
var coordinatorAgent = new Agent
{
    Name = "Coordinator",
    SystemPrompt = toolConfig.CoordinatorPrompt
};

// Planner: Creates execution plans
var plannerAgent = new Agent
{
    Name = "Planner",
    SystemPrompt = toolConfig.PlannerPrompt
};

// Executor: Executes actions using plugins
var executorAgent = new Agent
{
    Name = "Executor",
    SystemPrompt = toolConfig.ExecutorPrompt,
    Plugins = kernel.Plugins
};
```

### Workflow

1. User input → Coordinator
2. Coordinator → Planner (if planning needed)
3. Planner → Executor (with step-by-step plan)
4. Executor → Plugins (to perform actions)
5. Results → User

## Extension Points

### Custom AI Providers

Implement custom AI provider:

```csharp
public interface IAIProvider
{
    Task<string> GenerateResponse(string prompt);
    Task<string> GenerateWithFunctions(string prompt, IEnumerable<Function> functions);
}
```

### Custom Logging

Implement custom logger:

```csharp
public interface IPluginLogger
{
    void LogUsage(string plugin, string function, Dictionary<string, object> parameters);
    void LogError(string plugin, Exception ex);
}
```

### Custom UI Themes

Create custom theme:

```csharp
public class CustomTheme : ITheme
{
    public Color BackgroundColor { get; set; }
    public Color ForegroundColor { get; set; }
    public Color AccentColor { get; set; }
    public Font DefaultFont { get; set; }
}
```

## Integration Examples

### HTTP API Integration

```python
import requests

# Send command via HTTP
response = requests.post(
    'http://localhost:8080/command',
    json={'command': 'Open Notepad and type Hello World'}
)

print(response.json())
```

### Programmatic Control

```csharp
// Initialize Recursive Control programmatically
var config = new APIConfig { /* ... */ };
var executor = new MultiAgentActioner(config);

// Execute command
var result = await executor.ExecuteAsync("Take a screenshot");
Console.WriteLine(result);
```

## Best Practices

### Plugin Development
1. Use descriptive function and parameter names
2. Provide detailed descriptions for AI understanding
3. Handle errors gracefully and return meaningful messages
4. Keep functions focused on single responsibilities
5. Test with various AI models

### Performance
1. Cache expensive operations
2. Use async/await for I/O operations
3. Implement timeout mechanisms
4. Release resources properly

### Security
1. Validate all input parameters
2. Sanitize file paths and commands
3. Implement permission checks
4. Log security-relevant actions
5. Don't expose sensitive data in responses

## Further Reading

- [Multi-Agent Architecture](Multi-Agent-Architecture.html) - Deep dive into agent system
- [System Prompts Reference](System-Prompts-Reference.html) - Customizing agent behavior
- [GitHub Repository](https://github.com/flowdevs-io/Recursive-Control) - Source code

## Community Resources

- [Discord Developer Channel](https://discord.gg/mQWsWeHsVU) - Ask questions
- [GitHub Discussions](https://github.com/flowdevs-io/Recursive-Control/discussions) - Share ideas
- [Example Plugins](https://github.com/flowdevs-io/Recursive-Control/tree/master/FlowVision/lib/Plugins) - Reference implementations

---

Have questions? Join our [Discord](https://discord.gg/mQWsWeHsVU) or open a [GitHub Discussion](https://github.com/flowdevs-io/Recursive-Control/discussions)!
