using System;
using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using Azure;
using OpenAI;

namespace FlowVision.lib.Classes.ai
{
    public static class AIClientFactory
    {
        public static IChatClient CreateClient(APIConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrWhiteSpace(config.EndpointURL))
                throw new ArgumentException("EndpointURL is required but was null or empty", nameof(config));

            if (string.IsNullOrWhiteSpace(config.APIKey))
                throw new ArgumentException("APIKey is required but was null or empty", nameof(config));

            switch (config.ProviderType?.ToLowerInvariant())
            {
                case "gemini":
                    // Use standard OpenAI client pointing to Google's endpoint
                    // Endpoint format: https://generativelanguage.googleapis.com/v1beta/openai/
                    var geminiClient = new OpenAIClient(
                        new System.ClientModel.ApiKeyCredential(config.APIKey),
                        new OpenAIClientOptions { Endpoint = new Uri(config.EndpointURL) }
                    );
                    return geminiClient.GetChatClient(config.DeploymentName).AsIChatClient();

                case "lmstudio":
                case "openai": // Generic OpenAI compatible
                    var openAIClient = new OpenAIClient(
                        new System.ClientModel.ApiKeyCredential(config.APIKey),
                        new OpenAIClientOptions { Endpoint = new Uri(config.EndpointURL) }
                    );
                    return openAIClient.GetChatClient(config.DeploymentName).AsIChatClient();

                case "azureopenai":
                default:
                    // Default to Azure OpenAI
                    var azureClient = new AzureOpenAIClient(
                        new Uri(config.EndpointURL), 
                        new AzureKeyCredential(config.APIKey)
                    );
                    return azureClient.GetChatClient(config.DeploymentName).AsIChatClient();
            }
        }
    }
}
