---
layout: default
title: Installation
---

# Installation Guide

Get Recursive Control up and running on your Windows system.

## System Requirements

### Minimum Requirements
- **Operating System**: Windows 10 or Windows 11
- **.NET Framework**: 4.8 or later
- **RAM**: 4 GB minimum (8 GB recommended)
- **Disk Space**: 500 MB for application files

### Additional Requirements
- Internet connection for AI model API access
- API key for your preferred AI provider (OpenAI, Azure OpenAI, Anthropic, Google, etc.)

## Installation Steps

### Option 1: Download Pre-built Release (Recommended)

1. **Download the Latest Release**
   - Visit the [Releases page](https://github.com/flowdevs-io/Recursive-Control/releases)
   - Download the latest `recursivecontrol.zip` or installer
   - Extract to your preferred location

2. **Run the Application**
   - Double-click `recursivecontrol.exe`
   - The application will launch and prompt for initial setup

3. **Configure Your AI Provider**
   - Click the settings/configuration button
   - Select your AI provider (OpenAI, Azure, Anthropic, etc.)
   - Enter your API key
   - Choose your preferred model

4. **Test the Installation**
   - Try a simple command like "What can you do?"
   - Verify the AI responds correctly

### Option 2: Build from Source

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

## Initial Configuration

### Setting Up Your AI Provider

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

### Plugin Configuration

Enable or disable plugins based on your needs:

1. Open settings → Plugins
2. Toggle plugins on/off:
   - ✅ Keyboard/Mouse (recommended)
   - ✅ Screen Capture (recommended)
   - ✅ Command Line
   - ✅ PowerShell
   - ⚠️ Playwright (requires additional setup)
   - ⚠️ Remote Control (enable for HTTP API)

## Verifying Installation

Run these test commands to verify everything works:

1. **Basic Interaction**: "Hello, can you hear me?"
2. **Screen Capture**: "Take a screenshot"
3. **File Operations**: "Show me my Desktop folder"
4. **Application Control**: "Open Notepad"

If all tests pass, you're ready to go!

## Troubleshooting Installation Issues

### .NET Framework Not Found
- Download and install [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48)

### API Key Errors
- Verify your API key is correct
- Check that your API provider account has credits/active subscription
- Ensure internet connection is working

### Application Won't Start
- Run as Administrator
- Check Windows Event Viewer for error details
- Verify all dependencies are installed

### Performance Issues
- Close unnecessary applications
- Increase available RAM
- Consider using a lighter AI model

## Next Steps

- Continue to [Getting Started](Getting-Started.html) guide
- Explore [UI Features](UI-Features.html)
- Join our [Discord community](https://discord.gg/mQWsWeHsVU)

## Uninstallation

To remove Recursive Control:
1. Delete the application folder
2. Remove configuration files from `%APPDATA%\FlowVision`
3. (Optional) Remove any created shortcuts

---

Need help? Check the [Troubleshooting](Troubleshooting.html) guide or ask in our [Discord](https://discord.gg/mQWsWeHsVU).
