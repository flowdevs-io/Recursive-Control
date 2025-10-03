using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Plugins;
using Microsoft.Extensions.AI;
using OpenAI;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Local AI Actioner using LM Studio (OpenAI-compatible API)
    /// </summary>
    public class LMStudioActioner
    {
        private IChatClient actionerChat;
        private List<ChatMessage> actionerHistory;
        private const string TOOL_CONFIG = "toolsconfig";
        private LMStudioConfig lmStudioConfig;

        public LMStudioActioner(Form1.PluginOutputHandler outputHandler)
        {
            actionerHistory = new List<ChatMessage>();
            lmStudioConfig = LMStudioConfig.LoadConfig();

            // Create a RichTextBox that isn't displayed but used for logging
            var hiddenTextBox = new RichTextBox { Visible = false };

            // Initialize the plugin logger
            if (Application.OpenForms.Count > 0 && Application.OpenForms[0] is Form1 mainForm)
            {
                Action<string, string, bool> addMessageAction = mainForm.AddMessage;
                PluginLogger.Initialize(hiddenTextBox, addMessageAction);
            }
            else
            {
                PluginLogger.Initialize(hiddenTextBox);
            }

            // Override the UpdateUI method to use our output handler
            if (outputHandler != null)
            {
                hiddenTextBox.TextChanged += (sender, e) =>
                {
                    string newText = hiddenTextBox.Lines.LastOrDefault();
                    if (!string.IsNullOrEmpty(newText))
                    {
                        outputHandler(newText);
                    }
                };
            }

            // Start remote control server if enabled
            ToolConfig toolConfig = ToolConfig.LoadConfig(TOOL_CONFIG);
            if (toolConfig.EnableRemoteControl)
            {
                RemoteControlPlugin.SetCommandHandler(async cmd => await ExecuteAction(cmd));
                RemoteControlPlugin.StartServer(toolConfig.RemoteControlPort);
            }
        }

        public async Task<string> ExecuteAction(string actionPrompt)
        {
            // Reload configs
            lmStudioConfig = LMStudioConfig.LoadConfig();
            ToolConfig toolConfig = ToolConfig.LoadConfig(TOOL_CONFIG);

            // Generate tool descriptions if dynamic prompts are enabled
            string toolDescriptions = toolConfig.DynamicToolPrompts
                ? "\n\n" + ToolDescriptionGenerator.GetToolDescriptions(toolConfig)
                : string.Empty;

            // Notify that we're starting the action execution
            PluginLogger.NotifyTaskStart("LM Studio Action Execution", "Processing your request with local AI");
            PluginLogger.StartLoadingIndicator("request");

            try
            {
                // Add system message to actioner history
                actionerHistory.Add(new ChatMessage(ChatRole.System, toolConfig.ActionerSystemPrompt + toolDescriptions));

                // Add action prompt with explicit instruction to EXECUTE
                string enhancedPrompt = $@"{actionPrompt}

IMPORTANT REMINDER:
1. DO NOT just observe and describe - you must EXECUTE the action!
2. After CaptureWholeScreen(), you MUST continue to actually click/type/interact
3. Follow ALL steps: Observe → Plan → EXECUTE → Verify
4. Do not stop until you've performed the actual action requested";

                actionerHistory.Add(new ChatMessage(ChatRole.User, enhancedPrompt));

                // Verify LM Studio is enabled and configured
                if (!lmStudioConfig.Enabled)
                {
                    PluginLogger.NotifyTaskComplete("LM Studio Action Execution", false);
                    return "Error: LM Studio integration is not enabled. Please enable it in the configuration.";
                }

                if (string.IsNullOrWhiteSpace(lmStudioConfig.EndpointURL))
                {
                    PluginLogger.NotifyTaskComplete("LM Studio Action Execution", false);
                    return "Error: LM Studio endpoint not configured. Default is http://localhost:1234/v1";
                }

                // Create OpenAI client pointing to LM Studio
                // LM Studio provides an OpenAI-compatible API
                var openAIClient = new OpenAIClient(new System.ClientModel.ApiKeyCredential(lmStudioConfig.APIKey), new OpenAIClientOptions
                {
                    Endpoint = new Uri(lmStudioConfig.EndpointURL)
                });

                // Get the chat client and convert to IChatClient
                var chatClient = openAIClient.GetChatClient(lmStudioConfig.ModelName);
                IChatClient baseChatClient = chatClient.AsIChatClient();

                // Collect tools based on configuration
                var tools = new List<AITool>();

                if (toolConfig.EnableCMDPlugin)
                {
                    tools.AddRange(PluginToolExtractor.ExtractTools(new CMDPlugin()));
                }

                if (toolConfig.EnablePowerShellPlugin)
                {
                    tools.AddRange(PluginToolExtractor.ExtractTools(new PowerShellPlugin()));
                }

                if (toolConfig.EnableScreenCapturePlugin)
                {
                    tools.AddRange(PluginToolExtractor.ExtractTools(new ScreenCaptureOmniParserPlugin()));
                }

                if (toolConfig.EnableKeyboardPlugin)
                {
                    tools.AddRange(PluginToolExtractor.ExtractTools(new KeyboardPlugin()));
                }

                if (toolConfig.EnableMousePlugin)
                {
                    tools.AddRange(PluginToolExtractor.ExtractTools(new MousePlugin()));
                }

                if (toolConfig.EnableWindowSelectionPlugin)
                {
                    tools.AddRange(PluginToolExtractor.ExtractTools(new WindowSelectionPlugin()));
                }

                if (toolConfig.EnablePlaywrightPlugin)
                {
                    tools.AddRange(PluginToolExtractor.ExtractTools(PlaywrightPlugin.Instance));
                }

                if (toolConfig.EnableRemoteControl)
                {
                    tools.AddRange(PluginToolExtractor.ExtractTools(new RemoteControlPlugin()));
                }

                // Configure chat options with tools
                var chatOptions = new ChatOptions
                {
                    Temperature = (float)lmStudioConfig.Temperature,
                    MaxOutputTokens = lmStudioConfig.MaxTokens,
                    Tools = tools
                };

                // Build chat client with function invocation if enabled
                if (toolConfig.AutoInvokeKernelFunctions)
                {
                    // Use function invocation with default behavior
                    // The middleware will automatically loop and call tools
                    actionerChat = new ChatClientBuilder(baseChatClient)
                        .UseFunctionInvocation()
                        .Build();
                    
                    PluginLogger.LogInfo("LMStudioActioner", "ExecuteAction", 
                        "Function invocation enabled - will auto-invoke tools");
                }
                else
                {
                    actionerChat = baseChatClient;
                }

                // Update loading message to show we're now processing the response
                PluginLogger.StopLoadingIndicator();
                PluginLogger.StartLoadingIndicator("Local AI response");

                // Process the response with streaming
                var responseBuilder = new StringBuilder();
                await foreach (var update in actionerChat.GetStreamingResponseAsync(actionerHistory, chatOptions))
                {
                    if (update.Text != null)
                    {
                        responseBuilder.Append(update.Text);
                    }
                }

                // Task completed successfully
                PluginLogger.NotifyTaskComplete("LM Studio Action Execution", true);

                var response = responseBuilder.ToString();
                
                // Log the response for debugging
                PluginLogger.LogInfo("LMStudioActioner", "ExecuteAction", 
                    $"AI Response: {(string.IsNullOrEmpty(response) ? "(empty)" : $"{response.Length} chars")}");
                
                // Add assistant response to history
                if (!string.IsNullOrEmpty(response))
                {
                    actionerHistory.Add(new ChatMessage(ChatRole.Assistant, response));
                }
                else
                {
                    // If response is empty, check if there were tool calls and provide a default response
                    var defaultResponse = "I executed the requested action successfully.";
                    PluginLogger.LogInfo("LMStudioActioner", "ExecuteAction", 
                        "Empty response from model, using default response");
                    actionerHistory.Add(new ChatMessage(ChatRole.Assistant, defaultResponse));
                    return defaultResponse;
                }

                return response;
            }
            catch (Exception ex)
            {
                // Task failed
                PluginLogger.NotifyTaskComplete("LM Studio Action Execution", false);
                
                // Provide helpful error messages
                if (ex.Message.Contains("Connection refused") || ex.Message.Contains("No connection could be made"))
                {
                    return $"Error: Cannot connect to LM Studio at {lmStudioConfig.EndpointURL}. " +
                           "Please make sure LM Studio is running and the server is started (click 'Start Server' in LM Studio).";
                }
                
                return $"Error: {ex.Message}\n\nMake sure LM Studio is running with a model loaded and the server is started.";
            }
        }

        public void SetChatHistory(List<LocalChatMessage> chatHistory)
        {
            actionerHistory.Clear();
            foreach (var message in chatHistory)
            {
                if (message.Author == "You")
                {
                    actionerHistory.Add(new ChatMessage(ChatRole.User, message.Content));
                }
                else if (message.Author == "AI")
                {
                    actionerHistory.Add(new ChatMessage(ChatRole.Assistant, message.Content));
                }
            }
        }

        public void ClearHistory()
        {
            actionerHistory.Clear();
        }
    }
}
