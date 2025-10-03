using System;
using System.Drawing;
using System.Windows.Forms;
using FlowVision.lib.Classes;

namespace FlowVision
{
    public partial class LMStudioConfigForm : Form
    {
        private LMStudioConfig config;
        
        private CheckBox chkEnabled;
        private TextBox txtEndpoint;
        private TextBox txtModelName;
        private TextBox txtApiKey;
        private NumericUpDown numTemperature;
        private NumericUpDown numMaxTokens;
        private NumericUpDown numTimeout;
        private Button btnSave;
        private Button btnCancel;
        private Button btnTestConnection;
        private Label lblStatus;

        public LMStudioConfigForm()
        {
            InitializeComponent();
            config = LMStudioConfig.LoadConfig();
            LoadConfigToUI();
        }

        private void InitializeComponent()
        {
            this.Text = "LM Studio Configuration";
            this.Size = new Size(600, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            int y = 20;
            int labelWidth = 150;
            int controlWidth = 380;
            int leftMargin = 20;
            int controlX = leftMargin + labelWidth + 10;

            // Title
            var titleLabel = new Label
            {
                Text = "LM Studio Local AI Configuration",
                Font = new Font(this.Font.FontFamily, 12, FontStyle.Bold),
                Location = new Point(leftMargin, y),
                AutoSize = true
            };
            this.Controls.Add(titleLabel);
            y += 40;

            // Info label
            var infoLabel = new Label
            {
                Text = "Configure LM Studio to use local AI models instead of Azure OpenAI.\n" +
                       "Make sure LM Studio is running with a model loaded and the server started.",
                Location = new Point(leftMargin, y),
                Size = new Size(540, 40),
                ForeColor = Color.DarkGray
            };
            this.Controls.Add(infoLabel);
            y += 50;

            // Enabled checkbox
            chkEnabled = new CheckBox
            {
                Text = "Enable LM Studio (Use local AI instead of Azure)",
                Location = new Point(leftMargin, y),
                Width = 400,
                Checked = config.Enabled
            };
            this.Controls.Add(chkEnabled);
            y += 35;

            // Endpoint
            AddLabel("Endpoint URL:", leftMargin, y);
            txtEndpoint = new TextBox
            {
                Location = new Point(controlX, y),
                Width = controlWidth,
                Text = config.EndpointURL
            };
            this.Controls.Add(txtEndpoint);
            y += 30;

            // Model Name
            AddLabel("Model Name:", leftMargin, y);
            txtModelName = new TextBox
            {
                Location = new Point(controlX, y),
                Width = controlWidth,
                Text = config.ModelName
            };
            this.Controls.Add(txtModelName);
            y += 30;

            // API Key (placeholder)
            AddLabel("API Key (optional):", leftMargin, y);
            txtApiKey = new TextBox
            {
                Location = new Point(controlX, y),
                Width = controlWidth,
                Text = config.APIKey
            };
            this.Controls.Add(txtApiKey);
            y += 30;

            // Temperature
            AddLabel("Temperature:", leftMargin, y);
            numTemperature = new NumericUpDown
            {
                Location = new Point(controlX, y),
                Width = 100,
                Minimum = 0,
                Maximum = 2,
                DecimalPlaces = 2,
                Increment = 0.1M,
                Value = (decimal)config.Temperature
            };
            this.Controls.Add(numTemperature);
            y += 30;

            // Max Tokens
            AddLabel("Max Tokens:", leftMargin, y);
            numMaxTokens = new NumericUpDown
            {
                Location = new Point(controlX, y),
                Width = 100,
                Minimum = 128,
                Maximum = 32768,
                Increment = 128,
                Value = config.MaxTokens
            };
            this.Controls.Add(numMaxTokens);
            y += 30;

            // Timeout
            AddLabel("Timeout (seconds):", leftMargin, y);
            numTimeout = new NumericUpDown
            {
                Location = new Point(controlX, y),
                Width = 100,
                Minimum = 30,
                Maximum = 600,
                Increment = 30,
                Value = config.TimeoutSeconds
            };
            this.Controls.Add(numTimeout);
            y += 40;

            // Status label
            lblStatus = new Label
            {
                Location = new Point(leftMargin, y),
                Size = new Size(540, 20),
                ForeColor = Color.Blue
            };
            this.Controls.Add(lblStatus);
            y += 30;

            // Buttons
            btnTestConnection = new Button
            {
                Text = "Test Connection",
                Location = new Point(leftMargin, y),
                Width = 120,
                Height = 30
            };
            btnTestConnection.Click += BtnTestConnection_Click;
            this.Controls.Add(btnTestConnection);

            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(controlX + controlWidth - 160, y),
                Width = 75,
                Height = 30
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(controlX + controlWidth - 75, y),
                Width = 75,
                Height = 30,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void AddLabel(string text, int x, int y)
        {
            var label = new Label
            {
                Text = text,
                Location = new Point(x, y + 3),
                Width = 150
            };
            this.Controls.Add(label);
        }

        private void LoadConfigToUI()
        {
            chkEnabled.Checked = config.Enabled;
            txtEndpoint.Text = config.EndpointURL;
            txtModelName.Text = config.ModelName;
            txtApiKey.Text = config.APIKey;
            numTemperature.Value = (decimal)config.Temperature;
            numMaxTokens.Value = config.MaxTokens;
            numTimeout.Value = config.TimeoutSeconds;
        }

        private void SaveUIToConfig()
        {
            config.Enabled = chkEnabled.Checked;
            config.EndpointURL = txtEndpoint.Text;
            config.ModelName = txtModelName.Text;
            config.APIKey = txtApiKey.Text;
            config.Temperature = (double)numTemperature.Value;
            config.MaxTokens = (int)numMaxTokens.Value;
            config.TimeoutSeconds = (int)numTimeout.Value;
        }

        private async void BtnTestConnection_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Testing connection...";
            lblStatus.ForeColor = Color.Blue;
            btnTestConnection.Enabled = false;

            try
            {
                // Save current UI values temporarily
                SaveUIToConfig();

                // Try to create a client and make a simple request
                var client = new OpenAI.OpenAIClient(new System.ClientModel.ApiKeyCredential(config.APIKey), new OpenAI.OpenAIClientOptions
                {
                    Endpoint = new Uri(config.EndpointURL)
                });

                var chatClient = client.GetChatClient(config.ModelName);
                var ichatClient = (Microsoft.Extensions.AI.IChatClient)chatClient;
                
                // Simple test message  
                var messages = new System.Collections.Generic.List<Microsoft.Extensions.AI.ChatMessage>
                {
                    new Microsoft.Extensions.AI.ChatMessage(
                        Microsoft.Extensions.AI.ChatRole.User, 
                        "Say 'hello' in one word")
                };

                var response = await ichatClient.GetResponseAsync(messages);

                lblStatus.Text = "✓ Connection successful! LM Studio is responding.";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"✗ Connection failed: {ex.Message}";
                lblStatus.ForeColor = Color.Red;

                if (ex.Message.Contains("Connection refused") || ex.Message.Contains("No connection"))
                {
                    MessageBox.Show(
                        "Cannot connect to LM Studio. Please make sure:\n\n" +
                        "1. LM Studio is running\n" +
                        "2. A model is loaded\n" +
                        "3. The server is started (click 'Start Server' in LM Studio)\n" +
                        $"4. The endpoint is correct: {config.EndpointURL}",
                        "Connection Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            finally
            {
                btnTestConnection.Enabled = true;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveUIToConfig();
            config.SaveConfig();
            
            MessageBox.Show(
                "LM Studio configuration saved successfully!\n\n" +
                (config.Enabled 
                    ? "Local AI is now ENABLED. The application will use LM Studio for AI requests." 
                    : "Local AI is DISABLED. The application will use Azure OpenAI."),
                "Configuration Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
