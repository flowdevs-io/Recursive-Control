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
            this.components = new System.ComponentModel.Container();
            this.groupBoxPlugins = new System.Windows.Forms.GroupBox();
            this.indicatorWindow = new System.Windows.Forms.PictureBox();
            this.indicatorMouse = new System.Windows.Forms.PictureBox();
            this.indicatorKeyboard = new System.Windows.Forms.PictureBox();
            this.indicatorScreenCapture = new System.Windows.Forms.PictureBox();
            this.indicatorPowerShell = new System.Windows.Forms.PictureBox();
            this.indicatorCMD = new System.Windows.Forms.PictureBox();
            this.indicatorPlaywright = new System.Windows.Forms.PictureBox();
            this.chkMousePlugin = new System.Windows.Forms.CheckBox();
            this.chkKeyboardPlugin = new System.Windows.Forms.CheckBox();
            this.chkScreenCapturePlugin = new System.Windows.Forms.CheckBox();
            this.chkPowerShellPlugin = new System.Windows.Forms.CheckBox();
            this.chkCMDPlugin = new System.Windows.Forms.CheckBox();
            this.chkWindowSelectionPlugin = new System.Windows.Forms.CheckBox();
            this.chkPlaywrightPlugin = new System.Windows.Forms.CheckBox();
            this.enablePluginLoggingCheckBox = new System.Windows.Forms.CheckBox();
            this.chkEnableRemoteControl = new System.Windows.Forms.CheckBox();
            this.numRemotePort = new System.Windows.Forms.NumericUpDown();
            this.lblRemotePort = new System.Windows.Forms.Label();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.indicatorAutoInvoke = new System.Windows.Forms.PictureBox();
            this.indicatorMultiAgent = new System.Windows.Forms.PictureBox();
            this.chkMultiAgentMode = new System.Windows.Forms.CheckBox();
            this.chkAutoInvoke = new System.Windows.Forms.CheckBox();
            this.chkRetainChatHistory = new System.Windows.Forms.CheckBox();
            this.numTemperature = new System.Windows.Forms.NumericUpDown();
            this.lblTemperature = new System.Windows.Forms.Label();
            this.chkDynamicToolPrompts = new System.Windows.Forms.CheckBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBoxSpeechRecognition = new System.Windows.Forms.GroupBox();
            this.indicatorSpeech = new System.Windows.Forms.PictureBox();
            this.comboSpeechLanguage = new System.Windows.Forms.ComboBox();
            this.lblSpeechLanguage = new System.Windows.Forms.Label();
            this.chkEnableSpeechRecognition = new System.Windows.Forms.CheckBox();
            this.groupBoxVoiceCommands = new System.Windows.Forms.GroupBox();
            this.indicatorVoiceCmd = new System.Windows.Forms.PictureBox();
            this.txtVoiceCommandPhrase = new System.Windows.Forms.TextBox();
            this.lblVoiceCommandPhrase = new System.Windows.Forms.Label();
            this.chkEnableVoiceCommands = new System.Windows.Forms.CheckBox();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPlugins = new System.Windows.Forms.TabPage();
            this.tabAISettings = new System.Windows.Forms.TabPage();
            this.tabVoice = new System.Windows.Forms.TabPage();
            this.tabCoordinator = new System.Windows.Forms.TabPage();
            this.grpCoordinatorConfig = new System.Windows.Forms.GroupBox();
            this.lblCoordinatorPrompt = new System.Windows.Forms.Label();
            this.txtCoordinatorSystemPrompt = new System.Windows.Forms.TextBox();
            this.chkUseCustomCoordinatorConfig = new System.Windows.Forms.CheckBox();
            this.comboCoordinatorConfig = new System.Windows.Forms.ComboBox();
            this.btnConfigureCoordinator = new System.Windows.Forms.Button();
            this.tabPlanner = new System.Windows.Forms.TabPage();
            this.grpPlannerConfig = new System.Windows.Forms.GroupBox();
            this.lblPlannerPrompt = new System.Windows.Forms.Label();
            this.txtPlannerSystemPrompt = new System.Windows.Forms.TextBox();
            this.chkUseCustomPlannerConfig = new System.Windows.Forms.CheckBox();
            this.comboPlannerConfig = new System.Windows.Forms.ComboBox();
            this.btnConfigurePlanner = new System.Windows.Forms.Button();
            this.tabActioner = new System.Windows.Forms.TabPage();
            this.grpActionerConfig = new System.Windows.Forms.GroupBox();
            this.lblActionerPrompt = new System.Windows.Forms.Label();
            this.txtActionerSystemPrompt = new System.Windows.Forms.TextBox();
            this.chkUseCustomExecutorConfig = new System.Windows.Forms.CheckBox();
            this.comboActionerConfig = new System.Windows.Forms.ComboBox();
            this.btnConfigureActioner = new System.Windows.Forms.Button();
            this.tabAppearance = new System.Windows.Forms.TabPage();
            this.groupBoxTheme = new System.Windows.Forms.GroupBox();
            this.labelTheme = new System.Windows.Forms.Label();
            this.cbTheme = new System.Windows.Forms.ComboBox();
            this.tabProfiles = new System.Windows.Forms.TabPage();
            this.groupBoxProfiles = new System.Windows.Forms.GroupBox();
            this.btnDeleteProfile = new System.Windows.Forms.Button();
            this.btnLoadProfile = new System.Windows.Forms.Button();
            this.btnSaveProfile = new System.Windows.Forms.Button();
            this.labelProfile = new System.Windows.Forms.Label();
            this.cmbProfiles = new System.Windows.Forms.ComboBox();
            this.imageListStatus = new System.Windows.Forms.ImageList(this.components);
            this.panelSearch = new System.Windows.Forms.Panel();
            this.searchResultLabel = new System.Windows.Forms.Label();
            this.txtSearchSettings = new System.Windows.Forms.TextBox();
            this.lblSearchSettings = new System.Windows.Forms.Label();
            this.saveNotification = new System.Windows.Forms.Panel();
            this.lblSaveNotification = new System.Windows.Forms.Label();
            this.saveNotificationTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBoxPlugins.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorWindow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorMouse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorKeyboard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorScreenCapture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorPowerShell)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorCMD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorPlaywright)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRemotePort)).BeginInit();
            this.groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorAutoInvoke)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorMultiAgent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTemperature)).BeginInit();
            this.groupBoxSpeechRecognition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorSpeech)).BeginInit();
            this.groupBoxVoiceCommands.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorVoiceCmd)).BeginInit();
            this.tabControlMain.SuspendLayout();
            this.tabPlugins.SuspendLayout();
            this.tabAISettings.SuspendLayout();
            this.tabVoice.SuspendLayout();
            this.tabCoordinator.SuspendLayout();
            this.grpCoordinatorConfig.SuspendLayout();
            this.tabPlanner.SuspendLayout();
            this.grpPlannerConfig.SuspendLayout();
            this.tabActioner.SuspendLayout();
            this.grpActionerConfig.SuspendLayout();
            this.tabAppearance.SuspendLayout();
            this.groupBoxTheme.SuspendLayout();
            this.tabProfiles.SuspendLayout();
            this.groupBoxProfiles.SuspendLayout();
            this.panelSearch.SuspendLayout();
            this.saveNotification.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxPlugins
            // 
            this.groupBoxPlugins.Controls.Add(this.indicatorWindow);
            this.groupBoxPlugins.Controls.Add(this.indicatorMouse);
            this.groupBoxPlugins.Controls.Add(this.indicatorKeyboard);
            this.groupBoxPlugins.Controls.Add(this.indicatorScreenCapture);
            this.groupBoxPlugins.Controls.Add(this.indicatorPowerShell);
            this.groupBoxPlugins.Controls.Add(this.indicatorCMD);
            this.groupBoxPlugins.Controls.Add(this.indicatorPlaywright);
            this.groupBoxPlugins.Controls.Add(this.chkMousePlugin);
            this.groupBoxPlugins.Controls.Add(this.chkKeyboardPlugin);
            this.groupBoxPlugins.Controls.Add(this.chkScreenCapturePlugin);
            this.groupBoxPlugins.Controls.Add(this.chkPowerShellPlugin);
            this.groupBoxPlugins.Controls.Add(this.chkCMDPlugin);
            this.groupBoxPlugins.Controls.Add(this.chkWindowSelectionPlugin);
            this.groupBoxPlugins.Controls.Add(this.chkPlaywrightPlugin);
            this.groupBoxPlugins.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxPlugins.Location = new System.Drawing.Point(6, 6);
            this.groupBoxPlugins.Name = "groupBoxPlugins";
            this.groupBoxPlugins.Size = new System.Drawing.Size(436, 240);
            this.groupBoxPlugins.TabIndex = 0;
            this.groupBoxPlugins.TabStop = false;
            this.groupBoxPlugins.Text = "Available Plugins";
            // 
            // indicatorWindow
            // 
            this.indicatorWindow.BackColor = System.Drawing.Color.Transparent;
            this.indicatorWindow.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indicatorWindow.Location = new System.Drawing.Point(390, 171);
            this.indicatorWindow.Name = "indicatorWindow";
            this.indicatorWindow.Size = new System.Drawing.Size(24, 24);
            this.indicatorWindow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.indicatorWindow.TabIndex = 11;
            this.indicatorWindow.TabStop = false;
            // 
            // indicatorMouse
            // 
            this.indicatorMouse.BackColor = System.Drawing.Color.Transparent;
            this.indicatorMouse.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indicatorMouse.Location = new System.Drawing.Point(390, 142);
            this.indicatorMouse.Name = "indicatorMouse";
            this.indicatorMouse.Size = new System.Drawing.Size(24, 24);
            this.indicatorMouse.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.indicatorMouse.TabIndex = 10;
            this.indicatorMouse.TabStop = false;
            // 
            // indicatorKeyboard
            // 
            this.indicatorKeyboard.BackColor = System.Drawing.Color.Transparent;
            this.indicatorKeyboard.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indicatorKeyboard.Location = new System.Drawing.Point(390, 113);
            this.indicatorKeyboard.Name = "indicatorKeyboard";
            this.indicatorKeyboard.Size = new System.Drawing.Size(24, 24);
            this.indicatorKeyboard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.indicatorKeyboard.TabIndex = 9;
            this.indicatorKeyboard.TabStop = false;
            // 
            // indicatorScreenCapture
            // 
            this.indicatorScreenCapture.BackColor = System.Drawing.Color.Transparent;
            this.indicatorScreenCapture.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indicatorScreenCapture.Location = new System.Drawing.Point(390, 84);
            this.indicatorScreenCapture.Name = "indicatorScreenCapture";
            this.indicatorScreenCapture.Size = new System.Drawing.Size(24, 24);
            this.indicatorScreenCapture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.indicatorScreenCapture.TabIndex = 8;
            this.indicatorScreenCapture.TabStop = false;
            // 
            // indicatorPowerShell
            // 
            this.indicatorPowerShell.BackColor = System.Drawing.Color.Transparent;
            this.indicatorPowerShell.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indicatorPowerShell.Location = new System.Drawing.Point(390, 55);
            this.indicatorPowerShell.Name = "indicatorPowerShell";
            this.indicatorPowerShell.Size = new System.Drawing.Size(24, 24);
            this.indicatorPowerShell.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.indicatorPowerShell.TabIndex = 7;
            this.indicatorPowerShell.TabStop = false;
            // 
            // indicatorCMD
            // 
            this.indicatorCMD.BackColor = System.Drawing.Color.Transparent;
            this.indicatorCMD.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indicatorCMD.Location = new System.Drawing.Point(390, 26);
            this.indicatorCMD.Name = "indicatorCMD";
            this.indicatorCMD.Size = new System.Drawing.Size(24, 24);
            this.indicatorCMD.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.indicatorCMD.TabIndex = 6;
            this.indicatorCMD.TabStop = false;
            // 
            // indicatorPlaywright
            // 
            this.indicatorPlaywright.BackColor = System.Drawing.Color.Transparent;
            this.indicatorPlaywright.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indicatorPlaywright.Location = new System.Drawing.Point(390, 200);
            this.indicatorPlaywright.Name = "indicatorPlaywright";
            this.indicatorPlaywright.Size = new System.Drawing.Size(24, 24);
            this.indicatorPlaywright.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.indicatorPlaywright.TabIndex = 12;
            this.indicatorPlaywright.TabStop = false;
            // 
            // chkMousePlugin
            // 
            this.chkMousePlugin.AutoSize = true;
            this.chkMousePlugin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkMousePlugin.Location = new System.Drawing.Point(17, 142);
            this.chkMousePlugin.Name = "chkMousePlugin";
            this.chkMousePlugin.Size = new System.Drawing.Size(287, 23);
            this.chkMousePlugin.TabIndex = 4;
            this.chkMousePlugin.Text = "Enable Mouse Plugin (Mouse Automation)";
            this.chkMousePlugin.UseVisualStyleBackColor = true;
            this.chkMousePlugin.CheckedChanged += new System.EventHandler(this.chkPluginStatus_CheckedChanged);
            // 
            // chkKeyboardPlugin
            // 
            this.chkKeyboardPlugin.AutoSize = true;
            this.chkKeyboardPlugin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkKeyboardPlugin.Location = new System.Drawing.Point(17, 113);
            this.chkKeyboardPlugin.Name = "chkKeyboardPlugin";
            this.chkKeyboardPlugin.Size = new System.Drawing.Size(319, 23);
            this.chkKeyboardPlugin.TabIndex = 3;
            this.chkKeyboardPlugin.Text = "Enable Keyboard Plugin (Keyboard Automation)";
            this.chkKeyboardPlugin.UseVisualStyleBackColor = true;
            this.chkKeyboardPlugin.CheckedChanged += new System.EventHandler(this.chkPluginStatus_CheckedChanged);
            // 
            // chkScreenCapturePlugin
            // 
            this.chkScreenCapturePlugin.AutoSize = true;
            this.chkScreenCapturePlugin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkScreenCapturePlugin.Location = new System.Drawing.Point(17, 84);
            this.chkScreenCapturePlugin.Name = "chkScreenCapturePlugin";
            this.chkScreenCapturePlugin.Size = new System.Drawing.Size(292, 23);
            this.chkScreenCapturePlugin.TabIndex = 2;
            this.chkScreenCapturePlugin.Text = "Enable Screen Capture Plugin (Screenshots)";
            this.chkScreenCapturePlugin.UseVisualStyleBackColor = true;
            this.chkScreenCapturePlugin.CheckedChanged += new System.EventHandler(this.chkPluginStatus_CheckedChanged);
            // 
            // chkPowerShellPlugin
            // 
            this.chkPowerShellPlugin.AutoSize = true;
            this.chkPowerShellPlugin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPowerShellPlugin.Location = new System.Drawing.Point(17, 55);
            this.chkPowerShellPlugin.Name = "chkPowerShellPlugin";
            this.chkPowerShellPlugin.Size = new System.Drawing.Size(330, 23);
            this.chkPowerShellPlugin.TabIndex = 1;
            this.chkPowerShellPlugin.Text = "Enable PowerShell Plugin (PowerShell Commands)";
            this.chkPowerShellPlugin.UseVisualStyleBackColor = true;
            this.chkPowerShellPlugin.CheckedChanged += new System.EventHandler(this.chkPluginStatus_CheckedChanged);
            // 
            // chkCMDPlugin
            // 
            this.chkCMDPlugin.AutoSize = true;
            this.chkCMDPlugin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCMDPlugin.Location = new System.Drawing.Point(17, 26);
            this.chkCMDPlugin.Name = "chkCMDPlugin";
            this.chkCMDPlugin.Size = new System.Drawing.Size(272, 23);
            this.chkCMDPlugin.TabIndex = 0;
            this.chkCMDPlugin.Text = "Enable CMD Plugin (Command Prompt)";
            this.chkCMDPlugin.UseVisualStyleBackColor = true;
            this.chkCMDPlugin.CheckedChanged += new System.EventHandler(this.chkPluginStatus_CheckedChanged);
            // 
            // chkWindowSelectionPlugin
            // 
            this.chkWindowSelectionPlugin.AutoSize = true;
            this.chkWindowSelectionPlugin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkWindowSelectionPlugin.Location = new System.Drawing.Point(17, 171);
            this.chkWindowSelectionPlugin.Name = "chkWindowSelectionPlugin";
            this.chkWindowSelectionPlugin.Size = new System.Drawing.Size(337, 23);
            this.chkWindowSelectionPlugin.TabIndex = 5;
            this.chkWindowSelectionPlugin.Text = "Enable Window Selection Plugin (Window Handles)";
            this.chkWindowSelectionPlugin.UseVisualStyleBackColor = true;
            this.chkWindowSelectionPlugin.CheckedChanged += new System.EventHandler(this.chkPluginStatus_CheckedChanged);
            // 
            // chkPlaywrightPlugin
            // 
            this.chkPlaywrightPlugin.AutoSize = true;
            this.chkPlaywrightPlugin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPlaywrightPlugin.Location = new System.Drawing.Point(17, 200);
            this.chkPlaywrightPlugin.Name = "chkPlaywrightPlugin";
            this.chkPlaywrightPlugin.Size = new System.Drawing.Size(196, 23);
            this.chkPlaywrightPlugin.TabIndex = 6;
            this.chkPlaywrightPlugin.Text = "Playwright Browser Plugin";
            this.chkPlaywrightPlugin.UseVisualStyleBackColor = true;
            this.chkPlaywrightPlugin.CheckedChanged += new System.EventHandler(this.chkPluginStatus_CheckedChanged);
            // 
            // enablePluginLoggingCheckBox
            // 
            this.enablePluginLoggingCheckBox.AutoSize = true;
            this.enablePluginLoggingCheckBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enablePluginLoggingCheckBox.Location = new System.Drawing.Point(23, 252);
            this.enablePluginLoggingCheckBox.Name = "enablePluginLoggingCheckBox";
            this.enablePluginLoggingCheckBox.Size = new System.Drawing.Size(135, 23);
            this.enablePluginLoggingCheckBox.TabIndex = 7;
            this.enablePluginLoggingCheckBox.Text = "Log Plugin Usage";
            this.enablePluginLoggingCheckBox.UseVisualStyleBackColor = true;
            //
            // chkEnableRemoteControl
            //
            this.chkEnableRemoteControl.AutoSize = true;
            this.chkEnableRemoteControl.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnableRemoteControl.Location = new System.Drawing.Point(23, 281);
            this.chkEnableRemoteControl.Name = "chkEnableRemoteControl";
            this.chkEnableRemoteControl.Size = new System.Drawing.Size(171, 23);
            this.chkEnableRemoteControl.TabIndex = 8;
            this.chkEnableRemoteControl.Text = "Enable Remote Control";
            this.chkEnableRemoteControl.UseVisualStyleBackColor = true;
            //
            // lblRemotePort
            //
            this.lblRemotePort.AutoSize = true;
            this.lblRemotePort.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRemotePort.Location = new System.Drawing.Point(40, 310);
            this.lblRemotePort.Name = "lblRemotePort";
            this.lblRemotePort.Size = new System.Drawing.Size(82, 19);
            this.lblRemotePort.TabIndex = 9;
            this.lblRemotePort.Text = "Listen Port:";
            //
            // numRemotePort
            //
            this.numRemotePort.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numRemotePort.Location = new System.Drawing.Point(128, 308);
            this.numRemotePort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numRemotePort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numRemotePort.Name = "numRemotePort";
            this.numRemotePort.Size = new System.Drawing.Size(80, 25);
            this.numRemotePort.TabIndex = 10;
            this.numRemotePort.Value = new decimal(new int[] {
            8085,
            0,
            0,
            0});
            //
            // groupBoxSettings
            //
            this.groupBoxSettings.Controls.Add(this.indicatorAutoInvoke);
            this.groupBoxSettings.Controls.Add(this.indicatorMultiAgent);
            this.groupBoxSettings.Controls.Add(this.chkMultiAgentMode);
            this.groupBoxSettings.Controls.Add(this.chkAutoInvoke);
            this.groupBoxSettings.Controls.Add(this.chkRetainChatHistory);
            this.groupBoxSettings.Controls.Add(this.numTemperature);
            this.groupBoxSettings.Controls.Add(this.lblTemperature);
            this.groupBoxSettings.Controls.Add(this.chkDynamicToolPrompts);
            this.groupBoxSettings.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSettings.Location = new System.Drawing.Point(6, 6);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(436, 230);
            this.groupBoxSettings.TabIndex = 1;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "AI Settings";
            // 
            // indicatorAutoInvoke
            // 
            this.indicatorAutoInvoke.BackColor = System.Drawing.Color.Transparent;
            this.indicatorAutoInvoke.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indicatorAutoInvoke.Location = new System.Drawing.Point(390, 59);
            this.indicatorAutoInvoke.Name = "indicatorAutoInvoke";
            this.indicatorAutoInvoke.Size = new System.Drawing.Size(24, 24);
            this.indicatorAutoInvoke.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.indicatorAutoInvoke.TabIndex = 12;
            this.indicatorAutoInvoke.TabStop = false;
            // 
            // indicatorMultiAgent
            // 
            this.indicatorMultiAgent.BackColor = System.Drawing.Color.Transparent;
            this.indicatorMultiAgent.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indicatorMultiAgent.Location = new System.Drawing.Point(390, 117);
            this.indicatorMultiAgent.Name = "indicatorMultiAgent";
            this.indicatorMultiAgent.Size = new System.Drawing.Size(24, 24);
            this.indicatorMultiAgent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.indicatorMultiAgent.TabIndex = 5;
            this.indicatorMultiAgent.TabStop = false;
            // 
            // chkMultiAgentMode
            // 
            this.chkMultiAgentMode.AutoSize = true;
            this.chkMultiAgentMode.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkMultiAgentMode.Location = new System.Drawing.Point(17, 117);
            this.chkMultiAgentMode.Name = "chkMultiAgentMode";
            this.chkMultiAgentMode.Size = new System.Drawing.Size(187, 23);
            this.chkMultiAgentMode.TabIndex = 4;
            this.chkMultiAgentMode.Text = "Enable Multi-Agent Mode";
            this.chkMultiAgentMode.UseVisualStyleBackColor = true;
            this.chkMultiAgentMode.CheckedChanged += new System.EventHandler(this.chkMultiAgentMode_CheckedChanged);
            // 
            // chkAutoInvoke
            // 
            this.chkAutoInvoke.AutoSize = true;
            this.chkAutoInvoke.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAutoInvoke.Location = new System.Drawing.Point(17, 59);
            this.chkAutoInvoke.Name = "chkAutoInvoke";
            this.chkAutoInvoke.Size = new System.Drawing.Size(385, 23);
            this.chkAutoInvoke.TabIndex = 2;
            this.chkAutoInvoke.Text = "Auto-Invoke Functions (Let AI execute tools automatically)";
            this.chkAutoInvoke.UseVisualStyleBackColor = true;
            this.chkAutoInvoke.CheckedChanged += new System.EventHandler(this.chkPluginStatus_CheckedChanged);
            // 
            // chkRetainChatHistory
            // 
            this.chkRetainChatHistory.AutoSize = true;
            this.chkRetainChatHistory.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRetainChatHistory.Location = new System.Drawing.Point(17, 88);
            this.chkRetainChatHistory.Name = "chkRetainChatHistory";
            this.chkRetainChatHistory.Size = new System.Drawing.Size(392, 23);
            this.chkRetainChatHistory.TabIndex = 3;
            this.chkRetainChatHistory.Text = "Retain Chat History (Keep chat messages between sessions)";
            this.chkRetainChatHistory.UseVisualStyleBackColor = true;
            // 
            // numTemperature
            // 
            this.numTemperature.DecimalPlaces = 1;
            this.numTemperature.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.numTemperature.Size = new System.Drawing.Size(60, 25);
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
            this.lblTemperature.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTemperature.Location = new System.Drawing.Point(14, 29);
            this.lblTemperature.Name = "lblTemperature";
            this.lblTemperature.Size = new System.Drawing.Size(179, 19);
            this.lblTemperature.TabIndex = 0;
            this.lblTemperature.Text = "Temperature (Randomness):";
            // 
            // chkDynamicToolPrompts
            // 
            this.chkDynamicToolPrompts.AutoSize = true;
            this.chkDynamicToolPrompts.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDynamicToolPrompts.Location = new System.Drawing.Point(17, 200);
            this.chkDynamicToolPrompts.Name = "chkDynamicToolPrompts";
            this.chkDynamicToolPrompts.Size = new System.Drawing.Size(413, 23);
            this.chkDynamicToolPrompts.TabIndex = 7;
            this.chkDynamicToolPrompts.Text = "Dynamically update system prompt with tool descriptions";
            this.chkDynamicToolPrompts.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.Location = new System.Drawing.Point(452, 450);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(98, 30);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(556, 450);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(98, 30);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click_1);
            // 
            // groupBoxSpeechRecognition
            // 
            this.groupBoxSpeechRecognition.Controls.Add(this.indicatorSpeech);
            this.groupBoxSpeechRecognition.Controls.Add(this.comboSpeechLanguage);
            this.groupBoxSpeechRecognition.Controls.Add(this.lblSpeechLanguage);
            this.groupBoxSpeechRecognition.Controls.Add(this.chkEnableSpeechRecognition);
            this.groupBoxSpeechRecognition.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSpeechRecognition.Location = new System.Drawing.Point(6, 6);
            this.groupBoxSpeechRecognition.Name = "groupBoxSpeechRecognition";
            this.groupBoxSpeechRecognition.Size = new System.Drawing.Size(436, 92);
            this.groupBoxSpeechRecognition.TabIndex = 5;
            this.groupBoxSpeechRecognition.TabStop = false;
            this.groupBoxSpeechRecognition.Text = "Speech Recognition";
            // 
            // indicatorSpeech
            // 
            this.indicatorSpeech.BackColor = System.Drawing.Color.Transparent;
            this.indicatorSpeech.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indicatorSpeech.Location = new System.Drawing.Point(390, 29);
            this.indicatorSpeech.Name = "indicatorSpeech";
            this.indicatorSpeech.Size = new System.Drawing.Size(24, 24);
            this.indicatorSpeech.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.indicatorSpeech.TabIndex = 13;
            this.indicatorSpeech.TabStop = false;
            // 
            // comboSpeechLanguage
            // 
            this.comboSpeechLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSpeechLanguage.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.comboSpeechLanguage.Size = new System.Drawing.Size(239, 25);
            this.comboSpeechLanguage.TabIndex = 2;
            // 
            // lblSpeechLanguage
            // 
            this.lblSpeechLanguage.AutoSize = true;
            this.lblSpeechLanguage.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpeechLanguage.Location = new System.Drawing.Point(14, 59);
            this.lblSpeechLanguage.Name = "lblSpeechLanguage";
            this.lblSpeechLanguage.Size = new System.Drawing.Size(148, 19);
            this.lblSpeechLanguage.TabIndex = 1;
            this.lblSpeechLanguage.Text = "Recognition Language:";
            // 
            // chkEnableSpeechRecognition
            // 
            this.chkEnableSpeechRecognition.AutoSize = true;
            this.chkEnableSpeechRecognition.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnableSpeechRecognition.Location = new System.Drawing.Point(17, 29);
            this.chkEnableSpeechRecognition.Name = "chkEnableSpeechRecognition";
            this.chkEnableSpeechRecognition.Size = new System.Drawing.Size(191, 23);
            this.chkEnableSpeechRecognition.TabIndex = 0;
            this.chkEnableSpeechRecognition.Text = "Enable Speech Recognition";
            this.chkEnableSpeechRecognition.UseVisualStyleBackColor = true;
            this.chkEnableSpeechRecognition.CheckedChanged += new System.EventHandler(this.chkPluginStatus_CheckedChanged);
            // 
            // groupBoxVoiceCommands
            // 
            this.groupBoxVoiceCommands.Controls.Add(this.indicatorVoiceCmd);
            this.groupBoxVoiceCommands.Controls.Add(this.txtVoiceCommandPhrase);
            this.groupBoxVoiceCommands.Controls.Add(this.lblVoiceCommandPhrase);
            this.groupBoxVoiceCommands.Controls.Add(this.chkEnableVoiceCommands);
            this.groupBoxVoiceCommands.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxVoiceCommands.Location = new System.Drawing.Point(6, 104);
            this.groupBoxVoiceCommands.Name = "groupBoxVoiceCommands";
            this.groupBoxVoiceCommands.Size = new System.Drawing.Size(436, 88);
            this.groupBoxVoiceCommands.TabIndex = 6;
            this.groupBoxVoiceCommands.TabStop = false;
            this.groupBoxVoiceCommands.Text = "Voice Commands";
            // 
            // indicatorVoiceCmd
            // 
            this.indicatorVoiceCmd.BackColor = System.Drawing.Color.Transparent;
            this.indicatorVoiceCmd.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indicatorVoiceCmd.Location = new System.Drawing.Point(390, 29);
            this.indicatorVoiceCmd.Name = "indicatorVoiceCmd";
            this.indicatorVoiceCmd.Size = new System.Drawing.Size(24, 24);
            this.indicatorVoiceCmd.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.indicatorVoiceCmd.TabIndex = 14;
            this.indicatorVoiceCmd.TabStop = false;
            // 
            // txtVoiceCommandPhrase
            // 
            this.txtVoiceCommandPhrase.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVoiceCommandPhrase.Location = new System.Drawing.Point(180, 56);
            this.txtVoiceCommandPhrase.Name = "txtVoiceCommandPhrase";
            this.txtVoiceCommandPhrase.Size = new System.Drawing.Size(239, 25);
            this.txtVoiceCommandPhrase.TabIndex = 2;
            // 
            // lblVoiceCommandPhrase
            // 
            this.lblVoiceCommandPhrase.AutoSize = true;
            this.lblVoiceCommandPhrase.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVoiceCommandPhrase.Location = new System.Drawing.Point(14, 59);
            this.lblVoiceCommandPhrase.Name = "lblVoiceCommandPhrase";
            this.lblVoiceCommandPhrase.Size = new System.Drawing.Size(157, 19);
            this.lblVoiceCommandPhrase.TabIndex = 1;
            this.lblVoiceCommandPhrase.Text = "Voice Command Phrase:";
            // 
            // chkEnableVoiceCommands
            // 
            this.chkEnableVoiceCommands.AutoSize = true;
            this.chkEnableVoiceCommands.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnableVoiceCommands.Location = new System.Drawing.Point(17, 29);
            this.chkEnableVoiceCommands.Name = "chkEnableVoiceCommands";
            this.chkEnableVoiceCommands.Size = new System.Drawing.Size(178, 23);
            this.chkEnableVoiceCommands.TabIndex = 0;
            this.chkEnableVoiceCommands.Text = "Enable Voice Commands";
            this.chkEnableVoiceCommands.UseVisualStyleBackColor = true;
            this.chkEnableVoiceCommands.CheckedChanged += new System.EventHandler(this.chkPluginStatus_CheckedChanged);
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPlugins);
            this.tabControlMain.Controls.Add(this.tabAISettings);
            this.tabControlMain.Controls.Add(this.tabVoice);
            this.tabControlMain.Controls.Add(this.tabCoordinator);
            this.tabControlMain.Controls.Add(this.tabPlanner);
            this.tabControlMain.Controls.Add(this.tabActioner);
            this.tabControlMain.Controls.Add(this.tabAppearance);
            this.tabControlMain.Controls.Add(this.tabProfiles);
            this.tabControlMain.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlMain.Location = new System.Drawing.Point(12, 45);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(642, 393);
            this.tabControlMain.TabIndex = 8;
            // 
            // tabPlugins
            // 
            this.tabPlugins.Controls.Add(this.groupBoxPlugins);
            this.tabPlugins.Controls.Add(this.enablePluginLoggingCheckBox);
            this.tabPlugins.Controls.Add(this.chkEnableRemoteControl);
            this.tabPlugins.Controls.Add(this.lblRemotePort);
            this.tabPlugins.Controls.Add(this.numRemotePort);
            this.tabPlugins.Location = new System.Drawing.Point(4, 26);
            this.tabPlugins.Name = "tabPlugins";
            this.tabPlugins.Padding = new System.Windows.Forms.Padding(3);
            this.tabPlugins.Size = new System.Drawing.Size(634, 363);
            this.tabPlugins.TabIndex = 0;
            this.tabPlugins.Text = "Plugins";
            this.tabPlugins.UseVisualStyleBackColor = true;
            // 
            // tabAISettings
            // 
            this.tabAISettings.Controls.Add(this.groupBoxSettings);
            this.tabAISettings.Location = new System.Drawing.Point(4, 26);
            this.tabAISettings.Name = "tabAISettings";
            this.tabAISettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabAISettings.Size = new System.Drawing.Size(634, 363);
            this.tabAISettings.TabIndex = 1;
            this.tabAISettings.Text = "AI Settings";
            this.tabAISettings.UseVisualStyleBackColor = true;
            // 
            // tabVoice
            // 
            this.tabVoice.Controls.Add(this.groupBoxSpeechRecognition);
            this.tabVoice.Controls.Add(this.groupBoxVoiceCommands);
            this.tabVoice.Location = new System.Drawing.Point(4, 26);
            this.tabVoice.Name = "tabVoice";
            this.tabVoice.Size = new System.Drawing.Size(634, 363);
            this.tabVoice.TabIndex = 2;
            this.tabVoice.Text = "Voice";
            this.tabVoice.UseVisualStyleBackColor = true;
            // 
            // tabCoordinator
            // 
            this.tabCoordinator.Controls.Add(this.grpCoordinatorConfig);
            this.tabCoordinator.Location = new System.Drawing.Point(4, 26);
            this.tabCoordinator.Name = "tabCoordinator";
            this.tabCoordinator.Size = new System.Drawing.Size(634, 363);
            this.tabCoordinator.TabIndex = 3;
            this.tabCoordinator.Text = "Coordinator";
            this.tabCoordinator.UseVisualStyleBackColor = true;
            // 
            // grpCoordinatorConfig
            // 
            this.grpCoordinatorConfig.Controls.Add(this.lblCoordinatorPrompt);
            this.grpCoordinatorConfig.Controls.Add(this.txtCoordinatorSystemPrompt);
            this.grpCoordinatorConfig.Controls.Add(this.chkUseCustomCoordinatorConfig);
            this.grpCoordinatorConfig.Controls.Add(this.comboCoordinatorConfig);
            this.grpCoordinatorConfig.Controls.Add(this.btnConfigureCoordinator);
            this.grpCoordinatorConfig.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpCoordinatorConfig.Location = new System.Drawing.Point(6, 6);
            this.grpCoordinatorConfig.Name = "grpCoordinatorConfig";
            this.grpCoordinatorConfig.Size = new System.Drawing.Size(577, 351);
            this.grpCoordinatorConfig.TabIndex = 15;
            this.grpCoordinatorConfig.TabStop = false;
            this.grpCoordinatorConfig.Text = "Coordinator Configuration";
            // 
            // lblCoordinatorPrompt
            // 
            this.lblCoordinatorPrompt.AutoSize = true;
            this.lblCoordinatorPrompt.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCoordinatorPrompt.Location = new System.Drawing.Point(8, 24);
            this.lblCoordinatorPrompt.Name = "lblCoordinatorPrompt";
            this.lblCoordinatorPrompt.Size = new System.Drawing.Size(177, 19);
            this.lblCoordinatorPrompt.TabIndex = 16;
            this.lblCoordinatorPrompt.Text = "Coordinator System Prompt:";
            // 
            // txtCoordinatorSystemPrompt
            // 
            this.txtCoordinatorSystemPrompt.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCoordinatorSystemPrompt.Location = new System.Drawing.Point(8, 46);
            this.txtCoordinatorSystemPrompt.Multiline = true;
            this.txtCoordinatorSystemPrompt.Name = "txtCoordinatorSystemPrompt";
            this.txtCoordinatorSystemPrompt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCoordinatorSystemPrompt.Size = new System.Drawing.Size(555, 200);
            this.txtCoordinatorSystemPrompt.TabIndex = 17;
            // 
            // chkUseCustomCoordinatorConfig
            // 
            this.chkUseCustomCoordinatorConfig.AutoSize = true;
            this.chkUseCustomCoordinatorConfig.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUseCustomCoordinatorConfig.Location = new System.Drawing.Point(8, 284);
            this.chkUseCustomCoordinatorConfig.Name = "chkUseCustomCoordinatorConfig";
            this.chkUseCustomCoordinatorConfig.Size = new System.Drawing.Size(261, 23);
            this.chkUseCustomCoordinatorConfig.TabIndex = 20;
            this.chkUseCustomCoordinatorConfig.Text = "Use Custom Coordinator Model Config";
            this.chkUseCustomCoordinatorConfig.UseVisualStyleBackColor = true;
            // 
            // comboCoordinatorConfig
            // 
            this.comboCoordinatorConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCoordinatorConfig.Enabled = false;
            this.comboCoordinatorConfig.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboCoordinatorConfig.FormattingEnabled = true;
            this.comboCoordinatorConfig.Location = new System.Drawing.Point(275, 284);
            this.comboCoordinatorConfig.Name = "comboCoordinatorConfig";
            this.comboCoordinatorConfig.Size = new System.Drawing.Size(181, 25);
            this.comboCoordinatorConfig.TabIndex = 21;
            // 
            // btnConfigureCoordinator
            // 
            this.btnConfigureCoordinator.Enabled = false;
            this.btnConfigureCoordinator.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfigureCoordinator.Location = new System.Drawing.Point(463, 284);
            this.btnConfigureCoordinator.Name = "btnConfigureCoordinator";
            this.btnConfigureCoordinator.Size = new System.Drawing.Size(100, 30);
            this.btnConfigureCoordinator.TabIndex = 22;
            this.btnConfigureCoordinator.Text = "Configure";
            this.btnConfigureCoordinator.UseVisualStyleBackColor = true;
            // 
            // tabPlanner
            // 
            this.tabPlanner.Controls.Add(this.grpPlannerConfig);
            this.tabPlanner.Location = new System.Drawing.Point(4, 26);
            this.tabPlanner.Name = "tabPlanner";
            this.tabPlanner.Size = new System.Drawing.Size(634, 363);
            this.tabPlanner.TabIndex = 6;
            this.tabPlanner.Text = "Planner";
            this.tabPlanner.UseVisualStyleBackColor = true;
            // 
            // grpPlannerConfig
            // 
            this.grpPlannerConfig.Controls.Add(this.lblPlannerPrompt);
            this.grpPlannerConfig.Controls.Add(this.txtPlannerSystemPrompt);
            this.grpPlannerConfig.Controls.Add(this.chkUseCustomPlannerConfig);
            this.grpPlannerConfig.Controls.Add(this.comboPlannerConfig);
            this.grpPlannerConfig.Controls.Add(this.btnConfigurePlanner);
            this.grpPlannerConfig.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpPlannerConfig.Location = new System.Drawing.Point(6, 6);
            this.grpPlannerConfig.Name = "grpPlannerConfig";
            this.grpPlannerConfig.Size = new System.Drawing.Size(577, 351);
            this.grpPlannerConfig.TabIndex = 15;
            this.grpPlannerConfig.TabStop = false;
            this.grpPlannerConfig.Text = "Planner Configuration";
            // 
            // lblPlannerPrompt
            // 
            this.lblPlannerPrompt.AutoSize = true;
            this.lblPlannerPrompt.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlannerPrompt.Location = new System.Drawing.Point(8, 24);
            this.lblPlannerPrompt.Name = "lblPlannerPrompt";
            this.lblPlannerPrompt.Size = new System.Drawing.Size(156, 19);
            this.lblPlannerPrompt.TabIndex = 16;
            this.lblPlannerPrompt.Text = "Planner System Prompt:";
            // 
            // txtPlannerSystemPrompt
            // 
            this.txtPlannerSystemPrompt.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPlannerSystemPrompt.Location = new System.Drawing.Point(8, 46);
            this.txtPlannerSystemPrompt.Multiline = true;
            this.txtPlannerSystemPrompt.Name = "txtPlannerSystemPrompt";
            this.txtPlannerSystemPrompt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPlannerSystemPrompt.Size = new System.Drawing.Size(555, 200);
            this.txtPlannerSystemPrompt.TabIndex = 17;
            // 
            // chkUseCustomPlannerConfig
            // 
            this.chkUseCustomPlannerConfig.AutoSize = true;
            this.chkUseCustomPlannerConfig.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUseCustomPlannerConfig.Location = new System.Drawing.Point(8, 284);
            this.chkUseCustomPlannerConfig.Name = "chkUseCustomPlannerConfig";
            this.chkUseCustomPlannerConfig.Size = new System.Drawing.Size(240, 23);
            this.chkUseCustomPlannerConfig.TabIndex = 20;
            this.chkUseCustomPlannerConfig.Text = "Use Custom Planner Model Config";
            this.chkUseCustomPlannerConfig.UseVisualStyleBackColor = true;
            this.chkUseCustomPlannerConfig.CheckedChanged += new System.EventHandler(this.chkUseCustomPlannerConfig_CheckedChanged);
            // 
            // comboPlannerConfig
            // 
            this.comboPlannerConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPlannerConfig.Enabled = false;
            this.comboPlannerConfig.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboPlannerConfig.FormattingEnabled = true;
            this.comboPlannerConfig.Location = new System.Drawing.Point(275, 284);
            this.comboPlannerConfig.Name = "comboPlannerConfig";
            this.comboPlannerConfig.Size = new System.Drawing.Size(181, 25);
            this.comboPlannerConfig.TabIndex = 21;
            // 
            // btnConfigurePlanner
            // 
            this.btnConfigurePlanner.Enabled = false;
            this.btnConfigurePlanner.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfigurePlanner.Location = new System.Drawing.Point(463, 284);
            this.btnConfigurePlanner.Name = "btnConfigurePlanner";
            this.btnConfigurePlanner.Size = new System.Drawing.Size(100, 30);
            this.btnConfigurePlanner.TabIndex = 22;
            this.btnConfigurePlanner.Text = "Configure";
            this.btnConfigurePlanner.UseVisualStyleBackColor = true;
            this.btnConfigurePlanner.Click += new System.EventHandler(this.btnConfigurePlanner_Click);
            // 
            // tabActioner
            // 
            this.tabActioner.Controls.Add(this.grpActionerConfig);
            this.tabActioner.Location = new System.Drawing.Point(4, 26);
            this.tabActioner.Name = "tabActioner";
            this.tabActioner.Size = new System.Drawing.Size(634, 363);
            this.tabActioner.TabIndex = 7;
            this.tabActioner.Text = "Actioner";
            this.tabActioner.UseVisualStyleBackColor = true;
            // 
            // grpActionerConfig
            // 
            this.grpActionerConfig.Controls.Add(this.lblActionerPrompt);
            this.grpActionerConfig.Controls.Add(this.txtActionerSystemPrompt);
            this.grpActionerConfig.Controls.Add(this.chkUseCustomExecutorConfig);
            this.grpActionerConfig.Controls.Add(this.comboActionerConfig);
            this.grpActionerConfig.Controls.Add(this.btnConfigureActioner);
            this.grpActionerConfig.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpActionerConfig.Location = new System.Drawing.Point(6, 6);
            this.grpActionerConfig.Name = "grpActionerConfig";
            this.grpActionerConfig.Size = new System.Drawing.Size(577, 351);
            this.grpActionerConfig.TabIndex = 15;
            this.grpActionerConfig.TabStop = false;
            this.grpActionerConfig.Text = "Actioner Configuration";
            // 
            // lblActionerPrompt
            // 
            this.lblActionerPrompt.AutoSize = true;
            this.lblActionerPrompt.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActionerPrompt.Location = new System.Drawing.Point(8, 24);
            this.lblActionerPrompt.Name = "lblActionerPrompt";
            this.lblActionerPrompt.Size = new System.Drawing.Size(477, 19);
            this.lblActionerPrompt.TabIndex = 18;
            this.lblActionerPrompt.Text = "Actioner System Prompt (also used as system prompt in single agent mode):";
            // 
            // txtActionerSystemPrompt
            // 
            this.txtActionerSystemPrompt.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtActionerSystemPrompt.Location = new System.Drawing.Point(8, 46);
            this.txtActionerSystemPrompt.Multiline = true;
            this.txtActionerSystemPrompt.Name = "txtActionerSystemPrompt";
            this.txtActionerSystemPrompt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtActionerSystemPrompt.Size = new System.Drawing.Size(555, 200);
            this.txtActionerSystemPrompt.TabIndex = 19;
            // 
            // chkUseCustomExecutorConfig
            // 
            this.chkUseCustomExecutorConfig.AutoSize = true;
            this.chkUseCustomExecutorConfig.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUseCustomExecutorConfig.Location = new System.Drawing.Point(8, 284);
            this.chkUseCustomExecutorConfig.Name = "chkUseCustomExecutorConfig";
            this.chkUseCustomExecutorConfig.Size = new System.Drawing.Size(246, 23);
            this.chkUseCustomExecutorConfig.TabIndex = 23;
            this.chkUseCustomExecutorConfig.Text = "Use Custom Actioner Model Config";
            this.chkUseCustomExecutorConfig.UseVisualStyleBackColor = true;
            this.chkUseCustomExecutorConfig.CheckedChanged += new System.EventHandler(this.chkUseCustomExecutorConfig_CheckedChanged);
            // 
            // comboActionerConfig
            // 
            this.comboActionerConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboActionerConfig.Enabled = false;
            this.comboActionerConfig.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboActionerConfig.FormattingEnabled = true;
            this.comboActionerConfig.Location = new System.Drawing.Point(275, 284);
            this.comboActionerConfig.Name = "comboActionerConfig";
            this.comboActionerConfig.Size = new System.Drawing.Size(181, 25);
            this.comboActionerConfig.TabIndex = 24;
            // 
            // btnConfigureActioner
            // 
            this.btnConfigureActioner.Enabled = false;
            this.btnConfigureActioner.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfigureActioner.Location = new System.Drawing.Point(463, 284);
            this.btnConfigureActioner.Name = "btnConfigureActioner";
            this.btnConfigureActioner.Size = new System.Drawing.Size(100, 30);
            this.btnConfigureActioner.TabIndex = 25;
            this.btnConfigureActioner.Text = "Configure";
            this.btnConfigureActioner.UseVisualStyleBackColor = true;
            this.btnConfigureActioner.Click += new System.EventHandler(this.btnConfigureActioner_Click);
            // 
            // tabAppearance
            // 
            this.tabAppearance.Controls.Add(this.groupBoxTheme);
            this.tabAppearance.Location = new System.Drawing.Point(4, 26);
            this.tabAppearance.Name = "tabAppearance";
            this.tabAppearance.Size = new System.Drawing.Size(634, 363);
            this.tabAppearance.TabIndex = 4;
            this.tabAppearance.Text = "Appearance";
            this.tabAppearance.UseVisualStyleBackColor = true;
            // 
            // groupBoxTheme
            // 
            this.groupBoxTheme.Controls.Add(this.labelTheme);
            this.groupBoxTheme.Controls.Add(this.cbTheme);
            this.groupBoxTheme.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxTheme.Location = new System.Drawing.Point(6, 6);
            this.groupBoxTheme.Name = "groupBoxTheme";
            this.groupBoxTheme.Size = new System.Drawing.Size(436, 75);
            this.groupBoxTheme.TabIndex = 0;
            this.groupBoxTheme.TabStop = false;
            this.groupBoxTheme.Text = "Theme Settings";
            // 
            // labelTheme
            // 
            this.labelTheme.AutoSize = true;
            this.labelTheme.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTheme.Location = new System.Drawing.Point(14, 34);
            this.labelTheme.Name = "labelTheme";
            this.labelTheme.Size = new System.Drawing.Size(104, 19);
            this.labelTheme.TabIndex = 1;
            this.labelTheme.Text = "Current Theme:";
            // 
            // cbTheme
            // 
            this.cbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTheme.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbTheme.FormattingEnabled = true;
            this.cbTheme.Items.AddRange(new object[] {
            "Light",
            "Dark"});
            this.cbTheme.Location = new System.Drawing.Point(122, 31);
            this.cbTheme.Name = "cbTheme";
            this.cbTheme.Size = new System.Drawing.Size(150, 25);
            this.cbTheme.TabIndex = 0;
            this.cbTheme.SelectedIndexChanged += new System.EventHandler(this.cbTheme_SelectedIndexChanged);
            // 
            // tabProfiles
            // 
            this.tabProfiles.Controls.Add(this.groupBoxProfiles);
            this.tabProfiles.Location = new System.Drawing.Point(4, 26);
            this.tabProfiles.Name = "tabProfiles";
            this.tabProfiles.Size = new System.Drawing.Size(634, 363);
            this.tabProfiles.TabIndex = 5;
            this.tabProfiles.Text = "Profiles";
            this.tabProfiles.UseVisualStyleBackColor = true;
            // 
            // groupBoxProfiles
            // 
            this.groupBoxProfiles.Controls.Add(this.btnDeleteProfile);
            this.groupBoxProfiles.Controls.Add(this.btnLoadProfile);
            this.groupBoxProfiles.Controls.Add(this.btnSaveProfile);
            this.groupBoxProfiles.Controls.Add(this.labelProfile);
            this.groupBoxProfiles.Controls.Add(this.cmbProfiles);
            this.groupBoxProfiles.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxProfiles.Location = new System.Drawing.Point(6, 6);
            this.groupBoxProfiles.Name = "groupBoxProfiles";
            this.groupBoxProfiles.Size = new System.Drawing.Size(436, 136);
            this.groupBoxProfiles.TabIndex = 0;
            this.groupBoxProfiles.TabStop = false;
            this.groupBoxProfiles.Text = "Configuration Profiles";
            // 
            // btnDeleteProfile
            // 
            this.btnDeleteProfile.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDeleteProfile.Location = new System.Drawing.Point(297, 92);
            this.btnDeleteProfile.Name = "btnDeleteProfile";
            this.btnDeleteProfile.Size = new System.Drawing.Size(121, 30);
            this.btnDeleteProfile.TabIndex = 4;
            this.btnDeleteProfile.Text = "Delete Profile";
            this.btnDeleteProfile.UseVisualStyleBackColor = true;
            this.btnDeleteProfile.Click += new System.EventHandler(this.btnDeleteProfile_Click);
            // 
            // btnLoadProfile
            // 
            this.btnLoadProfile.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnLoadProfile.Location = new System.Drawing.Point(156, 92);
            this.btnLoadProfile.Name = "btnLoadProfile";
            this.btnLoadProfile.Size = new System.Drawing.Size(121, 30);
            this.btnLoadProfile.TabIndex = 3;
            this.btnLoadProfile.Text = "Load Profile";
            this.btnLoadProfile.UseVisualStyleBackColor = true;
            this.btnLoadProfile.Click += new System.EventHandler(this.btnLoadProfile_Click);
            // 
            // btnSaveProfile
            // 
            this.btnSaveProfile.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnSaveProfile.Location = new System.Drawing.Point(17, 92);
            this.btnSaveProfile.Name = "btnSaveProfile";
            this.btnSaveProfile.Size = new System.Drawing.Size(121, 30);
            this.btnSaveProfile.TabIndex = 2;
            this.btnSaveProfile.Text = "Save As New...";
            this.btnSaveProfile.UseVisualStyleBackColor = true;
            this.btnSaveProfile.Click += new System.EventHandler(this.btnSaveProfile_Click);
            // 
            // labelProfile
            // 
            this.labelProfile.AutoSize = true;
            this.labelProfile.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProfile.Location = new System.Drawing.Point(14, 44);
            this.labelProfile.Name = "labelProfile";
            this.labelProfile.Size = new System.Drawing.Size(89, 19);
            this.labelProfile.TabIndex = 1;
            this.labelProfile.Text = "Select Profile:";
            // 
            // cmbProfiles
            // 
            this.cmbProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProfiles.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbProfiles.FormattingEnabled = true;
            this.cmbProfiles.Location = new System.Drawing.Point(122, 41);
            this.cmbProfiles.Name = "cmbProfiles";
            this.cmbProfiles.Size = new System.Drawing.Size(296, 25);
            this.cmbProfiles.TabIndex = 0;
            // 
            // imageListStatus
            // 
            this.imageListStatus.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListStatus.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListStatus.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // panelSearch
            // 
            this.panelSearch.Controls.Add(this.searchResultLabel);
            this.panelSearch.Controls.Add(this.txtSearchSettings);
            this.panelSearch.Controls.Add(this.lblSearchSettings);
            this.panelSearch.Location = new System.Drawing.Point(12, 8);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(642, 31);
            this.panelSearch.TabIndex = 9;
            // 
            // searchResultLabel
            // 
            this.searchResultLabel.AutoSize = true;
            this.searchResultLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.searchResultLabel.ForeColor = System.Drawing.Color.RoyalBlue;
            this.searchResultLabel.Location = new System.Drawing.Point(460, 8);
            this.searchResultLabel.Name = "searchResultLabel";
            this.searchResultLabel.Size = new System.Drawing.Size(0, 15);
            this.searchResultLabel.TabIndex = 2;
            this.searchResultLabel.Visible = false;
            // 
            // txtSearchSettings
            // 
            this.txtSearchSettings.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSearchSettings.Location = new System.Drawing.Point(83, 3);
            this.txtSearchSettings.Name = "txtSearchSettings";
            this.txtSearchSettings.Size = new System.Drawing.Size(365, 25);
            this.txtSearchSettings.TabIndex = 1;
            this.txtSearchSettings.TextChanged += new System.EventHandler(this.txtSearchSettings_TextChanged);
            // 
            // lblSearchSettings
            // 
            this.lblSearchSettings.AutoSize = true;
            this.lblSearchSettings.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSearchSettings.Location = new System.Drawing.Point(3, 6);
            this.lblSearchSettings.Name = "lblSearchSettings";
            this.lblSearchSettings.Size = new System.Drawing.Size(73, 19);
            this.lblSearchSettings.TabIndex = 0;
            this.lblSearchSettings.Text = "Search for:";
            // 
            // saveNotification
            // 
            this.saveNotification.BackColor = System.Drawing.Color.LightGreen;
            this.saveNotification.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.saveNotification.Controls.Add(this.lblSaveNotification);
            this.saveNotification.Location = new System.Drawing.Point(254, 390);
            this.saveNotification.Name = "saveNotification";
            this.saveNotification.Size = new System.Drawing.Size(250, 45);
            this.saveNotification.TabIndex = 10;
            this.saveNotification.Visible = false;
            // 
            // lblSaveNotification
            // 
            this.lblSaveNotification.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSaveNotification.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSaveNotification.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblSaveNotification.Location = new System.Drawing.Point(0, 0);
            this.lblSaveNotification.Name = "lblSaveNotification";
            this.lblSaveNotification.Size = new System.Drawing.Size(248, 43);
            this.lblSaveNotification.TabIndex = 0;
            this.lblSaveNotification.Text = "✓ Settings saved successfully!";
            this.lblSaveNotification.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // saveNotificationTimer
            // 
            this.saveNotificationTimer.Interval = 3000;
            this.saveNotificationTimer.Tick += new System.EventHandler(this.saveNotificationTimer_Tick);
            // 
            // ToolConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 497);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.panelSearch);
            this.Controls.Add(this.saveNotification);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.cancelButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToolConfigForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tool Configuration";
            this.Load += new System.EventHandler(this.ToolConfigForm_Load);
            this.groupBoxPlugins.ResumeLayout(false);
            this.groupBoxPlugins.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorWindow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorMouse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorKeyboard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorScreenCapture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorPowerShell)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorCMD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorPlaywright)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRemotePort)).EndInit();
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorAutoInvoke)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorMultiAgent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTemperature)).EndInit();
            this.groupBoxSpeechRecognition.ResumeLayout(false);
            this.groupBoxSpeechRecognition.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorSpeech)).EndInit();
            this.groupBoxVoiceCommands.ResumeLayout(false);
            this.groupBoxVoiceCommands.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indicatorVoiceCmd)).EndInit();
            this.tabControlMain.ResumeLayout(false);
            this.tabPlugins.ResumeLayout(false);
            this.tabPlugins.PerformLayout();
            this.tabAISettings.ResumeLayout(false);
            this.tabVoice.ResumeLayout(false);
            this.tabCoordinator.ResumeLayout(false);
            this.grpCoordinatorConfig.ResumeLayout(false);
            this.grpCoordinatorConfig.PerformLayout();
            this.tabPlanner.ResumeLayout(false);
            this.grpPlannerConfig.ResumeLayout(false);
            this.grpPlannerConfig.PerformLayout();
            this.tabActioner.ResumeLayout(false);
            this.grpActionerConfig.ResumeLayout(false);
            this.grpActionerConfig.PerformLayout();
            this.tabAppearance.ResumeLayout(false);
            this.groupBoxTheme.ResumeLayout(false);
            this.groupBoxTheme.PerformLayout();
            this.tabProfiles.ResumeLayout(false);
            this.groupBoxProfiles.ResumeLayout(false);
            this.groupBoxProfiles.PerformLayout();
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.saveNotification.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxPlugins;
        private System.Windows.Forms.PictureBox indicatorWindow;
        private System.Windows.Forms.PictureBox indicatorMouse;
        private System.Windows.Forms.PictureBox indicatorKeyboard;
        private System.Windows.Forms.PictureBox indicatorScreenCapture;
        private System.Windows.Forms.PictureBox indicatorPowerShell;
        private System.Windows.Forms.PictureBox indicatorCMD;
        private System.Windows.Forms.PictureBox indicatorPlaywright;
        private System.Windows.Forms.CheckBox chkMousePlugin;
        private System.Windows.Forms.CheckBox chkKeyboardPlugin;
        private System.Windows.Forms.CheckBox chkScreenCapturePlugin;
        private System.Windows.Forms.CheckBox chkPowerShellPlugin;
        private System.Windows.Forms.CheckBox chkCMDPlugin;
        private System.Windows.Forms.CheckBox chkWindowSelectionPlugin;
        private System.Windows.Forms.CheckBox chkPlaywrightPlugin;
        private System.Windows.Forms.CheckBox enablePluginLoggingCheckBox;
        private System.Windows.Forms.CheckBox chkEnableRemoteControl;
        private System.Windows.Forms.NumericUpDown numRemotePort;
        private System.Windows.Forms.Label lblRemotePort;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.PictureBox indicatorAutoInvoke;
        private System.Windows.Forms.PictureBox indicatorMultiAgent;
        private System.Windows.Forms.NumericUpDown numTemperature;
        private System.Windows.Forms.Label lblTemperature;
        private System.Windows.Forms.CheckBox chkAutoInvoke;
        private System.Windows.Forms.CheckBox chkRetainChatHistory;
        private System.Windows.Forms.CheckBox chkMultiAgentMode;
        private System.Windows.Forms.CheckBox chkDynamicToolPrompts;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox groupBoxSpeechRecognition;
        private System.Windows.Forms.PictureBox indicatorSpeech;
        private System.Windows.Forms.CheckBox chkEnableSpeechRecognition;
        private System.Windows.Forms.Label lblSpeechLanguage;
        private System.Windows.Forms.ComboBox comboSpeechLanguage;
        private System.Windows.Forms.GroupBox groupBoxVoiceCommands;
        private System.Windows.Forms.PictureBox indicatorVoiceCmd;
        private System.Windows.Forms.CheckBox chkEnableVoiceCommands;
        private System.Windows.Forms.Label lblVoiceCommandPhrase;
        private System.Windows.Forms.TextBox txtVoiceCommandPhrase;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPlugins;
        private System.Windows.Forms.TabPage tabAISettings;
        private System.Windows.Forms.TabPage tabVoice;
        private System.Windows.Forms.TabPage tabCoordinator;
        private System.Windows.Forms.GroupBox grpCoordinatorConfig;
        private System.Windows.Forms.Label lblCoordinatorPrompt;
        private System.Windows.Forms.TextBox txtCoordinatorSystemPrompt;
        private System.Windows.Forms.CheckBox chkUseCustomCoordinatorConfig;
        private System.Windows.Forms.ComboBox comboCoordinatorConfig;
        private System.Windows.Forms.Button btnConfigureCoordinator;
        private System.Windows.Forms.TabPage tabPlanner;
        private System.Windows.Forms.GroupBox grpPlannerConfig;
        private System.Windows.Forms.Label lblPlannerPrompt;
        private System.Windows.Forms.TextBox txtPlannerSystemPrompt;
        private System.Windows.Forms.CheckBox chkUseCustomPlannerConfig;
        private System.Windows.Forms.ComboBox comboPlannerConfig;
        private System.Windows.Forms.Button btnConfigurePlanner;
        private System.Windows.Forms.TabPage tabActioner;
        private System.Windows.Forms.GroupBox grpActionerConfig;
        private System.Windows.Forms.Label lblActionerPrompt;
        private System.Windows.Forms.TextBox txtActionerSystemPrompt;
        private System.Windows.Forms.ComboBox comboActionerConfig;
        private System.Windows.Forms.Button btnConfigureActioner;
        private System.Windows.Forms.CheckBox chkUseCustomExecutorConfig;
        private System.Windows.Forms.TabPage tabAppearance;
        private System.Windows.Forms.GroupBox groupBoxTheme;
        private System.Windows.Forms.Label labelTheme;
        private System.Windows.Forms.ComboBox cbTheme;
        private System.Windows.Forms.TabPage tabProfiles;
        private System.Windows.Forms.GroupBox groupBoxProfiles;
        private System.Windows.Forms.Button btnDeleteProfile;
        private System.Windows.Forms.Button btnLoadProfile;
        private System.Windows.Forms.Button btnSaveProfile;
        private System.Windows.Forms.Label labelProfile;
        private System.Windows.Forms.ComboBox cmbProfiles;
        private System.Windows.Forms.ImageList imageListStatus;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Label searchResultLabel;
        private System.Windows.Forms.TextBox txtSearchSettings;
        private System.Windows.Forms.Label lblSearchSettings;
        private System.Windows.Forms.Panel saveNotification;
        private System.Windows.Forms.Label lblSaveNotification;
        private System.Windows.Forms.Timer saveNotificationTimer;
    }
}