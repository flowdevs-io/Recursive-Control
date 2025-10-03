---
layout: default
title: Getting Started
---

# Getting Started with Recursive Control

This guide will help you get up and running with Recursive Control quickly.

## Your First Commands

Once installed and configured, you can start using natural language commands:

### Basic Examples

1. **File Management**
   ```
   "Open File Explorer and navigate to my Documents folder"
   "Create a new folder called Projects on my Desktop"
   "Rename all .txt files in this folder to .md"
   ```

2. **Application Control**
   ```
   "Open Notepad and type Hello World"
   "Launch Chrome and navigate to github.com"
   "Open Excel and create a new spreadsheet"
   ```

3. **Screen Capture**
   ```
   "Take a screenshot of the current window"
   "Capture the entire screen"
   ```

4. **Web Automation**
   ```
   "Open a browser and search for AI automation tools"
   "Fill out this form with my information"
   "Extract data from this webpage"
   ```

## Understanding the Multi-Agent System

Recursive Control uses a sophisticated 3-agent architecture:

1. **Coordinator Agent**: Understands your request and determines the best approach
2. **Planner Agent**: Creates a step-by-step plan to accomplish the task
3. **Executor Agent**: Executes the plan using available plugins

This system ensures tasks are completed efficiently and accurately.

## Tips for Better Results

### Be Specific
❌ "Do something with files"
✅ "Move all PDF files from Downloads to Documents folder"

### Break Down Complex Tasks
For very complex workflows, break them into smaller steps and verify each step.

### Use Natural Language
You don't need to use technical commands - just describe what you want in plain English.

## Available Plugins

Your commands can utilize these built-in plugins:

- **Keyboard & Mouse**: Automate input and clicks
- **Command Line**: Execute CMD and PowerShell commands
- **Screen Capture**: Take and analyze screenshots
- **Window Management**: Control application windows
- **Web Browser**: Automate websites with Playwright
- **Remote Control**: Accept commands via HTTP API

## Next Steps

- Explore the [UI Features](UI-Features.html) guide
- Learn about [Multi-Agent Architecture](Multi-Agent-Architecture.html)
- Check out [System Prompts](System-Prompts-Reference.html) for customization
- Join our [Discord](https://discord.gg/mQWsWeHsVU) for community support

## Common First-Time Questions

**Q: How do I know if it's working?**
A: The UI will show the agent's thinking process and actions in real-time.

**Q: What if something goes wrong?**
A: You can interrupt execution at any time. Check the [Troubleshooting](Troubleshooting.html) guide for help.

**Q: Can I customize the behavior?**
A: Yes! You can adjust system prompts, enable/disable plugins, and configure various settings.

---

Ready to dive deeper? Check out our [advanced documentation](Multi-Agent-Architecture.html) or join the [community](https://discord.gg/mQWsWeHsVU)!
