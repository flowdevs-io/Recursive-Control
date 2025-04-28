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

namespace FlowVision
{
    public partial class ToolConfigForm : Form
    {
        private string toolFileName = "toolsconfig";
        private ToolConfig _toolConfig;
        private bool _isNewConfiguration = false;

        public ToolConfigForm(bool openAsNew = false)
        {
            InitializeComponent();
            _isNewConfiguration = openAsNew;

            // Populate speech recognition language options
            PopulateSpeechLanguages();

            LoadToolConfig();

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

        private void PopulateSpeechLanguages()
        {
            // Try to get installed recognizers if possible
            try
            {
                comboSpeechLanguage.Items.Clear();

                foreach (var recognizerInfo in SpeechRecognitionEngine.InstalledRecognizers())
                {
                    comboSpeechLanguage.Items.Add(recognizerInfo.Culture.Name);
                }

                // If no recognizers found, use the default items
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
            }
            catch (Exception ex)
            {
                // Fall back to default items if there's an error
                Console.WriteLine($"Error populating speech recognizers: {ex.Message}");
            }
        }

        private void LoadToolConfig()
        {
            // Check if config file exists
            string configPath = ToolConfig.ConfigFilePath(toolFileName);
            bool configExists = File.Exists(configPath);

            _toolConfig = ToolConfig.LoadConfig(toolFileName);

            // Existing plugins configuration
            chkCMDPlugin.Checked = _toolConfig.EnableCMDPlugin;
            chkPowerShellPlugin.Checked = _toolConfig.EnablePowerShellPlugin;
            chkScreenCapturePlugin.Checked = _toolConfig.EnableScreenCapturePlugin;
            chkKeyboardPlugin.Checked = _toolConfig.EnableKeyboardPlugin;
            chkMousePlugin.Checked = _toolConfig.EnableMousePlugin;
            chkWindowSelectionPlugin.Checked = _toolConfig.EnableWindowSelectionPlugin;
            enablePluginLoggingCheckBox.Checked = _toolConfig.EnablePluginLogging;

            // AI settings
            numTemperature.Value = (decimal)_toolConfig.Temperature;
            chkAutoInvoke.Checked = _toolConfig.AutoInvokeKernelFunctions;
            chkRetainChatHistory.Checked = _toolConfig.RetainChatHistory;
            txtSystemPrompt.Text = _toolConfig.SystemPrompt;

            // Speech recognition settings
            chkEnableSpeechRecognition.Checked = _toolConfig.EnableSpeechRecognition;

            // Voice command settings
            chkEnableVoiceCommands.Checked = _toolConfig.EnableVoiceCommands;
            txtVoiceCommandPhrase.Text = _toolConfig.VoiceCommandPhrase;

            // Set speech language if it exists in items
            if (comboSpeechLanguage.Items.Contains(_toolConfig.SpeechRecognitionLanguage))
            {
                comboSpeechLanguage.SelectedItem = _toolConfig.SpeechRecognitionLanguage;
            }
            else if (comboSpeechLanguage.Items.Count > 0)
            {
                // Default to first item if language not in list
                comboSpeechLanguage.SelectedIndex = 0;
            }

            // If this is a new configuration being created, save the default values
            if (_isNewConfiguration && !configExists)
            {
                _toolConfig.SaveConfig(toolFileName);
            }
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
            _toolConfig.SystemPrompt = txtSystemPrompt.Text;

            // Save speech recognition settings
            _toolConfig.EnableSpeechRecognition = chkEnableSpeechRecognition.Checked;
            if (comboSpeechLanguage.SelectedItem != null)
            {
                _toolConfig.SpeechRecognitionLanguage = comboSpeechLanguage.SelectedItem.ToString();
            }

            // Save voice command settings
            _toolConfig.EnableVoiceCommands = chkEnableVoiceCommands.Checked;
            _toolConfig.VoiceCommandPhrase = txtVoiceCommandPhrase.Text;

            _toolConfig.SaveConfig(toolFileName);

            // Update any active speech recognition services with the new command phrase
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

            MessageBox.Show("Configuration saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void cancelButton_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
