using System;
using System.IO;
using System.Text.Json;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Configuration for LM Studio local AI integration
    /// </summary>
    public class LMStudioConfig
    {
        /// <summary>
        /// LM Studio server endpoint (default: http://localhost:1234/v1)
        /// </summary>
        public string EndpointURL { get; set; } = "http://localhost:1234/v1";
        
        /// <summary>
        /// Model name (e.g., "gpt-3.5-turbo", "local-model", or whatever LM Studio shows)
        /// Can be left as default - LM Studio typically uses the loaded model automatically
        /// </summary>
        public string ModelName { get; set; } = "local-model";
        
        /// <summary>
        /// API key (LM Studio doesn't require one, but field kept for compatibility)
        /// Use "lm-studio" or "not-needed" as placeholder
        /// </summary>
        public string APIKey { get; set; } = "lm-studio";
        
        /// <summary>
        /// Whether to use LM Studio or fall back to Azure
        /// </summary>
        public bool Enabled { get; set; } = false;
        
        /// <summary>
        /// Temperature setting for local model
        /// </summary>
        public double Temperature { get; set; } = 0.7;
        
        /// <summary>
        /// Max tokens for completion
        /// </summary>
        public int MaxTokens { get; set; } = 2048;
        
        /// <summary>
        /// Timeout in seconds for LM Studio requests
        /// </summary>
        public int TimeoutSeconds { get; set; } = 300;

        private static string ConfigFilePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FlowVision",
                "lmstudioconfig.json");
        }

        public static LMStudioConfig LoadConfig()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath()));

                if (File.Exists(ConfigFilePath()))
                {
                    string jsonContent = File.ReadAllText(ConfigFilePath());
                    if (!string.IsNullOrWhiteSpace(jsonContent))
                    {
                        var config = JsonSerializer.Deserialize<LMStudioConfig>(jsonContent);
                        return config ?? new LMStudioConfig();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading LM Studio config: {ex.Message}");
            }
            
            return new LMStudioConfig();
        }

        public void SaveConfig()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath()));

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonContent = JsonSerializer.Serialize(this, options);
                File.WriteAllText(ConfigFilePath(), jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving LM Studio config: {ex.Message}");
            }
        }
    }
}
