---
layout: default
title: Troubleshooting
---

# Troubleshooting Guide

Common issues and their solutions for Recursive Control.

## Installation Issues

### Application Won't Start

**Problem**: Double-clicking the executable does nothing or shows an error.

**Solutions**:
1. Verify .NET Framework 4.8 is installed
2. Run as Administrator (right-click → Run as Administrator)
3. Check Windows Event Viewer for error details
4. Ensure antivirus isn't blocking the application

### Missing Dependencies

**Problem**: Error about missing DLL files.

**Solutions**:
1. Install [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48)
2. Install [Visual C++ Redistributables](https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads)
3. Reinstall the application

## Configuration Issues

### API Key Not Working

**Problem**: "Invalid API key" or authentication errors.

**Solutions**:
1. Verify the API key is copied correctly (no extra spaces)
2. Check that your API provider account is active
3. Ensure you have sufficient credits/quota
4. Verify you're using the correct endpoint URL (for Azure)

### Can't Save Settings

**Problem**: Settings don't persist after restart.

**Solutions**:
1. Run the application as Administrator
2. Check that `%APPDATA%\FlowVision` folder has write permissions
3. Verify no antivirus is blocking file writes

## Runtime Issues

### Commands Not Executing

**Problem**: AI responds but doesn't perform actions.

**Solutions**:
1. Check that required plugins are enabled in settings
2. Verify Windows UAC isn't blocking automated actions
3. Ensure the target application/window is accessible
4. Try running Recursive Control as Administrator

### Slow Performance

**Problem**: Application is laggy or unresponsive.

**Solutions**:
1. Close unnecessary applications to free RAM
2. Switch to a faster AI model (e.g., GPT-3.5 instead of GPT-4)
3. Disable unused plugins
4. Check internet connection speed
5. Consider using a local model with LM Studio

### High Token Usage

**Problem**: Burning through API credits quickly.

**Solutions**:
1. Use more specific commands to reduce back-and-forth
2. Disable verbose logging in system prompts
3. Use smaller/cheaper models for simple tasks
4. Implement caching where possible

## Plugin-Specific Issues

### Screen Capture Not Working

**Problem**: Screenshots are blank or fail to capture.

**Solutions**:
1. Grant screen capture permissions in Windows settings
2. Check that display scaling is at 100% (or adjust DPI settings)
3. Verify graphics drivers are up to date
4. Try running as Administrator

### Playwright/Browser Automation Failing

**Problem**: Browser automation commands fail.

**Solutions**:
1. Ensure Playwright is properly installed
2. Download required browser binaries
3. Check firewall isn't blocking browser processes
4. Verify Playwright plugin is enabled in settings

### Keyboard/Mouse Input Not Working

**Problem**: Automated keyboard/mouse actions don't execute.

**Solutions**:
1. Run application as Administrator
2. Check that UAC isn't blocking input simulation
3. Verify target window has focus
4. Disable "Filter keyboard input" if enabled in accessibility settings

### Remote Control Plugin Not Responding

**Problem**: HTTP API not accepting commands.

**Solutions**:
1. Verify plugin is enabled in settings
2. Check configured port isn't already in use
3. Ensure firewall allows incoming connections on that port
4. Test with curl: `curl -X POST http://localhost:PORT -d '{"command":"test"}'`

## Error Messages

### "Model not found" or "Deployment not found"

**Cause**: Model name or deployment name is incorrect.

**Solution**: Verify the exact model/deployment name in your AI provider dashboard and update settings.

### "Rate limit exceeded"

**Cause**: Too many API requests in a short time.

**Solution**: Wait a few moments and try again. Consider upgrading your API plan for higher rate limits.

### "Context length exceeded"

**Cause**: Conversation history too long for the model.

**Solution**: Start a new conversation or use a model with larger context window.

### "Insufficient permissions"

**Cause**: Application doesn't have required Windows permissions.

**Solution**: Run as Administrator and check UAC settings.

## Performance Optimization

### Best Practices for Speed

1. Use specific, clear commands
2. Enable only needed plugins
3. Use faster AI models for simple tasks
4. Keep conversation history manageable
5. Close resource-heavy applications

### Memory Management

- Restart application periodically for long sessions
- Clear conversation history when not needed
- Monitor Task Manager for memory leaks
- Close unnecessary browser tabs if using Playwright

## Logging and Diagnostics

### Enable Debug Logging

1. Open settings
2. Enable "Debug Mode" or "Verbose Logging"
3. Reproduce the issue
4. Check logs in `%APPDATA%\FlowVision\logs`

### Log Locations

- **Plugin Usage**: `%APPDATA%\FlowVision\plugin_usage.log`
- **Application Logs**: `%APPDATA%\FlowVision\logs\`
- **Error Logs**: Windows Event Viewer → Application

## Getting Help

If you can't resolve your issue:

1. **Check Documentation**
   - [Installation Guide](Installation.html)
   - [Getting Started](Getting-Started.html)
   - [FAQ](FAQ.html)

2. **Community Support**
   - [Discord Server](https://discord.gg/mQWsWeHsVU) - Fast response from community
   - [GitHub Discussions](https://github.com/flowdevs-io/Recursive-Control/discussions) - Q&A
   
3. **Report Bugs**
   - [GitHub Issues](https://github.com/flowdevs-io/Recursive-Control/issues) - Bug reports
   - Include: OS version, .NET version, error messages, steps to reproduce

## Known Issues

### Windows 11 24H2
- Some screen capture APIs may require additional permissions
- Workaround: Grant screen recording permission in Settings → Privacy

### High DPI Displays
- UI elements may appear small on 4K displays
- Workaround: Adjust display scaling or DPI awareness settings

### Antivirus False Positives
- Some antivirus software flags automation tools
- Workaround: Add Recursive Control to antivirus exclusions

---

Still stuck? Reach out on [Discord](https://discord.gg/mQWsWeHsVU) - we're here to help!
