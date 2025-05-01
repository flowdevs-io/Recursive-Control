<picture>  
   <img alt="Shows the logo of recursive control" src="./content/recursive-control-banner-dark-1280x640.jpeg"  width="full">
</picture>

<h1 align="center">📎 AI Control for Windows Computers 📎</h1>

[![Join us on Discord](https://img.shields.io/badge/Join_our_Discord-7289DA?logo=discord&logoColor=white&labelColor=5865F2)](https://discord.gg/mQWsWeHsVU)

Recursive Control is an innovative project designed to enable artificial intelligence (AI) to interact seamlessly with your computer, automating tasks, performing complex workflows, and enhancing productivity.

## Project Goal

Our mission is to create an AI-driven interface that can autonomously control your computer, intelligently perform tasks, open applications, execute commands, and streamline workflows, effectively turning natural language into actionable operations.

## Features

- **AI-Powered Interaction**: Utilize AI models (such as GPT-based models) to interpret user input and intelligently execute actions.
- **Automated Workflow Execution**: Automate repetitive or complex sequences of computer actions.
- **Natural Language Commands**: Simply describe tasks in plain language, and let the AI handle execution.

## Getting Started

### Prerequisites

- .NET 4.8 or later
- Windows Operating System
- Azure OpenAI API Key (More models will be supported in the future)

### Local Setup

Download the latest release from the [Releases](https://github.com/flowdevs-io/Recursive-Control/releases) page and follow three easy steps.

1. Run recursivecontrol.exe
2. Setup your LLM 
![image](https://github.com/user-attachments/assets/5dc4f034-794c-46c8-bf96-a4c95db05886)
3. Input your commands directly into the UI, and watch as AI automate your tasks.


### Development

1. Clone this repository:
   ```bash
   git clone https://github.com/flowdevs-io/Recursive-Control.git
   ```

2. Navigate to the cloned directory:
   ```bash
   cd Recursive-Control
   ```

3. Restore dependencies and build the project:
   ```bash
   dotnet restore
   dotnet build
   ```

## Plugin System

Recursive Control supports a modular plugin system, allowing you to extend its capabilities. Plugins can automate keyboard, mouse, window management, screen capture, command line, and more. You can find plugin implementations in the `FlowVision/lib/Plugins/` directory. To add your own plugin, implement the required interface and register it in the application.

### Built-in Plugins
- **CMDPlugin**: Execute Windows command line instructions.
- **PowershellPlugin**: Run PowerShell scripts and commands.
- **KeyboardPlugin**: Automate keyboard input.
- **MousePlugin**: Automate mouse actions.
- **ScreenCapturePlugin**: Capture screenshots.
- **WindowSelectionPlugin**: Select and interact with application windows.

## Folder Structure

```
FlowVision.sln                # Solution file
FlowVision/                   # Main application source
  lib/                        # Core libraries and plugins
    Classes/                  # Helper and service classes
    Plugins/                  # Built-in plugins
    UI/                       # UI theming
  Models/                     # Data models
  Properties/                 # .NET project properties
content/                      # Images and assets
```

## Example Use Cases=
- Control applications via natural language (e.g., "Open Excel and create a new spreadsheet")
- Capture and process screenshots for documentation
- Batch rename files or organize folders

## Troubleshooting
- Ensure you have .NET 4.8+ installed
- Check your API key and network connection for LLM access
- For plugin errors, review the application logs in %appdata%\FlowVision\plugin_usage.log

## Contributing

We welcome contributions! Please feel free to submit issues, suggestions, or pull requests. Your collaboration is essential for making Recursive Control powerful and versatile.

## Community & Support
- [GitHub Issues](https://github.com/flowdevs-io/Recursive-Control/issues) for bug reports and feature requests
- [Discussions](https://github.com/flowdevs-io/Recursive-Control/discussions) for Q&A and ideas
- [LinkedIn](https://www.linkedin.com/company/flowdevs) for updates and networking

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

For any questions, feedback, or collaboration inquiries, please connect with us through our GitHub repository,  or via LinkedIn.

## Citation

If you use Browser Use in your research or project, please cite:

```bibtex
@software{recursive-control2025,
  author = {Trantham, Justin},
  title = {Recursive Control: AI Control for Windows Computers },
  year = {2025},
  publisher = {GitHub},
  url = {https://github.com/flowdevs-io/Recursive-Contro}
}
```
<div align="center">
Made and owned by Engineers
</div>
