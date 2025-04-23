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
        private RichTextBox outputTextBox;
        private const string ACTIONER_CONFIG = "actioner";
        private const string TOOL_CONFIG = "toolsconfig"; // Added constant for tool config

        public object ToolCallBehavior { get; private set; }

        // Update the Actioner constructor to support both the delegate and the RichTextBox approaches
        public Actioner(Form1.PluginOutputHandler outputHandler)
        {
            actionerHistory = new ChatHistory();

            // Create a RichTextBox that isn't displayed but used for logging
            var hiddenTextBox = new RichTextBox { Visible = false };

            // Initialize the plugin logger with the hidden text box
            PluginLogger.Initialize(hiddenTextBox);

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

            this.outputTextBox = hiddenTextBox;
        }

        public async Task<string> ExecuteAction(string actionPrompt)
        {

            ToolConfig toolConfig = ToolConfig.LoadConfig(TOOL_CONFIG);
            // Add system message to actioner history
            actionerHistory.AddSystemMessage(toolConfig.SystemPrompt);

            // Add action prompt to actioner history
            actionerHistory.AddUserMessage(actionPrompt);

            // Load actioner model config
            APIConfig config = APIConfig.LoadConfig(ACTIONER_CONFIG);
            // Changed to use the same config file as ToolConfigForm

            if (string.IsNullOrWhiteSpace(config.DeploymentName) ||
                string.IsNullOrWhiteSpace(config.EndpointURL) ||
                string.IsNullOrWhiteSpace(config.APIKey))
            {
                outputTextBox.AppendText("Error: Actioner model not configured\n\n");
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
            
            // Log which plugins are being enabled
            outputTextBox.AppendText("Enabling the following plugins:\n");
            
            // Add plugins dynamically based on tool configuration
            if (toolConfig.EnableCMDPlugin)
            {
                builder.Plugins.AddFromType<CMDPlugin>();
                outputTextBox.AppendText("- CMD Plugin\n");
            }
                
            if (toolConfig.EnablePowerShellPlugin)
            {
                builder.Plugins.AddFromType<PowerShellPlugin>();
                outputTextBox.AppendText("- PowerShell Plugin\n");
            }
                
            if (toolConfig.EnableScreenCapturePlugin)
            {
                builder.Plugins.AddFromType<ScreenCaptureOmniParserPlugin>();
                outputTextBox.AppendText("- Screen Capture Plugin\n");
            }
                
            if (toolConfig.EnableKeyboardPlugin)
            {
                builder.Plugins.AddFromType<KeyboardPlugin>();
                outputTextBox.AppendText("- Keyboard Plugin\n");
            }
                
            if (toolConfig.EnableMousePlugin)
            {
                builder.Plugins.AddFromType<MousePlugin>();
                outputTextBox.AppendText("- Mouse Plugin\n");
            }
                
            if (toolConfig.EnableWindowSelectionPlugin) // Add this conditional
            {
                builder.Plugins.AddFromType<WindowSelectionPlugin>();
                outputTextBox.AppendText("- Window Selection Plugin\n");
            }

            outputTextBox.AppendText("\n");

            builder.Services.AddSingleton(outputTextBox);

            actionerKernel = builder.Build();
            actionerChat = actionerKernel.GetRequiredService<IChatCompletionService>();

            // Process the response
            var responseBuilder = new StringBuilder();
            // Uncomment for Open AI
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

            string response = responseBuilder.ToString();

            return responseBuilder.ToString();
        }

        internal void SetChatHistory(List<ChatMessage> chatHistory)
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
        }
    }
}