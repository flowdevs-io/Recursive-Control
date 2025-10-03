# LM Studio Integration Guide

## Overview

FlowVision now supports **local AI** through LM Studio integration! This allows you to run AI models locally on your machine without requiring Azure OpenAI or other cloud services.

## 🎯 What is LM Studio?

[LM Studio](https://lmstudio.ai/) is a desktop application that lets you:
- Run open-source LLMs (Large Language Models) locally
- No internet connection required (after downloading models)
- Complete privacy - your data stays on your machine
- OpenAI-compatible API for easy integration
- Support for various models (Llama, Mistral, Phi, etc.)

## ✅ Features

- **Local AI Execution** - All AI processing happens on your machine
- **Privacy** - No data sent to cloud services
- **Cost Savings** - No API costs or usage limits
- **Offline Support** - Works without internet (after setup)
- **Compatible Models** - Works with any model supported by LM Studio
- **Tool Calling Support** - Full function/tool calling capabilities
- **Easy Configuration** - Simple UI for setup

## 📋 Prerequisites

### 1. Install LM Studio
1. Download from [https://lmstudio.ai/](https://lmstudio.ai/)
2. Install and launch LM Studio
3. Download a model (recommendations below)

### 2. System Requirements
- **RAM**: At least 16GB (32GB+ recommended for larger models)
- **GPU**: Optional but recommended (CUDA/Metal support)
- **Storage**: Depends on model size (2GB - 50GB+)
- **OS**: Windows, macOS, or Linux

## 🚀 Quick Start Guide

### Step 1: Download and Load a Model in LM Studio

1. **Open LM Studio**
2. **Go to the "Discover" or "Models" tab**
3. **Download a recommended model**:
   - **For general use**: `TheBloke/Mistral-7B-Instruct-v0.2-GGUF`
   - **For coding**: `TheBloke/deepseek-coder-6.7b-instruct-GGUF`
   - **Lightweight**: `TheBloke/TinyLlama-1.1B-Chat-v1.0-GGUF`
   - **Advanced**: `TheBloke/Llama-2-13B-chat-GGUF`

4. **Load the model**:
   - Click on the downloaded model
   - Select a quantization level (Q4_K_M is a good balance)
   - Click "Load Model"

### Step 2: Start the LM Studio Server

1. **In LM Studio, go to the "Local Server" tab**
2. **Click "Start Server"**
3. **Verify the server is running** (default: `http://localhost:1234`)
4. **Keep LM Studio running** while using FlowVision

### Step 3: Configure FlowVision

1. **Open FlowVision**
2. **Go to Settings → LM Studio Configuration** (or create menu option)
3. **Configure settings**:
   - ✅ Check "Enable LM Studio"
   - Endpoint: `http://localhost:1234/v1` (default)
   - Model Name: `local-model` (or as shown in LM Studio)
   - Temperature: `0.7` (adjust as needed)
   - Max Tokens: `2048` (or higher for longer responses)

4. **Click "Test Connection"** to verify
5. **Click "Save"**

### Step 4: Test It!

1. **Ask FlowVision a question**
2. **The request will now go to LM Studio instead of Azure**
3. **You'll see "Local AI response" in the loading indicator**

## 🔧 Configuration Options

### LMStudioConfig Settings

```csharp
// Configuration file location
%APPDATA%\FlowVision\lmstudioconfig.json
```

| Setting | Description | Default Value |
|---------|-------------|---------------|
| `Enabled` | Use LM Studio instead of Azure | `false` |
| `EndpointURL` | LM Studio server endpoint | `http://localhost:1234/v1` |
| `ModelName` | Model identifier | `local-model` |
| `APIKey` | Placeholder (not needed) | `lm-studio` |
| `Temperature` | Response randomness (0-2) | `0.7` |
| `MaxTokens` | Maximum response length | `2048` |
| `TimeoutSeconds` | Request timeout | `300` |

## 📊 Recommended Models by Use Case

### For Tool Calling (Our Primary Use Case)
```
Model: TheBloke/Hermes-2-Pro-Mistral-7B-GGUF
Size: ~4GB (Q4_K_M)
Why: Excellent at function calling and following instructions
```

### For Fast Responses
```
Model: TheBloke/Phi-2-GGUF
Size: ~1.6GB (Q4_K_M)
Why: Very fast, good for quick commands
```

### For Best Quality
```
Model: TheBloke/Mixtral-8x7B-Instruct-v0.1-GGUF
Size: ~26GB (Q4_K_M)
Why: Highest quality, great reasoning
Requirements: 32GB+ RAM
```

### For Coding Tasks
```
Model: TheBloke/deepseek-coder-6.7b-instruct-GGUF
Size: ~3.8GB (Q4_K_M)
Why: Optimized for code generation
```

## 🛠️ Programmatic Usage

### Creating LMStudioActioner

```csharp
// LM Studio actioner is automatically used when enabled
var actioner = new Actioner(outputHandler);

// Check if LM Studio is enabled
var lmConfig = LMStudioConfig.LoadConfig();
if (lmConfig.Enabled)
{
    Console.WriteLine("Using LM Studio for local AI");
}
```

### Manual Configuration

```csharp
// Load config
var config = LMStudioConfig.LoadConfig();

// Modify settings
config.Enabled = true;
config.EndpointURL = "http://localhost:1234/v1";
config.ModelName = "local-model";
config.Temperature = 0.7;

// Save config
config.SaveConfig();
```

### Priority Order

When you make a request, FlowVision checks in this order:

1. **LM Studio** (if `LMStudioConfig.Enabled = true`)
2. **Multi-Agent Mode** (if enabled)
3. **Azure OpenAI** (default fallback)

## 🐛 Troubleshooting

### "Cannot connect to LM Studio"

**Solutions**:
1. ✅ Make sure LM Studio is running
2. ✅ Verify a model is loaded
3. ✅ Click "Start Server" in LM Studio
4. ✅ Check endpoint URL: `http://localhost:1234/v1`
5. ✅ Disable firewall/antivirus temporarily
6. ✅ Try restarting LM Studio

### "Model not responding" or slow responses

**Solutions**:
1. ✅ Check RAM usage (models need sufficient memory)
2. ✅ Try a smaller/faster model
3. ✅ Increase `TimeoutSeconds` in config
4. ✅ Enable GPU acceleration in LM Studio settings
5. ✅ Close other applications to free up resources

### "Tool calling not working"

**Solutions**:
1. ✅ Use a model that supports function calling
2. ✅ Try: `Hermes-2-Pro-Mistral-7B` or similar
3. ✅ Ensure `AutoInvokeKernelFunctions` is enabled in ToolConfig
4. ✅ Check LM Studio logs for errors

### "Connection timeout"

**Solutions**:
1. ✅ Increase `TimeoutSeconds` (default 300)
2. ✅ First request is slower (model loading)
3. ✅ Reduce `MaxTokens` for faster responses
4. ✅ Consider a smaller/faster model

## 🔐 Privacy & Security

### Benefits
- ✅ **Complete Privacy** - No data sent to cloud
- ✅ **Offline Operation** - Works without internet
- ✅ **No API Costs** - Free to use
- ✅ **Full Control** - You control the model and data

### Considerations
- ⚠️ **Local Processing** - Uses your computer resources
- ⚠️ **Model Storage** - Models can be large (1GB - 50GB+)
- ⚠️ **Performance** - Depends on your hardware

## 📈 Performance Tips

### 1. Choose the Right Model Size
- **Lightweight (1-3GB)**: Fast responses, good for simple tasks
- **Medium (4-7GB)**: Balanced performance and quality
- **Large (8GB+)**: Best quality, requires powerful hardware

### 2. Optimize Settings
```
Temperature: 0.2-0.5  (More focused responses)
MaxTokens: 1024-2048  (Faster generation)
```

### 3. Hardware Recommendations
- **Minimum**: 16GB RAM, CPU only
- **Recommended**: 32GB RAM, GPU (NVIDIA/AMD/Apple Silicon)
- **Optimal**: 64GB RAM, High-end GPU

### 4. Use GPU Acceleration
In LM Studio settings:
- Enable GPU acceleration
- Allocate sufficient GPU layers
- Monitor GPU usage

## 🔄 Switching Between Local and Cloud AI

### Enable Local AI
```csharp
var config = LMStudioConfig.LoadConfig();
config.Enabled = true;
config.SaveConfig();
```

### Disable Local AI (Use Azure)
```csharp
var config = LMStudioConfig.LoadConfig();
config.Enabled = false;
config.SaveConfig();
```

### Dynamic Switching
FlowVision automatically uses LM Studio when enabled, and falls back to Azure when disabled. No need to restart the application!

## 📚 Additional Resources

### LM Studio
- Website: https://lmstudio.ai/
- Documentation: https://lmstudio.ai/docs
- Discord: https://discord.gg/lmstudio

### Models
- Hugging Face: https://huggingface.co/models
- TheBloke's Models: https://huggingface.co/TheBloke
- Model Comparison: https://huggingface.co/spaces/lmsys/chatbot-arena-leaderboard

### OpenAI-Compatible API
- LM Studio uses OpenAI's API format
- Compatible with OpenAI SDK
- Supports most OpenAI features

## 🎉 Benefits Summary

| Feature | Azure OpenAI | LM Studio Local |
|---------|--------------|-----------------|
| **Privacy** | Cloud-based | ✅ 100% Local |
| **Cost** | Pay per use | ✅ Free |
| **Internet** | Required | ✅ Not needed |
| **Speed** | Fast | Depends on hardware |
| **Quality** | Excellent | Good to Excellent |
| **Tool Calling** | ✅ Full support | ✅ Supported |
| **Setup** | Easy | Moderate |

## 📝 Example Configuration File

```json
{
  "EndpointURL": "http://localhost:1234/v1",
  "ModelName": "local-model",
  "APIKey": "lm-studio",
  "Enabled": true,
  "Temperature": 0.7,
  "MaxTokens": 2048,
  "TimeoutSeconds": 300
}
```

## 🚀 Next Steps

1. **Download LM Studio** and experiment with different models
2. **Test with FlowVision** using simple commands first
3. **Try tool calling** features (CMD, PowerShell, etc.)
4. **Optimize settings** for your use case
5. **Consider hybrid approach**: Local for privacy-sensitive tasks, Cloud for performance

---

**Happy Local AI Usage!** 🎉

For support, check the [Troubleshooting](#-troubleshooting) section or open an issue on GitHub.
