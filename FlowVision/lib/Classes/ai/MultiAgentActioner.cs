using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Plugins;
using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using Azure;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Multi-agent actioner that coordinates between a coordinator, planner agent and an execution agent
    /// </summary>
    public class MultiAgentActioner
    {
        private IChatClient coordinatorChat;
        private IChatClient plannerChat;
        private IChatClient actionerChat;
        private List<ChatMessage> coordinatorHistory;
        private List<ChatMessage> plannerHistory;
        private List<ChatMessage> actionerHistory;
                                private AgentCoordinator agentCoordinator;

        // Configuration constants
        private const string TOOL_CONFIG = "toolsconfig";
        private const string ACTIONER_CONFIG = "actioner";

        // ToolConfig instance to store and access configuration
        private ToolConfig toolConfig;

        public MultiAgentActioner(Form1.PluginOutputHandler outputHandler)
        {
            coordinatorHistory = new List<ChatMessage>();
            plannerHistory = new List<ChatMessage>();
            actionerHistory = new List<ChatMessage>();
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

            // Build dynamic tool description segment if enabled
            string toolDescriptions = toolConfig.DynamicToolPrompts
                ? "\n\n" + ToolDescriptionGenerator.GetToolDescriptions(toolConfig)
                : string.Empty;

            PluginLogger.NotifyTaskStart("Multi-Agent Action", "Planning and executing your request");
            PluginLogger.StartLoadingIndicator("coordination");

            try
            {
                // Configure coordinator first
                coordinatorHistory.Add(new ChatMessage(ChatRole.System, toolConfig.CoordinatorSystemPrompt + toolDescriptions));
                coordinatorHistory.Add(new ChatMessage(ChatRole.User, actionPrompt));

                // Configure planner for later use
                plannerHistory.Clear();
                plannerHistory.Add(new ChatMessage(ChatRole.System, toolConfig.PlannerSystemPrompt + toolDescriptions));

                
                // Configure actioner for later use
                actionerHistory.Clear();
                actionerHistory.Add(new ChatMessage(ChatRole.System, toolConfig.ActionerSystemPrompt + toolDescriptions));


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

                // Setup coordinator chat client (no tools, only coordination capabilities)
                var coordinatorAzureClient = new AzureOpenAIClient(new Uri(coordinatorConfig.EndpointURL), new AzureKeyCredential(coordinatorConfig.APIKey));
                coordinatorChat = coordinatorAzureClient.GetChatClient(coordinatorConfig.DeploymentName).AsIChatClient();

                // Setup planner chat client (no tools, only planning capabilities)
                var plannerAzureClient = new AzureOpenAIClient(new Uri(plannerConfig.EndpointURL), new AzureKeyCredential(plannerConfig.APIKey));
                plannerChat = plannerAzureClient.GetChatClient(plannerConfig.DeploymentName).AsIChatClient();

                // Setup actioner chat client with all tools
                var actionerAzureClient = new AzureOpenAIClient(new Uri(actionerConfig.EndpointURL), new AzureKeyCredential(actionerConfig.APIKey));
                IChatClient actionerChatBase = actionerAzureClient.GetChatClient(actionerConfig.DeploymentName).AsIChatClient();

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

                // Setup actioner with function invocation using builder pattern
                actionerChat = new ChatClientBuilder(actionerChatBase).UseFunctionInvocation().Build();

                // Get initial coordination from coordinator agent
                PluginLogger.StopLoadingIndicator();
                PluginLogger.LogPluginUsage("🗣️ Coordinating request...");
                PluginLogger.StartLoadingIndicator("coordination");

                var coordinatorOptions = new ChatOptions
                {
                    Temperature = 0.2f
                };

                var plannerOptions = new ChatOptions
                {
                    Temperature = 0.2f
                };

                var actionerOptions = new ChatOptions
                {
                    Temperature = (float)toolConfig.Temperature,
                    Tools = tools
                };

                // Get the initial coordination
                string coordinatorResponse = await GetAgentResponseAsync(coordinatorChat, coordinatorHistory, coordinatorOptions);
                PluginLogger.LogPluginUsage("🎯 Coordinator Assessment:\n" + coordinatorResponse);

                agentCoordinator.AddMessage(AgentRole.Coordinator, AgentRole.Planner,
                    "COORDINATION_RESPONSE", coordinatorResponse);

                // Send the task to the planner
                plannerHistory.Add(new ChatMessage(ChatRole.User, coordinatorResponse));

                PluginLogger.StopLoadingIndicator();
                PluginLogger.LogPluginUsage("🧠 Planning approach...");
                PluginLogger.StartLoadingIndicator("planning");

                // Get the initial plan
                string plan = await GetAgentResponseAsync(plannerChat, plannerHistory, plannerOptions);
                PluginLogger.LogPluginUsage("📝 Initial Plan:\n" + plan);

                
                agentCoordinator.AddMessage(AgentRole.Planner, AgentRole.Actioner, 

                    "PLAN_RESPONSE", plan);

                // Now execute the plan step by step
                bool isComplete = false;
                int maxIterations = 10; // Safety limit
                int currentIteration = 0;
                string finalResult = "";
                List<string> executionResults = new List<string>();

                while (!isComplete && currentIteration < maxIterations)
                {
                    currentIteration++;
                    PluginLogger.LogPluginUsage($"⚙️ Iteration {currentIteration} of {maxIterations}");

                    
                    // Ask actioner to perform the current step
                    actionerHistory.Add(new ChatMessage(ChatRole.User, $"Please execute the following step of our plan: {plan}"));
                    
                    agentCoordinator.AddMessage(AgentRole.Planner, AgentRole.Actioner, 
                        "EXECUTION_REQUEST", plan);
                    

                    PluginLogger.StopLoadingIndicator();
                    PluginLogger.LogPluginUsage("🔧 Processing step...");
                    PluginLogger.StartLoadingIndicator("executing");

                    
                    // Get actioner response with tools
                    string executionResult = await GetAgentResponseAsync(actionerChat, actionerHistory, actionerOptions);
                    
                    // Store the execution result for the final response
                    executionResults.Add(executionResult);
                    
                    PluginLogger.LogPluginUsage("📊 Execution result:\n" + executionResult);
                    
                    agentCoordinator.AddMessage(AgentRole.Actioner, AgentRole.Planner, 

                        "EXECUTION_RESPONSE", executionResult);

                    // Add the execution result to the planner's history

                    plannerHistory.Add(new ChatMessage(ChatRole.User, $"The actioner agent performed the requested step. Here is the result:\n\n{executionResult}\n\nIs the task fully completed, or do we need additional steps? If additional steps are needed, provide just the next step to execute. If the task is complete, respond with 'TASK COMPLETED' followed by a summary of what was accomplished."));
                    

                    PluginLogger.StopLoadingIndicator();
                    PluginLogger.LogPluginUsage("🔄 Evaluating progress...");
                    PluginLogger.StartLoadingIndicator("planning");

                    // Get planner's evaluation of the result
                    plan = await GetAgentResponseAsync(plannerChat, plannerHistory, plannerOptions);

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

                        coordinatorHistory.Add(new ChatMessage(ChatRole.User, 
                            $"The task has been completed. Here are the detailed results from the process:\n\n{executionSummary}\n\n" +
                            "Please provide a comprehensive response for the user that includes the actual results and information obtained during the process. " +
                            "Do not include phrases like 'TASK_COMPLETE' or similar tags."
                        ));

                        PluginLogger.StopLoadingIndicator();
                        PluginLogger.LogPluginUsage("✅ Task completed, generating detailed response...");
                        PluginLogger.StartLoadingIndicator("coordination");

                        // Get coordinator's final response with detailed results
                        finalResult = await GetAgentResponseAsync(coordinatorChat, coordinatorHistory, coordinatorOptions);

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
                    coordinatorHistory.Add(new ChatMessage(ChatRole.User, 
                        $"The task could not be completed within {maxIterations} iterations, but here are the results so far:\n\n{allResults}\n\n" +
                        "Please provide a detailed response for the user that contains all the information gathered, even though the task wasn't fully completed."
                    ));

                    PluginLogger.LogPluginUsage("⚠️ Maximum iterations reached, generating explanation with results...");
                    PluginLogger.StartLoadingIndicator("coordination");

                    // Get coordinator's explanation with detailed results
                    string resultWithExplanation = await GetAgentResponseAsync(coordinatorChat, coordinatorHistory, coordinatorOptions);

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
            IChatClient chatService,
            List<ChatMessage> history,
            ChatOptions options)
        {
            var responseBuilder = new StringBuilder();
            
            await foreach (var update in chatService.GetStreamingResponseAsync(history, options))
            {
                if (update.Text != null)
                {
                    responseBuilder.Append(update.Text);
                }
            }

            string response = responseBuilder.ToString();
            history.Add(new ChatMessage(ChatRole.Assistant, response));

            return response;
        }

        /// <summary>
        /// Extracts the first actionable step from the planner's plan.
        /// Looks for lines that mention a tool/plugin or a direct action.
        /// </summary>
        private string ExtractActionableStep(string plan)
        {
            if (string.IsNullOrWhiteSpace(plan))
                return null;

            // Look for lines that mention 'use', 'plugin', or 'tool'
            var lines = plan.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var lower = line.ToLowerInvariant();
                if (lower.Contains("use") && (lower.Contains("plugin") || lower.Contains("tool")))
                {
                    return line.Trim();
                }
                // Also allow direct imperative instructions
                if (lower.StartsWith("set ") || lower.StartsWith("capture ") || lower.StartsWith("extract "))
                {
                    return line.Trim();
                }
            }
            // Fallback: if plan is a single actionable sentence
            if (lines.Length == 1 && lines[0].Length < 200)
                return lines[0].Trim();

            return null;
        }

        internal void SetChatHistory(List<LocalChatMessage> chatHistory)
        {
            // Set up coordinator history with system prompt
            coordinatorHistory.Clear();
            string toolDescriptions = toolConfig.DynamicToolPrompts
                ? "\n\n" + ToolDescriptionGenerator.GetToolDescriptions(toolConfig)
                : string.Empty;
            coordinatorHistory.Add(new ChatMessage(ChatRole.System, toolConfig.CoordinatorSystemPrompt + toolDescriptions));

            // Set up planner history with system prompt
            plannerHistory.Clear();
            plannerHistory.Add(new ChatMessage(ChatRole.System, toolConfig.PlannerSystemPrompt + toolDescriptions));

            foreach (var message in chatHistory)
            {
                if (message.Author == "You")
                {
                    coordinatorHistory.Add(new ChatMessage(ChatRole.User, message.Content));
                }
                else if (message.Author == "AI")
                {
                    coordinatorHistory.Add(new ChatMessage(ChatRole.Assistant, message.Content));
                }
            }
        }
    }
}