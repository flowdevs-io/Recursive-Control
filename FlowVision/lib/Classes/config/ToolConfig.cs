using System;
using System.IO;
using System.Text.Json;

namespace FlowVision.lib.Classes
{
    public class ToolConfig
    {
        // Basic settings
        public string SystemPrompt { get; set; } = "You are a helpful AI assistant with access to tools.";
        public float Temperature { get; set; } = 0.2f;
        public bool AutoInvokeKernelFunctions { get; set; } = true;
        
        // Plugin enablement flags
        public bool EnableCMDPlugin { get; set; } = true;
        public bool EnablePowerShellPlugin { get; set; } = true;
        public bool EnableScreenCapturePlugin { get; set; } = true;
        public bool EnableKeyboardPlugin { get; set; } = true;
        public bool EnableMousePlugin { get; set; } = true;
        public bool EnableWindowSelectionPlugin { get; set; } = true;
        
        // Multi-agent settings
        public bool EnableMultiAgentMode { get; set; } = false; // Default to single-agent mode
        public string PlannerSystemPrompt { get; set; } = "You are a planning agent responsible for breaking down complex tasks into steps.";
        public string ExecutorSystemPrompt { get; set; } = "You are an execution agent with access to various tools.";
        public int MaxAgentIterations { get; set; } = 10;
        
        private static readonly string ConfigDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FlowVision", "Config");

        public static ToolConfig LoadConfig(string configName)
        {
            try
            {
                string configPath = Path.Combine(ConfigDir, $"{configName}.json");
                
                // If config doesn't exist, create default
                if (!File.Exists(configPath))
                {
                    var defaultConfig = new ToolConfig();
                    SaveConfig(defaultConfig, configName);
                    return defaultConfig;
                }
                
                string json = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<ToolConfig>(json) ?? new ToolConfig();
            }
            catch (Exception)
            {
                // Return default config if loading fails
                return new ToolConfig();
            }
        }
        
        public static void SaveConfig(ToolConfig config, string configName)
        {
            try
            {
                // Ensure directory exists
                if (!Directory.Exists(ConfigDir))
                {
                    Directory.CreateDirectory(ConfigDir);
                }
                
                string configPath = Path.Combine(ConfigDir, $"{configName}.json");
                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configPath, json);
            }
            catch (Exception)
            {
                // Handle saving error if needed
            }
        }
    }
}
