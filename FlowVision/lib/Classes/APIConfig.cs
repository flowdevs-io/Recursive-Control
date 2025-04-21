using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace FlowVision.lib.Classes
{
    // Make the class public if you intend to access it from other assemblies.
    public class APIConfig
    {
        public string DeploymentName { get; set; }
        public string EndpointURL { get; set; }
        public string APIKey { get; set; }

        private static string ConfigFilePath(string model)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FlowVision",
                $"{model}apiconfig.json");
        }

        public static APIConfig LoadConfig(string model)
        {
            try
            {
                // Ensure the directory exists.
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath(model)));

                if (File.Exists(ConfigFilePath(model)))
                {
                    string jsonContent = File.ReadAllText(ConfigFilePath(model));
                    if (!string.IsNullOrWhiteSpace(jsonContent))
                    {
                        APIConfig config = JsonSerializer.Deserialize<APIConfig>(jsonContent);
                        // Return a new config if deserialization returns null.
                        return config ?? new APIConfig();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
            }
            // Return a new APIConfig instance if file doesn't exist or deserialization fails.
            return new APIConfig();
        }

        public void SaveConfig(string model)
        {
            try
            {
                // Ensure the directory exists.
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath(model)));

                var options = new JsonSerializerOptions { WriteIndented = true };
                // Serialize the current instance (this)
                string jsonContent = JsonSerializer.Serialize(this, options);
                File.WriteAllText(ConfigFilePath(model), jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving config: {ex.Message}");
            }
        }
    }
}
