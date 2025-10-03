using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Classes;

namespace FlowVision
{
    /// <summary>
    /// Unified AI Provider Configuration Form
    /// Allows switching between Azure OpenAI, LM Studio, GitHub Models, etc.
    /// </summary>
    public partial class AIProviderConfigForm : Form
    {
        private string currentModel;
        private ComboBox providerComboBox;
        private Panel configPanel;
        
        // Azure OpenAI controls
        private Panel azurePanel;
        private TextBox azureDeploymentTextBox;
        private TextBox azureEndpointTextBox;
        private TextBox azureApiKeyTextBox;
        
        // LM Studio controls
        private Panel lmStudioPanel;
        private TextBox lmStudioEndpointTextBox;
        private TextBox lmStudioModelTextBox;
        private NumericUpDown lmStudioTemperatureUpDown;
        private NumericUpDown lmStudioMaxTokensUpDown;
        
        // Common controls
        private Button saveButton;
        private Button testButton;
        private Label statusLabel;
        private CheckBox enableProviderCheckBox;

        public AIProviderConfigForm(string model)
        {
            currentModel = model;
            InitializeComponent();
            LoadConfiguration();
        }

        private void InitializeComponent()
        {
            this.Text = $"AI Provider Configuration - {currentModel}";
            this.Size = new Size(650, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            int y = 20;
            int leftMargin = 20;
            int labelWidth = 150;
            int controlWidth = 450;

            // Title
            var titleLabel = new Label
            {
                Text = "Choose Your AI Provider",
                Font = new Font(this.Font.FontFamily, 14, FontStyle.Bold),
                Location = new Point(leftMargin, y),
                AutoSize = true
            };
            this.Controls.Add(titleLabel);
            y += 40;

            // Info
            var infoLabel = new Label
            {
                Text = "Select which AI service to use for this agent. You can switch anytime!",
                Location = new Point(leftMargin, y),
                Size = new Size(580, 30),
                ForeColor = Color.Gray
            };
            this.Controls.Add(infoLabel);
            y += 40;

            // Provider selector
            var providerLabel = new Label
            {
                Text = "AI Provider:",
                Location = new Point(leftMargin, y + 3),
                Width = labelWidth
            };
            this.Controls.Add(providerLabel);

            providerComboBox = new ComboBox
            {
                Location = new Point(leftMargin + labelWidth + 10, y),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            providerComboBox.Items.AddRange(new object[] {
                "Azure OpenAI (Cloud)",
                "LM Studio (Local)",
                "GitHub Models (Free Tier)"
            });
            providerComboBox.SelectedIndexChanged += ProviderComboBox_SelectedIndexChanged;
            this.Controls.Add(providerComboBox);
            y += 40;

            // Enable checkbox
            enableProviderCheckBox = new CheckBox
            {
                Text = "Enable this provider (if unchecked, will use default Azure OpenAI)",
                Location = new Point(leftMargin, y),
                Width = 500,
                Checked = true
            };
            this.Controls.Add(enableProviderCheckBox);
            y += 35;

            // Separator
            var separator = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(leftMargin, y),
                Size = new Size(580, 2)
            };
            this.Controls.Add(separator);
            y += 15;

            // Config panel (will hold provider-specific controls)
            configPanel = new Panel
            {
                Location = new Point(leftMargin, y),
                Size = new Size(580, 300),
                BorderStyle = BorderStyle.None
            };
            this.Controls.Add(configPanel);
            y += 310;

            // Status label
            statusLabel = new Label
            {
                Location = new Point(leftMargin, y),
                Size = new Size(580, 25),
                ForeColor = Color.Blue,
                Text = ""
            };
            this.Controls.Add(statusLabel);
            y += 30;

            // Buttons
            testButton = new Button
            {
                Text = "Test Connection",
                Location = new Point(leftMargin, y),
                Width = 130,
                Height = 32
            };
            testButton.Click += TestButton_Click;
            this.Controls.Add(testButton);

            saveButton = new Button
            {
                Text = "Save Configuration",
                Location = new Point(leftMargin + 450, y),
                Width = 150,
                Height = 32
            };
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            var cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(leftMargin + 310, y),
                Width = 130,
                Height = 32,
                DialogResult = DialogResult.Cancel
            };
            cancelButton.Click += (s, e) => this.Close();
            this.Controls.Add(cancelButton);

            this.AcceptButton = saveButton;
            this.CancelButton = cancelButton;

            // Create provider-specific panels
            CreateAzurePanel();
            CreateLMStudioPanel();
        }

        private void CreateAzurePanel()
        {
            azurePanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(580, 300),
                Visible = false
            };

            int y = 0;
            int labelWidth = 150;
            int controlWidth = 400;

            // Deployment Name
            var deployLabel = new Label { Text = "Deployment Name:", Location = new Point(0, y + 3), Width = labelWidth };
            azurePanel.Controls.Add(deployLabel);
            azureDeploymentTextBox = new TextBox { Location = new Point(labelWidth + 10, y), Width = controlWidth };
            azurePanel.Controls.Add(azureDeploymentTextBox);
            y += 35;

            // Endpoint URL
            var endpointLabel = new Label { Text = "Endpoint URL:", Location = new Point(0, y + 3), Width = labelWidth };
            azurePanel.Controls.Add(endpointLabel);
            azureEndpointTextBox = new TextBox { Location = new Point(labelWidth + 10, y), Width = controlWidth };
            azurePanel.Controls.Add(azureEndpointTextBox);
            y += 35;

            // API Key
            var apiKeyLabel = new Label { Text = "API Key:", Location = new Point(0, y + 3), Width = labelWidth };
            azurePanel.Controls.Add(apiKeyLabel);
            azureApiKeyTextBox = new TextBox 
            { 
                Location = new Point(labelWidth + 10, y), 
                Width = controlWidth,
                UseSystemPasswordChar = true
            };
            azurePanel.Controls.Add(azureApiKeyTextBox);
            y += 35;

            // Help text
            var helpLabel = new Label
            {
                Text = "Get your Azure OpenAI credentials from:\nhttps://portal.azure.com → Azure OpenAI → Keys and Endpoint",
                Location = new Point(0, y),
                Size = new Size(550, 40),
                ForeColor = Color.Gray
            };
            azurePanel.Controls.Add(helpLabel);

            configPanel.Controls.Add(azurePanel);
        }

        private void CreateLMStudioPanel()
        {
            lmStudioPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(580, 300),
                Visible = false
            };

            int y = 0;
            int labelWidth = 150;
            int controlWidth = 400;

            // Endpoint URL
            var endpointLabel = new Label { Text = "Server Endpoint:", Location = new Point(0, y + 3), Width = labelWidth };
            lmStudioPanel.Controls.Add(endpointLabel);
            lmStudioEndpointTextBox = new TextBox 
            { 
                Location = new Point(labelWidth + 10, y), 
                Width = controlWidth,
                Text = "http://localhost:1234/v1"
            };
            lmStudioPanel.Controls.Add(lmStudioEndpointTextBox);
            y += 35;

            // Model Name
            var modelLabel = new Label { Text = "Model Name:", Location = new Point(0, y + 3), Width = labelWidth };
            lmStudioPanel.Controls.Add(modelLabel);
            lmStudioModelTextBox = new TextBox 
            { 
                Location = new Point(labelWidth + 10, y), 
                Width = controlWidth,
                Text = "local-model"
            };
            lmStudioPanel.Controls.Add(lmStudioModelTextBox);
            y += 35;

            // Temperature
            var tempLabel = new Label { Text = "Temperature:", Location = new Point(0, y + 3), Width = labelWidth };
            lmStudioPanel.Controls.Add(tempLabel);
            lmStudioTemperatureUpDown = new NumericUpDown
            {
                Location = new Point(labelWidth + 10, y),
                Width = 100,
                Minimum = 0,
                Maximum = 2,
                DecimalPlaces = 2,
                Increment = 0.1M,
                Value = 0.7M
            };
            lmStudioPanel.Controls.Add(lmStudioTemperatureUpDown);
            y += 35;

            // Max Tokens
            var tokensLabel = new Label { Text = "Max Tokens:", Location = new Point(0, y + 3), Width = labelWidth };
            lmStudioPanel.Controls.Add(tokensLabel);
            lmStudioMaxTokensUpDown = new NumericUpDown
            {
                Location = new Point(labelWidth + 10, y),
                Width = 100,
                Minimum = 128,
                Maximum = 32768,
                Increment = 128,
                Value = 2048
            };
            lmStudioPanel.Controls.Add(lmStudioMaxTokensUpDown);
            y += 40;

            // Help text
            var helpLabel = new Label
            {
                Text = "1. Download LM Studio from https://lmstudio.ai/\n" +
                       "2. Load a model (recommended: Hermes-2-Pro-Mistral-7B)\n" +
                       "3. Click 'Start Server' in LM Studio\n" +
                       "4. Make sure the endpoint matches (usually http://localhost:1234/v1)",
                Location = new Point(0, y),
                Size = new Size(550, 80),
                ForeColor = Color.DarkGreen
            };
            lmStudioPanel.Controls.Add(helpLabel);

            configPanel.Controls.Add(lmStudioPanel);
        }

        private void LoadConfiguration()
        {
            // Try to load existing configuration to determine current provider
            var lmConfig = LMStudioConfig.LoadConfig();
            var azureConfig = APIConfig.LoadConfig(currentModel);

            if (lmConfig.Enabled)
            {
                // LM Studio is enabled
                providerComboBox.SelectedIndex = 1; // LM Studio
                enableProviderCheckBox.Checked = true;
                
                lmStudioEndpointTextBox.Text = lmConfig.EndpointURL;
                lmStudioModelTextBox.Text = lmConfig.ModelName;
                lmStudioTemperatureUpDown.Value = (decimal)lmConfig.Temperature;
                lmStudioMaxTokensUpDown.Value = lmConfig.MaxTokens;
            }
            else
            {
                // Azure OpenAI (default)
                providerComboBox.SelectedIndex = 0; // Azure OpenAI
                enableProviderCheckBox.Checked = true;
                
                azureDeploymentTextBox.Text = azureConfig.DeploymentName;
                azureEndpointTextBox.Text = azureConfig.EndpointURL;
                azureApiKeyTextBox.Text = azureConfig.APIKey;
            }
        }

        private void ProviderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Hide all panels
            azurePanel.Visible = false;
            lmStudioPanel.Visible = false;

            // Show selected panel
            switch (providerComboBox.SelectedIndex)
            {
                case 0: // Azure OpenAI
                    azurePanel.Visible = true;
                    break;
                case 1: // LM Studio
                    lmStudioPanel.Visible = true;
                    break;
                case 2: // GitHub Models
                    MessageBox.Show("GitHub Models support coming soon!\nFor now, configure as Azure OpenAI with GitHub endpoint.",
                        "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    providerComboBox.SelectedIndex = 0;
                    break;
            }
        }

        private async void TestButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Testing connection...";
            statusLabel.ForeColor = Color.Blue;
            testButton.Enabled = false;

            try
            {
                if (providerComboBox.SelectedIndex == 1) // LM Studio
                {
                    await TestLMStudioConnection();
                }
                else // Azure OpenAI
                {
                    await TestAzureConnection();
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Test failed: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
            }
            finally
            {
                testButton.Enabled = true;
            }
        }

        private async Task TestAzureConnection()
        {
            try
            {
                var client = new Azure.AI.OpenAI.AzureOpenAIClient(
                    new Uri(azureEndpointTextBox.Text),
                    new Azure.AzureKeyCredential(azureApiKeyTextBox.Text));
                
                var chatClient = client.GetChatClient(azureDeploymentTextBox.Text);
                // Use cast for .NET 4.8 compatibility
                var ichatClient = (Microsoft.Extensions.AI.IChatClient)(object)chatClient;
                
                var messages = new System.Collections.Generic.List<Microsoft.Extensions.AI.ChatMessage>
                {
                    new Microsoft.Extensions.AI.ChatMessage(
                        Microsoft.Extensions.AI.ChatRole.User,
                        "Say 'test' in one word")
                };
                
                var response = await ichatClient.GetResponseAsync(messages);
                
                statusLabel.Text = "✓ Azure OpenAI connection successful!";
                statusLabel.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"✗ Connection failed: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private async Task TestLMStudioConnection()
        {
            try
            {
                var client = new OpenAI.OpenAIClient(
                    new System.ClientModel.ApiKeyCredential("lm-studio"),
                    new OpenAI.OpenAIClientOptions { Endpoint = new Uri(lmStudioEndpointTextBox.Text) });
                
                var chatClient = client.GetChatClient(lmStudioModelTextBox.Text);
                
                var messages = new System.Collections.Generic.List<Microsoft.Extensions.AI.ChatMessage>
                {
                    new Microsoft.Extensions.AI.ChatMessage(
                        Microsoft.Extensions.AI.ChatRole.User,
                        "Say 'test' in one word")
                };
                
                // Cast to IChatClient - can't use AsIChatClient on OpenAI.ChatClient directly in .NET 4.8
                var ichatClient = (Microsoft.Extensions.AI.IChatClient)(object)chatClient;
                var response = await ichatClient.GetResponseAsync(messages);
                
                statusLabel.Text = "✓ LM Studio connection successful!";
                statusLabel.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Connection refused") || ex.Message.Contains("No connection"))
                {
                    statusLabel.Text = "✗ Cannot connect. Is LM Studio running with server started?";
                }
                else
                {
                    statusLabel.Text = $"✗ Connection failed: {ex.Message}";
                }
                statusLabel.ForeColor = Color.Red;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (providerComboBox.SelectedIndex == 1) // LM Studio
                {
                    SaveLMStudioConfig();
                }
                else // Azure OpenAI
                {
                    SaveAzureConfig();
                }

                MessageBox.Show(
                    $"Configuration saved successfully!\n\n" +
                    $"Provider: {providerComboBox.SelectedItem}\n" +
                    $"Status: {(enableProviderCheckBox.Checked ? "Enabled" : "Disabled")}\n\n" +
                    $"The new provider will be used immediately.",
                    "Configuration Saved",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveAzureConfig()
        {
            var config = new APIConfig
            {
                DeploymentName = azureDeploymentTextBox.Text,
                EndpointURL = azureEndpointTextBox.Text,
                APIKey = azureApiKeyTextBox.Text,
                ProviderType = "AzureOpenAI"
            };
            config.SaveConfig(currentModel);

            // Disable LM Studio if Azure is being configured
            var lmConfig = LMStudioConfig.LoadConfig();
            lmConfig.Enabled = false;
            lmConfig.SaveConfig();
        }

        private void SaveLMStudioConfig()
        {
            var config = new LMStudioConfig
            {
                EndpointURL = lmStudioEndpointTextBox.Text,
                ModelName = lmStudioModelTextBox.Text,
                Temperature = (double)lmStudioTemperatureUpDown.Value,
                MaxTokens = (int)lmStudioMaxTokensUpDown.Value,
                Enabled = enableProviderCheckBox.Checked
            };
            config.SaveConfig();

            // Also save to Azure config for compatibility
            var azureConfig = new APIConfig
            {
                DeploymentName = lmStudioModelTextBox.Text,
                EndpointURL = lmStudioEndpointTextBox.Text,
                APIKey = "lm-studio",
                ProviderType = "LMStudio"
            };
            azureConfig.SaveConfig(currentModel);
        }
    }
}
