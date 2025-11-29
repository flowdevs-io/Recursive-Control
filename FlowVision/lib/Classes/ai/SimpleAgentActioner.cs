using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Plugins;
using Microsoft.Extensions.AI;
using FlowVision.lib.Classes.ai;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Simple single-agent actioner with tool verification.
    /// No complex multi-agent coordination - just one agent that uses tools directly.
    /// Tracks actual tool invocations to prevent hallucination.
    /// </summary>
    public class SimpleAgentActioner
    {
        private IChatClient agentChat;
        private List<ChatMessage> chatHistory;
        private ToolConfig toolConfig;
        
        // Track tool invocations
        private static int _toolCallCount = 0;
        private static readonly object _toolLock = new object();
        
        private const string TOOL_CONFIG = "toolsconfig";
        private const string ACTIONER_CONFIG = "actioner";
        private const int MAX_TURNS = 15;

        public SimpleAgentActioner(Form1.PluginOutputHandler outputHandler)
        {
            chatHistory = new List<ChatMessage>();
            
            var hiddenTextBox = new RichTextBox { Visible = false };
            
            if (Application.OpenForms.Count > 0 && Application.OpenForms[0] is Form1 mainForm)
            {
                PluginLogger.Initialize(hiddenTextBox, mainForm.AddMessage);
            }
            else
            {
                PluginLogger.Initialize(hiddenTextBox);
            }

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

        /// <summary>
        /// Reset tool call counter before each agent turn
        /// </summary>
        public static void ResetToolCallCounter()
        {
            lock (_toolLock)
            {
                _toolCallCount = 0;
            }
        }

        /// <summary>
        /// Increment tool call counter (called by plugins)
        /// </summary>
        public static void IncrementToolCallCounter()
        {
            lock (_toolLock)
            {
                _toolCallCount++;
            }
        }

        /// <summary>
        /// Get current tool call count
        /// </summary>
        public static int GetToolCallCount()
        {
            lock (_toolLock)
            {
                return _toolCallCount;
            }
        }

        public async Task<string> ExecuteAction(string userRequest)
        {
            toolConfig = ToolConfig.LoadConfig(TOOL_CONFIG);
            
            PluginLogger.NotifyTaskStart("Agent Task", "Processing your request...");

            try
            {
                // Initialize the agent
                await InitializeAgent();

                // Build system prompt
                string systemPrompt = BuildSystemPrompt();
                chatHistory.Clear();
                chatHistory.Add(new ChatMessage(ChatRole.System, systemPrompt));

                // Add user request
                chatHistory.Add(new ChatMessage(ChatRole.User, userRequest));

                // Get available tools
                var tools = GetTools();
                var chatOptions = new ChatOptions { Tools = tools };

                // Agentic loop
                StringBuilder conversationLog = new StringBuilder();
                int turn = 0;

                while (turn < MAX_TURNS)
                {
                    turn++;
                    PluginLogger.LogPluginUsage($"🔄 Turn {turn}/{MAX_TURNS}");

                    // Reset tool counter before this turn
                    ResetToolCallCounter();

                    // Get agent response (with automatic function invocation)
                    var responseBuilder = new StringBuilder();
                    
                    try
                    {
                        await foreach (var update in agentChat.GetStreamingResponseAsync(chatHistory, chatOptions))
                        {
                            if (update.Text != null)
                            {
                                responseBuilder.Append(update.Text);
                            }
                        }
                    }
                    catch (Exception ex) when (ex.Message.Contains("Unknown ChatFinishReason") || 
                                               ex.Message.Contains("function_call"))
                    {
                        // Ignore known SDK finish reason errors
                        PluginLogger.LogInfo("SimpleAgentActioner", "ExecuteAction", 
                            $"Ignored SDK error: {ex.Message}");
                    }

                    string response = responseBuilder.ToString().Trim();
                    
                    // Log the response
                    if (!string.IsNullOrEmpty(response))
                    {
                        chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
                        conversationLog.AppendLine($"[Turn {turn}]: {response}");
                        PluginLogger.LogPluginUsage($"💬 Agent: {TruncateForLog(response)}");
                    }

                    // Check how many tools were actually called
                    int toolsCalled = GetToolCallCount();
                    PluginLogger.LogInfo("SimpleAgentActioner", "ExecuteAction", 
                        $"Turn {turn}: {toolsCalled} tools called");

                    // Check for task completion
                    if (IsTaskComplete(response))
                    {
                        PluginLogger.NotifyTaskComplete("Agent Task", true);
                        return ExtractFinalResponse(response, conversationLog.ToString());
                    }

                    // Check for task failure
                    if (IsTaskFailed(response))
                    {
                        PluginLogger.NotifyTaskComplete("Agent Task", false);
                        return ExtractFailureResponse(response, conversationLog.ToString());
                    }

                    // If no tools were called and agent is just talking, prompt to take action
                    if (toolsCalled == 0 && !string.IsNullOrEmpty(response))
                    {
                        chatHistory.Add(new ChatMessage(ChatRole.User, 
                            "[SYSTEM] You must call tools to perform actions. " +
                            "Don't just describe what you would do - actually call the tools. " +
                            "Start by calling GetPageElements() to see what's on the page."));
                    }

                    // Small delay between turns
                    await Task.Delay(500);
                }

                // Max turns reached
                PluginLogger.NotifyTaskComplete("Agent Task", false);
                return $"I made progress but couldn't complete the task in {MAX_TURNS} turns.\n\n" +
                       $"Here's what happened:\n{conversationLog}";
            }
            catch (Exception ex)
            {
                PluginLogger.NotifyTaskComplete("Agent Task", false);
                PluginLogger.LogError("SimpleAgentActioner", "ExecuteAction", ex.Message);
                return $"Error: {ex.Message}";
            }
        }

        private async Task InitializeAgent()
        {
            // Get API config
            var apiConfig = APIConfig.LoadConfig(ACTIONER_CONFIG);
            
            // Validate config before creating client
            if (string.IsNullOrWhiteSpace(apiConfig.EndpointURL) || 
                string.IsNullOrWhiteSpace(apiConfig.APIKey))
            {
                throw new InvalidOperationException(
                    "API not configured. Please go to Settings and configure your AI provider (Azure OpenAI, OpenAI, LM Studio, or Gemini).");
            }
            
            // Create chat client using factory
            var baseChatClient = AIClientFactory.CreateClient(apiConfig);

            // Wrap with function invocation support
            if (toolConfig.AutoInvokeKernelFunctions)
            {
                agentChat = new ChatClientBuilder(baseChatClient)
                    .UseFunctionInvocation()
                    .Build();
            }
            else
            {
                agentChat = baseChatClient;
            }

            await Task.CompletedTask;
        }

        private string BuildSystemPrompt()
        {
            return @"You are a Windows computer control agent. You complete tasks by calling tools.

## CRITICAL RULES

1. **ALWAYS call tools** - Don't describe actions, perform them
2. **Use Playwright for web tasks** - LaunchBrowser, NavigateTo, ClickElement, TypeText
3. **One action at a time** - Perform action, verify result, continue

## PLAYWRIGHT WORKFLOW (for web automation)

1. LaunchBrowser(browserType, headless) - Start browser ('chromium', 'firefox', or 'webkit')
2. NavigateTo(url) - Go to a website
3. ClickElement(selector) - Click using CSS selector (e.g., 'button.submit', '#login', 'a[href*=login]')
4. TypeText(selector, text) - Type into input field
5. GetPageContent() - Get page HTML/text to find elements
6. CloseBrowser() - When done

## EXAMPLE: Login to a website

1. LaunchBrowser('chromium', false)
2. NavigateTo('https://example.com/login')
3. TypeText('#username', 'myuser')
4. TypeText('#password', 'mypass')  
5. ClickElement('button[type=submit]')
6. GetPageContent() to verify login worked

## AVAILABLE TOOLS

**Playwright (Web):**
- LaunchBrowser(browserType, headless) - Start browser
- NavigateTo(url) - Go to URL
- ClickElement(selector) - Click element by CSS selector
- TypeText(selector, text) - Type into input
- GetPageContent() - Get page content
- CloseBrowser() - Close browser

**System:**
- ExecuteCommand(cmd) - Run shell command
- ListWindowHandles() - Get open windows

## CSS SELECTOR TIPS

- By ID: '#loginButton'
- By class: '.submit-btn'
- By tag: 'button', 'input', 'a'
- By attribute: 'input[name=email]', 'a[href*=login]'
- By text (Playwright): 'text=Sign In', 'button:has-text(Submit)'

## COMPLETION

When done: TASK COMPLETE: [what you did]
If stuck: TASK FAILED: [why]";
        }

        private IList<AITool> GetTools()
        {
            var tools = new List<AITool>();

            // Playwright is the primary tool for web automation
            if (toolConfig.EnablePlaywrightPlugin)
            {
                tools.AddRange(PluginToolExtractor.ExtractTools(PlaywrightPlugin.Instance));
            }

            if (toolConfig.EnableWindowSelectionPlugin)
            {
                tools.AddRange(PluginToolExtractor.ExtractTools(new WindowSelectionPlugin()));
            }

            if (toolConfig.EnableMousePlugin)
            {
                tools.AddRange(PluginToolExtractor.ExtractTools(new MousePlugin()));
            }

            if (toolConfig.EnableKeyboardPlugin)
            {
                tools.AddRange(PluginToolExtractor.ExtractTools(new KeyboardPlugin()));
            }

            if (toolConfig.EnableCMDPlugin)
            {
                tools.AddRange(PluginToolExtractor.ExtractTools(new CMDPlugin()));
            }

            if (toolConfig.EnablePowerShellPlugin)
            {
                tools.AddRange(PluginToolExtractor.ExtractTools(new PowerShellPlugin()));
            }

            return tools;
        }

        private bool IsTaskComplete(string response)
        {
            if (string.IsNullOrEmpty(response)) return false;
            var lower = response.ToLowerInvariant();
            return lower.Contains("task complete:") || 
                   lower.Contains("task completed:") ||
                   lower.Contains("successfully completed");
        }

        private bool IsTaskFailed(string response)
        {
            if (string.IsNullOrEmpty(response)) return false;
            var lower = response.ToLowerInvariant();
            return lower.Contains("task failed:") ||
                   lower.Contains("cannot complete") ||
                   lower.Contains("unable to complete");
        }

        private string ExtractFinalResponse(string lastResponse, string conversationLog)
        {
            // Try to extract just the completion message
            var lines = lastResponse.Split('\n');
            foreach (var line in lines)
            {
                if (line.ToLowerInvariant().Contains("task complete"))
                {
                    return line.Trim();
                }
            }
            return lastResponse;
        }

        private string ExtractFailureResponse(string lastResponse, string conversationLog)
        {
            var lines = lastResponse.Split('\n');
            foreach (var line in lines)
            {
                if (line.ToLowerInvariant().Contains("task failed"))
                {
                    return line.Trim();
                }
            }
            return $"Task could not be completed.\n\nDetails:\n{lastResponse}";
        }

        private string TruncateForLog(string text, int maxLength = 100)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (text.Length <= maxLength) return text.Replace("\n", " ");
            return text.Substring(0, maxLength).Replace("\n", " ") + "...";
        }

        public void SetChatHistory(List<LocalChatMessage> history)
        {
            chatHistory.Clear();
            foreach (var msg in history)
            {
                if (msg.Author == "You")
                    chatHistory.Add(new ChatMessage(ChatRole.User, msg.Content));
                else if (msg.Author == "AI")
                    chatHistory.Add(new ChatMessage(ChatRole.Assistant, msg.Content));
            }
        }
    }
}
