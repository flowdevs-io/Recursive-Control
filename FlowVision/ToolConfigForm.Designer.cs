namespace FlowVision
{
    partial class ToolConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxPlugins = new System.Windows.Forms.GroupBox();
            this.chkMousePlugin = new System.Windows.Forms.CheckBox();
            this.chkKeyboardPlugin = new System.Windows.Forms.CheckBox();
            this.chkScreenCapturePlugin = new System.Windows.Forms.CheckBox();
            this.chkPowerShellPlugin = new System.Windows.Forms.CheckBox();
            this.chkCMDPlugin = new System.Windows.Forms.CheckBox();
            this.chkWindowSelectionPlugin = new System.Windows.Forms.CheckBox();
            this.enablePluginLoggingCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.chkMultiAgentMode = new System.Windows.Forms.CheckBox();
            this.chkAutoInvoke = new System.Windows.Forms.CheckBox();
            this.chkRetainChatHistory = new System.Windows.Forms.CheckBox();
            this.numTemperature = new System.Windows.Forms.NumericUpDown();
            this.lblTemperature = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBoxSystemPrompt = new System.Windows.Forms.GroupBox();
            this.txtSystemPrompt = new System.Windows.Forms.TextBox();
            this.groupBoxSpeechRecognition = new System.Windows.Forms.GroupBox();
            this.chkEnableSpeechRecognition = new System.Windows.Forms.CheckBox();
            this.lblSpeechLanguage = new System.Windows.Forms.Label();
            this.comboSpeechLanguage = new System.Windows.Forms.ComboBox();
            this.groupBoxVoiceCommands = new System.Windows.Forms.GroupBox();
            this.chkEnableVoiceCommands = new System.Windows.Forms.CheckBox();
            this.lblVoiceCommandPhrase = new System.Windows.Forms.Label();
            this.txtVoiceCommandPhrase = new System.Windows.Forms.TextBox();
            this.groupBoxPlugins.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTemperature)).BeginInit();
            this.groupBoxSystemPrompt.SuspendLayout();
            this.groupBoxSpeechRecognition.SuspendLayout();
            this.groupBoxVoiceCommands.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxPlugins
            // 
            this.groupBoxPlugins.Controls.Add(this.chkMousePlugin);
            this.groupBoxPlugins.Controls.Add(this.chkKeyboardPlugin);
            this.groupBoxPlugins.Controls.Add(this.chkScreenCapturePlugin);
            this.groupBoxPlugins.Controls.Add(this.chkPowerShellPlugin);
            this.groupBoxPlugins.Controls.Add(this.chkCMDPlugin);
            this.groupBoxPlugins.Controls.Add(this.chkWindowSelectionPlugin);
            this.groupBoxPlugins.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxPlugins.Location = new System.Drawing.Point(12, 12);
            this.groupBoxPlugins.Name = "groupBoxPlugins";
            this.groupBoxPlugins.Size = new System.Drawing.Size(436, 200);
            this.groupBoxPlugins.TabIndex = 0;
            this.groupBoxPlugins.TabStop = false;
            this.groupBoxPlugins.Text = "Available Plugins";
            // 
            // chkMousePlugin
            // 
            this.chkMousePlugin.AutoSize = true;
            this.chkMousePlugin.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkMousePlugin.Location = new System.Drawing.Point(17, 142);
            this.chkMousePlugin.Name = "chkMousePlugin";
            this.chkMousePlugin.Size = new System.Drawing.Size(288, 23);
            this.chkMousePlugin.TabIndex = 4;
            this.chkMousePlugin.Text = "Enable Mouse Plugin (Mouse Automation)";
            this.chkMousePlugin.UseVisualStyleBackColor = true;
            // 
            // chkKeyboardPlugin
            // 
            this.chkKeyboardPlugin.AutoSize = true;
            this.chkKeyboardPlugin.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkKeyboardPlugin.Location = new System.Drawing.Point(17, 113);
            this.chkKeyboardPlugin.Name = "chkKeyboardPlugin";
            this.chkKeyboardPlugin.Size = new System.Drawing.Size(328, 23);
            this.chkKeyboardPlugin.TabIndex = 3;
            this.chkKeyboardPlugin.Text = "Enable Keyboard Plugin (Keyboard Automation)";
            this.chkKeyboardPlugin.UseVisualStyleBackColor = true;
            // 
            // chkScreenCapturePlugin
            // 
            this.chkScreenCapturePlugin.AutoSize = true;
            this.chkScreenCapturePlugin.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkScreenCapturePlugin.Location = new System.Drawing.Point(17, 84);
            this.chkScreenCapturePlugin.Name = "chkScreenCapturePlugin";
            this.chkScreenCapturePlugin.Size = new System.Drawing.Size(313, 23);
            this.chkScreenCapturePlugin.TabIndex = 2;
            this.chkScreenCapturePlugin.Text = "Enable Screen Capture Plugin (Screenshots)";
            this.chkScreenCapturePlugin.UseVisualStyleBackColor = true;
            // 
            // chkPowerShellPlugin
            // 
            this.chkPowerShellPlugin.AutoSize = true;
            this.chkPowerShellPlugin.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPowerShellPlugin.Location = new System.Drawing.Point(17, 55);
            this.chkPowerShellPlugin.Name = "chkPowerShellPlugin";
            this.chkPowerShellPlugin.Size = new System.Drawing.Size(343, 23);
            this.chkPowerShellPlugin.TabIndex = 1;
            this.chkPowerShellPlugin.Text = "Enable PowerShell Plugin (PowerShell Commands)";
            this.chkPowerShellPlugin.UseVisualStyleBackColor = true;
            // 
            // chkCMDPlugin
            // 
            this.chkCMDPlugin.AutoSize = true;
            this.chkCMDPlugin.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCMDPlugin.Location = new System.Drawing.Point(17, 26);
            this.chkCMDPlugin.Name = "chkCMDPlugin";
            this.chkCMDPlugin.Size = new System.Drawing.Size(268, 23);
            this.chkCMDPlugin.TabIndex = 0;
            this.chkCMDPlugin.Text = "Enable CMD Plugin (Command Prompt)";
            this.chkCMDPlugin.UseVisualStyleBackColor = true;
            // 
            // chkWindowSelectionPlugin
            // 
            this.chkWindowSelectionPlugin.AutoSize = true;
            this.chkWindowSelectionPlugin.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkWindowSelectionPlugin.Location = new System.Drawing.Point(17, 171);
            this.chkWindowSelectionPlugin.Name = "chkWindowSelectionPlugin";
            this.chkWindowSelectionPlugin.Size = new System.Drawing.Size(351, 23);
            this.chkWindowSelectionPlugin.TabIndex = 5;
            this.chkWindowSelectionPlugin.Text = "Enable Window Selection Plugin (Window Handles)";
            this.chkWindowSelectionPlugin.UseVisualStyleBackColor = true;
            // 
            // enablePluginLoggingCheckBox
            // 
            this.enablePluginLoggingCheckBox.AutoSize = true;
            this.enablePluginLoggingCheckBox.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enablePluginLoggingCheckBox.Location = new System.Drawing.Point(12, 218);
            this.enablePluginLoggingCheckBox.Name = "enablePluginLoggingCheckBox";
            this.enablePluginLoggingCheckBox.Size = new System.Drawing.Size(133, 23);
            this.enablePluginLoggingCheckBox.TabIndex = 6;
            this.enablePluginLoggingCheckBox.Text = "Log Plugin Usage";
            this.enablePluginLoggingCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.chkMultiAgentMode);
            this.groupBoxSettings.Controls.Add(this.chkAutoInvoke);
            this.groupBoxSettings.Controls.Add(this.chkRetainChatHistory);
            this.groupBoxSettings.Controls.Add(this.numTemperature);
            this.groupBoxSettings.Controls.Add(this.lblTemperature);
            this.groupBoxSettings.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 253);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(436, 150);
            this.groupBoxSettings.TabIndex = 1;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "AI Settings";
            // 
            // chkMultiAgentMode
            // 
            this.chkMultiAgentMode.AutoSize = true;
            this.chkMultiAgentMode.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkMultiAgentMode.Location = new System.Drawing.Point(17, 117);
            this.chkMultiAgentMode.Name = "chkMultiAgentMode";
            this.chkMultiAgentMode.Size = new System.Drawing.Size(173, 23);
            this.chkMultiAgentMode.TabIndex = 4;
            this.chkMultiAgentMode.Text = "Enable Multi-Agent Mode";
            this.chkMultiAgentMode.UseVisualStyleBackColor = true;
            this.chkMultiAgentMode.CheckedChanged += new System.EventHandler(this.chkMultiAgentMode_CheckedChanged);
            // 
            // chkAutoInvoke
            // 
            this.chkAutoInvoke.AutoSize = true;
            this.chkAutoInvoke.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAutoInvoke.Location = new System.Drawing.Point(17, 59);
            this.chkAutoInvoke.Name = "chkAutoInvoke";
            this.chkAutoInvoke.Size = new System.Drawing.Size(415, 23);
            this.chkAutoInvoke.TabIndex = 2;
            this.chkAutoInvoke.Text = "Auto-Invoke Functions (Let AI execute tools automatically)";
            this.chkAutoInvoke.UseVisualStyleBackColor = true;
            // 
            // chkRetainChatHistory
            // 
            this.chkRetainChatHistory.AutoSize = true;
            this.chkRetainChatHistory.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRetainChatHistory.Location = new System.Drawing.Point(17, 88);
            this.chkRetainChatHistory.Name = "chkRetainChatHistory";
            this.chkRetainChatHistory.Size = new System.Drawing.Size(419, 23);
            this.chkRetainChatHistory.TabIndex = 3;
            this.chkRetainChatHistory.Text = "Retain Chat History (Keep chat messages between sessions)";
            this.chkRetainChatHistory.UseVisualStyleBackColor = true;
            // 
            // numTemperature
            // 
            this.numTemperature.DecimalPlaces = 1;
            this.numTemperature.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numTemperature.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numTemperature.Location = new System.Drawing.Point(199, 27);
            this.numTemperature.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTemperature.Name = "numTemperature";
            this.numTemperature.Size = new System.Drawing.Size(60, 26);
            this.numTemperature.TabIndex = 1;
            this.numTemperature.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            // 
            // lblTemperature
            // 
            this.lblTemperature.AutoSize = true;
            this.lblTemperature.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTemperature.Location = new System.Drawing.Point(14, 29);
            this.lblTemperature.Name = "lblTemperature";
            this.lblTemperature.Size = new System.Drawing.Size(192, 19);
            this.lblTemperature.TabIndex = 0;
            this.lblTemperature.Text = "Temperature (Randomness):";
            // 
            // saveButton
            // 
            this.saveButton.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.Location = new System.Drawing.Point(236, 725);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(98, 30);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(340, 725);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(98, 30);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click_1);
            // 
            // groupBoxSystemPrompt
            // 
            this.groupBoxSystemPrompt.Controls.Add(this.txtSystemPrompt);
            this.groupBoxSystemPrompt.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSystemPrompt.Location = new System.Drawing.Point(12, 571);
            this.groupBoxSystemPrompt.Name = "groupBoxSystemPrompt";
            this.groupBoxSystemPrompt.Size = new System.Drawing.Size(436, 146);
            this.groupBoxSystemPrompt.TabIndex = 4;
            this.groupBoxSystemPrompt.TabStop = false;
            this.groupBoxSystemPrompt.Text = "System Prompt";
            // 
            // txtSystemPrompt
            // 
            this.txtSystemPrompt.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSystemPrompt.Location = new System.Drawing.Point(17, 29);
            this.txtSystemPrompt.Multiline = true;
            this.txtSystemPrompt.Name = "txtSystemPrompt";
            this.txtSystemPrompt.Size = new System.Drawing.Size(402, 100);
            this.txtSystemPrompt.TabIndex = 0;
            // 
            // groupBoxSpeechRecognition
            // 
            this.groupBoxSpeechRecognition.Controls.Add(this.comboSpeechLanguage);
            this.groupBoxSpeechRecognition.Controls.Add(this.lblSpeechLanguage);
            this.groupBoxSpeechRecognition.Controls.Add(this.chkEnableSpeechRecognition);
            this.groupBoxSpeechRecognition.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSpeechRecognition.Location = new System.Drawing.Point(12, 409);
            this.groupBoxSpeechRecognition.Name = "groupBoxSpeechRecognition";
            this.groupBoxSpeechRecognition.Size = new System.Drawing.Size(436, 92);
            this.groupBoxSpeechRecognition.TabIndex = 5;
            this.groupBoxSpeechRecognition.TabStop = false;
            this.groupBoxSpeechRecognition.Text = "Speech Recognition";
            // 
            // chkEnableSpeechRecognition
            // 
            this.chkEnableSpeechRecognition.AutoSize = true;
            this.chkEnableSpeechRecognition.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnableSpeechRecognition.Location = new System.Drawing.Point(17, 29);
            this.chkEnableSpeechRecognition.Name = "chkEnableSpeechRecognition";
            this.chkEnableSpeechRecognition.Size = new System.Drawing.Size(192, 23);
            this.chkEnableSpeechRecognition.TabIndex = 0;
            this.chkEnableSpeechRecognition.Text = "Enable Speech Recognition";
            this.chkEnableSpeechRecognition.UseVisualStyleBackColor = true;
            // 
            // lblSpeechLanguage
            // 
            this.lblSpeechLanguage.AutoSize = true;
            this.lblSpeechLanguage.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpeechLanguage.Location = new System.Drawing.Point(14, 59);
            this.lblSpeechLanguage.Name = "lblSpeechLanguage";
            this.lblSpeechLanguage.Size = new System.Drawing.Size(160, 19);
            this.lblSpeechLanguage.TabIndex = 1;
            this.lblSpeechLanguage.Text = "Recognition Language:";
            // 
            // comboSpeechLanguage
            // 
            this.comboSpeechLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSpeechLanguage.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboSpeechLanguage.FormattingEnabled = true;
            this.comboSpeechLanguage.Items.AddRange(new object[] {
            "en-US",
            "en-GB",
            "en-AU",
            "fr-FR",
            "es-ES",
            "de-DE",
            "it-IT",
            "ja-JP",
            "zh-CN",
            "ru-RU"});
            this.comboSpeechLanguage.Location = new System.Drawing.Point(180, 56);
            this.comboSpeechLanguage.Name = "comboSpeechLanguage";
            this.comboSpeechLanguage.Size = new System.Drawing.Size(239, 27);
            this.comboSpeechLanguage.TabIndex = 2;
            // 
            // groupBoxVoiceCommands
            // 
            this.groupBoxVoiceCommands.Controls.Add(this.txtVoiceCommandPhrase);
            this.groupBoxVoiceCommands.Controls.Add(this.lblVoiceCommandPhrase);
            this.groupBoxVoiceCommands.Controls.Add(this.chkEnableVoiceCommands);
            this.groupBoxVoiceCommands.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxVoiceCommands.Location = new System.Drawing.Point(12, 507);
            this.groupBoxVoiceCommands.Name = "groupBoxVoiceCommands";
            this.groupBoxVoiceCommands.Size = new System.Drawing.Size(436, 88);
            this.groupBoxVoiceCommands.TabIndex = 6;
            this.groupBoxVoiceCommands.TabStop = false;
            this.groupBoxVoiceCommands.Text = "Voice Commands";
            // 
            // chkEnableVoiceCommands
            // 
            this.chkEnableVoiceCommands.AutoSize = true;
            this.chkEnableVoiceCommands.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnableVoiceCommands.Location = new System.Drawing.Point(17, 29);
            this.chkEnableVoiceCommands.Name = "chkEnableVoiceCommands";
            this.chkEnableVoiceCommands.Size = new System.Drawing.Size(173, 23);
            this.chkEnableVoiceCommands.TabIndex = 0;
            this.chkEnableVoiceCommands.Text = "Enable Voice Commands";
            this.chkEnableVoiceCommands.UseVisualStyleBackColor = true;
            // 
            // lblVoiceCommandPhrase
            // 
            this.lblVoiceCommandPhrase.AutoSize = true;
            this.lblVoiceCommandPhrase.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVoiceCommandPhrase.Location = new System.Drawing.Point(14, 59);
            this.lblVoiceCommandPhrase.Name = "lblVoiceCommandPhrase";
            this.lblVoiceCommandPhrase.Size = new System.Drawing.Size(151, 19);
            this.lblVoiceCommandPhrase.TabIndex = 1;
            this.lblVoiceCommandPhrase.Text = "Voice Command Phrase:";
            // 
            // txtVoiceCommandPhrase
            // 
            this.txtVoiceCommandPhrase.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVoiceCommandPhrase.Location = new System.Drawing.Point(180, 56);
            this.txtVoiceCommandPhrase.Name = "txtVoiceCommandPhrase";
            this.txtVoiceCommandPhrase.Size = new System.Drawing.Size(239, 26);
            this.txtVoiceCommandPhrase.TabIndex = 2;
            // 
            // ToolConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 766);
            this.Controls.Add(this.enablePluginLoggingCheckBox);
            this.Controls.Add(this.groupBoxVoiceCommands);
            this.Controls.Add(this.groupBoxSpeechRecognition);
            this.Controls.Add(this.groupBoxSystemPrompt);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.groupBoxPlugins);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToolConfigForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tool Configuration";
            this.groupBoxPlugins.ResumeLayout(false);
            this.groupBoxPlugins.PerformLayout();
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTemperature)).EndInit();
            this.groupBoxSystemPrompt.ResumeLayout(false);
            this.groupBoxSystemPrompt.PerformLayout();
            this.groupBoxSpeechRecognition.ResumeLayout(false);
            this.groupBoxSpeechRecognition.PerformLayout();
            this.groupBoxVoiceCommands.ResumeLayout(false);
            this.groupBoxVoiceCommands.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxPlugins;
        private System.Windows.Forms.CheckBox chkMousePlugin;
        private System.Windows.Forms.CheckBox chkKeyboardPlugin;
        private System.Windows.Forms.CheckBox chkScreenCapturePlugin;
        private System.Windows.Forms.CheckBox chkPowerShellPlugin;
        private System.Windows.Forms.CheckBox chkCMDPlugin;
        private System.Windows.Forms.CheckBox chkWindowSelectionPlugin;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.NumericUpDown numTemperature;
        private System.Windows.Forms.Label lblTemperature;
        private System.Windows.Forms.CheckBox chkAutoInvoke;
        private System.Windows.Forms.CheckBox chkRetainChatHistory;
        private System.Windows.Forms.CheckBox chkMultiAgentMode;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox groupBoxSystemPrompt;
        private System.Windows.Forms.TextBox txtSystemPrompt;
        private System.Windows.Forms.CheckBox enablePluginLoggingCheckBox;
        private System.Windows.Forms.GroupBox groupBoxSpeechRecognition;
        private System.Windows.Forms.CheckBox chkEnableSpeechRecognition;
        private System.Windows.Forms.Label lblSpeechLanguage;
        private System.Windows.Forms.ComboBox comboSpeechLanguage;
        private System.Windows.Forms.GroupBox groupBoxVoiceCommands;
        private System.Windows.Forms.CheckBox chkEnableVoiceCommands;
        private System.Windows.Forms.Label lblVoiceCommandPhrase;
        private System.Windows.Forms.TextBox txtVoiceCommandPhrase;
    }
}