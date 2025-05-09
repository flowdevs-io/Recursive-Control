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
    /// Multi-agent actioner that coordinates between a coordinator, planner agent and an execution agent
    /// </summary>
    public class MultiAgentActioner
    {
        private IChatCompletionService coordinatorChat;
        private IChatCompletionService plannerChat;
        private IChatCompletionService actionerChat;
        private ChatHistory coordinatorHistory;
        private ChatHistory plannerHistory;
        private ChatHistory actionerHistory;
        private Kernel coordinatorKernel;
        private Kernel plannerKernel;
        private Kernel actionerKernel;
        private AgentCoordinator agentCoordinator;
        
        // Configuration constants
        private const string TOOL_CONFIG = "toolsconfig";
        private const string ACTIONER_CONFIG = "actioner";
        
        // ToolConfig instance to store and access configuration
        private ToolConfig toolConfig;
        
        public MultiAgentActioner(Form1.PluginOutputHandler outputHandler)
        {
            coordinatorHistory = new ChatHistory();
            plannerHistory = new ChatHistory();
            actionerHistory = new ChatHistory();
            agentCoordinator = new AgentCoordinator();
            
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
            PluginLogger.StartLoadingIndicator("coordination");
            
            try
            {
                // Configure coordinator first
                //coordinatorHistory.Clear();
                coordinatorHistory.AddSystemMessage(toolConfig.CoordinatorSystemPrompt);
                coordinatorHistory.AddUserMessage(actionPrompt);
                
                // Configure planner for later use
                plannerHistory.Clear();
                plannerHistory.AddSystemMessage(toolConfig.PlannerSystemPrompt);
                
                // Configure actioner for later use
                actionerHistory.Clear();
                actionerHistory.AddSystemMessage(toolConfig.ActionerSystemPrompt);

                // Clear agent coordinator message history
                agentCoordinator.Clear();
                agentCoordinator.AddMessage(AgentRole.User, AgentRole.Coordinator, 
                    "USER_REQUEST", actionPrompt);

                // Load model configurations - use either custom configs or default
                APIConfig coordinatorConfig = toolConfig.UseCustomCoordinatorConfig 
                    ? APIConfig.LoadConfig(toolConfig.CoordinatorConfigName)
                    : APIConfig.LoadConfig(ACTIONER_CONFIG);
                
                APIConfig plannerConfig = toolConfig.UseCustomPlannerConfig 
                    ? APIConfig.LoadConfig(toolConfig.PlannerConfigName)
                    : APIConfig.LoadConfig(ACTIONER_CONFIG);

                APIConfig actionerConfig = toolConfig.UseCustomActionerConfig
                    ? APIConfig.LoadConfig(toolConfig.ActionerConfigName)
                    : APIConfig.LoadConfig(ACTIONER_CONFIG);

                // Verify coordinator config
                if (string.IsNullOrWhiteSpace(coordinatorConfig.DeploymentName) ||
                    string.IsNullOrWhiteSpace(coordinatorConfig.EndpointURL) ||
                    string.IsNullOrWhiteSpace(coordinatorConfig.APIKey))
                {
                    PluginLogger.NotifyTaskComplete("Multi-Agent Action", false);
                    return "Error: Coordinator model not configured";
                }

                // Verify planner config
                if (string.IsNullOrWhiteSpace(plannerConfig.DeploymentName) ||
                    string.IsNullOrWhiteSpace(plannerConfig.EndpointURL) ||
                    string.IsNullOrWhiteSpace(plannerConfig.APIKey))
                {
                    PluginLogger.NotifyTaskComplete("Multi-Agent Action", false);
                    return "Error: Planner model not configured";
                }

                // Verify actioner config
                if (string.IsNullOrWhiteSpace(actionerConfig.DeploymentName) ||
                    string.IsNullOrWhiteSpace(actionerConfig.EndpointURL) ||
                    string.IsNullOrWhiteSpace(actionerConfig.APIKey))
                {
                    PluginLogger.NotifyTaskComplete("Multi-Agent Action", false);
                    return "Error: Actioner model not configured";
                }

                // Setup the kernel for coordinator (no tools, only coordination capabilities)
                var coordinatorBuilder = Kernel.CreateBuilder();
                coordinatorBuilder.AddAzureOpenAIChatCompletion(
                    coordinatorConfig.DeploymentName,
                    coordinatorConfig.EndpointURL,
                    coordinatorConfig.APIKey);

                coordinatorKernel = coordinatorBuilder.Build();
                coordinatorChat = coordinatorKernel.GetRequiredService<IChatCompletionService>();

                // Setup the kernel for planner (no tools, only planning capabilities)
                var plannerBuilder = Kernel.CreateBuilder();
                plannerBuilder.AddAzureOpenAIChatCompletion(
                    plannerConfig.DeploymentName,
                    plannerConfig.EndpointURL,
                    plannerConfig.APIKey);

                plannerKernel = plannerBuilder.Build();
                plannerChat = plannerKernel.GetRequiredService<IChatCompletionService>();

                // Setup the kernel for actioner with all tools
                var actionerBuilder = Kernel.CreateBuilder();
                actionerBuilder.AddAzureOpenAIChatCompletion(
                    actionerConfig.DeploymentName,
                    actionerConfig.EndpointURL,
                    actionerConfig.APIKey);

                // Add plugins dynamically based on tool configuration
                if (toolConfig.EnableCMDPlugin)
                {
                    actionerBuilder.Plugins.AddFromType<CMDPlugin>();
                }
                    
                if (toolConfig.EnablePowerShellPlugin)
                {
                    actionerBuilder.Plugins.AddFromType<PowerShellPlugin>();
                }
                    
                if (toolConfig.EnableScreenCapturePlugin)
                {
                    actionerBuilder.Plugins.AddFromType<ScreenCaptureOmniParserPlugin>();
                }
                    
                if (toolConfig.EnableKeyboardPlugin)
                {
                    actionerBuilder.Plugins.AddFromType<KeyboardPlugin>();
                }
                    
                if (toolConfig.EnableMousePlugin)
                {
                    actionerBuilder.Plugins.AddFromType<MousePlugin>();
                }
                    
                if (toolConfig.EnableWindowSelectionPlugin)
                {
                    actionerBuilder.Plugins.AddFromType<WindowSelectionPlugin>();
                }

                if (toolConfig.EnablePlaywrightPlugin)
                {
                    actionerBuilder.Plugins.AddFromType<PlaywrightPlugin>();
                }

                actionerKernel = actionerBuilder.Build();
                actionerChat = actionerKernel.GetRequiredService<IChatCompletionService>();

                // Get initial coordination from coordinator agent
                PluginLogger.StopLoadingIndicator();
                PluginLogger.LogPluginUsage("🗣️ Coordinating request...");
                PluginLogger.StartLoadingIndicator("coordination");
                
                var coordinatorSettings = new OpenAIPromptExecutionSettings
                {
                    Temperature = 0.2,
                    // No tools for coordinator
                };

                var plannerSettings = new OpenAIPromptExecutionSettings
                {
                    Temperature = 0.2,
                    // No tools for planner
                };

                var actionerSettings = new OpenAIPromptExecutionSettings
                {
                    Temperature = toolConfig.Temperature,
                    ToolCallBehavior = toolConfig.AutoInvokeKernelFunctions
                        ? ToolCallBehavior.AutoInvokeKernelFunctions
                        : ToolCallBehavior.EnableKernelFunctions
                };

                // Get the initial coordination
                string coordinatorResponse = await GetAgentResponseAsync(coordinatorChat, coordinatorHistory, coordinatorSettings, coordinatorKernel);
                PluginLogger.LogPluginUsage("🎯 Coordinator Assessment:\n" + coordinatorResponse);
                
                agentCoordinator.AddMessage(AgentRole.Coordinator, AgentRole.Planner, 
                    "COORDINATION_RESPONSE", coordinatorResponse);

                // Send the task to the planner
                plannerHistory.AddUserMessage(coordinatorResponse);
                
                PluginLogger.StopLoadingIndicator();
                PluginLogger.LogPluginUsage("🧠 Planning approach...");
                PluginLogger.StartLoadingIndicator("planning");
                
                // Get the initial plan
                string plan = await GetAgentResponseAsync(plannerChat, plannerHistory, plannerSettings, plannerKernel);
                PluginLogger.LogPluginUsage("📝 Initial Plan:\n" + plan);
                
                agentCoordinator.AddMessage(AgentRole.Planner, AgentRole.Actioner, 
                    "PLAN_RESPONSE", plan);
                
                // Now execute the plan step by step
                bool isComplete = false;
                int maxIterations = 10; // Safety limit
                int currentIteration = 0;
                string finalResult = "";
                // Store all execution results for final response
                List<string> executionResults = new List<string>();
                
                while (!isComplete && currentIteration < maxIterations)
                {
                    currentIteration++;
                    PluginLogger.LogPluginUsage($"⚙️ Iteration {currentIteration} of {maxIterations}");
                    
                    // Ask actioner to perform the current step
                    actionerHistory.AddUserMessage($"Please execute the following step of our plan: {plan}");
                    
                    agentCoordinator.AddMessage(AgentRole.Planner, AgentRole.Actioner, 
                        "EXECUTION_REQUEST", plan);
                    
                    PluginLogger.StopLoadingIndicator();
                    PluginLogger.LogPluginUsage("🔧 Executing step...");
                    PluginLogger.StartLoadingIndicator("executing");
                    
                    // Get actioner response with tools
                    string executionResult = await GetAgentResponseAsync(actionerChat, actionerHistory, actionerSettings, actionerKernel);
                    
                    // Store the execution result for the final response
                    executionResults.Add(executionResult);
                    
                    PluginLogger.LogPluginUsage("📊 Execution result:\n" + executionResult);
                    
                    agentCoordinator.AddMessage(AgentRole.Actioner, AgentRole.Planner, 
                        "EXECUTION_RESPONSE", executionResult);
                    
                    // Add the execution result to the planner's history
                    plannerHistory.AddUserMessage($"The actioner agent performed the requested step. Here is the result:\n\n{executionResult}\n\nIs the task fully completed, or do we need additional steps? If additional steps are needed, provide just the next step to execute. If the task is complete, respond with 'TASK COMPLETED' followed by a summary of what was accomplished.");
                    
                    PluginLogger.StopLoadingIndicator();
                    PluginLogger.LogPluginUsage("🔄 Evaluating progress...");
                    PluginLogger.StartLoadingIndicator("planning");
                    
                    // Get planner's evaluation of the result
                    plan = await GetAgentResponseAsync(plannerChat, plannerHistory, plannerSettings, plannerKernel);
                    
                    agentCoordinator.AddMessage(AgentRole.Planner, AgentRole.Coordinator, 
                        "STATUS_UPDATE", plan);
                    
                    // Check if the task is complete
                    if (plan.Contains("TASK COMPLETED"))
                    {
                        isComplete = true;
                        
                        // Extract the summary from the "TASK COMPLETED" message
                        string taskSummary = plan.Replace("TASK COMPLETED", "").Trim();
                        
                        // Send all execution results to the coordinator for final formatting
                        string executionSummary = string.Join("\n\n", executionResults);
                        
                        coordinatorHistory.AddUserMessage($"The task has been completed. Here are the detailed results from execution:\n\n{executionSummary}\n\nPlease provide a comprehensive response for the user that includes the actual results and information obtained during execution. Do not include phrases like 'TASK_COMPLETE' or similar tags.");
                        
                        PluginLogger.StopLoadingIndicator();
                        PluginLogger.LogPluginUsage("✅ Task completed, generating detailed response...");
                        PluginLogger.StartLoadingIndicator("coordination");
                        
                        // Get coordinator's final response with detailed results
                        finalResult = await GetAgentResponseAsync(coordinatorChat, coordinatorHistory, coordinatorSettings, coordinatorKernel);
                        
                        // Store this as a completed response but without the TASK_COMPLETE tag
                        agentCoordinator.AddMessage(AgentRole.Coordinator, AgentRole.User, 
                            "USER_RESPONSE", finalResult);
                    }
                    else
                    {
                        PluginLogger.LogPluginUsage("🔍 Progress evaluation:\n" + plan);
                    }
                }

                PluginLogger.StopLoadingIndicator();
                
                if (isComplete)
                {
                    PluginLogger.NotifyTaskComplete("Multi-Agent Action", true);
                    return finalResult;
                }
                else
                {
                    // Compile all execution results into a comprehensive response
                    string allResults = string.Join("\n\n", executionResults);
                    
                    // Get coordinator to explain the incomplete task status with the results
                    coordinatorHistory.AddUserMessage($"The task could not be completed within {maxIterations} iterations, but here are the execution results so far:\n\n{allResults}\n\nPlease provide a detailed response for the user that contains all the information gathered, even though the task wasn't fully completed.");
                    
                    PluginLogger.LogPluginUsage("⚠️ Maximum iterations reached, generating explanation with results...");
                    PluginLogger.StartLoadingIndicator("coordination");
                    
                    // Get coordinator's explanation with detailed results
                    string resultWithExplanation = await GetAgentResponseAsync(coordinatorChat, coordinatorHistory, coordinatorSettings, coordinatorKernel);
                    
                    agentCoordinator.AddMessage(AgentRole.Coordinator, AgentRole.User, 
                        "STATUS_UPDATE", resultWithExplanation);
                    
                    PluginLogger.StopLoadingIndicator();
                    PluginLogger.NotifyTaskComplete("Multi-Agent Action", false);
                    return resultWithExplanation;
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
            // Set up coordinator history with system prompt
            coordinatorHistory.Clear();
            coordinatorHistory.AddSystemMessage(toolConfig.CoordinatorSystemPrompt);
            
            // Set up planner history with system prompt
            plannerHistory.Clear();
            plannerHistory.AddSystemMessage(toolConfig.PlannerSystemPrompt);
            
            foreach (var message in chatHistory)
            {
                if (message.Author == "You")
                {
                    coordinatorHistory.AddUserMessage(message.Content);
                }
                else if (message.Author == "AI")
                {
                    coordinatorHistory.AddAssistantMessage(message.Content);
                }
            }
        }
    }
}
