using System;
using System.IO;
using System.Text.Json;

namespace FlowVision.lib.UI
{
    /// <summary>
    /// Manages application themes and theme-related settings
    /// </summary>
    public class ThemeManager
    {
        private const string DefaultTheme = "Light";
        private string _currentTheme;
        private readonly string _themePath;

        public ThemeManager()
        {
            // Create theme settings path
            _themePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FlowVision", "Settings", "theme.json");
            
            // Create directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(_themePath));
            
            // Load theme from file or use default
            _currentTheme = LoadThemeFromFile();
        }

        /// <summary>
        /// Gets or sets the current theme
        /// </summary>
        public string CurrentTheme
        {
            get => _currentTheme;
            set
            {
                _currentTheme = value;
                SaveThemeToFile(_currentTheme);
            }
        }

        private string LoadThemeFromFile()
        {
            try
            {
                if (File.Exists(_themePath))
                {
                    string jsonContent = File.ReadAllText(_themePath);
                    var themeSetting = JsonSerializer.Deserialize<ThemeSetting>(jsonContent);
                    return themeSetting?.Name ?? DefaultTheme;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading theme: {ex.Message}");
            }
            
            return DefaultTheme;
        }

        private void SaveThemeToFile(string themeName)
        {
            try
            {
                var themeSetting = new ThemeSetting { Name = themeName };
                string jsonContent = JsonSerializer.Serialize(themeSetting, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_themePath, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving theme: {ex.Message}");
            }
        }

        private class ThemeSetting
        {
            public string Name { get; set; }
        }
    }
}
