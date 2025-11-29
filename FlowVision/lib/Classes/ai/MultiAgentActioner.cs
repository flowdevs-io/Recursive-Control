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
                    tools.AddRange(PluginToolExtractor.ExtractTools(new ScreenCapturePlugin()));
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
                int maxIterations = 25;
                int currentIteration = 0;
                string finalResult = "";
                List<string> executionResults = new List<string>();
                
                // Novel Feature: Focus Tracking
                var windowTracker = new WindowSelectionPlugin();
                
                // Novel Feature: Reflection Memory
                // Stores lessons learned from failures to prevent repeating mistakes
                List<string> lessonsLearned = new List<string>();
                int consecutiveFailures = 0;
                const int MAX_CONSECUTIVE_FAILURES = 3;

                while (!isComplete && currentIteration < maxIterations)
                {
                    currentIteration++;
                    PluginLogger.LogPluginUsage($"⚙️ Step {currentIteration}/{maxIterations}");

                    // Capture pre-action state
                    string preActionWindow = windowTracker.GetForegroundWindowInfo();

                    // Build context with lessons learned (if any)
                    string lessonsContext = lessonsLearned.Count > 0
                        ? $"\n\n⚠️ LESSONS FROM PREVIOUS ATTEMPTS:\n" + string.Join("\n", lessonsLearned.Select((l, i) => $"{i + 1}. {l}"))
                        : "";

                    // Ask actioner to perform the current step with enhanced instructions
                    actionerHistory.Add(new ChatMessage(ChatRole.User, 
                        $"Execute this step:\n\n{plan}\n\n" +
                        $"Current progress: {currentIteration}/{maxIterations} steps" +
                        lessonsContext + "\n\n" +
                        "CRITICAL RULES:\n" +
                        "1. Use GetPageElements() to find selectors for elements\n" +
                        "2. Use ClickElement with waitForNavigation=\"true\" for submit buttons\n" +
                        "3. If an action fails, STOP and explain exactly what went wrong\n" +
                        "4. Report: [ACTION TAKEN] + [RESULT OBSERVED] + [SUCCESS/FAILURE]"));
                    
                    agentCoordinator.AddMessage(AgentRole.Planner, AgentRole.Actioner, 
                        "EXECUTION_REQUEST", plan);

                    PluginLogger.StopLoadingIndicator();
                    PluginLogger.LogPluginUsage("🔧 Executing step...");
                    PluginLogger.StartLoadingIndicator("executing");

                    // Get actioner response with tools
                    string executionResult = await GetAgentResponseAsync(actionerChat, actionerHistory, actionerOptions);
                    
                    // ═══════════════════════════════════════════════════════════════
                    // ANTI-HALLUCINATION CHECK
                    // Detect if actioner claimed to do something without tool calls
                    // ═══════════════════════════════════════════════════════════════
                    bool possibleHallucination = DetectHallucination(executionResult);
                    if (possibleHallucination)
                    {
                        PluginLogger.LogPluginUsage("⚠️ Possible hallucination detected - no tool calls found");
                        executionResult = "[SYSTEM WARNING: The actioner responded without calling any tools. " +
                            "This response may be hallucinated. The actioner MUST call tools to perform actions.]\n\n" +
                            "Original response: " + executionResult;
                    }
                    
                    // Capture post-action state
                    string postActionWindow = windowTracker.GetForegroundWindowInfo();

                    // Handle empty execution results
                    if (string.IsNullOrWhiteSpace(executionResult))
                    {
                        executionResult = "[No tool was called. The actioner must call a tool to perform an action.]";
                    }

                    // Focus Change Alert
                    if (preActionWindow != postActionWindow)
                    {
                        string alert = $"\n\n[SYSTEM ALERT]: Active window focus changed!\n" +
                                       $"Previous: {preActionWindow}\n" +
                                       $"Current:  {postActionWindow}\n" +
                                       $"Use the new Handle ({postActionWindow.Split(',')[0]}) for subsequent interactions.";
                        
                        executionResult += alert;
                        PluginLogger.LogInfo("MultiAgentActioner", "ExecuteAction", "Detected window focus change.");
                    }

                    // ═══════════════════════════════════════════════════════════════
                    // SELF-REFLECTION LOOP
                    // Detect failures and learn from them to prevent repetition
                    // ═══════════════════════════════════════════════════════════════
                    bool stepFailed = DetectStepFailure(executionResult) || possibleHallucination;
                    
                    if (stepFailed)
                    {
                        consecutiveFailures++;
                        PluginLogger.LogPluginUsage($"⚠️ Step appears to have failed ({consecutiveFailures}/{MAX_CONSECUTIVE_FAILURES})");
                        
                        // Ask the actioner to reflect on what went wrong
                        actionerHistory.Add(new ChatMessage(ChatRole.User, 
                            "🔍 REFLECTION REQUIRED: The previous step appears to have failed.\n\n" +
                            "Analyze what went wrong and provide:\n" +
                            "1. WHAT FAILED: What specific action didn't work?\n" +
                            "2. WHY IT FAILED: What was the root cause?\n" +
                            "3. HOW TO FIX: What should be done differently?\n\n" +
                            "Be specific and actionable."));
                        
                        PluginLogger.LogPluginUsage("🔍 Reflecting on failure...");
                        string reflection = await GetAgentResponseAsync(actionerChat, actionerHistory, actionerOptions);
                        
                        // Extract the lesson and add to memory
                        string lesson = ExtractLesson(reflection, plan);
                        if (!string.IsNullOrEmpty(lesson))
                        {
                            lessonsLearned.Add(lesson);
                            PluginLogger.LogPluginUsage($"📚 Lesson learned: {lesson}");
                        }
                        
                        // Add reflection to execution result
                        executionResult += $"\n\n[REFLECTION]:\n{reflection}";
                        
                        // If too many consecutive failures, escalate to coordinator
                        if (consecutiveFailures >= MAX_CONSECUTIVE_FAILURES)
                        {
                            PluginLogger.LogPluginUsage("🚨 Multiple failures detected, requesting coordinator intervention...");
                            
                            coordinatorHistory.Add(new ChatMessage(ChatRole.User,
                                $"⚠️ INTERVENTION NEEDED: The actioner has failed {consecutiveFailures} times in a row.\n\n" +
                                $"Original task: {actionPrompt}\n\n" +
                                $"Current step that keeps failing: {plan}\n\n" +
                                $"Lessons learned:\n{string.Join("\n", lessonsLearned)}\n\n" +
                                "Please provide a DIFFERENT approach to complete this task."));
                            
                            string intervention = await GetAgentResponseAsync(coordinatorChat, coordinatorHistory, coordinatorOptions);
                            
                            // Reset the planner with the new approach
                            plannerHistory.Clear();
                            plannerHistory.Add(new ChatMessage(ChatRole.System, toolConfig.PlannerSystemPrompt + toolDescriptions));
                            plannerHistory.Add(new ChatMessage(ChatRole.User, 
                                $"New approach from coordinator:\n{intervention}\n\n" +
                                $"Previous lessons learned:\n{string.Join("\n", lessonsLearned)}"));
                            
                            consecutiveFailures = 0;
                            PluginLogger.LogPluginUsage("🔄 Trying new approach from coordinator...");
                        }
                    }
                    else
                    {
                        consecutiveFailures = 0; // Reset on success
                    }
                    // ═══════════════════════════════════════════════════════════════

                    // Store the execution result
                    executionResults.Add(executionResult);
                    
                    PluginLogger.LogPluginUsage("📊 Step result:\n" + executionResult);
                    
                    agentCoordinator.AddMessage(AgentRole.Actioner, AgentRole.Planner, 
                        "EXECUTION_RESPONSE", executionResult);

                    // Manage context window to prevent token overflow
                    ManageContextWindow(actionerHistory, 12);
                    ManageContextWindow(plannerHistory, 12);

                    // Add the execution result to the planner's history with enhanced prompting
                    string successIndicator = stepFailed ? "⚠️ STEP FAILED" : "✅ STEP SUCCEEDED";
                    
                    plannerHistory.Add(new ChatMessage(ChatRole.User, 
                        $"{successIndicator}\n\n" +
                        $"Step {currentIteration} Result:\n{executionResult}\n\n" +
                        $"Progress: {currentIteration}/{maxIterations} steps\n\n" +
                        (lessonsLearned.Count > 0 ? $"Lessons learned:\n{string.Join("\n", lessonsLearned)}\n\n" : "") +
                        "Evaluate and respond with EXACTLY ONE of:\n" +
                        "A) 'TASK COMPLETED: [summary of what was accomplished]'\n" +
                        "B) 'NEXT STEP: [specific action to take with exact tool call]'\n" +
                        "C) 'TASK FAILED: [explanation of why it cannot be completed]'"));
                    

                    PluginLogger.StopLoadingIndicator();
                    PluginLogger.LogPluginUsage("🔄 Evaluating progress...");
                    PluginLogger.StartLoadingIndicator("planning");

                    // Get planner's evaluation of the result
                    plan = await GetAgentResponseAsync(plannerChat, plannerHistory, plannerOptions);

                    agentCoordinator.AddMessage(AgentRole.Planner, AgentRole.Coordinator,
                        "STATUS_UPDATE", plan);

                    // Check if the task is complete or failed
                    bool taskCompleted = plan.IndexOf("TASK COMPLETED", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool taskFailed = plan.IndexOf("TASK FAILED", StringComparison.OrdinalIgnoreCase) >= 0;
                    
                    if (taskCompleted || taskFailed)
                    {
                        isComplete = true;

                        if (taskCompleted)
                        {
                            PluginLogger.LogPluginUsage("✅ Task marked as complete by planner");
                        }
                        else
                        {
                            PluginLogger.LogPluginUsage("❌ Task marked as failed by planner");
                        }

                        // Send all execution results to the coordinator for final formatting
                        string executionSummary = string.Join("\n\n", executionResults);
                        string lessonsText = lessonsLearned.Count > 0 
                            ? $"\n\nLessons learned during execution:\n{string.Join("\n", lessonsLearned)}" 
                            : "";

                        coordinatorHistory.Add(new ChatMessage(ChatRole.User, 
                            $"The task has {(taskCompleted ? "been completed" : "failed")} after {currentIteration} steps.\n\n" +
                            $"Complete execution log:\n{executionSummary}\n\n" +
                            $"Planner's final message:\n{plan}" + lessonsText + "\n\n" +
                            "Please provide a clear, user-friendly summary of what was accomplished (or what went wrong). " +
                            "Include specific results, any important details, and the current state. " +
                            "Be concise but informative. Do not use technical tags or internal markers."
                        ));

                        PluginLogger.StopLoadingIndicator();
                        PluginLogger.LogPluginUsage("📝 Generating user response...");
                        PluginLogger.StartLoadingIndicator("coordination");

                        // Get coordinator's final response with detailed results
                        finalResult = await GetAgentResponseAsync(coordinatorChat, coordinatorHistory, coordinatorOptions);

                        // Store this as a completed response
                        agentCoordinator.AddMessage(AgentRole.Coordinator, AgentRole.User,
                            "USER_RESPONSE", finalResult);
                    }
                    else
                    {
                        // Extract just the next step from the plan (remove "NEXT STEP:" prefix if present)
                        if (plan.IndexOf("NEXT STEP:", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            int idx = plan.IndexOf("NEXT STEP:", StringComparison.OrdinalIgnoreCase);
                            plan = plan.Substring(idx + 10).Trim();
                        }
                        PluginLogger.LogPluginUsage($"⏭️ Next step:\n{plan}");
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

        /// <summary>
        /// Detects if the actioner hallucinated (claimed to do something without calling tools)
        /// </summary>
        private bool DetectHallucination(string executionResult)
        {
            if (string.IsNullOrWhiteSpace(executionResult))
                return true; // Empty response = no tool called
            
            string lower = executionResult.ToLowerInvariant();
            
            // CRITICAL: Check for fake tool names that don't exist
            string[] fakeTools = new[]
            {
                "findtextonscreen", "searchtext", "locatetext", "findtext",
                "gettext", "readtext", "scanscreen", "analyzescreen",
                "task.delay", "wait(", "sleep(", "pause(",
                "downloadfile", "savefile", "openfile"
            };
            
            foreach (var fakeTool in fakeTools)
            {
                if (lower.Contains(fakeTool))
                {
                    PluginLogger.LogInfo("MultiAgentActioner", "DetectHallucination", 
                        $"Detected fake tool: {fakeTool}");
                    return true;
                }
            }
            
            // Check for code block tool calls (model writing code instead of calling functions)
            if (executionResult.Contains("```tool_code") || 
                executionResult.Contains("```python") ||
                executionResult.Contains("```csharp"))
            {
                PluginLogger.LogInfo("MultiAgentActioner", "DetectHallucination", 
                    "Detected code block - model writing code instead of calling tools");
                return true;
            }
            
            // Check for fake API responses
            if (lower.Contains("the api returned") && 
                (lower.Contains("'found': true") || lower.Contains("\"found\": true")))
            {
                PluginLogger.LogInfo("MultiAgentActioner", "DetectHallucination", 
                    "Detected fake API response");
                return true;
            }
            
            // Signs that a tool was actually called (real tool output markers)
            string[] realToolIndicators = new[]
            {
                "window handle", "windowhandle", "handle:",
                "screenshot saved", "captured screen", "ui element #",
                "bbox:", "bounding box", "[left", 
                "process started", "command executed",
                "browser launched", "navigated to",
                "clicked at", "typed text", "sent key"
            };
            
            bool hasRealToolOutput = false;
            foreach (var indicator in realToolIndicators)
            {
                if (lower.Contains(indicator))
                {
                    hasRealToolOutput = true;
                    break;
                }
            }
            
            // Signs of hallucination (claims without evidence)
            string[] hallucinationPatterns = new[]
            {
                "i have successfully", "i've successfully", "i successfully",
                "i logged in", "i signed in", "i clicked", "i typed",
                "i opened", "i navigated", "i scrolled", "i liked",
                "the task is complete", "task completed", "done",
                "i performed", "i executed", "i did",
                "[action taken]", "[result observed]", "[success]"
            };
            
            foreach (var pattern in hallucinationPatterns)
            {
                if (lower.Contains(pattern) && !hasRealToolOutput)
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Detects if a step failed based on the execution result
        /// </summary>
        private bool DetectStepFailure(string executionResult)
        {
            if (string.IsNullOrWhiteSpace(executionResult))
                return false;

            string lower = executionResult.ToLowerInvariant();
            
            // Explicit failure indicators
            string[] failurePatterns = new[]
            {
                "error:", "exception:", "failed to", "could not", "unable to",
                "not found", "does not exist", "permission denied", "access denied",
                "timeout", "timed out", "no such", "invalid", "cannot find",
                "null reference", "object reference", "index out of range",
                "window handle is invalid", "element not found", "selector not found",
                "no matching element", "click failed", "type failed"
            };

            foreach (var pattern in failurePatterns)
            {
                if (lower.Contains(pattern))
                    return true;
            }

            // Check for empty or non-actionable results
            if (executionResult.Trim().Length < 10)
                return false; // Too short to determine, assume success
            
            return false;
        }

        /// <summary>
        /// Extracts a concise lesson from a reflection response
        /// </summary>
        private string ExtractLesson(string reflection, string failedStep)
        {
            if (string.IsNullOrWhiteSpace(reflection))
                return $"Step '{TruncateString(failedStep, 50)}' failed - reason unknown";

            // Look for key phrases that indicate lessons
            var lines = reflection.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                var lower = line.ToLowerInvariant();
                // Look for fix recommendations
                if (lower.Contains("should") || lower.Contains("instead") || 
                    lower.Contains("fix:") || lower.Contains("solution:") ||
                    lower.Contains("correct way") || lower.Contains("need to"))
                {
                    return TruncateString(line.Trim(), 150);
                }
            }

            // Fallback: summarize the failure
            return $"Avoid: {TruncateString(failedStep, 50)} - {TruncateString(reflection, 100)}";
        }

        /// <summary>
        /// Truncates a string to a maximum length with ellipsis
        /// </summary>
        private string TruncateString(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
                return input;
            return input.Substring(0, maxLength - 3) + "...";
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
