using System;
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

        public Actioner(RichTextBox outputTextBox)
        {
            this.outputTextBox = outputTextBox;
            actionerHistory = new ChatHistory();

            // Set system message
            actionerHistory.AddSystemMessage("You are an AI Agent that can run powershell commands to help the user." +
                "Do not restart the server");
        }


        public async Task<string> ExecuteAction(string actionPrompt)
        {
            // Add action prompt to actioner history
            actionerHistory.AddUserMessage(actionPrompt);

            // Load actioner model config
            APIConfig config = APIConfig.LoadConfig(ACTIONER_CONFIG);

            if (string.IsNullOrWhiteSpace(config.DeploymentName) ||
                string.IsNullOrWhiteSpace(config.EndpointURL) ||
                string.IsNullOrWhiteSpace(config.APIKey))
            {
                outputTextBox.AppendText("Error: Actioner model not configured\n\n");
                return "Error: Actioner model not configured";
            }
            PromptExecutionSettings settings;
            // Setup the kernel for actioner with CMD plugin
            var builder = Kernel.CreateBuilder();
            if ("1" == "1")
            {
                builder.AddAzureOpenAIChatCompletion(
                config.DeploymentName,
                config.EndpointURL,
                config.APIKey);

                settings = new OpenAIPromptExecutionSettings
                {
                    //Temperature = 0.2,
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                };
                builder.Plugins.AddFromType<CMDPlugin>();
                builder.Plugins.AddFromType<PowerShellPlugin>();
                builder.Plugins.AddFromType<ScreenCapturePlugin>();
                builder.Plugins.AddFromType<KeyboardPlugin>();
                builder.Plugins.AddFromType<MousePlugin>();

            }
            else
            {
                //builder.Plugins.AddFromType<CMDPlugin>();
            }
            //check if settings is null

            // Add plugins for command execution

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

            /*
            var response = await actionerChat.GetChatMessageContentsAsync(actionerHistory, settings, actionerKernel);
            responseBuilder.Append(response[0].Content);
            outputTextBox.AppendText(response[0].Content);

            actionerHistory.AddAssistantMessage(responseBuilder.ToString());
            */
            //check if response contains command


            return responseBuilder.ToString();
        }
    }
}