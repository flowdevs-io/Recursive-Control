using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Azure;
using Azure.AI.Inference;
using FlowVision.lib.Plugins;
using FlowVision.Properties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureAIInference;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FlowVision.lib.Classes
{
    public class Github_Actioner
    {
        private IChatCompletionService _chat;
        private ChatHistory _history;
        private Kernel _kernel;
        private RichTextBox _output;
        private const string ACTIONER_CONFIG = "github";

        public Github_Actioner(RichTextBox outputTextBox)
        {
            _output = outputTextBox;
            _history = new ChatHistory();
            _history.AddSystemMessage(
                "You are an AI Agent that uses tools to control a windows computer"
            );
        }

        public async Task<string> ExecuteAction(string actionPrompt)
        {
            _history.AddUserMessage(actionPrompt);

            // 1. Load your GitHub‑Models config
            APIConfig config = APIConfig.LoadConfig(ACTIONER_CONFIG);
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

            // 3. Wire it into Semantic Kernel
            var builder = Kernel.CreateBuilder();
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            builder.AddAzureAIInferenceChatCompletion(
                modelId: config.DeploymentName,  // e.g. "openai/gpt-4.1"
                chatClient: chatClient
            );
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            // 4. Add your tool‑plugins
            builder.Plugins.AddFromType<CMDPlugin>();
            builder.Plugins.AddFromType<PowerShellPlugin>();
            builder.Plugins.AddFromType<ScreenCapturePlugin>();
            builder.Plugins.AddFromType<KeyboardPlugin>();
            builder.Plugins.AddFromType<MousePlugin>();

            // 5. Inject the output box for plugins that need it
            builder.Services.AddSingleton(_output);

            // 6. Build and grab IChatCompletionService
            _kernel = builder.Build();
            _chat = _kernel.GetRequiredService<IChatCompletionService>();

            // 7. Define your PromptExecutionSettings
            var settings = new OpenAIPromptExecutionSettings
            {
                Temperature = 0.2,
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };


            // Process the response
            var responseBuilder = new StringBuilder();
            // Uncomment for Open AI
            var responseStream = _chat.GetStreamingChatMessageContentsAsync(_history, settings, _kernel);
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

            _history.AddAssistantMessage(response);
            return response;
        }
    }
}
