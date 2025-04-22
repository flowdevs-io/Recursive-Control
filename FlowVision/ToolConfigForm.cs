using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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

        private void LoadToolConfig()
        {
            // Check if config file exists
            string configPath = ToolConfig.ConfigFilePath(toolFileName);
            bool configExists = File.Exists(configPath);
            
            _toolConfig = ToolConfig.LoadConfig(toolFileName);

            chkCMDPlugin.Checked = _toolConfig.EnableCMDPlugin;
            chkPowerShellPlugin.Checked = _toolConfig.EnablePowerShellPlugin;
            chkScreenCapturePlugin.Checked = _toolConfig.EnableScreenCapturePlugin;
            chkKeyboardPlugin.Checked = _toolConfig.EnableKeyboardPlugin;
            chkMousePlugin.Checked = _toolConfig.EnableMousePlugin;
            chkWindowSelectionPlugin.Checked = _toolConfig.EnableWindowSelectionPlugin; // Add this line

            numTemperature.Value = (decimal)_toolConfig.Temperature;
            chkAutoInvoke.Checked = _toolConfig.AutoInvokeKernelFunctions;
            chkRetainChatHistory.Checked = _toolConfig.RetainChatHistory;
            txtSystemPrompt.Text = _toolConfig.SystemPrompt;
            
            // If this is a new configuration being created, save the default values
            if (_isNewConfiguration && !configExists)
            {
                _toolConfig.SaveConfig(toolFileName);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Save tool config
            _toolConfig.EnableCMDPlugin = chkCMDPlugin.Checked;
            _toolConfig.EnablePowerShellPlugin = chkPowerShellPlugin.Checked;
            _toolConfig.EnableScreenCapturePlugin = chkScreenCapturePlugin.Checked;
            _toolConfig.EnableKeyboardPlugin = chkKeyboardPlugin.Checked;
            _toolConfig.EnableMousePlugin = chkMousePlugin.Checked;
            _toolConfig.EnableWindowSelectionPlugin = chkWindowSelectionPlugin.Checked; // Add this line

            _toolConfig.Temperature = (double)numTemperature.Value;
            _toolConfig.AutoInvokeKernelFunctions = chkAutoInvoke.Checked;
            _toolConfig.RetainChatHistory = chkRetainChatHistory.Checked;
            _toolConfig.SystemPrompt = txtSystemPrompt.Text;

            _toolConfig.SaveConfig(toolFileName);

            MessageBox.Show("Configuration saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void cancelButton_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
