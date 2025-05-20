using System;
using System.IO;
using System.Text.Json;

namespace FlowVision.lib.Classes
{
    public class ToolConfig
    {
        public bool EnableCMDPlugin { get; set; } = true;
        public bool EnablePowerShellPlugin { get; set; } = true;
        public bool EnableScreenCapturePlugin { get; set; } = false; // Changed default to false
        public bool EnableKeyboardPlugin { get; set; } = true;
        public bool EnableMousePlugin { get; set; } = false; // Changed default to false
        public bool EnableWindowSelectionPlugin { get; set; } = true; // Added WindowSelectionPlugin
        public bool EnableSpeechRecognition { get; set; } = true; // Added Speech Recognition option
        public string SpeechRecognitionLanguage { get; set; } = "en-US"; // Default language
        public string VoiceCommandPhrase { get; set; } = "send message"; // Default voice command phrase
        public bool EnableVoiceCommands { get; set; } = true; // Enable voice commands feature
        public bool EnablePluginLogging { get; set; } = true;
        public double Temperature { get; set; } = 0.2;
        public bool AutoInvokeKernelFunctions { get; set; } = true;
        public bool RetainChatHistory { get; set; } = true;
        public bool EnableMultiAgentMode { get; set; } = false; // Changed default to false
        public string ThemeName { get; set; } = "Light"; // Added theme property
        public bool DynamicToolPrompts { get; set; } = true; // New property to control dynamic tool prompts

        // New properties for planner and actioner configuration
        public string PlannerSystemPrompt { get; set; } = @"You are the Planner Agent.
Your job is to break down the user's request into a sequence of clear, numbered, tool-specific steps.
Instructions:
- For each step, output only ONE actionable instruction, in imperative form, referencing the exact tool or plugin to use (e.g., 'Use WindowSelectionPlugin to list all windows').
- When the user requests to close or stop an application, always use the safest available method (e.g., 'Use WindowSelectionPlugin to close the Microsoft Edge window'), and avoid using words like 'terminate', 'kill', or 'force close'.
- Do NOT output greetings, conceptual statements, or summaries unless the user is only greeting.
- After each step, review the Actioner Agent's result and adapt the next step as needed.
- If the user is only greeting, respond with a simple greeting and nothing else.
- If a step fails, re-plan and output a new, actionable step.
- Never output more than one step at a time.
- Never output a step that is not directly actionable by the Actioner Agent.";

        public string ActionerSystemPrompt { get; set; } = @"You are an execution agent with access to various tools.
Your job is to:
1. Execute the specific step provided by the planner agent
2. Use available tools to accomplish the requested action
3. Report back the results and any observations
4. Do not go beyond the specific step you were asked to perform
5. Be precise and thorough in your execution

You have access to tools like CMD, PowerShell, screen capture, keyboard input, mouse control, and window selection.

When working with the Playwright browser automation:
1. Before launching a browser, check if one is already active using IsBrowserActive()
2. If a browser is already running, use the existing browser rather than launching a new one
3. Only launch a new browser if necessary or if you need to switch browser types
4. Use GetBrowserStatus() to check detailed information about the current browser state
5. You can force a new browser instance with LaunchBrowser's forceNew parameter if needed";

        public string CoordinatorSystemPrompt { get; set; } = @"You are a coordinator agent responsible for managing the conversation between the user and the AI system.
Your job is to:
1. Understand the user's request
2. Send appropriate tasks to the planning agent
3. Receive and evaluate the final results from the planner
4. Format responses back to the user in a helpful, conversational way
5. Handle any follow-up questions from the user
6. Maintain context throughout the conversation

You are the main interface between the human user and the AI system including the planning and execution agents.
Focus on providing clear, helpful responses that address the user's needs completely.";


        // Adding missing properties for custom model configurations
        public bool UseCustomPlannerConfig { get; set; } = false;
        public bool UseCustomActionerConfig { get; set; } = false;
        public bool UseCustomCoordinatorConfig { get; set; } = false;
        public string PlannerConfigName { get; set; } = "planner";
        public string ActionerConfigName { get; set; } = "actioner";
        public string CoordinatorConfigName { get; set; } = "coordinator";

        public bool EnablePlaywrightPlugin { get; set; } = false; // Default to false for security

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
    }
}
