using System;
// ...existing code...

// Ensure there is no duplicate declaration of 'chkRetainChatHistory' in this file
// If necessary, remove or rename any conflicting declaration

// ...existing code...
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        public ToolConfigForm()
        {
            InitializeComponent();

            LoadToolConfig();
        }

        private void LoadToolConfig()
        {
            _toolConfig = ToolConfig.LoadConfig(toolFileName);

            chkCMDPlugin.Checked = _toolConfig.EnableCMDPlugin;
            chkPowerShellPlugin.Checked = _toolConfig.EnablePowerShellPlugin;
            chkScreenCapturePlugin.Checked = _toolConfig.EnableScreenCapturePlugin;
            chkKeyboardPlugin.Checked = _toolConfig.EnableKeyboardPlugin;
            chkMousePlugin.Checked = _toolConfig.EnableMousePlugin;

            numTemperature.Value = (decimal)_toolConfig.Temperature;
            chkAutoInvoke.Checked = _toolConfig.AutoInvokeKernelFunctions;
            chkRetainChatHistory.Checked = _toolConfig.RetainChatHistory;
            txtSystemPrompt.Text = _toolConfig.SystemPrompt;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Save tool config
            _toolConfig.EnableCMDPlugin = chkCMDPlugin.Checked;
            _toolConfig.EnablePowerShellPlugin = chkPowerShellPlugin.Checked;
            _toolConfig.EnableScreenCapturePlugin = chkScreenCapturePlugin.Checked;
            _toolConfig.EnableKeyboardPlugin = chkKeyboardPlugin.Checked;
            _toolConfig.EnableMousePlugin = chkMousePlugin.Checked;

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
