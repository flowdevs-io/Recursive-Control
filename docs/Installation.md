---
layout: default
title: Installation
---

# Installation Capsule

Your mission dock for landing Recursive Control on Windows. Choose your payload, confirm dependencies, and ignite the thrusters.

## Flight Readiness Checklist

### Baseline Specs
- **Operating System**: Windows 10 or Windows 11
- **.NET Framework**: 4.8 or later
- **RAM**: 4 GB minimum (8 GB recommended)
- **Disk Space**: 500 MB for application files

### Network & Auth
- Internet connection for AI model API access
- API key for your preferred AI provider (OpenAI, Azure OpenAI, Anthropic, Google, etc.)

## Deploy Options

### Rapid Drop (Recommended)

1. **Pull the latest build**
   - Hop to the [Releases hangar](https://github.com/flowdevs-io/Recursive-Control/releases)
   - Snag the newest `recursivecontrol.zip` payload
   - Extract anywhere friendly (no admin elevation required)

2. **Boot the cockpit**
   - Launch `recursivecontrol.exe`
   - First boot unlocks the setup co-pilot

3. **Wire your pilot**
   - Open Settings → Providers
   - Pick your model stack (OpenAI, Azure, Claude, Gemini, LM Studio…)
   - Drop in your API credentials

4. **Ping the tower**
   - Ask "What can you do?"
   - Confirm you see agent chatter + completion

### Source Build (Engineers’ Track)

For developers who want to build from source:

```bash
# Clone the repository
git clone https://github.com/flowdevs-io/Recursive-Control.git

# Navigate to the directory
cd Recursive-Control

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run --project FlowVision
```

## Configure Your Pilot

### OpenAI

#### OpenAI
1. Get your API key from [OpenAI Platform](https://platform.openai.com)
2. In Recursive Control settings:
   - Provider: OpenAI
   - API Key: Your OpenAI key
   - Model: gpt-4 or gpt-3.5-turbo

#### Azure OpenAI
1. Set up Azure OpenAI service in Azure Portal
2. In Recursive Control settings:
   - Provider: Azure OpenAI
   - Endpoint: Your Azure endpoint URL
   - API Key: Your Azure key
   - Deployment Name: Your model deployment

#### Anthropic Claude
1. Get your API key from [Anthropic Console](https://console.anthropic.com)
2. In Recursive Control settings:
   - Provider: Anthropic
   - API Key: Your Anthropic key
   - Model: claude-3-opus or claude-3-sonnet

#### LM Studio (Local)
1. Download and run [LM Studio](https://lmstudio.ai)
2. Load your preferred local model
3. Start the local server
4. In Recursive Control settings:
   - Provider: LM Studio
   - Endpoint: http://localhost:1234 (or your configured port)

### Plugin Bay

Enable or disable plugins based on your needs:

1. Open settings → Plugins
2. Toggle plugins on/off:
   - ✅ Keyboard/Mouse (recommended)
   - ✅ Screen Capture (recommended)
   - ✅ Command Line
   - ✅ PowerShell
   - ⚠️ Playwright (requires additional setup)
   - ⚠️ Remote Control (enable for HTTP API)

## System Checks

Run these test commands to verify everything works:

1. **Basic Interaction**: "Hello, can you hear me?"
2. **Screen Capture**: "Take a screenshot"
3. **File Operations**: "Show me my Desktop folder"
4. **Application Control**: "Open Notepad"

If all tests pass, you're ready to go!

## Troubleshooting Bay

### .NET Framework Missing
- Download and install [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48)

### API Key Errors
- Verify your API key is correct
- Check that your API provider account has credits/active subscription
- Ensure internet connection is working

### Application Won’t Start
- Run as Administrator
- Check Windows Event Viewer for error details
- Verify all dependencies are installed

### Performance Issues
- Close unnecessary applications
- Increase available RAM
- Consider using a lighter AI model

## Next Flight Plans

- Continue to [Getting Started](Getting-Started.html) guide
- Explore [UI Features](UI-Features.html)
- Join our [Discord community](https://discord.gg/mQWsWeHsVU)

## Uninstall Playbook

To remove Recursive Control:
1. Delete the application folder
2. Remove configuration files from `%APPDATA%\FlowVision`
3. (Optional) Remove any created shortcuts

---

Need help? Check the [Troubleshooting](Troubleshooting.html) guide or ask in our [Discord](https://discord.gg/mQWsWeHsVU).
