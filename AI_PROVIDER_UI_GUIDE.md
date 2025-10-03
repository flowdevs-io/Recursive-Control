# AI Provider Configuration UI Guide

## 🎉 New Feature: Unified AI Provider Selection!

FlowVision now has a **beautiful, unified configuration UI** that lets you easily switch between different AI providers!

## 🎯 What You Get

✅ **Visual Provider Selection** - Choose from dropdown menu
✅ **One-Click Switching** - Switch between Azure and LM Studio instantly
✅ **Built-in Test** - Test connection before saving
✅ **Smart UI** - Shows only relevant settings for selected provider
✅ **Easy Setup** - Step-by-step guidance for each provider
✅ **Status Feedback** - Clear success/error messages

## 📖 How to Access

### Method 1: Menu Bar
```
Settings → Azure OpenAI
```
(Now opens the unified AI Provider Config)

### Method 2: Keyboard Shortcut
```
Alt + S → A (Settings → Azure OpenAI)
```

## 🎨 The Configuration Form

### Layout

```
┌────────────────────────────────────────────────┐
│  Choose Your AI Provider                      │
├────────────────────────────────────────────────┤
│                                                │
│  AI Provider: [Azure OpenAI (Cloud)    ▼]     │
│                                                │
│  ☑ Enable this provider                       │
│                                                │
├────────────────────────────────────────────────┤
│                                                │
│  [Provider-Specific Settings Here]            │
│                                                │
│                                                │
├────────────────────────────────────────────────┤
│  Status: ✓ Connection successful!             │
│                                                │
│  [Test Connection]  [Cancel]  [Save Config]   │
└────────────────────────────────────────────────┘
```

## 🔧 Supported Providers

### 1. Azure OpenAI (Cloud) ☁️

**Best For:** Production use, enterprise applications, latest models

**Settings:**
- **Deployment Name** - Your model deployment (e.g., "gpt-4")
- **Endpoint URL** - Your Azure endpoint
- **API Key** - Your Azure API key

**How to Get Credentials:**
```
1. Go to https://portal.azure.com
2. Navigate to Azure OpenAI
3. Go to Keys and Endpoint
4. Copy your credentials
```

### 2. LM Studio (Local) 🖥️

**Best For:** Privacy, cost-free operation, offline work

**Settings:**
- **Server Endpoint** - Usually `http://localhost:1234/v1`
- **Model Name** - Usually `local-model` (auto-detected)
- **Temperature** - 0.0 to 2.0 (default: 0.7)
- **Max Tokens** - 128 to 32768 (default: 2048)

**Setup Steps:**
```
1. Download LM Studio from https://lmstudio.ai/
2. Load a model (Hermes-2-Pro-Mistral-7B recommended)
3. Click "Start Server" in LM Studio
4. Verify endpoint: http://localhost:1234/v1
5. Configure in FlowVision
```

### 3. GitHub Models (Free Tier) 🆓

**Coming Soon!**
For now, you can configure it as Azure OpenAI with GitHub endpoint.

## 📋 Step-by-Step Setup

### Setting Up Azure OpenAI

1. **Open Configuration**
   - Go to Settings → Azure OpenAI

2. **Select Provider**
   - Choose "Azure OpenAI (Cloud)" from dropdown

3. **Enter Credentials**
   - Deployment Name: Your model name
   - Endpoint URL: Your Azure endpoint
   - API Key: Your API key

4. **Test Connection**
   - Click "Test Connection"
   - Wait for "✓ Connection successful!"

5. **Save**
   - Click "Save Configuration"
   - Done! ✓

### Setting Up LM Studio

1. **Prepare LM Studio**
   - Open LM Studio
   - Load a model
   - Click "Start Server"

2. **Open Configuration**
   - Go to Settings → Azure OpenAI

3. **Select Provider**
   - Choose "LM Studio (Local)" from dropdown

4. **Verify Settings**
   - Endpoint: `http://localhost:1234/v1`
   - Model: `local-model`
   - Temperature: Adjust as needed
   - Max Tokens: Adjust as needed

5. **Test Connection**
   - Click "Test Connection"
   - Should show "✓ LM Studio connection successful!"
   - If error, check LM Studio server is running

6. **Save**
   - Click "Save Configuration"
   - LM Studio is now active! ✓

## 🔄 Switching Providers

### Switch from Azure to LM Studio

```
1. Open Settings → Azure OpenAI
2. Change dropdown to "LM Studio (Local)"
3. Verify/adjust settings
4. Click "Save Configuration"
→ Now using LM Studio! ✓
```

### Switch from LM Studio to Azure

```
1. Open Settings → Azure OpenAI
2. Change dropdown to "Azure OpenAI (Cloud)"
3. Enter Azure credentials
4. Click "Save Configuration"
→ Now using Azure! ✓
```

**Note:** No restart needed! Changes take effect immediately.

## 🎛️ Configuration Options

### Enable/Disable Provider

```
☑ Enable this provider
```

- **Checked:** Use this provider
- **Unchecked:** Fall back to default (Azure OpenAI)

### Temperature Setting (LM Studio)

| Value | Behavior | Best For |
|-------|----------|----------|
| 0.0-0.3 | Focused, deterministic | Code generation, facts |
| 0.4-0.7 | Balanced | General use, conversations |
| 0.8-1.2 | Creative, varied | Creative writing, brainstorming |
| 1.3-2.0 | Very random | Experimental, chaos |

### Max Tokens (LM Studio)

| Value | Response Length | Speed |
|-------|----------------|-------|
| 512 | Short | Fast ⚡ |
| 1024 | Medium | Normal |
| 2048 | Long | Slower |
| 4096+ | Very long | Slow 🐌 |

## ✅ Test Connection Feature

### What It Does

Sends a simple test message: "Say 'test' in one word"

### Success Indicators

**Azure OpenAI:**
```
✓ Azure OpenAI connection successful!
[Green text]
```

**LM Studio:**
```
✓ LM Studio connection successful!
[Green text]
```

### Common Errors

**Azure:**
```
✗ Connection failed: Unauthorized
→ Check API key is correct
```

```
✗ Connection failed: Not Found
→ Check deployment name and endpoint
```

**LM Studio:**
```
✗ Cannot connect. Is LM Studio running with server started?
→ Start LM Studio server
```

```
✗ Connection timeout
→ Increase timeout or use smaller model
```

## 💡 Pro Tips

### 1. Test Before Saving
Always click "Test Connection" before saving to verify settings work.

### 2. Keep Both Configured
You can switch between providers easily if both are configured:
- Azure for production
- LM Studio for testing/privacy

### 3. Model-Specific Settings
Each model (actioner, planner, coordinator) can use different providers!

### 4. Save Time with Defaults
LM Studio pre-fills sensible defaults - usually you just need to click Save.

### 5. Connection Issues?
- **Azure:** Check internet connection
- **LM Studio:** Check server is running
- **Both:** Click Test to see exact error

## 🎯 Use Cases

### Use Azure When:
- ✅ You need best quality (GPT-4)
- ✅ You want guaranteed uptime
- ✅ You're okay with cloud usage
- ✅ You have API budget

### Use LM Studio When:
- ✅ You want complete privacy
- ✅ You want zero costs
- ✅ You need offline capability
- ✅ You have good hardware

### Switch Between Them:
- 📊 Development: LM Studio (free testing)
- 🚀 Production: Azure (reliability)
- 🔒 Sensitive Data: LM Studio (privacy)
- 💰 High Volume: LM Studio (no costs)

## 🐛 Troubleshooting

### Form Not Opening
- Check if another config window is open
- Close other dialogs first

### Settings Not Saving
- Ensure you clicked "Save Configuration"
- Check file permissions for %APPDATA%\FlowVision

### Provider Not Switching
- Verify you saved after changing
- Check status message confirms save
- Try restarting app if issue persists

### Test Connection Fails
- **Timeout:** Increase wait time, check connection
- **Unauthorized:** Verify API key
- **Not Found:** Check endpoint URL
- **Connection Refused:** Start LM Studio server

## 📊 Configuration Files

### Where Settings Are Stored

**Azure OpenAI:**
```
%APPDATA%\FlowVision\actionerapiconfig.json
```

**LM Studio:**
```
%APPDATA%\FlowVision\lmstudioconfig.json
```

### Manual Editing

You can manually edit these JSON files if needed:

**Azure Config:**
```json
{
  "DeploymentName": "gpt-4",
  "EndpointURL": "https://your-endpoint.openai.azure.com/",
  "APIKey": "your-key-here",
  "ProviderType": "AzureOpenAI"
}
```

**LM Studio Config:**
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

## 🎉 Summary

The new AI Provider Configuration UI makes it **super easy** to:

- ✅ Choose your AI provider visually
- ✅ Test connections before committing
- ✅ Switch providers with one click
- ✅ See only relevant settings
- ✅ Get instant feedback

**No more editing JSON files! No more guessing! Everything is visual and intuitive!** 🎊

---

**Access:** Settings → Azure OpenAI
**Keyboard:** Alt + S, A
**Ready to Use:** YES ✓

Enjoy your new provider flexibility! 🚀
