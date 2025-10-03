namespace FlowVision
{
    partial class Form1
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
            
            // Clean up speech recognition resources
            if (speechRecognition != null)
            {
                speechRecognition.Dispose();
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
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newChatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToMarkdownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDebugLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.agentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionerAgentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plannerAgentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.coordinatorAgentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.githubAgentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiAgentModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.omniParserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.activityMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.executionVisualizerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.documentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.menuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip2
            // 
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(367, 24);
            this.menuStrip2.TabIndex = 1;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // filesToolStripMenuItem
            // 
            this.filesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem,
            this.newChatToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
            this.filesToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.filesToolStripMenuItem.Text = "File";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.toolsToolStripMenuItem.Text = "Tools";
            this.toolsToolStripMenuItem.Click += new System.EventHandler(this.toolsToolStripMenuItem_Click);
            // 
            // newChatToolStripMenuItem
            // 
            this.newChatToolStripMenuItem.Name = "newChatToolStripMenuItem";
            this.newChatToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newChatToolStripMenuItem.Text = "New Chat";
            this.newChatToolStripMenuItem.Click += new System.EventHandler(this.newChatToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToJSONToolStripMenuItem,
            this.exportToMarkdownToolStripMenuItem,
            this.exportDebugLogToolStripMenuItem,
            this.copyToClipboardToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportToolStripMenuItem.Text = "Export Chat";
            // 
            // exportToJSONToolStripMenuItem
            // 
            this.exportToJSONToolStripMenuItem.Name = "exportToJSONToolStripMenuItem";
            this.exportToJSONToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.exportToJSONToolStripMenuItem.Text = "Export to JSON";
            this.exportToJSONToolStripMenuItem.Click += new System.EventHandler(this.exportToJSONToolStripMenuItem_Click);
            // 
            // exportToMarkdownToolStripMenuItem
            // 
            this.exportToMarkdownToolStripMenuItem.Name = "exportToMarkdownToolStripMenuItem";
            this.exportToMarkdownToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.exportToMarkdownToolStripMenuItem.Text = "Export to Markdown";
            this.exportToMarkdownToolStripMenuItem.Click += new System.EventHandler(this.exportToMarkdownToolStripMenuItem_Click);
            // 
            // exportDebugLogToolStripMenuItem
            // 
            this.exportDebugLogToolStripMenuItem.Name = "exportDebugLogToolStripMenuItem";
            this.exportDebugLogToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.exportDebugLogToolStripMenuItem.Text = "Export Debug Log (with Tools)";
            this.exportDebugLogToolStripMenuItem.Click += new System.EventHandler(this.exportDebugLogToolStripMenuItem_Click);
            // 
            // copyToClipboardToolStripMenuItem
            // 
            this.copyToClipboardToolStripMenuItem.Name = "copyToClipboardToolStripMenuItem";
            this.copyToClipboardToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.copyToClipboardToolStripMenuItem.Text = "Copy to Clipboard";
            this.copyToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyToClipboardToolStripMenuItem_Click);
            // 
            // visionToolStripMenuItem
            // 
            this.visionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.omniParserToolStripMenuItem});
            this.visionToolStripMenuItem.Name = "visionToolStripMenuItem";
            this.visionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.visionToolStripMenuItem.Text = "🔭 Vision Tools";
            // 
            // omniParserToolStripMenuItem
            // 
            this.omniParserToolStripMenuItem.Name = "omniParserToolStripMenuItem";
            this.omniParserToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.omniParserToolStripMenuItem.Text = "📸 OmniParser Config";
            this.omniParserToolStripMenuItem.Click += new System.EventHandler(this.omniParserToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem,
            this.agentsToolStripMenuItem,
            this.visionToolStripMenuItem,
            this.multiAgentModeToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "⚙️ Setup";
            // 
            // agentsToolStripMenuItem
            // 
            this.agentsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.actionerAgentToolStripMenuItem,
            this.plannerAgentToolStripMenuItem,
            this.coordinatorAgentToolStripMenuItem,
            this.githubAgentToolStripMenuItem});
            this.agentsToolStripMenuItem.Name = "agentsToolStripMenuItem";
            this.agentsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.agentsToolStripMenuItem.Text = "🤖 AI Agents";
            // 
            // actionerAgentToolStripMenuItem
            // 
            this.actionerAgentToolStripMenuItem.Name = "actionerAgentToolStripMenuItem";
            this.actionerAgentToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.actionerAgentToolStripMenuItem.Text = "⚡ Actioner Agent (Primary)";
            this.actionerAgentToolStripMenuItem.Click += new System.EventHandler(this.actionerAgentToolStripMenuItem_Click);
            // 
            // plannerAgentToolStripMenuItem
            // 
            this.plannerAgentToolStripMenuItem.Name = "plannerAgentToolStripMenuItem";
            this.plannerAgentToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.plannerAgentToolStripMenuItem.Text = "📋 Planner Agent";
            this.plannerAgentToolStripMenuItem.Click += new System.EventHandler(this.plannerAgentToolStripMenuItem_Click);
            // 
            // coordinatorAgentToolStripMenuItem
            // 
            this.coordinatorAgentToolStripMenuItem.Name = "coordinatorAgentToolStripMenuItem";
            this.coordinatorAgentToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.coordinatorAgentToolStripMenuItem.Text = "🎯 Coordinator Agent";
            this.coordinatorAgentToolStripMenuItem.Click += new System.EventHandler(this.coordinatorAgentToolStripMenuItem_Click);
            // 
            // githubAgentToolStripMenuItem
            // 
            this.githubAgentToolStripMenuItem.Name = "githubAgentToolStripMenuItem";
            this.githubAgentToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.githubAgentToolStripMenuItem.Text = "🐙 GitHub Agent";
            this.githubAgentToolStripMenuItem.Click += new System.EventHandler(this.githubAgentToolStripMenuItem_Click);
            // 
            // multiAgentModeToolStripMenuItem
            // 
            this.multiAgentModeToolStripMenuItem.CheckOnClick = true;
            this.multiAgentModeToolStripMenuItem.Name = "multiAgentModeToolStripMenuItem";
            this.multiAgentModeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.multiAgentModeToolStripMenuItem.Text = "🔀 Multi-Agent Mode";
            this.multiAgentModeToolStripMenuItem.Click += new System.EventHandler(this.multiAgentModeToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.activityMonitorToolStripMenuItem,
            this.executionVisualizerToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "👁️ View";
            // 
            // activityMonitorToolStripMenuItem
            // 
            this.activityMonitorToolStripMenuItem.CheckOnClick = true;
            this.activityMonitorToolStripMenuItem.Name = "activityMonitorToolStripMenuItem";
            this.activityMonitorToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.activityMonitorToolStripMenuItem.Text = "📊 Activity Monitor";
            this.activityMonitorToolStripMenuItem.Click += new System.EventHandler(this.activityMonitorToolStripMenuItem_Click);
            // 
            // executionVisualizerToolStripMenuItem
            // 
            this.executionVisualizerToolStripMenuItem.CheckOnClick = true;
            this.executionVisualizerToolStripMenuItem.Name = "executionVisualizerToolStripMenuItem";
            this.executionVisualizerToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.executionVisualizerToolStripMenuItem.Text = "🎯 Execution Visualizer";
            this.executionVisualizerToolStripMenuItem.Click += new System.EventHandler(this.executionVisualizerToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.documentationToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "❓ Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.aboutToolStripMenuItem.Text = "ℹ️ About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // documentationToolStripMenuItem
            // 
            this.documentationToolStripMenuItem.Name = "documentationToolStripMenuItem";
            this.documentationToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.documentationToolStripMenuItem.Text = "📚 Documentation";
            this.documentationToolStripMenuItem.Click += new System.EventHandler(this.documentationToolStripMenuItem_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 24);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(367, 543);
            this.mainPanel.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 567);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.menuStrip2);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Human Interface";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem agentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionerAgentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plannerAgentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem coordinatorAgentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem githubAgentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visionToolStripMenuItem;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.ToolStripMenuItem omniParserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newChatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToMarkdownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportDebugLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multiAgentModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem activityMonitorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem executionVisualizerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem documentationToolStripMenuItem;
    }
}

