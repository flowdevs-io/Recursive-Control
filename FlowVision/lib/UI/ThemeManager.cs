using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

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

        /// <summary>
        /// Applies theme colors to a button control
        /// </summary>
        /// <param name="button">Button to apply theme to</param>
        public void ApplyThemeToButton(Button button)
        {
            if (_currentTheme == "Dark")
            {
                button.BackColor = ThemeColors.Dark.ButtonBackground;
                button.ForeColor = ThemeColors.Dark.ButtonText;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = ThemeColors.Dark.ButtonBorder;
            }
            else
            {
                button.BackColor = ThemeColors.Light.ButtonBackground;
                button.ForeColor = ThemeColors.Light.ButtonText;
                button.FlatStyle = FlatStyle.Standard;
            }
        }

        /// <summary>
        /// Applies theme colors to a textbox control
        /// </summary>
        /// <param name="textBox">TextBox to apply theme to</param>
        public void ApplyThemeToTextBox(TextBoxBase textBox)
        {
            if (_currentTheme == "Dark")
            {
                textBox.BackColor = ThemeColors.Dark.TextBoxBackground;
                textBox.ForeColor = ThemeColors.Dark.TextBoxText;
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }
            else
            {
                textBox.BackColor = ThemeColors.Light.TextBoxBackground;
                textBox.ForeColor = ThemeColors.Light.TextBoxText;
                textBox.BorderStyle = BorderStyle.Fixed3D;
            }
        }

        /// <summary>
        /// Applies theme colors to all controls in a container
        /// </summary>
        /// <param name="container">Container with controls to theme</param>
        public void ApplyThemeToControls(Control container)
        {
            // Set container colors
            if (_currentTheme == "Dark")
            {
                container.BackColor = ThemeColors.Dark.Background;
                container.ForeColor = ThemeColors.Dark.Text;
            }
            else
            {
                container.BackColor = ThemeColors.Light.Background;
                container.ForeColor = ThemeColors.Light.Text;
            }

            // Process all child controls recursively
            foreach (Control control in container.Controls)
            {
                if (control is Button button)
                {
                    ApplyThemeToButton(button);
                }
                else if (control is TextBoxBase textBox)
                {
                    ApplyThemeToTextBox(textBox);
                }
                else if (control is TabPage tabPage)
                {
                    // Apply theme to tab page
                    if (_currentTheme == "Dark")
                    {
                        tabPage.BackColor = ThemeColors.Dark.TabBackground;
                        tabPage.ForeColor = ThemeColors.Dark.TabText;
                    }
                    else
                    {
                        tabPage.BackColor = ThemeColors.Light.TabBackground;
                        tabPage.ForeColor = ThemeColors.Light.TabText;
                    }

                    // Process tab page controls
                    ApplyThemeToControls(tabPage);
                }
                else if (control is GroupBox)
                {
                    // Apply theme to group box
                    if (_currentTheme == "Dark")
                    {
                        control.BackColor = ThemeColors.Dark.Background;
                        control.ForeColor = ThemeColors.Dark.Text;
                    }
                    else
                    {
                        control.BackColor = ThemeColors.Light.Background;
                        control.ForeColor = ThemeColors.Light.Text;
                    }

                    // Process group box controls
                    ApplyThemeToControls(control);
                }
                else if (control.Controls.Count > 0)
                {
                    // Recursively apply theme to container controls
                    ApplyThemeToControls(control);
                }
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
