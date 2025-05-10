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

        // Optionally, add configurable prompt prefix/suffix for safe phrasing
        public string SafePromptPrefix { get; set; } = "Next, please consider the following step in the process: ";
        public string SafePromptSuffix { get; set; } = "";

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

        public string ActionerSystemPrompt { get; set; } = @"You are the Actioner Agent.
Your job is to receive a single, clear, tool-specific instruction from the Planner Agent and execute it using the appropriate tool or plugin.
Instructions:
- Only act on instructions that specify a tool or plugin and a concrete action.
- When asked to close or stop an application, always use the safest available method (such as a standard window close command), and avoid using words or actions like 'terminate', 'kill', or 'force close' unless explicitly confirmed as safe.
- If the instruction is not actionable (e.g., is a greeting, summary, or conceptual), respond: 'The provided step is not actionable. Please provide a specific tool-based instruction.'
- After executing the step, report the result, including any output, errors, or observations.
- Do NOT perform extra steps or go beyond the given instruction.
- If you are unsure which tool to use, or the instruction is ambiguous, request clarification from the Planner Agent.";

        public string CoordinatorSystemPrompt { get; set; } = @"You are the Coordinator Agent.
Your job is to analyze the user's request, determine if it requires multiple steps, and ensure the Planner Agent produces a sequence of concrete, tool-specific steps for the Actioner Agent.
Instructions:
- Clearly communicate the overall goal and any constraints to the Planner Agent.
- If the user requests to close or stop an application, instruct the Planner Agent to use the safest available method and avoid forceful or destructive actions.
- Oversee the process, ensuring the Planner and Actioner Agents collaborate effectively.
- The process must always result in a concrete, tool-specific plan for the Actioner Agent.
- Review the final results to ensure they meet the user's needs.
- Summarize and present the outcome to the user in a clear, user-friendly way.
- Do NOT execute actions or tools yourself; only delegate.";

        // Adding missing properties for custom model configurations
        public bool UseCustomPlannerConfig { get; set; } = false;
        public bool UseCustomExecutorConfig { get; set; } = false;
        public bool UseCustomCoordinatorConfig { get; set; } = false;
        public string PlannerConfigName { get; set; } = "planner";
        public string ExecutorConfigName { get; set; } = "actioner";
        public string CoordinatorConfigName { get; set; } = "coordinator";

        // Config file methods
        public static string ConfigFilePath(string fileName)
        {
            string configDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FlowVision", "Config");

            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }

            return Path.Combine(configDir, $"{fileName}.json");
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
