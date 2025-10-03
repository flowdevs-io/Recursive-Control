# LM Studio Quick Start - 5 Minutes to Local AI! 🚀

## What You Get
- ✅ **Run AI locally** on your machine
- ✅ **Complete privacy** - no data sent to cloud
- ✅ **No API costs** - completely free
- ✅ **Works offline** after setup
- ✅ **Same features** as Azure OpenAI

## Step 1: Download LM Studio (2 min)

1. Go to **https://lmstudio.ai/**
2. Click **Download** for your OS (Windows/Mac/Linux)
3. Install and launch LM Studio

## Step 2: Get a Model (3 min)

### Recommended Starting Model: **Hermes-2-Pro-Mistral-7B**

In LM Studio:
1. Click the **🔍 Search** icon (top left)
2. Search for: **`hermes-2-pro`**
3. Find: **`NousResearch/Hermes-2-Pro-Mistral-7B-GGUF`**
4. Click **Download** next to **`Q4_K_M`** version (~4GB)
5. Wait for download to complete

### Alternative Quick Models:
- **Fast & Lightweight**: `Phi-2-GGUF` (~1.6GB) - Great for testing
- **Best Quality**: `Mixtral-8x7B-Instruct-GGUF` (~26GB) - Needs 32GB+ RAM

## Step 3: Start LM Studio Server (30 sec)

1. In LM Studio, click **💻 Local Server** tab (left sidebar)
2. Select your downloaded model from the dropdown
3. Click **Start Server** button
4. Wait for "Server started on http://localhost:1234"
5. **Keep LM Studio running!**

## Step 4: Configure FlowVision (1 min)

### Option A: Using the UI (Easiest)
1. Open FlowVision
2. Go to **Settings** → **LM Studio Configuration**
3. Check ☑ **"Enable LM Studio"**
4. Click **"Test Connection"** (should show ✓ success)
5. Click **"Save"**

### Option B: Manual Configuration
Create file: `%APPDATA%\FlowVision\lmstudioconfig.json`
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

## Step 5: Test It! (30 sec)

1. In FlowVision, type: **"What is 2+2?"**
2. You should see: **"Local AI response"** indicator
3. Get a response from your local model!

## ✅ Success Indicators

You'll know it's working when you see:
- ✅ "LM Studio Action Execution" in the task indicator
- ✅ "Processing your request with local AI" message
- ✅ "Local AI response" during generation
- ✅ Responses coming without internet connection

## 🎯 Try These Commands

### Simple Test
```
"Tell me a joke"
```

### Tool Calling Test
```
"Open Notepad"
"What windows are open?"
"Create a file called test.txt with the content 'Hello World'"
```

### Code Generation
```
"Write a Python function to calculate fibonacci numbers"
```

## 🐛 Troubleshooting

### "Cannot connect to LM Studio"
**Fix**: 
1. Make sure LM Studio is running
2. Check that server is started (green "Stop Server" button visible)
3. Verify endpoint: `http://localhost:1234/v1`

### Model is too slow
**Fix**:
1. Try a smaller model (Phi-2)
2. Enable GPU acceleration in LM Studio settings
3. Close other applications
4. Reduce Max Tokens to 1024

### "Connection timeout"
**Fix**:
1. First request is slower (model loading)
2. Increase timeout in config (300 → 600 seconds)
3. Wait a bit longer
4. Check CPU/RAM usage

### Test Connection shows error
**Fix**:
1. Restart LM Studio server
2. Reload model in LM Studio
3. Check firewall isn't blocking port 1234
4. Try changing port in LM Studio server settings

## 📊 Performance Tips

### For Faster Responses:
- **Temperature**: 0.2 (more focused)
- **Max Tokens**: 1024 (shorter responses)
- **Model**: Use Phi-2 or TinyLlama

### For Better Quality:
- **Temperature**: 0.7-0.8 (more creative)
- **Max Tokens**: 2048-4096 (longer responses)
- **Model**: Use Mixtral or Llama-2-13B

### For Tool Calling:
- **Best Model**: Hermes-2-Pro-Mistral-7B
- **Temperature**: 0.3-0.5
- **Max Tokens**: 1024

## 🔄 Switching Between Local and Azure

### Enable Local AI:
Check ☑ "Enable LM Studio" in settings

### Disable (Use Azure):
Uncheck ☐ "Enable LM Studio" in settings

**No restart needed!** FlowVision automatically switches.

## 💡 Pro Tips

1. **Keep LM Studio running** - Don't close it while using FlowVision
2. **First request is slow** - Model loading takes time, be patient
3. **Monitor resources** - Check Task Manager for CPU/RAM usage
4. **Try different models** - Each has different strengths
5. **Use GPU** - Enable in LM Studio settings for huge speed boost

## 📱 Quick Reference

| Setting | Recommended | Fast | Quality |
|---------|------------|------|---------|
| Model | Hermes-2-Pro-7B | Phi-2 | Mixtral-8x7B |
| Temperature | 0.5 | 0.2 | 0.7 |
| Max Tokens | 2048 | 1024 | 4096 |
| RAM Needed | 16GB | 8GB | 32GB+ |

## 🎉 That's It!

You're now running AI locally! 

**Next Steps:**
- Try different models
- Experiment with settings
- Test tool calling features
- Read full documentation: LMSTUDIO_INTEGRATION.md

## 🆘 Need Help?

1. Check full guide: `LMSTUDIO_INTEGRATION.md`
2. LM Studio docs: https://lmstudio.ai/docs
3. LM Studio Discord: https://discord.gg/lmstudio
4. GitHub Issues: (your repo)

---

**Enjoy your private, local AI!** 🎊

No more API costs! No more cloud dependency! Complete control! 🚀
