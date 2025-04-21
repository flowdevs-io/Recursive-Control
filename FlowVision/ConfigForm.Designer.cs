namespace FlowVision
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            this.DeploymentNameLabel = new System.Windows.Forms.Label();
            this.EndpointURLLabel = new System.Windows.Forms.Label();
            this.deploymentNameTextBox = new System.Windows.Forms.TextBox();
            this.endpointURLTextBox = new System.Windows.Forms.TextBox();
            this.apiKeyTextBox = new System.Windows.Forms.TextBox();
            this.APIKeyLabel = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // DeploymentNameLabel
            // 
            this.DeploymentNameLabel.AutoSize = true;
            this.DeploymentNameLabel.Font = new System.Drawing.Font("Comic Sans MS", 14F);
            this.DeploymentNameLabel.Location = new System.Drawing.Point(12, 9);
            this.DeploymentNameLabel.Name = "DeploymentNameLabel";
            this.DeploymentNameLabel.Size = new System.Drawing.Size(170, 26);
            this.DeploymentNameLabel.TabIndex = 0;
            this.DeploymentNameLabel.Text = "Model Name";
            // 
            // EndpointURLLabel
            // 
            this.EndpointURLLabel.AutoSize = true;
            this.EndpointURLLabel.Font = new System.Drawing.Font("Comic Sans MS", 14F);
            this.EndpointURLLabel.Location = new System.Drawing.Point(12, 49);
            this.EndpointURLLabel.Name = "EndpointURLLabel";
            this.EndpointURLLabel.Size = new System.Drawing.Size(129, 26);
            this.EndpointURLLabel.TabIndex = 1;
            this.EndpointURLLabel.Text = "Endpoint URL";
            // 
            // deploymentNameTextBox
            // 
            this.deploymentNameTextBox.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deploymentNameTextBox.Location = new System.Drawing.Point(188, 6);
            this.deploymentNameTextBox.Name = "deploymentNameTextBox";
            this.deploymentNameTextBox.Size = new System.Drawing.Size(484, 34);
            this.deploymentNameTextBox.TabIndex = 2;
            // 
            // endpointURLTextBox
            // 
            this.endpointURLTextBox.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.endpointURLTextBox.Location = new System.Drawing.Point(147, 46);
            this.endpointURLTextBox.Name = "endpointURLTextBox";
            this.endpointURLTextBox.Size = new System.Drawing.Size(525, 34);
            this.endpointURLTextBox.TabIndex = 3;
            // 
            // apiKeyTextBox
            // 
            this.apiKeyTextBox.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.apiKeyTextBox.Location = new System.Drawing.Point(101, 89);
            this.apiKeyTextBox.Name = "apiKeyTextBox";
            this.apiKeyTextBox.PasswordChar = '*';
            this.apiKeyTextBox.Size = new System.Drawing.Size(571, 34);
            this.apiKeyTextBox.TabIndex = 5;
            // 
            // APIKeyLabel
            // 
            this.APIKeyLabel.AutoSize = true;
            this.APIKeyLabel.Font = new System.Drawing.Font("Comic Sans MS", 14F);
            this.APIKeyLabel.Location = new System.Drawing.Point(12, 89);
            this.APIKeyLabel.Name = "APIKeyLabel";
            this.APIKeyLabel.Size = new System.Drawing.Size(83, 26);
            this.APIKeyLabel.TabIndex = 4;
            this.APIKeyLabel.Text = "API Key";
            // 
            // saveButton
            // 
            this.saveButton.Font = new System.Drawing.Font("Comic Sans MS", 10.25F);
            this.saveButton.Location = new System.Drawing.Point(316, 129);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(98, 30);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 161);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.apiKeyTextBox);
            this.Controls.Add(this.APIKeyLabel);
            this.Controls.Add(this.endpointURLTextBox);
            this.Controls.Add(this.deploymentNameTextBox);
            this.Controls.Add(this.EndpointURLLabel);
            this.Controls.Add(this.DeploymentNameLabel);
            this.MaximumSize = new System.Drawing.Size(700, 200);
            this.MinimumSize = new System.Drawing.Size(700, 200);
            this.Name = "ConfigForm";
            this.Text = "ConfigForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label DeploymentNameLabel;
        private System.Windows.Forms.Label EndpointURLLabel;
        private System.Windows.Forms.TextBox deploymentNameTextBox;
        private System.Windows.Forms.TextBox endpointURLTextBox;
        private System.Windows.Forms.TextBox apiKeyTextBox;
        private System.Windows.Forms.Label APIKeyLabel;
        private System.Windows.Forms.Button saveButton;
    }
}