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
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.chkAutoInvoke = new System.Windows.Forms.CheckBox();
            this.chkRetainChatHistory = new System.Windows.Forms.CheckBox();
            this.numTemperature = new System.Windows.Forms.NumericUpDown();
            this.lblTemperature = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBoxSystemPrompt = new System.Windows.Forms.GroupBox();
            this.txtSystemPrompt = new System.Windows.Forms.TextBox();
            this.groupBoxPlugins.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTemperature)).BeginInit();
            this.groupBoxSystemPrompt.SuspendLayout();
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
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.chkAutoInvoke);
            this.groupBoxSettings.Controls.Add(this.chkRetainChatHistory);
            this.groupBoxSettings.Controls.Add(this.numTemperature);
            this.groupBoxSettings.Controls.Add(this.lblTemperature);
            this.groupBoxSettings.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 218);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(436, 120);
            this.groupBoxSettings.TabIndex = 1;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "AI Settings";
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
            this.saveButton.Location = new System.Drawing.Point(236, 496);
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
            this.cancelButton.Location = new System.Drawing.Point(340, 496);
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
            this.groupBoxSystemPrompt.Location = new System.Drawing.Point(12, 344);
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
            // ToolConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 536);
            this.Controls.Add(this.groupBoxSystemPrompt);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.groupBoxPlugins);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToolConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tool Configuration";
            this.groupBoxPlugins.ResumeLayout(false);
            this.groupBoxPlugins.PerformLayout();
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTemperature)).EndInit();
            this.groupBoxSystemPrompt.ResumeLayout(false);
            this.groupBoxSystemPrompt.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox groupBoxSystemPrompt;
        private System.Windows.Forms.TextBox txtSystemPrompt;
    }
}