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
    /// <summary>
    /// Multi-agent actioner that coordinates between a planner agent and an execution agent
    /// </summary>
    public class MultiAgentActioner
    {
        private IChatCompletionService plannerChat;
        private IChatCompletionService executorChat;
        private ChatHistory plannerHistory;
        private ChatHistory executorHistory;
        private Kernel plannerKernel;
        private Kernel executorKernel;
        
        // Configuration constants
        private const string TOOL_CONFIG = "toolsconfig";
        private const string ACTIONER_CONFIG = "actioner";
        
        // ToolConfig instance to store and access configuration
        private ToolConfig toolConfig;
        
        public MultiAgentActioner(Form1.PluginOutputHandler outputHandler)
        {
            plannerHistory = new ChatHistory();
            executorHistory = new ChatHistory();
            
            // Load tool configuration when the MultiAgentActioner is initialized
            toolConfig = ToolConfig.LoadConfig(TOOL_CONFIG);

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
        }

        public async Task<string> ExecuteAction(string actionPrompt)
        {
            // Reload tool configuration to ensure we have the most recent settings
            toolConfig = ToolConfig.LoadConfig(TOOL_CONFIG);
            
            PluginLogger.NotifyTaskStart("Multi-Agent Action", "Planning and executing your request");
            PluginLogger.StartLoadingIndicator("planning");
            
            try
            {
                // Configure planner first
                plannerHistory.Clear();
                plannerHistory.AddSystemMessage(toolConfig.PlannerSystemPrompt);
                plannerHistory.AddUserMessage(actionPrompt);

                // Configure executor for later use
                executorHistory.Clear();
                executorHistory.AddSystemMessage(toolConfig.ExecutorSystemPrompt);

                // Load model configurations - use either custom configs or default
                APIConfig plannerConfig = toolConfig.UseCustomPlannerConfig 
                    ? APIConfig.LoadConfig(toolConfig.PlannerConfigName)
                    : APIConfig.LoadConfig(ACTIONER_CONFIG);

                APIConfig executorConfig = toolConfig.UseCustomExecutorConfig
                    ? APIConfig.LoadConfig(toolConfig.ExecutorConfigName)
                    : APIConfig.LoadConfig(ACTIONER_CONFIG);

                // Verify planner config
                if (string.IsNullOrWhiteSpace(plannerConfig.DeploymentName) ||
                    string.IsNullOrWhiteSpace(plannerConfig.EndpointURL) ||
                    string.IsNullOrWhiteSpace(plannerConfig.APIKey))
                {
                    PluginLogger.NotifyTaskComplete("Multi-Agent Action", false);
                    return "Error: Planner model not configured";
                }

                // Verify executor config
                if (string.IsNullOrWhiteSpace(executorConfig.DeploymentName) ||
                    string.IsNullOrWhiteSpace(executorConfig.EndpointURL) ||
                    string.IsNullOrWhiteSpace(executorConfig.APIKey))
                {
                    PluginLogger.NotifyTaskComplete("Multi-Agent Action", false);
                    return "Error: Executor model not configured";
                }

                // Setup the kernel for planner (no tools, only planning capabilities)
                var plannerBuilder = Kernel.CreateBuilder();
                plannerBuilder.AddAzureOpenAIChatCompletion(
                    plannerConfig.DeploymentName,
                    plannerConfig.EndpointURL,
                    plannerConfig.APIKey);

                plannerKernel = plannerBuilder.Build();
                plannerChat = plannerKernel.GetRequiredService<IChatCompletionService>();

                // Setup the kernel for executor with all tools
                var executorBuilder = Kernel.CreateBuilder();
                executorBuilder.AddAzureOpenAIChatCompletion(
                    executorConfig.DeploymentName,
                    executorConfig.EndpointURL,
                    executorConfig.APIKey);

                // Add plugins dynamically based on tool configuration
                if (toolConfig.EnableCMDPlugin)
                {
                    executorBuilder.Plugins.AddFromType<CMDPlugin>();
                }
                    
                if (toolConfig.EnablePowerShellPlugin)
                {
                    executorBuilder.Plugins.AddFromType<PowerShellPlugin>();
                }
                    
                if (toolConfig.EnableScreenCapturePlugin)
                {
                    executorBuilder.Plugins.AddFromType<ScreenCaptureOmniParserPlugin>();
                }
                    
                if (toolConfig.EnableKeyboardPlugin)
                {
                    executorBuilder.Plugins.AddFromType<KeyboardPlugin>();
                }
                    
                if (toolConfig.EnableMousePlugin)
                {
                    executorBuilder.Plugins.AddFromType<MousePlugin>();
                }
                    
                if (toolConfig.EnableWindowSelectionPlugin)
                {
                    executorBuilder.Plugins.AddFromType<WindowSelectionPlugin>();
                }

                executorKernel = executorBuilder.Build();
                executorChat = executorKernel.GetRequiredService<IChatCompletionService>();

                // Get initial plan from planner agent
                PluginLogger.StopLoadingIndicator();
                PluginLogger.LogPluginUsage("🧠 Planning approach...");
                PluginLogger.StartLoadingIndicator("planning");
                
                var planSettings = new OpenAIPromptExecutionSettings
                {
                    Temperature = 0.2,
                    // No tools for planner
                };

                var executorSettings = new OpenAIPromptExecutionSettings
                {
                    Temperature = toolConfig.Temperature,
                    ToolCallBehavior = toolConfig.AutoInvokeKernelFunctions
                        ? ToolCallBehavior.AutoInvokeKernelFunctions
                        : ToolCallBehavior.EnableKernelFunctions
                };

                // Get the initial plan
                string plan = await GetAgentResponseAsync(plannerChat, plannerHistory, planSettings, plannerKernel);
                PluginLogger.LogPluginUsage("📝 Initial Plan:\n" + plan);
                
                // Now execute the plan step by step
                bool isComplete = false;
                int maxIterations = 10; // Safety limit
                int currentIteration = 0;
                string finalResult = "";
                
                while (!isComplete && currentIteration < maxIterations)
                {
                    currentIteration++;
                    PluginLogger.LogPluginUsage($"⚙️ Iteration {currentIteration} of {maxIterations}");
                    
                    // Ask executor to perform the current step
                    executorHistory.AddUserMessage($"Please execute the following step of our plan: {plan}");
                    
                    PluginLogger.StopLoadingIndicator();
                    PluginLogger.LogPluginUsage("🔧 Executing step...");
                    PluginLogger.StartLoadingIndicator("executing");
                    
                    // Get executor response with tools
                    string executionResult = await GetAgentResponseAsync(executorChat, executorHistory, executorSettings, executorKernel);
                    
                    PluginLogger.LogPluginUsage("📊 Execution result:\n" + executionResult);
                    
                    // Add the execution result to the planner's history
                    plannerHistory.AddUserMessage($"The executor agent performed the requested step. Here is the result:\n\n{executionResult}\n\nIs the task fully completed, or do we need additional steps? If additional steps are needed, provide just the next step to execute. If the task is complete, respond with 'TASK COMPLETED' followed by a summary of what was accomplished.");
                    
                    PluginLogger.StopLoadingIndicator();
                    PluginLogger.LogPluginUsage("🔄 Evaluating progress...");
                    PluginLogger.StartLoadingIndicator("planning");
                    
                    // Get planner's evaluation of the result
                    plan = await GetAgentResponseAsync(plannerChat, plannerHistory, planSettings, plannerKernel);
                    
                    // Check if the task is complete
                    if (plan.Contains("TASK COMPLETED"))
                    {
                        isComplete = true;
                        finalResult = plan;
                    }
                    
                    PluginLogger.LogPluginUsage("🔍 Progress evaluation:\n" + plan);
                }

                PluginLogger.StopLoadingIndicator();
                
                if (isComplete)
                {
                    PluginLogger.NotifyTaskComplete("Multi-Agent Action", true);
                    return finalResult;
                }
                else
                {
                    PluginLogger.NotifyTaskComplete("Multi-Agent Action", false);
                    return $"Task execution reached maximum iterations ({maxIterations}) without completion. Last status: {plan}";
                }
            }
            catch (Exception ex)
            {
                PluginLogger.StopLoadingIndicator();
                PluginLogger.NotifyTaskComplete("Multi-Agent Action", false);
                return $"Error: {ex.Message}";
            }
        }

        private async Task<string> GetAgentResponseAsync(
            IChatCompletionService chatService, 
            ChatHistory history, 
            OpenAIPromptExecutionSettings settings, 
            Kernel kernel)
        {
            var responseBuilder = new StringBuilder();
            var responseStream = chatService.GetStreamingChatMessageContentsAsync(history, settings, kernel);
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
            history.AddAssistantMessage(response);
            
            return response;
        }

        internal void SetChatHistory(List<LocalChatMessage> chatHistory)
        {
            plannerHistory.Clear();
            plannerHistory.AddSystemMessage(toolConfig.PlannerSystemPrompt);
            
            foreach (var message in chatHistory)
            {
                if (message.Author == "You")
                {
                    plannerHistory.AddUserMessage(message.Content);
                }
                else if (message.Author == "AI")
                {
                    plannerHistory.AddAssistantMessage(message.Content);
                }
            }
        }
    }
}
