using System;
using System.IO;
using System.Text.Json;

namespace FlowVision.lib.Classes
{
    public class LMStudioConfig
    {
        public string EndpointURL { get; set; } = "http://localhost:1234/v1";
        public string ModelName { get; set; } = "local-model";
        public double Temperature { get; set; } = 0.7;
        public int MaxTokens { get; set; } = 2048;
        public bool Enabled { get; set; } = false;
        public string APIKey { get; set; } = "lm-studio"; // OpenAI client requires a key even if local
        public int TimeoutSeconds { get; set; } = 120;

        // Track if the config was successfully loaded from disk
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsValid { get; set; } = true;

        private static string ConfigFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FlowVision",
            "lmstudioconfig.json");

        public static LMStudioConfig LoadConfig()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));

                if (File.Exists(ConfigFilePath))
                {
                    string jsonContent = File.ReadAllText(ConfigFilePath);
                    // Basic validation for empty file
                    if (string.IsNullOrWhiteSpace(jsonContent))
                    {
                        return new LMStudioConfig { IsValid = false };
                    }

                    var config = JsonSerializer.Deserialize<LMStudioConfig>(jsonContent);
                    if (config != null)
                    {
                        config.IsValid = true;
                        return config;
                    }
                }
            }
            catch (Exception ex)
            {
                // Return a config marked as invalid so the UI can warn the user
                return new LMStudioConfig 
                {
                    Enabled = false,
                    IsValid = false
                };
            }
            
            // Return default if file doesn't exist
            return new LMStudioConfig();
        }

        public void SaveConfig()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonContent = JsonSerializer.Serialize(this, options);
                File.WriteAllText(ConfigFilePath, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving LM Studio config: {ex.Message}");
                throw; // Re-throw so the UI can show the error
            }
        }
    }
}
