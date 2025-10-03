# Update Main README.md with Documentation Links

Add this section to your main README.md file to link to all documentation:

```markdown
## 📚 Documentation

### Getting Started
- **[Installation Guide](docs/Installation.md)** - Complete setup instructions
- **[Getting Started Tutorial](docs/Getting-Started.md)** - Your first tasks
- **[Multi-Agent Architecture](docs/Multi-Agent-Architecture.md)** - How the 3-agent system works

### Configuration
- **[AI Provider Setup](docs/Installation.md#configure-ai-provider)** - Azure, LM Studio, GitHub Models
- **[Plugin Configuration](docs/Installation.md#configure-plugins)** - Enable/disable features
- **[Multi-Agent Mode](docs/Multi-Agent-Architecture.md#configuration)** - Complex task handling

### Reference
- **[FAQ](docs/FAQ.md)** - Frequently asked questions
- **[Troubleshooting](docs/Troubleshooting.md)** - Common issues and solutions
- **[API Reference](docs/API-Reference.md)** - Developer documentation

### Blog & Updates
- **[Version 2.0 Release Post](docs/Blog-Post-v2.0.md)** - Major upgrade announcement
- **[System Prompts Reference](docs/System-Prompts-Reference.md)** - Complete prompt library
- **[UI Features](docs/UI-Features.md)** - New interface improvements
- **[UI Redesign](docs/UI-Redesign.md)** - Modern menu structure

---

## 🌐 Online Documentation

**GitHub Pages** (Recommended): https://flowdevs-io.github.io/Recursive-Control/

*Enable GitHub Pages in Settings → Pages → Source: `/docs` folder*

---
```

## Alternative: Simple TOC

If you want a minimal approach:

```markdown
## 📖 Documentation

📥 [Installation](docs/Installation.md) • 
🚀 [Getting Started](docs/Getting-Started.md) • 
🤖 [Multi-Agent System](docs/Multi-Agent-Architecture.md) • 
🔧 [API Reference](docs/API-Reference.md)

**More:** [FAQ](docs/FAQ.md) | [Troubleshooting](docs/Troubleshooting.md) | [v2.0 Release Notes](docs/Blog-Post-v2.0.md)
```

## Alternative: Detailed TOC

If you want full visibility:

```markdown
## 📚 Complete Documentation

### 🚀 Quick Start
1. [Installation Guide](docs/Installation.md) - Download, install, configure
2. [First-Time Setup](docs/Installation.md#initial-setup) - AI provider and plugins
3. [Your First Task](docs/Getting-Started.md#your-first-task) - Test the system

### 📖 Core Guides
- **[Getting Started](docs/Getting-Started.md)** - Tutorials and examples
  - Opening applications
  - Window management
  - Keyboard control
  - Taking screenshots
  - Browser automation
  
- **[Multi-Agent Architecture](docs/Multi-Agent-Architecture.md)** - How it works
  - Coordinator agent (routing)
  - Planner agent (breakdown)
  - Actioner agent (execution)
  - When to use multi-agent mode
  
### ⚙️ Configuration
- **AI Providers**: Configure [Azure OpenAI](docs/Installation.md#option-a-azure-openai), [LM Studio](docs/Installation.md#option-b-lm-studio), or [GitHub Models](docs/Installation.md#option-c-github-models)
- **Plugins**: Enable features in [Plugin Configuration](docs/Installation.md#step-2-configure-plugins)
- **Multi-Agent Mode**: Toggle in [Setup menu](docs/Multi-Agent-Architecture.md#enabledisable-multi-agent-mode)

### 🔧 Developer Resources
- **[API Reference](docs/API-Reference.md)** - Plugin API and extension guide
- **[System Prompts](docs/System-Prompts-Reference.md)** - Complete prompt library
- **[Plugin Development](docs/API-Reference.md)** - Create custom tools

### 🆘 Support
- **[FAQ](docs/FAQ.md)** - Common questions
- **[Troubleshooting](docs/Troubleshooting.md)** - Problem solving
- **[Discord Community](https://discord.gg/mQWsWeHsVU)** - Get help
- **[Report Issue](https://github.com/flowdevs-io/Recursive-Control/issues)** - Bug reports

### 📰 Latest Updates
- **[Version 2.0 Release](docs/Blog-Post-v2.0.md)** - From Good to Great
  - Complete system prompt rewrite (800+ lines)
  - Window-targeted keyboard/mouse control
  - ONNX auto-initialization
  - Multi-agent improvements (25 steps)
  - Chat export in 4 formats
  - Modern emoji-enhanced UI

- **[UI Improvements](docs/UI-Features.md)** - New interface features
  - Chat export system
  - Execution visualizer
  - Activity monitor
  
- **[UI Redesign](docs/UI-Redesign.md)** - Modern menu structure
  - All 4 AI agents accessible
  - Per-agent model configuration
  - Multi-agent mode toggle
  - Emoji visual language
```

---

Choose the style that fits your README best!
