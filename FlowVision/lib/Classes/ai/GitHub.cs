using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Plugins;
using Azure;
using Azure.AI.Inference;

namespace FlowVision.lib.Classes
{
    public class GitHub
    {
        private ChatCompletionsClient client;
        private RichTextBox outputTextBox;
        private const string GITHUB_CONFIG = "github";
        private Uri endpoint;
        private AzureKeyCredential credential;

        public GitHub(RichTextBox outputTextBox)
        {
            this.outputTextBox = outputTextBox;
        }

        public async Task<string> ExecuteAction(string actionPrompt)
        {
            // Load GitHub model config
            APIConfig config = APIConfig.LoadConfig(GITHUB_CONFIG);

            if (string.IsNullOrWhiteSpace(config.DeploymentName) ||
                string.IsNullOrWhiteSpace(config.EndpointURL) ||
                string.IsNullOrWhiteSpace(config.APIKey))
            {
                outputTextBox.AppendText("Error: GitHub model not configured\n\n");
                return "Error: GitHub model not configured";
            }

            try
            {
                // Initialize the GitHub API client
                endpoint = new Uri(config.EndpointURL);
                credential = new AzureKeyCredential(config.APIKey);
                string model = config.DeploymentName; // Using deploymentName field for model name

                client = new ChatCompletionsClient(
                    endpoint,
                    credential,
                    new AzureAIInferenceClientOptions());

                var requestOptions = new ChatCompletionsOptions()
                {
                    Messages =
                    {
                        new ChatRequestSystemMessage("You are an AI Assistant that helps users with GitHub-related tasks."),
                        new ChatRequestUserMessage(actionPrompt),
                    },
                    Temperature = 0.7f,
                    NucleusSamplingFactor = 0.95f,
                    Model = model
                };

                Response<ChatCompletions> response = await client.CompleteAsync(requestOptions);
                string result = response.Value.Content;

                return result;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error using GitHub model: {ex.Message}";
                outputTextBox.AppendText($"{errorMessage}\n\n");
                return errorMessage;
            }
        }
    }
}
