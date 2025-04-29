using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Manages user configuration profiles
    /// </summary>
    public class SettingsProfileManager
    {
        private readonly string _profilesDirectory;

        public SettingsProfileManager()
        {
            // Create profiles directory path
            _profilesDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FlowVision", "Profiles");
            
            // Ensure the profiles directory exists
            Directory.CreateDirectory(_profilesDirectory);
        }

        /// <summary>
        /// Gets a list of available profile names
        /// </summary>
        public List<string> GetAvailableProfiles()
        {
            try
            {
                // Get all profile files and extract names
                return Directory.GetFiles(_profilesDirectory, "*.json")
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting profiles: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Saves the current configuration to a named profile
        /// </summary>
        public void SaveProfile(string profileName, ToolConfig config)
        {
            try
            {
                string profilePath = Path.Combine(_profilesDirectory, $"{profileName}.json");
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonContent = JsonSerializer.Serialize(config, options);
                File.WriteAllText(profilePath, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving profile: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads a configuration from a named profile
        /// </summary>
        public ToolConfig LoadProfile(string profileName)
        {
            try
            {
                string profilePath = Path.Combine(_profilesDirectory, $"{profileName}.json");
                if (File.Exists(profilePath))
                {
                    string jsonContent = File.ReadAllText(profilePath);
                    return JsonSerializer.Deserialize<ToolConfig>(jsonContent) ?? new ToolConfig();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading profile: {ex.Message}");
            }
            
            // Return default config if profile can't be loaded
            return new ToolConfig();
        }

        /// <summary>
        /// Deletes a named profile
        /// </summary>
        public void DeleteProfile(string profileName)
        {
            try
            {
                string profilePath = Path.Combine(_profilesDirectory, $"{profileName}.json");
                if (File.Exists(profilePath))
                {
                    File.Delete(profilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting profile: {ex.Message}");
            }
        }
    }
}
