using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Plugins;
using Microsoft.Extensions.AI;
using FlowVision.lib.Classes.ai;
using FlowVision; // Required for Form1

namespace FlowVision.lib.Classes
{
    public class Actioner
    {
        private IChatClient actionerChat;
        private List<ChatMessage> actionerHistory;
        private const string ACTIONER_CONFIG = "actioner";
        private const string TOOL_CONFIG = "toolsconfig";

        private MultiAgentActioner multiAgentActioner;
        private LMStudioActioner lmStudioActioner;
        private bool useMultiAgentMode = false;

        public object ToolCallBehavior { get; private set; }

        // Update the Actioner constructor to support both the delegate and the RichTextBox approaches
        public Actioner(Form1.PluginOutputHandler outputHandler)
        {
            actionerHistory = new List<ChatMessage>();

            // Create a RichTextBox that isn't displayed but used for logging
            var hiddenTextBox = new RichTextBox { Visible = false };

            // Initialize the plugin logger with the hidden text box
            // Pass the direct UI update delegate that takes the message and adds it to the chat UI
            if (Application.OpenForms.Count > 0 && Application.OpenForms[0] is Form1 mainForm)
            {
                // Get reference to the AddMessage method on the Form1 instance
                Action<string, string, bool> addMessageAction = mainForm.AddMessage;
                PluginLogger.Initialize(hiddenTextBox, addMessageAction);
            }
            else
            {
                // Fall back to just using the hidden text box
                PluginLogger.Initialize(hiddenTextBox);
            }

            // Override the UpdateUI method to use our output handler
            if (outputHandler != null)
            {
                // Update UI via the handler when text is added to the hidden textbox
                hiddenTextBox.TextChanged += (sender, e) =>
                {
                    string newText = hiddenTextBox.Lines.LastOrDefault();
                    if (!string.IsNullOrEmpty(newText))
                    {
                        outputHandler(newText);
                    }
                };
            }

            // Initialize the multi-agent actioner and LM Studio actioner with the same output handler
            multiAgentActioner = new MultiAgentActioner(outputHandler);
            lmStudioActioner = new LMStudioActioner(outputHandler);

            // Start remote control server if enabled
            ToolConfig toolConfigInit = ToolConfig.LoadConfig(TOOL_CONFIG);
            if (toolConfigInit.EnableRemoteControl)
            {
                RemoteControlPlugin.SetCommandHandler(async cmd => await ExecuteAction(cmd));
                RemoteControlPlugin.StartServer(toolConfigInit.RemoteControlPort);
            }
        }

        // Add method to toggle multi-agent mode
        public void SetMultiAgentMode(bool enabled)
        {
            useMultiAgentMode = enabled;
        }

        public async Task<string> ExecuteAction(string actionPrompt)
        {
            // Check if LM Studio is enabled first
            var lmStudioConfig = LMStudioConfig.LoadConfig();
            if (lmStudioConfig.Enabled)
            {
                return await lmStudioActioner.ExecuteAction(actionPrompt);
            }

            // If multi-agent mode is enabled, use the multi-agent actioner
            if (useMultiAgentMode)
            {
                return await multiAgentActioner.ExecuteAction(actionPrompt);
            }

            // Otherwise use the original Azure implementation
            ToolConfig toolConfig = ToolConfig.LoadConfig(TOOL_CONFIG);

            // Generate tool descriptions if dynamic prompts are enabled
            string toolDescriptions = toolConfig.DynamicToolPrompts
                ? "\n\n" + ToolDescriptionGenerator.GetToolDescriptions(toolConfig)
                : string.Empty;

            // Notify that we're starting the action execution
            PluginLogger.NotifyTaskStart("Action Execution", "Processing your request");
            PluginLogger.StartLoadingIndicator("request");

            try
            {
                // Add system message to actioner history
                actionerHistory.Add(new ChatMessage(ChatRole.System, toolConfig.ActionerSystemPrompt + toolDescriptions));

                // Add action prompt to actioner history
                actionerHistory.Add(new ChatMessage(ChatRole.User, actionPrompt));

                // Load actioner model config
                APIConfig config = APIConfig.LoadConfig(ACTIONER_CONFIG);

                if (string.IsNullOrWhiteSpace(config.DeploymentName) ||
                    string.IsNullOrWhiteSpace(config.EndpointURL) ||
                    string.IsNullOrWhiteSpace(config.APIKey))
                {
                    PluginLogger.NotifyTaskComplete("Action Execution", false);
                    return "Error: Actioner model not configured";
                }

                // Use the Factory to create the client based on ProviderType
                IChatClient baseChatClient = AIClientFactory.CreateClient(config);

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

                if (toolConfig.EnableClipboardPlugin)
                {
                    tools.AddRange(PluginToolExtractor.ExtractTools(new ClipboardPlugin()));
                }

                if (toolConfig.EnableFileSystemPlugin)
                {
                    tools.AddRange(PluginToolExtractor.ExtractTools(new FileSystemPlugin()));
                }

                // Configure chat options with tools
                var chatOptions = new ChatOptions
                {
                    Temperature = (float)toolConfig.Temperature,
                    Tools = tools
                };

                // Build chat client with function invocation if enabled
                actionerChat = toolConfig.AutoInvokeKernelFunctions 
                    ? new ChatClientBuilder(baseChatClient).UseFunctionInvocation().Build()
                    : baseChatClient;

                // Update loading message to show we're now processing the response
                PluginLogger.StopLoadingIndicator();
                PluginLogger.StartLoadingIndicator("AI response");

                // Process the response with streaming
                var responseBuilder = new StringBuilder();
                try
                {
                    await foreach (var update in actionerChat.GetStreamingResponseAsync(actionerHistory, chatOptions))
                    {
                        if (update.Text != null)
                        {
                            responseBuilder.Append(update.Text);
                        }
                    }
                }
                catch (Exception ex) when (ex.Message.Contains("Unknown ChatFinishReason") || ex.Message.Contains("function_call_filter"))
                {
                    PluginLogger.LogInfo("Actioner", "ExecuteAction", $"Ignored known SDK finish reason error: {ex.Message}");
                }

                // Task completed successfully
                PluginLogger.NotifyTaskComplete("Action Execution", true);

                var response = responseBuilder.ToString();
                
                // Add assistant response to history
                if (!string.IsNullOrEmpty(response))
                {
                    actionerHistory.Add(new ChatMessage(ChatRole.Assistant, response));
                }

                return response;
            }
            catch (Exception ex)
            {
                // Task failed
                PluginLogger.NotifyTaskComplete("Action Execution", false);
                return $"Error: {ex.Message}";
            }
        }

        internal void SetChatHistory(System.Collections.Generic.List<FlowVision.LocalChatMessage> chatHistory)
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

            // Also update the multi-agent and LM Studio chat history
            multiAgentActioner.SetChatHistory(chatHistory);
            lmStudioActioner.SetChatHistory(chatHistory);
        }
    }
}