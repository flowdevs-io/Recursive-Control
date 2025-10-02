using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Azure;
using Azure.AI.Inference;
using FlowVision.lib.Plugins;
using FlowVision.Properties;
using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;
using ChatRole = Microsoft.Extensions.AI.ChatRole;

namespace FlowVision.lib.Classes
{
    public class Github_Actioner
    {
        private IChatClient _chat;
        private List<ChatMessage> _history;
                private RichTextBox _output;
        private const string ACTIONER_CONFIG = "github";
        private const string TOOL_CONFIG = "toolsconfig";

        public Github_Actioner(RichTextBox outputTextBox)
        {
            _output = outputTextBox;
            _history = new List<ChatMessage>();
            
            // Load tool config to get system message
            ToolConfig toolConfig = ToolConfig.LoadConfig(TOOL_CONFIG);
            
            // Set system message from config
            _history.Add(new ChatMessage(ChatRole.System, toolConfig.ActionerSystemPrompt));
        }

        public async Task<string> ExecuteAction(string actionPrompt)
        {
            _history.Add(new ChatMessage(ChatRole.User, actionPrompt));

            // 1. Load your GitHub‑Models config
            APIConfig config = APIConfig.LoadConfig(ACTIONER_CONFIG);
            ToolConfig toolConfig = ToolConfig.LoadConfig(TOOL_CONFIG);

            if (string.IsNullOrWhiteSpace(config.EndpointURL) ||
                string.IsNullOrWhiteSpace(config.APIKey) ||
                string.IsNullOrWhiteSpace(config.DeploymentName))
            {
                _output.AppendText("Error: Actioner model not configured\n\n");
                return "Error: Actioner model not configured";
            }

            // 2. Create the GitHub Models client via Azure AI Inference
            var chatClient = new ChatCompletionsClient(
                new Uri(config.EndpointURL),            // e.g. "https://models.inference.ai.azure.com"
                new AzureKeyCredential(config.APIKey)   // your GITHUB_TOKEN
            );

            // 3. Convert to IChatClient using Microsoft.Extensions.AI
            var aiChatClient = chatClient.AsIChatClient(config.DeploymentName);

            // 4. Add tool plugins using helper
            var tools = new List<AITool>();
            
            tools.AddRange(PluginToolExtractor.ExtractTools(new CMDPlugin()));
            tools.AddRange(PluginToolExtractor.ExtractTools(new PowerShellPlugin()));
            tools.AddRange(PluginToolExtractor.ExtractTools(new ScreenCapturePlugin()));
            tools.AddRange(PluginToolExtractor.ExtractTools(new KeyboardPlugin()));
            tools.AddRange(PluginToolExtractor.ExtractTools(new MousePlugin()));

            // 5. Enable function invocation
            _chat = new ChatClientBuilder(aiChatClient).UseFunctionInvocation().Build();

            // 6. Define your chat options
            var options = new ChatOptions
            {
                Temperature = 0.2f,
                Tools = tools
            };

            // Process the response with streaming
            var responseBuilder = new StringBuilder();
            await foreach (var update in _chat.GetStreamingResponseAsync(_history, options))
            {
                if (update.Text != null)
                {
                    responseBuilder.Append(update.Text);
                }
            }

            string response = responseBuilder.ToString();
            
            // Add response to history
            if (!string.IsNullOrEmpty(response))
            {
                _history.Add(new ChatMessage(ChatRole.Assistant, response));
            }

            return response;
        }
    }
}
