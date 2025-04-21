using System;
using System.IO;
using System.Text.Json;

namespace FlowVision.lib.Classes
{
    public class OmniParserConfig
    {
        public string ServerURL { get; set; }

        private static string ConfigFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FlowVision",
            "OmniParserConfig.json");

        public static OmniParserConfig LoadConfig()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));

                if (File.Exists(ConfigFilePath))
                {
                    string jsonContent = File.ReadAllText(ConfigFilePath);
                    if (!string.IsNullOrWhiteSpace(jsonContent))
                    {
                        return JsonSerializer.Deserialize<OmniParserConfig>(jsonContent) ?? new OmniParserConfig();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading OmniParser config: {ex.Message}");
            }
            return new OmniParserConfig();
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
                Console.WriteLine($"Error saving OmniParser config: {ex.Message}");
            }
        }
    }
}
