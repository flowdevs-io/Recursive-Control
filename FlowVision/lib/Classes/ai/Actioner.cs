using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FlowVision.lib.Classes
{
    public class Actioner
    {
        private IChatCompletionService actionerChat;
        private ChatHistory actionerHistory;
        private Kernel actionerKernel;
        private const string ACTIONER_CONFIG = "actioner";
        private const string TOOL_CONFIG = "toolsconfig"; // Added constant for tool config

        private MultiAgentActioner multiAgentActioner;
        private bool useMultiAgentMode = false;

        public object ToolCallBehavior { get; private set; }

        // Update the Actioner constructor to support both the delegate and the RichTextBox approaches
        public Actioner(Form1.PluginOutputHandler outputHandler)
        {
            actionerHistory = new ChatHistory();

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

            // Initialize the multi-agent actioner with the same output handler
            multiAgentActioner = new MultiAgentActioner(outputHandler);
        }

        // Add method to toggle multi-agent mode
        public void SetMultiAgentMode(bool enabled)
        {
            useMultiAgentMode = enabled;
        }

        public async Task<string> ExecuteAction(string actionPrompt)
        {
            // If multi-agent mode is enabled, use the multi-agent actioner
            if (useMultiAgentMode)
            {
                return await multiAgentActioner.ExecuteAction(actionPrompt);
            }

            // Otherwise use the original implementation
            ToolConfig toolConfig = ToolConfig.LoadConfig(TOOL_CONFIG);

            // Notify that we're starting the action execution
            PluginLogger.NotifyTaskStart("Action Execution", "Processing your request");
            PluginLogger.StartLoadingIndicator("request");

            try
            {
                // Add system message to actioner history
                actionerHistory.AddSystemMessage(toolConfig.ActionerSystemPrompt);

                // Add action prompt to actioner history
                actionerHistory.AddUserMessage(actionPrompt);

                // Load actioner model config
                APIConfig config = APIConfig.LoadConfig(ACTIONER_CONFIG);

                if (string.IsNullOrWhiteSpace(config.DeploymentName) ||
                    string.IsNullOrWhiteSpace(config.EndpointURL) ||
                    string.IsNullOrWhiteSpace(config.APIKey))
                {
                    PluginLogger.NotifyTaskComplete("Action Execution", false);
                    return "Error: Actioner model not configured";
                }

                // Setup the kernel for actioner with plugins
                var builder = Kernel.CreateBuilder();
                builder.AddAzureOpenAIChatCompletion(
                    config.DeploymentName,
                    config.EndpointURL,
                    config.APIKey);

                // Configure OpenAI settings based on toolConfig
                var settings = new OpenAIPromptExecutionSettings
                {
                    Temperature = toolConfig.Temperature,
                    ToolCallBehavior = toolConfig.AutoInvokeKernelFunctions
                        ? Microsoft.SemanticKernel.Connectors.OpenAI.ToolCallBehavior.AutoInvokeKernelFunctions
                        : Microsoft.SemanticKernel.Connectors.OpenAI.ToolCallBehavior.EnableKernelFunctions
                };

                // Add plugins dynamically based on tool configuration
                if (toolConfig.EnableCMDPlugin)
                {
                    builder.Plugins.AddFromType<CMDPlugin>();
                }

                if (toolConfig.EnablePowerShellPlugin)
                {
                    builder.Plugins.AddFromType<PowerShellPlugin>();
                }

                if (toolConfig.EnableScreenCapturePlugin)
                {
                    builder.Plugins.AddFromType<ScreenCaptureOmniParserPlugin>();
                }

                if (toolConfig.EnableKeyboardPlugin)
                {
                    builder.Plugins.AddFromType<KeyboardPlugin>();
                }

                if (toolConfig.EnableMousePlugin)
                {
                    builder.Plugins.AddFromType<MousePlugin>();
                }

                if (toolConfig.EnableWindowSelectionPlugin)
                {
                    builder.Plugins.AddFromType<WindowSelectionPlugin>();
                }

                if (toolConfig.EnablePlaywrightPlugin)
                {
                    builder.Plugins.AddFromType<PlaywrightPlugin>();
                }   

                actionerKernel = builder.Build();
                actionerChat = actionerKernel.GetRequiredService<IChatCompletionService>();

                // Update loading message to show we're now processing the response
                PluginLogger.StopLoadingIndicator();
                PluginLogger.StartLoadingIndicator("AI response");

                // Process the response
                var responseBuilder = new StringBuilder();
                var responseStream = actionerChat.GetStreamingChatMessageContentsAsync(actionerHistory, settings, actionerKernel);
                var enumerator = responseStream.GetAsyncEnumerator();
                try
                {
                    while (await enumerator.MoveNextAsync())
                    {
                        var message = enumerator.Current;
                        if (message.Content == "None") continue;
                        responseBuilder.Append(message.Content);
                    }
                }
                finally
                {
                    await enumerator.DisposeAsync();
                }

                // Task completed successfully
                PluginLogger.NotifyTaskComplete("Action Execution", true);

                return responseBuilder.ToString();
            }
            catch (Exception ex)
            {
                // Task failed
                PluginLogger.NotifyTaskComplete("Action Execution", false);
                return $"Error: {ex.Message}";
            }
        }

        internal void SetChatHistory(List<LocalChatMessage> chatHistory)
        {
            actionerHistory.Clear();
            foreach (var message in chatHistory)
            {
                if (message.Author == "You")
                {
                    actionerHistory.AddUserMessage(message.Content);
                }
                else if (message.Author == "AI")
                {
                    actionerHistory.AddAssistantMessage(message.Content);
                }
            }

            // Also update the multi-agent chat history
            multiAgentActioner.SetChatHistory(chatHistory);
        }
    }
}