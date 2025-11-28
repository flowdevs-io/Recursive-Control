using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Plugins;
using Microsoft.Extensions.AI;
using FlowVision.lib.Classes.ai;
using Azure.AI.OpenAI; // Needed for some types if referenced, but Factory returns IChatClient
using Azure; // Needed for AzureKeyCredential if strictly typed, but Factory handles it. 
using FlowVision; // Required for Form1

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

                // Setup clients using the Factory (supports Azure, Gemini, etc.)
                coordinatorChat = AIClientFactory.CreateClient(coordinatorConfig);
                plannerChat = AIClientFactory.CreateClient(plannerConfig);
                
                // Setup actioner client base
                IChatClient actionerChatBase = AIClientFactory.CreateClient(actionerConfig);

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

                if (toolConfig.EnableClipboardPlugin)
                {
                    tools.AddRange(PluginToolExtractor.ExtractTools(new ClipboardPlugin()));
                }

                if (toolConfig.EnableFileSystemPlugin)
                {
                    tools.AddRange(PluginToolExtractor.ExtractTools(new FileSystemPlugin()));
                }

                // Setup actioner with function invocation using builder pattern
                actionerChat = new ChatClientBuilder(actionerChatBase).UseFunctionInvocation().Build();

                // Get initial coordination from coordinator agent
                PluginLogger.StopLoadingIndicator();
                PluginLogger.LogPluginUsage("🗣️ Coordinating request...");
                PluginLogger.StartLoadingIndicator("coordination");

                var coordinatorOptions = new ChatOptions
                {
                    Temperature = (float)toolConfig.Temperature
                };

                var plannerOptions = new ChatOptions
                {
                    Temperature = (float)toolConfig.Temperature
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
                int maxIterations = 25; // Increased from 10 to 25 for complex tasks
                int currentIteration = 0;
                string finalResult = "";
                List<string> executionResults = new List<string>();
                
                // Novel Feature: Focus Tracking
                // Keep track of window focus to detect popups (like "Save As") automatically
                var windowTracker = new WindowSelectionPlugin();

                while (!isComplete && currentIteration < maxIterations)
                {
                    currentIteration++;
                    PluginLogger.LogPluginUsage($"⚙️ Step {currentIteration}/{maxIterations}");

                    // Capture pre-action state
                    string preActionWindow = windowTracker.GetForegroundWindowInfo();

                    // Ask actioner to perform the current step with clearer instructions
                    actionerHistory.Add(new ChatMessage(ChatRole.User, 
                        $"Execute this step:\n\n{plan}\n\n" +
                        $"Current progress: {currentIteration}/{maxIterations} steps\n\n" +
                        "Remember to:\n" +
                        "1. Use window handles for keyboard/mouse actions (SendKeyToWindow, not SendKey)\n" +
                        "2. Take screenshots to verify state when needed\n" +
                        "3. Report exactly what you did and what you observed\n" +
                        "4. If something fails, explain what went wrong"));
                    
                    agentCoordinator.AddMessage(AgentRole.Planner, AgentRole.Actioner, 
                        "EXECUTION_REQUEST", plan);
                    

                    PluginLogger.StopLoadingIndicator();
                    PluginLogger.LogPluginUsage("🔧 Executing step...");
                    PluginLogger.StartLoadingIndicator("executing");

                    
                    // Get actioner response with tools
                    string executionResult = await GetAgentResponseAsync(actionerChat, actionerHistory, actionerOptions);
                    
                    // Capture post-action state
                    string postActionWindow = windowTracker.GetForegroundWindowInfo();

                    // FIX 1: Handle empty execution results (common with successful shell commands)
                    if (string.IsNullOrWhiteSpace(executionResult))
                    {
                        executionResult = "The command executed successfully with no output.";
                    }

                    // Novel Feature: Inject Focus Change Alert
                    // If the active window changed (e.g. "Save As" dialog popped up), explicitly tell the Planner.
                    if (preActionWindow != postActionWindow)
                    {
                        string alert = $"\n\n[SYSTEM ALERT]: Active window focus changed!\n" +
                                       $"Previous: {preActionWindow}\n" +
                                       $"Current:  {postActionWindow}\n" +
                                       $"Use the new Handle ({postActionWindow.Split(',')[0]}) for subsequent interactions.";
                        
                        executionResult += alert;
                        PluginLogger.LogInfo("MultiAgentActioner", "ExecuteAction", "Detected window focus change, alerting Planner.");
                    }

                    // Store the execution result for the final response
                    executionResults.Add(executionResult);
                    
                    PluginLogger.LogPluginUsage("📊 Step result:\n" + executionResult);
                    
                    agentCoordinator.AddMessage(AgentRole.Actioner, AgentRole.Planner, 

                        "EXECUTION_RESPONSE", executionResult);

                    // FIX 2: Manage context window to prevent token overflow
                    ManageContextWindow(actionerHistory, 10);
                    ManageContextWindow(plannerHistory, 10);

                    // Add the execution result to the planner's history with clearer prompting

                    plannerHistory.Add(new ChatMessage(ChatRole.User, 
                        $"Step {currentIteration} Result:\n{executionResult}\n\n" +
                        $"Progress: {currentIteration}/{maxIterations} steps completed\n\n" +
                        "Evaluate:\n" +
                        "1. Did this step succeed?\n" +
                        "2. Is the overall task now complete?\n" +
                        "3. If not complete, what is the NEXT SINGLE step?\n\n" +
                        "If task is COMPLETE, respond with:\n" +
                        "'TASK COMPLETED: [brief summary]'\n\n" +
                        "If task needs more work, provide ONLY the next single step to execute."));
                    

                    PluginLogger.StopLoadingIndicator();
                    PluginLogger.LogPluginUsage("🔄 Evaluating progress...");
                    PluginLogger.StartLoadingIndicator("planning");

                    // Get planner's evaluation of the result
                    plan = await GetAgentResponseAsync(plannerChat, plannerHistory, plannerOptions);

                    agentCoordinator.AddMessage(AgentRole.Planner, AgentRole.Coordinator,
                        "STATUS_UPDATE", plan);

                    // Check if the task is complete (case insensitive)
                    if (plan.IndexOf("TASK COMPLETED", StringComparison.OrdinalIgnoreCase) >= 0 || 
                        plan.IndexOf("Task completed", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        isComplete = true;

                        PluginLogger.LogPluginUsage("✅ Task marked as complete by planner");

                        // Send all execution results to the coordinator for final formatting
                        string executionSummary = string.Join("\n\n", executionResults);

                        coordinatorHistory.Add(new ChatMessage(ChatRole.User, 
                            $"The task has been completed after {currentIteration} steps.\n\n" +
                            $"Complete execution log:\n{executionSummary}\n\n" +
                            $"Planner's completion message:\n{plan}\n\n" +
                            "Please provide a clear, user-friendly summary of what was accomplished. " +
                            "Include specific results, any important details, and the current state. " +
                            "Be concise but informative. Do not use technical tags or internal markers."
                        ));

                        PluginLogger.StopLoadingIndicator();
                        PluginLogger.LogPluginUsage("📝 Generating user response...");
                        PluginLogger.StartLoadingIndicator("coordination");

                        // Get coordinator's final response with detailed results
                        finalResult = await GetAgentResponseAsync(coordinatorChat, coordinatorHistory, coordinatorOptions);

                        // Store this as a completed response but without the TASK_COMPLETE tag
                        agentCoordinator.AddMessage(AgentRole.Coordinator, AgentRole.User,
                            "USER_RESPONSE", finalResult);
                    }
                    else
                    {
                        PluginLogger.LogPluginUsage($"⏭️  Next step:\n{plan}");
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
            
            try
            {
                await foreach (var update in chatService.GetStreamingResponseAsync(history, options))
                {
                    if (update.Text != null)
                    {
                        responseBuilder.Append(update.Text);
                    }
                }
            }
            catch (Exception ex) when (ex.Message.Contains("Unknown ChatFinishReason") || ex.Message.Contains("function_call_filter"))
            {
                // Swallow known SDK mapping errors for specific provider finish reasons
                // This allows us to keep the text generated so far
                PluginLogger.LogInfo("MultiAgentActioner", "GetAgentResponseAsync", $"Ignored known SDK finish reason error: {ex.Message}");
            }

            string response = responseBuilder.ToString();
            history.Add(new ChatMessage(ChatRole.Assistant, response));

            return response;
        }

        private void ManageContextWindow(List<ChatMessage> history, int maxMessages)
        {
            // Always keep the system prompt (assumed to be at index 0)
            if (history.Count <= maxMessages + 1) return;

            // Calculate how many messages to remove
            // We want to keep: SystemPrompt (1) + Last N messages
            int messagesToRemove = history.Count - (maxMessages + 1);

            if (messagesToRemove > 0)
            {
                // Remove messages starting from index 1 (preserve System Prompt)
                history.RemoveRange(1, messagesToRemove);
            }
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

        public void SetChatHistory(System.Collections.Generic.List<FlowVision.LocalChatMessage> chatHistory)
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
