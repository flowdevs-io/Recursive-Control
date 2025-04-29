// If this file doesn't exist, here's a potential implementation based on observed usage
// Otherwise, you would modify the existing file to add the new properties for the planner and executor

using System;
using System.IO;
using System.Text.Json;

namespace FlowVision.lib.Classes
{
    public class APIConfig
    {
        // Base API configuration
        public string DeploymentName { get; set; } = "";
        public string EndpointURL { get; set; } = "";
        public string APIKey { get; set; } = "";
        
        // Model specific settings
        public int MaxTokens { get; set; } = 4000;
        public float Temperature { get; set; } = 0.7f;
        
        // For multi-agent configurations (optional - can use same model for both)
        public bool UseCustomPlannerModel { get; set; } = false;
        public string PlannerDeploymentName { get; set; } = "";
        public string PlannerEndpointURL { get; set; } = "";
        public string PlannerAPIKey { get; set; } = "";
        
        public bool UseCustomExecutorModel { get; set; } = false;
        public string ExecutorDeploymentName { get; set; } = "";
        public string ExecutorEndpointURL { get; set; } = "";
        public string ExecutorAPIKey { get; set; } = "";
        
        private static readonly string ConfigDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FlowVision", "Config");

        public static APIConfig LoadConfig(string configName)
        {
            try
            {
                string configPath = Path.Combine(ConfigDir, $"{configName}.json");
                
                // If config doesn't exist, create default
                if (!File.Exists(configPath))
                {
                    var defaultConfig = new APIConfig();
                    SaveConfig(defaultConfig, configName);
                    return defaultConfig;
                }
                
                string json = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<APIConfig>(json) ?? new APIConfig();
            }
            catch (Exception)
            {
                // Return default config if loading fails
                return new APIConfig();
            }
        }
        
        public static void SaveConfig(APIConfig config, string configName)
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
