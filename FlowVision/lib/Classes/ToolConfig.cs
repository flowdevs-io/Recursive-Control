using System;
using System.IO;
using System.Text.Json;

namespace FlowVision.lib.Classes
{
    public class ToolConfig
    {t
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

        // New properties for planner and executor configuration
        public string PlannerSystemPrompt { get; set; } = @"You are a planning agent responsible for breaking down complex tasks into clear steps. 
Your job is to:
1. Analyze the user's request
2. Create a step-by-step plan to accomplish the goal
3. Send each step to the executor agent
4. Review the executor's results after each step
5. Adapt the plan as needed based on the results
6. Continue until the entire task is complete
7. If use is just greeting respond with hello

YOU CANNOT EXECUTE TOOLS DIRECTLY. Only the execution agent can use tools.
You must work with the execution agent to accomplish the goals.";

        public string ExecutorSystemPrompt { get; set; } = @"You are an execution agent with access to various tools.
Your job is to:
1. Execute the specific step provided by the planner agent
2. Use available tools to accomplish the requested action
3. Report back the results and any observations
4. Do not go beyond the specific step you were asked to perform
5. Be precise and thorough in your execution

You have access to tools like CMD, PowerShell, screen capture, keyboard input, mouse control, and window selection.";

        public bool UseCustomPlannerConfig { get; set; } = false;
        public bool UseCustomExecutorConfig { get; set; } = false;
        public string PlannerConfigName { get; set; } = "planner";
        public string ExecutorConfigName { get; set; } = "executor";

        public static string ConfigFilePath(string configName)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FlowVision",
                $"{configName}.json");
        }

        public static bool IsConfigured(string configName)
        {
            return File.Exists(ConfigFilePath(configName));
        }

        public static ToolConfig LoadConfig(string configName)
        {
            try
            {
                // Ensure the directory exists.
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath(configName)));

                if (File.Exists(ConfigFilePath(configName)))
                {
                    string jsonContent = File.ReadAllText(ConfigFilePath(configName));
                    if (!string.IsNullOrWhiteSpace(jsonContent))
                    {
                        return JsonSerializer.Deserialize<ToolConfig>(jsonContent) ?? new ToolConfig();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tool config: {ex.Message}");
            }
            return new ToolConfig();
        }

        public void SaveConfig(string configName)
        {
            try
            {
                // Ensure the directory exists.
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath(configName)));

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonContent = JsonSerializer.Serialize(this, options);
                File.WriteAllText(ConfigFilePath(configName), jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tool config: {ex.Message}");
            }
        }
    }
}
