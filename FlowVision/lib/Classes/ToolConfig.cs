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
        public bool EnablePluginLogging { get; set; } = true;
        public double Temperature { get; set; } = 0.2;
        public bool AutoInvokeKernelFunctions { get; set; } = true;
        public bool RetainChatHistory { get; set; } = true;
        public string SystemPrompt { get; set; } = @"You are an AI Agent that can use tools to help the user. Use your tools to come up with novel ideas to answer the users requests you dont have the answer to. Do not restart the server";

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
