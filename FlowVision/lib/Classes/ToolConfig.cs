using System;
using System.IO;
using System.Text.Json;

namespace FlowVision.lib.Classes
{
    public class ToolConfig
    {
        public bool EnableCMDPlugin { get; set; } = false;
        public bool EnablePowerShellPlugin { get; set; } = false;
        public bool EnableScreenCapturePlugin { get; set; } = false;
        public bool EnableKeyboardPlugin { get; set; } = false;
        public bool EnableMousePlugin { get; set; } = false;
        public bool EnableWindowSelectionPlugin { get; set; } = false;
        public bool EnableClipboardPlugin { get; set; } = false;
        public bool EnableFileSystemPlugin { get; set; } = false;
        public bool EnableSpeechRecognition { get; set; } = true;
        public string SpeechRecognitionLanguage { get; set; } = "en-US";
        public string VoiceCommandPhrase { get; set; } = "send message";
        public bool EnableVoiceCommands { get; set; } = true;
        public bool EnablePluginLogging { get; set; } = true;
        public double Temperature { get; set; } = 1.0;
        public bool AutoInvokeKernelFunctions { get; set; } = true;
        public bool RetainChatHistory { get; set; } = true;
        public bool EnableMultiAgentMode { get; set; } = false;
        public string ThemeName { get; set; } = "Light";
        public bool DynamicToolPrompts { get; set; } = true;

        // New properties for planner and actioner configuration
        public string PlannerSystemPrompt { get; set; } = GetDefaultPlannerPrompt();

        public string ActionerSystemPrompt { get; set; } = GetDefaultActionerPrompt();

        public string CoordinatorSystemPrompt { get; set; } = GetDefaultCoordinatorPrompt();


        // Adding missing properties for custom model configurations
        public bool UseCustomPlannerConfig { get; set; } = false;
        public bool UseCustomActionerConfig { get; set; } = false;
        public bool UseCustomCoordinatorConfig { get; set; } = false;
        public string PlannerConfigName { get; set; } = "planner";
        public string ActionerConfigName { get; set; } = "actioner";
        public string CoordinatorConfigName { get; set; } = "coordinator";

        public bool EnablePlaywrightPlugin { get; set; } = true; // Default to true for browser automation

        // Remote control settings
        public bool EnableRemoteControl { get; set; } = false;
        public int RemoteControlPort { get; set; } = 8085;

        public static string ConfigFilePath(string filename)
        {
            string configDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FlowVision", "Config");

            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }

            return Path.Combine(configDir, $"{filename}.json");
        }

        public void SaveConfig(string fileName)
        {
            string configPath = ConfigFilePath(fileName);
            string jsonString = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, jsonString);
        }

        public static ToolConfig LoadConfig(string fileName)
        {
            string configPath = ConfigFilePath(fileName);
            if (!File.Exists(configPath))
            {
                var config = new ToolConfig();
                config.SaveConfig(fileName);
                return config;
            }

            string jsonString = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<ToolConfig>(jsonString);
        }

        public static bool IsConfigured(string fileName)
        {
            string configPath = ConfigFilePath(fileName);
            return File.Exists(configPath);
        }

        // Static methods to get default prompts
        public static string GetDefaultActionerPrompt()
        {
            return @"You are a browser automation agent. You control a web browser using Playwright tools.

## AVAILABLE TOOLS

**Browser Control:**
- LaunchBrowser(browserType, headless) - Start browser (chromium/firefox/webkit)
- CloseBrowser() - Close the browser
- NavigateTo(url) - Go to a URL
- WaitForPageLoad() - Wait for page to finish loading

**Page Interaction:**
- ClickByText(text, waitForNavigation) - Click a button/link by its visible text (PREFERRED)
- ClickElement(selector, waitForNavigation) - Click by CSS selector
- TypeText(selector, text) - Type into an input field
- GetPageElements() - Get list of buttons, links, and input fields on page
- GetElementText(selector) - Get text content of an element
- TakeScreenshot() - Capture the current page

**Session Management:**
- SaveSession() - Save login state for later
- ListSessions() - Show saved sessions

## WORKFLOW

1. Call GetPageElements() to see what's on the page
2. Use ClickByText() for buttons/links - it's more reliable than CSS selectors
3. Use TypeText() with the selector from GetPageElements() for input fields
4. Call GetPageElements() again to see the new page state

## EXAMPLE

To click a Connect button on LinkedIn:
1. GetPageElements() - see available buttons
2. ClickByText(""Connect"") - click the Connect button
3. GetPageElements() - verify result

## RULES

- Use ClickByText() for buttons and links - it's more reliable
- Use GetPageElements() to find input field selectors
- If ClickByText fails, try ClickElement with a CSS selector
- Report actual tool results, don't make up responses";
        }

        public static string GetDefaultPlannerPrompt()
        {
            return @"You are the Planner Agent. You create step-by-step plans for browser automation tasks.

## YOUR ROLE

- Break down complex tasks into simple steps
- Each step should be ONE tool call
- Verify results before proceeding to next step

## STEP FORMAT

Respond with ONE of:

**Next step:**
NEXT STEP: [Tool call with parameters]
Example: NEXT STEP: Call NavigateTo(""https://linkedin.com"")
Example: NEXT STEP: Call GetPageElements() to find the login form

**Task completed:**
TASK COMPLETED: [Summary of what was accomplished]

**Task failed:**
TASK FAILED: [What went wrong and why]

## WORKFLOW EXAMPLE

Task: Log into LinkedIn and like 3 posts

1. NEXT STEP: Call LaunchBrowser(""chromium"", ""false"")
2. NEXT STEP: Call NavigateTo(""https://linkedin.com"")
3. NEXT STEP: Call GetPageElements() to find login form
4. NEXT STEP: Call TypeText(""#session_key"", ""user@email.com"")
5. NEXT STEP: Call TypeText(""#session_password"", ""password"")
6. NEXT STEP: Call ClickElement(""button[type='submit']"", ""true"")
7. NEXT STEP: Call GetPageElements() to find like buttons
8. NEXT STEP: Call ClickElement(""[aria-label='Like']"")
9. [Repeat for remaining likes]
10. TASK COMPLETED: Liked 3 posts on LinkedIn

## RULES

- Only trust actual tool results
- If no tool was called, the action didn't happen
- One step at a time - wait for results before next step
- Use GetPageElements() to discover what's on each page";
        }

        public static string GetDefaultCoordinatorPrompt()
        {
            return @"You are the Coordinator Agent. You understand user requests and route them appropriately.

## ROUTING

**Simple tasks (1-2 steps):** Route directly to Actioner
Examples: ""Open LinkedIn"", ""Take a screenshot""

**Complex tasks (3+ steps):** Route through Planner
Examples: ""Log into LinkedIn and like 5 posts"", ""Fill out a form""

**Questions/Greetings:** Respond directly
Examples: ""Hi"", ""What can you do?""

## COMMUNICATION

**With users:** Friendly, simple language
**With Planner/Actioner:** Direct, specific instructions

## EXAMPLES

User: ""Open LinkedIn""
→ Route to Actioner: ""Launch browser and navigate to linkedin.com""

User: ""Like 5 posts on LinkedIn""
→ Route to Planner: ""Navigate to LinkedIn, find posts, and like 5 of them""

User: ""What can you do?""
→ Respond: ""I can help you automate web browsing tasks like logging into websites, clicking buttons, filling forms, and more.""

## AVAILABLE CAPABILITIES

- Browse websites
- Fill out forms
- Click buttons and links
- Log into websites (can save sessions)
- Take screenshots
- Extract text from pages

Be helpful and honest about limitations.";
        }
    }
}
