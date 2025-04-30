using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Classes;
using FlowVision.lib.UI;

namespace FlowVision
{
    public partial class ToolConfigForm : Form
    {
        private string toolFileName = "toolsconfig";
        private ToolConfig _toolConfig;
        private bool _isNewConfiguration = false;
        private ThemeManager _themeManager;
        private SettingsProfileManager _profileManager;
        private ToolTip _formToolTip;

        public ToolConfigForm(bool openAsNew = false)
        {
            InitializeComponent();
            _isNewConfiguration = openAsNew;

            // Initialize theme manager
            _themeManager = new ThemeManager();

            // Initialize profile manager
            _profileManager = new SettingsProfileManager();

            // Initialize tooltip
            InitializeToolTips();

            // Populate theme options
            cbTheme.Items.Clear();
            cbTheme.Items.Add("Light");
            cbTheme.Items.Add("Dark");

            // Populate speech recognition language options
            PopulateSpeechLanguages();

            // Populate available API configuration files
            PopulateAPIConfigurations();

            // Populate configuration profiles
            PopulateConfigurationProfiles();

            LoadToolConfig();

            // Apply current theme
            ApplyTheme(_themeManager.CurrentTheme);

            // Make sure the executor prompt is always editable by forcing the tab to be enabled
            tabExecutioner.Enabled = true;
            txtExecutorSystemPrompt.Enabled = true;
            lblExecutorPrompt.Enabled = true;

            // If this is a new configuration being opened automatically,
            // show a helpful message to the user
            if (_isNewConfiguration)
            {
                MessageBox.Show(
                    "Welcome to Tool Configuration! Default settings have been applied. " +
                    "You can customize these settings and save them for future sessions.",
                    "Tool Configuration",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void InitializeToolTips()
        {
            _formToolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 1000,
                ReshowDelay = 500,
                ShowAlways = true
            };

            // Define tooltips in a dictionary for easier management
            var tooltips = new Dictionary<Control, string>
            {
                { chkCMDPlugin, "Enable command prompt access for the AI assistant" },
                { chkPowerShellPlugin, "Enable PowerShell script execution for the AI assistant" },
                { chkScreenCapturePlugin, "Allow the AI assistant to capture screenshots" },
                { chkKeyboardPlugin, "Allow the AI assistant to control keyboard input" },
                { chkMousePlugin, "Allow the AI assistant to control mouse movements" },
                { chkWindowSelectionPlugin, "Allow the AI assistant to interact with specific windows" },
                { enablePluginLoggingCheckBox, "Record all plugin activities for troubleshooting" },
                { numTemperature, "Controls AI randomness: Higher values = more creative, lower values = more deterministic" },
                { chkMultiAgentMode, "Enable planner and executor agent mode for complex tasks" },
                { chkAutoInvoke, "Allow AI to automatically execute tools without confirmation" },
                { chkRetainChatHistory, "Save conversation history between sessions" },
                { chkEnableSpeechRecognition, "Enable voice input recognition" },
                { comboSpeechLanguage, "Select language for speech recognition" },
                { chkEnableVoiceCommands, "Enable voice command activation" },
                { txtVoiceCommandPhrase, "Phrase to activate voice commands (e.g. 'Hey Assistant')" },
                { cbTheme, "Choose light or dark theme for the interface" },
                { cmbProfiles, "Load or save different configuration profiles" }
            };

            foreach (var kvp in tooltips)
            {
                _formToolTip.SetToolTip(kvp.Key, kvp.Value);
            }
        }

        private void PopulateConfigurationProfiles()
        {
            var profiles = _profileManager.GetAvailableProfiles();
            cmbProfiles.Items.Clear();
            foreach (var profile in profiles)
            {
                cmbProfiles.Items.Add(profile);
            }

            if (cmbProfiles.Items.Count > 0)
            {
                cmbProfiles.SelectedIndex = 0;
            }
        }

        private void ApplyTheme(string themeName)
        {
            if (themeName == "Dark")
            {
                ApplyDarkTheme();
            }
            else
            {
                ApplyLightTheme();
            }
        }

        private void ApplyLightTheme()
        {
            // Set light theme colors
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;

            // Apply to all tab pages
            foreach (TabPage tab in tabControlMain.TabPages)
            {
                tab.BackColor = Color.White;
                tab.ForeColor = Color.Black;
            }

            // Apply to group boxes
            foreach (Control control in this.Controls)
            {
                if (control is GroupBox)
                {
                    control.BackColor = Color.White;
                    control.ForeColor = Color.Black;
                }
            }

            // Update status indicator
            UpdateStatusIndicators();
        }

        private void ApplyDarkTheme()
        {
            // Set dark theme colors
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;

            // Apply to all tab pages
            foreach (TabPage tab in tabControlMain.TabPages)
            {
                tab.BackColor = Color.FromArgb(45, 45, 48);
                tab.ForeColor = Color.White;
            }

            // Apply to group boxes
            foreach (Control control in this.Controls)
            {
                if (control is GroupBox)
                {
                    control.BackColor = Color.FromArgb(45, 45, 48);
                    control.ForeColor = Color.White;
                }
            }

            // Update status indicator
            UpdateStatusIndicators();
        }

        private void PopulateAPIConfigurations()
        {
            try
            {
                string configDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "FlowVision", "Config");

                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                string[] configFiles = Directory.GetFiles(configDir, "*.json");
                var configNames = configFiles.Select(Path.GetFileNameWithoutExtension).ToList();

                if (!configNames.Contains("actioner")) configNames.Add("actioner");
                if (!configNames.Contains("planner")) configNames.Add("planner");
                if (!configNames.Contains("executor")) configNames.Add("executor");

                comboPlannerConfig.Items.Clear();
                comboExecutorConfig.Items.Clear();

                foreach (var name in configNames)
                {
                    comboPlannerConfig.Items.Add(name);
                    comboExecutorConfig.Items.Add(name);
                }

                if (comboPlannerConfig.Items.Count > 0)
                {
                    comboPlannerConfig.SelectedItem = comboPlannerConfig.Items.Contains("planner") ? 
                        "planner" : comboPlannerConfig.Items[0];
                }
                
                if (comboExecutorConfig.Items.Count > 0)
                {
                    comboExecutorConfig.SelectedItem = comboExecutorConfig.Items.Contains("executor") ? 
                        "executor" : comboExecutorConfig.Items[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating API configurations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateSpeechLanguages()
        {
            comboSpeechLanguage.Items.Clear();
            
            try
            {
                foreach (var recognizerInfo in SpeechRecognitionEngine.InstalledRecognizers())
                {
                    comboSpeechLanguage.Items.Add(recognizerInfo.Culture.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting speech recognizers: {ex.Message}");
            }

            // If no recognizers found or an error occurred, add default items
            if (comboSpeechLanguage.Items.Count == 0)
            {
                comboSpeechLanguage.Items.AddRange(new object[] {
                    "en-US",
                    "en-GB",
                    "en-AU",
                    "fr-FR",
                    "es-ES",
                    "de-DE"
                });
            }
            
            // Select the first item if none is selected
            if (comboSpeechLanguage.SelectedIndex == -1 && comboSpeechLanguage.Items.Count > 0)
            {
                comboSpeechLanguage.SelectedIndex = 0;
            }
        }

        private void LoadToolConfig()
        {
            try
            {
                string configPath = ToolConfig.ConfigFilePath(toolFileName);
                bool configExists = File.Exists(configPath);

                _toolConfig = ToolConfig.LoadConfig(toolFileName) ?? new ToolConfig();

                // Safely update UI elements
                chkCMDPlugin.Checked = _toolConfig.EnableCMDPlugin;
                chkPowerShellPlugin.Checked = _toolConfig.EnablePowerShellPlugin;
                chkScreenCapturePlugin.Checked = _toolConfig.EnableScreenCapturePlugin;
                chkKeyboardPlugin.Checked = _toolConfig.EnableKeyboardPlugin;
                chkMousePlugin.Checked = _toolConfig.EnableMousePlugin;
                chkWindowSelectionPlugin.Checked = _toolConfig.EnableWindowSelectionPlugin;
                enablePluginLoggingCheckBox.Checked = _toolConfig.EnablePluginLogging;

                numTemperature.Value = (decimal)_toolConfig.Temperature;
                chkAutoInvoke.Checked = _toolConfig.AutoInvokeKernelFunctions;
                chkRetainChatHistory.Checked = _toolConfig.RetainChatHistory;
                chkMultiAgentMode.Checked = _toolConfig.EnableMultiAgentMode;
                chkEnableSpeechRecognition.Checked = _toolConfig.EnableSpeechRecognition;
                chkEnableVoiceCommands.Checked = _toolConfig.EnableVoiceCommands;
                txtVoiceCommandPhrase.Text = _toolConfig.VoiceCommandPhrase;

                // Check if the item exists in the combo box before setting it
                if (comboSpeechLanguage.Items.Contains(_toolConfig.SpeechRecognitionLanguage))
                {
                    comboSpeechLanguage.SelectedItem = _toolConfig.SpeechRecognitionLanguage;
                }
                else if (comboSpeechLanguage.Items.Count > 0)
                {
                    comboSpeechLanguage.SelectedIndex = 0;
                }

                txtPlannerSystemPrompt.Text = _toolConfig.PlannerSystemPrompt;
                txtExecutorSystemPrompt.Text = _toolConfig.ExecutorSystemPrompt;
                chkUseCustomPlannerConfig.Checked = _toolConfig.UseCustomPlannerConfig;
                chkUseCustomExecutorConfig.Checked = _toolConfig.UseCustomExecutorConfig;

                // Check if the item exists in the combo box before setting it
                if (comboPlannerConfig.Items.Contains(_toolConfig.PlannerConfigName))
                {
                    comboPlannerConfig.SelectedItem = _toolConfig.PlannerConfigName;
                }
                else if (comboPlannerConfig.Items.Count > 0)
                {
                    comboPlannerConfig.SelectedIndex = 0;
                }

                if (comboExecutorConfig.Items.Contains(_toolConfig.ExecutorConfigName))
                {
                    comboExecutorConfig.SelectedItem = _toolConfig.ExecutorConfigName;
                }
                else if (comboExecutorConfig.Items.Count > 0)
                {
                    comboExecutorConfig.SelectedIndex = 0;
                }

                // Set the theme in the combo box
                if (cbTheme.Items.Contains(_toolConfig.ThemeName))
                {
                    cbTheme.SelectedItem = _toolConfig.ThemeName;
                }
                else
                {
                    cbTheme.SelectedIndex = 0; // Default to the first item
                }

                UpdateMultiAgentUIState();
                UpdatePlannerConfigUIState();
                UpdateExecutorConfigUIState();
                UpdateStatusIndicators();

                if (_isNewConfiguration && !configExists)
                {
                    _toolConfig.SaveConfig(toolFileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatusIndicators()
        {
            var statusMappings = new Dictionary<CheckBox, PictureBox>
            {
                { chkCMDPlugin, indicatorCMD },
                { chkPowerShellPlugin, indicatorPowerShell },
                { chkScreenCapturePlugin, indicatorScreenCapture },
                { chkKeyboardPlugin, indicatorKeyboard },
                { chkMousePlugin, indicatorMouse },
                { chkWindowSelectionPlugin, indicatorWindow },
                { chkMultiAgentMode, indicatorMultiAgent },
                { chkEnableSpeechRecognition, indicatorSpeech },
                { chkEnableVoiceCommands, indicatorVoiceCmd },
                { chkAutoInvoke, indicatorAutoInvoke }
            };

            foreach (var mapping in statusMappings)
            {
                UpdateStatusIndicator(mapping.Key, mapping.Value);
            }
        }

        private void UpdateStatusIndicator(CheckBox checkBox, PictureBox indicator)
        {
            if (checkBox.Checked)
            {
                // Use a green checkmark emoji for enabled status
                indicator.Image = null;
                indicator.BackColor = Color.Transparent;
                indicator.Text = "✅";
                indicator.Visible = true;
            }
            else
            {
                // Use a red X emoji for disabled status
                indicator.Image = null;
                indicator.BackColor = Color.Transparent;
                indicator.Text = "❌";
                indicator.Visible = true;
            }
        }

        private void UpdateMultiAgentUIState()
        {
            // Instead of disabling the entire tab, only disable planner-specific controls
            // while keeping executor-related controls enabled
            tabPlanner.Enabled = true;
            tabExecutioner.Enabled = true;
            tabCoordinator.Enabled = true;

            // Only enable planner-specific controls when multi-agent mode is checked
            txtPlannerSystemPrompt.Enabled = chkMultiAgentMode.Checked;
            lblPlannerPrompt.Enabled = chkMultiAgentMode.Checked;
            chkUseCustomPlannerConfig.Enabled = chkMultiAgentMode.Checked;
            comboPlannerConfig.Enabled = chkMultiAgentMode.Checked && chkUseCustomPlannerConfig.Checked;
            btnConfigurePlanner.Enabled = chkMultiAgentMode.Checked && chkUseCustomPlannerConfig.Checked;

            // Executor controls remain enabled regardless of multi-agent mode

            UpdateStatusIndicator(chkMultiAgentMode, indicatorMultiAgent);
        }

        private void UpdatePlannerConfigUIState()
        {
            // Enable or disable the planner config dropdown based on checkbox
            comboPlannerConfig.Enabled = chkUseCustomPlannerConfig.Checked;
            btnConfigurePlanner.Enabled = chkUseCustomPlannerConfig.Checked;
        }

        private void UpdateExecutorConfigUIState()
        {
            // Enable or disable the executor config dropdown based on checkbox
            comboExecutorConfig.Enabled = chkUseCustomExecutorConfig.Checked;
            btnConfigureExecutor.Enabled = chkUseCustomExecutorConfig.Checked;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Save existing tool config options
            _toolConfig.EnableCMDPlugin = chkCMDPlugin.Checked;
            _toolConfig.EnablePowerShellPlugin = chkPowerShellPlugin.Checked;
            _toolConfig.EnableScreenCapturePlugin = chkScreenCapturePlugin.Checked;
            _toolConfig.EnableKeyboardPlugin = chkKeyboardPlugin.Checked;
            _toolConfig.EnableMousePlugin = chkMousePlugin.Checked;
            _toolConfig.EnableWindowSelectionPlugin = chkWindowSelectionPlugin.Checked;
            _toolConfig.EnablePluginLogging = enablePluginLoggingCheckBox.Checked;

            _toolConfig.Temperature = (double)numTemperature.Value;
            _toolConfig.AutoInvokeKernelFunctions = chkAutoInvoke.Checked;
            _toolConfig.RetainChatHistory = chkRetainChatHistory.Checked;

            // Save multi-agent mode setting
            _toolConfig.EnableMultiAgentMode = chkMultiAgentMode.Checked;

            // Save speech recognition settings
            _toolConfig.EnableSpeechRecognition = chkEnableSpeechRecognition.Checked;
            if (comboSpeechLanguage.SelectedItem != null)
            {
                _toolConfig.SpeechRecognitionLanguage = comboSpeechLanguage.SelectedItem.ToString();
            }

            // Save voice command settings
            _toolConfig.EnableVoiceCommands = chkEnableVoiceCommands.Checked;
            _toolConfig.VoiceCommandPhrase = txtVoiceCommandPhrase.Text;

            // Save planner and executor settings
            _toolConfig.PlannerSystemPrompt = txtPlannerSystemPrompt.Text;
            _toolConfig.ExecutorSystemPrompt = txtExecutorSystemPrompt.Text;

            // Save custom model configuration options
            _toolConfig.UseCustomPlannerConfig = chkUseCustomPlannerConfig.Checked;
            _toolConfig.UseCustomExecutorConfig = chkUseCustomExecutorConfig.Checked;

            if (comboPlannerConfig.SelectedItem != null)
            {
                _toolConfig.PlannerConfigName = comboPlannerConfig.SelectedItem.ToString();
            }

            if (comboExecutorConfig.SelectedItem != null)
            {
                _toolConfig.ExecutorConfigName = comboExecutorConfig.SelectedItem.ToString();
            }

            // Save theme configuration
            if (cbTheme.SelectedItem != null)
            {
                _toolConfig.ThemeName = cbTheme.SelectedItem.ToString();
                _themeManager.CurrentTheme = cbTheme.SelectedItem.ToString();
            }

            _toolConfig.SaveConfig(toolFileName);

            // Update any active speech recognition services with the new command phrase
            try {
                foreach (Form form in Application.OpenForms)
                {
                    if (form is Form1 mainForm)
                    {
                        var field = typeof(Form1).GetField("speechRecognition",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (field != null)
                        {
                            var speechService = field.GetValue(mainForm) as SpeechRecognitionService;
                            speechService?.UpdateCommandPhrase(_toolConfig.VoiceCommandPhrase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update speech service: {ex.Message}");
            }

            // Show save notification
            ShowSaveNotification();
        }

        private void ShowSaveNotification()
        {
            // Show save notification with fade effect
            saveNotification.Visible = true;
            saveNotification.BringToFront();

            // Start the timer to hide the notification
            saveNotificationTimer.Start();
        }

        private void saveNotificationTimer_Tick(object sender, EventArgs e)
        {
            // Hide notification after timer expires
            saveNotification.Visible = false;
            saveNotificationTimer.Stop();
        }

        private void cancelButton_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        // Event handler for multi-agent mode checkbox
        private void chkMultiAgentMode_CheckedChanged(object sender, EventArgs e)
        {
            // Update UI state when the multi-agent checkbox is toggled
            UpdateMultiAgentUIState();
        }

        // Event handlers for custom config checkboxes
        private void chkUseCustomPlannerConfig_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePlannerConfigUIState();
        }

        private void chkUseCustomExecutorConfig_CheckedChanged(object sender, EventArgs e)
        {
            UpdateExecutorConfigUIState();
        }

        // Event handlers for config buttons
        private void btnConfigurePlanner_Click(object sender, EventArgs e)
        {
            if (comboPlannerConfig.SelectedItem != null)
            {
                string configName = comboPlannerConfig.SelectedItem.ToString();
                OpenAPIConfigForm(configName);
            }
        }

        private void btnConfigureExecutor_Click(object sender, EventArgs e)
        {
            if (comboExecutorConfig.SelectedItem != null)
            {
                string configName = comboExecutorConfig.SelectedItem.ToString();
                OpenAPIConfigForm(configName);
            }
        }

        // Helper method to open API config form
        private void OpenAPIConfigForm(string configName)
        {
            using (var apiConfigForm = new ConfigForm(configName))
            {
                apiConfigForm.ShowDialog();

                // Refresh the API configurations after editing
                PopulateAPIConfigurations();
            }
        }

        // Event handler for theme selection
        private void cbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTheme.SelectedItem != null)
            {
                string selectedTheme = cbTheme.SelectedItem.ToString();
                _themeManager.CurrentTheme = selectedTheme;
                ApplyTheme(selectedTheme);
            }
        }

        // Event handlers for profile management
        private void btnSaveProfile_Click(object sender, EventArgs e)
        {
            using (var inputDialog = new InputDialog("Save Profile", "Enter profile name:"))
            {
                if (inputDialog.ShowDialog() == DialogResult.OK)
                {
                    string profileName = inputDialog.InputText;
                    if (!string.IsNullOrEmpty(profileName))
                    {
                        // Save current settings to profile
                        _profileManager.SaveProfile(profileName, _toolConfig);

                        // Refresh profile list
                        PopulateConfigurationProfiles();

                        // Select the newly created profile
                        cmbProfiles.SelectedItem = profileName;

                        // Show confirmation
                        MessageBox.Show($"Profile '{profileName}' saved successfully.",
                            "Profile Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void btnLoadProfile_Click(object sender, EventArgs e)
        {
            if (cmbProfiles.SelectedItem != null)
            {
                string profileName = cmbProfiles.SelectedItem.ToString();

                // Load the selected profile
                ToolConfig profileConfig = _profileManager.LoadProfile(profileName);
                if (profileConfig != null)
                {
                    _toolConfig = profileConfig;
                    LoadToolConfig(); // Refresh UI with loaded config

                    // Show confirmation
                    MessageBox.Show($"Profile '{profileName}' loaded successfully.",
                        "Profile Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            if (cmbProfiles.SelectedItem != null)
            {
                string profileName = cmbProfiles.SelectedItem.ToString();

                // Confirm deletion
                if (MessageBox.Show($"Are you sure you want to delete profile '{profileName}'?",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Delete the profile
                    _profileManager.DeleteProfile(profileName);

                    // Refresh profile list
                    PopulateConfigurationProfiles();

                    // Show confirmation
                    MessageBox.Show($"Profile '{profileName}' deleted.",
                        "Profile Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void txtSearchSettings_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearchSettings.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // If search is empty, show all tabs and reset visuals
                foreach (TabPage tab in tabControlMain.TabPages)
                {
                    tab.Text = tab.Name.Replace("tab", "");
                }

                searchResultLabel.Visible = false;
                return;
            }

            // Search through all controls on all tabs
            int matchCount = 0;
            List<string> matchedTabs = new List<string>();

            foreach (TabPage tab in tabControlMain.TabPages)
            {
                bool tabHasMatch = false;

                // Check tab name/title first
                if (tab.Text.ToLower().Contains(searchText))
                {
                    tabHasMatch = true;
                    matchCount++;
                }

                // Check all controls in this tab
                foreach (Control control in tab.Controls)
                {
                    // Check if control text/name contains search text
                    if (control.Text.ToLower().Contains(searchText))
                    {
                        tabHasMatch = true;
                        matchCount++;
                    }

                    // For group boxes, check their child controls
                    if (control is GroupBox groupBox)
                    {
                        foreach (Control childControl in groupBox.Controls)
                        {
                            if (childControl.Text.ToLower().Contains(searchText))
                            {
                                tabHasMatch = true;
                                matchCount++;
                            }
                        }
                    }
                }

                if (tabHasMatch)
                {
                    matchedTabs.Add(tab.Name);
                    tab.Text = "✓ " + tab.Name.Replace("tab", "");
                }
                else
                {
                    tab.Text = tab.Name.Replace("tab", "");
                }
            }

            // Display results
            searchResultLabel.Text = $"Found {matchCount} matches in {matchedTabs.Count} tabs";
            searchResultLabel.Visible = true;

            // If there's a match and only one tab has matches, switch to that tab
            if (matchedTabs.Count == 1)
            {
                TabPage matchedTab = tabControlMain.TabPages[matchedTabs[0]];
                tabControlMain.SelectedTab = matchedTab;
            }
        }

        private void chkPluginStatus_CheckedChanged(object sender, EventArgs e)
        {
            // Update status indicators when any plugin checkbox changes
            UpdateStatusIndicators();
        }
    }

    // Simple input dialog for profile name entry
    public class InputDialog : Form
    {
        private TextBox textBox;
        private Button okButton;
        private Button cancelButton;
        private Label promptLabel;

        public string InputText
        {
            get { return textBox.Text; }
        }

        public InputDialog(string title, string prompt)
        {
            this.Text = title;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 350;
            this.Height = 150;

            promptLabel = new Label();
            promptLabel.Text = prompt;
            promptLabel.Left = 10;
            promptLabel.Top = 10;
            promptLabel.Width = 330;
            this.Controls.Add(promptLabel);

            textBox = new TextBox();
            textBox.Left = 10;
            textBox.Top = 40;
            textBox.Width = 330;
            this.Controls.Add(textBox);

            okButton = new Button();
            okButton.Text = "OK";
            okButton.Left = 180;
            okButton.Top = 80;
            okButton.Width = 75;
            okButton.DialogResult = DialogResult.OK;
            this.Controls.Add(okButton);

            cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Left = 265;
            cancelButton.Top = 80;
            cancelButton.Width = 75;
            cancelButton.DialogResult = DialogResult.Cancel;
            this.Controls.Add(cancelButton);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }
    }
}
