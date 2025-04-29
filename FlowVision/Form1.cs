using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Classes;
using Microsoft.SemanticKernel.ChatCompletion;
// Add System.Speech namespace
using System.Speech.Recognition;

namespace FlowVision
{
    public partial class Form1 : Form
    {
        private FlowLayoutPanel messagesPanel;
        private RichTextBox userInputTextBox;
        private Button sendButton;
        private List<LocalChatMessage> chatHistory = new List<LocalChatMessage>();
        private Button microphoneButton; // New microphone button
        // Speech recognition components
        private SpeechRecognitionService speechRecognition;
        private bool isListening = false;

        // Add a delegate for handling plugin output messages
        public delegate void PluginOutputHandler(string message);

        public Form1()
        {
            InitializeComponent();
        }

        private void azureOpenAIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if the config form is already open
            if (Application.OpenForms.OfType<ConfigForm>().Count() == 1)
            {
                // If it is, bring it to the front
                Application.OpenForms.OfType<ConfigForm>().First().BringToFront();
            }
            else
            {
                // If it isn't, create a new instance of the form
                ConfigForm configForm = new ConfigForm("actioner");
                configForm.Show();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Check if tools are configured, if not, create default configuration
            string toolConfigName = "toolsconfig";
            if (!ToolConfig.IsConfigured(toolConfigName))
            {
                // Create and save default configuration with mouse and screen capture disabled
                var defaultConfig = new ToolConfig();
                defaultConfig.SaveConfig(toolConfigName);
            }
            
            // Create messages panel as a FlowLayoutPanel with auto-scroll
            messagesPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            // This is critical - it makes the controls in the panel fill the width
            messagesPanel.SetFlowBreak(messagesPanel, true);

            mainPanel.Controls.Add(messagesPanel);

            // Create input panel at the bottom
            Panel inputPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40
            };
            mainPanel.Controls.Add(inputPanel);

            // Create text input
            userInputTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F)
            };
            inputPanel.Controls.Add(userInputTextBox);

            // Create send button
            sendButton = new Button
            {
                Dock = DockStyle.Right,
                Text = "Send",
                Width = 80
            };
            sendButton.Click += SendButton_Click;
            inputPanel.Controls.Add(sendButton);

            // Create microphone button
            microphoneButton = new Button
            {
                Dock = DockStyle.Right,
                Text = "🎤",
                Width = 40,
                Font = new Font("Segoe UI", 12F)
            };
            microphoneButton.Click += MicrophoneButton_Click;
            inputPanel.Controls.Add(microphoneButton);

            // Initialize speech recognition service
            InitializeSpeechRecognition();
            
            // Handle window resize to adjust message widths
            this.Resize += (s, args) =>
            {
                foreach (Control control in messagesPanel.Controls)
                {
                    if (control is Panel panel)
                    {
                        int newWidth = messagesPanel.ClientSize.Width - 60;
                        panel.MaximumSize = new Size(newWidth, 0);
                        panel.Width = newWidth;

                        // Adjust message TextBox width
                        foreach (Control innerControl in panel.Controls)
                        {
                            if (innerControl is TextBox textBox && textBox.Font.Size == 10F)
                            {
                                textBox.Width = newWidth - 20;
                            }
                        }
                    }
                }
            };

            // Add welcome message
            AddMessage("AI Assistant", "Welcome! How can I help you today?", true);
        }

        private void InitializeSpeechRecognition()
        {
            try
            {
                var toolConfig = ToolConfig.LoadConfig("toolsconfig");
                
                // Only initialize if speech recognition is enabled in config
                if (toolConfig.EnableSpeechRecognition)
                {
                    speechRecognition = new SpeechRecognitionService();
                    speechRecognition.SpeechRecognized += (sender, result) => 
                    {
                        // Use Invoke to update UI from a different thread
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action<string>((text) => {
                                userInputTextBox.Text = text;
                            }), result);
                        }
                        else
                        {
                            userInputTextBox.Text = result;
                        }
                    };
                    
                    // Add handler for voice commands
                    if (toolConfig.EnableVoiceCommands)
                    {
                        speechRecognition.CommandRecognized += (sender, command) =>
                        {
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(() => {
                                    if (!string.IsNullOrWhiteSpace(userInputTextBox.Text))
                                    {
                                        AddMessage("System", $"Voice command recognized: \"{command}\"", true);
                                        SendButton_Click(this, EventArgs.Empty);
                                    }
                                }));
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(userInputTextBox.Text))
                                {
                                    AddMessage("System", $"Voice command recognized: \"{command}\"", true);
                                    SendButton_Click(this, EventArgs.Empty);
                                }
                            }
                        };
                        
                        // Start continuous listening if voice commands are enabled
                        if (toolConfig.EnableVoiceCommands)
                        {
                            speechRecognition.StartListening();
                        }
                    }
                }
                else
                {
                    microphoneButton.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing speech recognition: {ex.Message}", "Speech Recognition Error");
                microphoneButton.Enabled = false;
            }
        }

        private void MicrophoneButton_Click(object sender, EventArgs e)
        {
            if (speechRecognition == null)
            {
                MessageBox.Show("Speech recognition is not available.", "Feature Not Available");
                return;
            }

            if (isListening)
            {
                StopListening();
            }
            else
            {
                StartListening();
            }
        }

        private void StartListening()
        {
            try
            {
                var toolConfig = ToolConfig.LoadConfig("toolsconfig");
                
                // Clear existing text before starting to listen
                userInputTextBox.Text = "";
                
                // Change button appearance to indicate recording
                microphoneButton.Text = "⏹️";
                microphoneButton.BackColor = Color.Red;
                isListening = true;
                
                // Start listening
                speechRecognition.StartListening();
                
                // Add temporary message to indicate we're listening
                AddMessage("System", "Listening... Speak now." + 
                    (toolConfig.EnableVoiceCommands ? 
                        $" Say \"{toolConfig.VoiceCommandPhrase}\" to send your message." : ""), true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting voice recognition: {ex.Message}", "Voice Recognition Error");
                StopListening();
            }
        }

        private void StopListening()
        {
            try
            {
                // Change button appearance back to normal
                microphoneButton.Text = "🎤";
                microphoneButton.BackColor = SystemColors.Control;
                isListening = false;
                
                // Stop listening
                speechRecognition.StopListening();
                
                // Remove the listening message
                RemoveMessagesByAuthor("System");
                
                // If we have recognized text, send it automatically
                if (!string.IsNullOrWhiteSpace(userInputTextBox.Text))
                {
                    SendButton_Click(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping voice recognition: {ex.Message}", "Voice Recognition Error");
            }
        }

        private void allowUserInput(bool enable)
        {
            userInputTextBox.Enabled = enable;
            sendButton.Enabled = enable;
            microphoneButton.Enabled = enable && speechRecognition != null;
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            allowUserInput(false);

            // Check if the user input is empty
            string userInput = userInputTextBox.Text;
            if (string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("Please enter a message before sending.", "Input Required");
                allowUserInput(true);
                return;
            }

            // Add user message to UI
            AddMessage("You", userInput, false);

            try
            {
                string aiResponse = await GetAIResponseAsync(userInput);
                AddMessage("AI", aiResponse, true);
                
                // Check if we should retain chat history
                var toolConfig = ToolConfig.LoadConfig("toolsconfig");
                if (!toolConfig.RetainChatHistory)
                {
                    // Keep only the latest exchange in chat history
                    if (chatHistory.Count > 2)
                    {
                        chatHistory.RemoveRange(0, chatHistory.Count - 2);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error communicating with AI: {ex.Message}", "Error");
                
                allowUserInput(true);
            }

            userInputTextBox.Clear();
            RemoveMessagesByAuthor("System");
            allowUserInput(true);
        }

        private async Task<string> GetAIResponseAsync(string userInput)
        {
            // Get the current config to determine which model to use
            var actionerConfig = APIConfig.LoadConfig("actioner");
            var githubConfig = APIConfig.LoadConfig("github");

            // Create a StringBuilder to collect plugin output
            StringBuilder pluginOutput = new StringBuilder();
            
            // Define a plugin output handler that adds messages to the plugin output
            PluginOutputHandler outputHandler = (message) => {
                pluginOutput.AppendLine(message);
            };

            // Show a pre-execution notification to the user in the UI
            AddMessage("System", "Your request is being processed. Please wait...", true);
            
            string aiResponse = string.Empty;
            
            // Use TaskNotifier to wrap the action execution with notifications 
            try
            {
                await TaskNotifier.RunWithNotificationAsync(
                    "Request Processing", 
                    "Processing your request and preparing tools",
                    async () =>
                    {
                        var toolConfig = ToolConfig.LoadConfig("toolsconfig");

                        // Use Actioner model with the notification infrastructure
                        Actioner actioner = new Actioner(outputHandler);
                        
                        // Set multi-agent mode based on tool configuration
                        actioner.SetMultiAgentMode(toolConfig.EnableMultiAgentMode);
                        
                        if (toolConfig.RetainChatHistory)
                        {
                            actioner.SetChatHistory(chatHistory);
                        }
                        
                        // Execute the action and get the AI response
                        aiResponse = await actioner.ExecuteAction(userInput);
                    });
                
                // If there's plugin output, prepend it to the AI response
                if (pluginOutput.Length > 0)
                {
                    aiResponse = $"{pluginOutput}\n\n{aiResponse}";
                }
                
                // Remove the "processing" message from the UI
                RemoveMessageIfSystem();
                return aiResponse;
            }
            catch (Exception ex)
            {
                // Remove the "processing" message from the UI
                RemoveMessageIfSystem();
                
                return $"Error processing request: {ex.Message}";
            }
        }

        // Helper method to remove system messages
        private void RemoveMessageIfSystem()
        {

        }

        public void AddMessage(string author, string message, bool isInbound)
        {
            // If called from a non-UI thread, use Invoke to switch to UI thread
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, string, bool>(AddMessage), new object[] { author, message, isInbound });
                return;
            }

            // Create a new chat message and add to history
            var chatMessage = new LocalChatMessage
            {
                Author = author,
                Content = message,
                IsInbound = isInbound,
                Timestamp = DateTime.Now
            };
            chatHistory.Add(chatMessage);

            // Calculate appropriate width for the bubble
            int bubbleWidth = messagesPanel.ClientSize.Width - 60;

            // Create message bubble panel
            Panel bubblePanel = new Panel
            {
                AutoSize = true,
                Width = bubbleWidth,
                MaximumSize = new Size(bubbleWidth, 0),
                MinimumSize = new Size(100, 0),
                Padding = new Padding(10),
                Margin = new Padding(10, 5, 10, 5),
                BackColor = isInbound ? Color.LightBlue : Color.LightGreen
            };

            // Layout for controls inside the bubble panel
            int currentY = 10;

            // Add author label
            Label authorLabel = new Label
            {
                Text = author,
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Location = new Point(10, currentY)
            };
            bubblePanel.Controls.Add(authorLabel);
            currentY += authorLabel.Height + 2;

            // Check if the message contains markdown
            bool hasMarkdown = MarkdownHelper.ContainsMarkdown(message);

            if (hasMarkdown)
            {
                // Use RichTextBox for markdown formatting
                RichTextBox messageRichTextBox = new RichTextBox
                {
                    ReadOnly = true,
                    BorderStyle = BorderStyle.None,
                    BackColor = bubblePanel.BackColor,
                    Location = new Point(10, currentY),
                    Width = bubbleWidth - 20,
                    ScrollBars = RichTextBoxScrollBars.None,
                    DetectUrls = true
                };

                // Apply markdown formatting
                MarkdownHelper.ApplyMarkdownFormatting(messageRichTextBox, message);
                
                // Adjust height to fit content
                messageRichTextBox.Height = messageRichTextBox.GetPositionFromCharIndex(messageRichTextBox.TextLength).Y + 20;
                
                // Add copy context menu
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                ToolStripMenuItem copyMenuItem = new ToolStripMenuItem("Copy");
                copyMenuItem.Click += (sender, args) => {
                    if (messageRichTextBox.SelectedText.Length > 0)
                        Clipboard.SetText(messageRichTextBox.SelectedText);
                    else
                        Clipboard.SetText(messageRichTextBox.Text);
                };
                contextMenu.Items.Add(copyMenuItem);
                messageRichTextBox.ContextMenuStrip = contextMenu;
                
                bubblePanel.Controls.Add(messageRichTextBox);
                currentY += messageRichTextBox.Height + 2;
            }
            else
            {
                // Regular TextBox for plain text
                TextBox messageTextBox = new TextBox
                {
                    Text = message,
                    Multiline = true,
                    ReadOnly = true,
                    BorderStyle = BorderStyle.None,
                    BackColor = bubblePanel.BackColor,
                    Location = new Point(10, currentY),
                    Width = bubbleWidth - 20,
                    Font = new Font("Segoe UI", 10F),
                    ScrollBars = ScrollBars.None,
                    WordWrap = true
                };
                
                // Auto-size the TextBox to fit content
                int textHeight = TextRenderer.MeasureText(
                    messageTextBox.Text, 
                    messageTextBox.Font, 
                    new Size(bubbleWidth - 20, int.MaxValue), 
                    TextFormatFlags.WordBreak
                ).Height;
                messageTextBox.Height = textHeight + 10;
                
                // Add copy context menu
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                ToolStripMenuItem copyMenuItem = new ToolStripMenuItem("Copy");
                copyMenuItem.Click += (sender, args) => {
                    if (messageTextBox.SelectedText.Length > 0)
                        Clipboard.SetText(messageTextBox.SelectedText);
                    else
                        Clipboard.SetText(messageTextBox.Text);
                };
                contextMenu.Items.Add(copyMenuItem);
                messageTextBox.ContextMenuStrip = contextMenu;
                
                bubblePanel.Controls.Add(messageTextBox);
                currentY += messageTextBox.Height + 2;
            }

            // Add timestamp
            Label timeLabel = new Label
            {
                Text = chatMessage.Timestamp.ToString("HH:mm"),
                AutoSize = true,
                Font = new Font("Segoe UI", 7F, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(10, currentY)
            };
            bubblePanel.Controls.Add(timeLabel);
            currentY += timeLabel.Height + 10;

            // Set final height based on content
            bubblePanel.Height = currentY;

            // Add to messages panel
            messagesPanel.Controls.Add(bubblePanel);

            // Ensure the FlowLayoutPanel's properties are correct
            messagesPanel.AutoScroll = true;
            messagesPanel.FlowDirection = FlowDirection.TopDown;
            messagesPanel.WrapContents = false;
            messagesPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            // Set the width of the bubblePanel to be appropriate
            bubblePanel.Width = bubbleWidth;

            // Make sure each bubble takes its own line
            bubblePanel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            // Scroll to bottom
            messagesPanel.ScrollControlIntoView(bubblePanel);

            // Force layout update
            messagesPanel.PerformLayout();
        }

        private void githubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if the config form is already open
            if (Application.OpenForms.OfType<ConfigForm>().Count() == 1)
            {
                // If it is, bring it to the front
                Application.OpenForms.OfType<ConfigForm>().First().BringToFront();
            }
            else
            {
                // If it isn't, create a new instance of the form
                ConfigForm configForm = new ConfigForm("github");
                configForm.Show();
            }
        }

        private void omniParserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if the config form is already open
            if (Application.OpenForms.OfType<OmniParserForm>().Count() == 1)
            {
                // If it is, bring it to the front
                Application.OpenForms.OfType<OmniParserForm>().First().BringToFront();
            }
            else
            {
                // If it isn't, create a new instance of the form
                OmniParserForm omniParserForm = new OmniParserForm();
                omniParserForm.Show();
            }
        }

        private void RemoveMessagesByAuthor(string author)
        {
            if (string.IsNullOrWhiteSpace(author)) return;

            /* 1. Strip them out of the in-memory conversation -------- */
            chatHistory.RemoveAll(m =>
                m.Author.Equals(author, StringComparison.OrdinalIgnoreCase));

            /* 2. Find every message bubble whose first child Label      *
             *    contains the requested author name, queue for removal  */
            var doomed = new List<Control>();

            foreach (Control c in messagesPanel.Controls)
            {
                if (c is Panel bubble)
                {
                    // the author label is the first control we added
                    var lbl = bubble.Controls.OfType<Label>().FirstOrDefault();
                    if (lbl != null &&
                        lbl.Text.Equals(author, StringComparison.OrdinalIgnoreCase))
                    {
                        doomed.Add(bubble);
                    }
                }
            }

            /* 3. Remove from UI and dispose to free resources --------- */
            foreach (var bubble in doomed)
            {
                messagesPanel.Controls.Remove(bubble);
                bubble.Dispose();
            }

            messagesPanel.PerformLayout();
        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if the config form is already open
            if (Application.OpenForms.OfType<ToolConfigForm>().Count() == 1)
            {
                // If it is, bring it to the front
                Application.OpenForms.OfType<ToolConfigForm>().First().BringToFront();
            }
            else
            {
                // Check if tool configuration exists
                bool isConfigured = ToolConfig.IsConfigured("toolsconfig");
                
                // If it isn't, create a new instance of the form
                // Pass true to open as new configuration if not configured
                ToolConfigForm toolConfigForm = new ToolConfigForm(!isConfigured);
                toolConfigForm.Show();
            }
        }

        private void newChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //clear all chat messages and start conversation over
            messagesPanel.Controls.Clear();
            chatHistory.Clear();
            AddMessage("AI Assistant", "Welcome! How can I help you today?", true);
            userInputTextBox.Clear();
            userInputTextBox.Enabled = true;
            sendButton.Enabled = true;
        }
    }

    public class LocalChatMessage
    {
        public string Author { get; set; }
        public string Content { get; set; }
        public bool IsInbound { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
