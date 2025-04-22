using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FlowVision.lib.Classes;

namespace FlowVision.lib.Controls
{
    public class LogViewer : UserControl
    {
        private RichTextBox _logTextBox;
        private ComboBox _filterPluginComboBox;
        private ComboBox _filterLevelComboBox;
        private Button _clearButton;
        private CheckBox _autoScrollCheckBox;
        private Button _exportButton;
        private Panel _toolbarPanel;
        private readonly LLMPluginInteractionManager _logManager;
        private readonly Dictionary<LogLevel, Color> _logColors;
        private readonly object _logUpdateLock = new object();

        public LogViewer()
        {
            InitializeComponent();
            _logManager = LLMPluginInteractionManager.Instance;
            _logManager.SetLogViewer(this);
            _logManager.LogAdded += LogManager_LogAdded;

            _logColors = new Dictionary<LogLevel, Color>
            {
                { LogLevel.Info, Color.Black },
                { LogLevel.Warning, Color.DarkOrange },
                { LogLevel.Error, Color.Red }
            };

            // Initialize filter combo boxes
            _filterLevelComboBox.Items.Add("All Levels");
            foreach (LogLevel level in Enum.GetValues(typeof(LogLevel)))
            {
                _filterLevelComboBox.Items.Add(level);
            }
            _filterLevelComboBox.SelectedIndex = 0;

            _filterPluginComboBox.Items.Add("All Plugins");
            _filterPluginComboBox.SelectedIndex = 0;

            // Set auto-scroll by default
            _autoScrollCheckBox.Checked = true;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Toolbar panel
            _toolbarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30
            };

            // Plugin filter combo box
            _filterPluginComboBox = new ComboBox
            {
                Location = new Point(10, 5),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _filterPluginComboBox.SelectedIndexChanged += FilterChanged;

            // Level filter combo box
            _filterLevelComboBox = new ComboBox
            {
                Location = new Point(170, 5),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _filterLevelComboBox.SelectedIndexChanged += FilterChanged;

            // Auto-scroll checkbox
            _autoScrollCheckBox = new CheckBox
            {
                Text = "Auto-Scroll",
                Location = new Point(280, 5),
                Width = 90,
                Checked = true
            };

            // Clear button
            _clearButton = new Button
            {
                Text = "Clear",
                Location = new Point(380, 5),
                Width = 60
            };
            _clearButton.Click += ClearButton_Click;

            // Export button
            _exportButton = new Button
            {
                Text = "Export Logs",
                Location = new Point(450, 5),
                Width = 80
            };
            _exportButton.Click += ExportButton_Click;

            _toolbarPanel.Controls.Add(_filterPluginComboBox);
            _toolbarPanel.Controls.Add(_filterLevelComboBox);
            _toolbarPanel.Controls.Add(_autoScrollCheckBox);
            _toolbarPanel.Controls.Add(_clearButton);
            _toolbarPanel.Controls.Add(_exportButton);

            // Log text box
            _logTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Consolas", 9F),
                DetectUrls = true
            };
            _logTextBox.KeyDown += LogTextBox_KeyDown;

            // Add context menu for copy
            var contextMenu = new ContextMenuStrip();
            var copyMenuItem = new ToolStripMenuItem("Copy");
            copyMenuItem.Click += (sender, e) => {
                if (_logTextBox.SelectionLength > 0)
                    Clipboard.SetText(_logTextBox.SelectedText);
            };
            var selectAllMenuItem = new ToolStripMenuItem("Select All");
            selectAllMenuItem.Click += (sender, e) => _logTextBox.SelectAll();
            contextMenu.Items.Add(copyMenuItem);
            contextMenu.Items.Add(selectAllMenuItem);
            _logTextBox.ContextMenuStrip = contextMenu;

            this.Controls.Add(_logTextBox);
            this.Controls.Add(_toolbarPanel);
            this.Size = new Size(600, 300);
            this.ResumeLayout(false);
        }

        private void LogManager_LogAdded(object sender, LogEntryEventArgs e)
        {
            // Update the filter plugin dropdown if needed
            if (!_filterPluginComboBox.Items.Contains(e.LogEntry.PluginName))
            {
                BeginInvoke(new Action(() => {
                    if (!_filterPluginComboBox.Items.Contains(e.LogEntry.PluginName))
                        _filterPluginComboBox.Items.Add(e.LogEntry.PluginName);
                }));
            }
        }

        public void AppendLogEntry(LogEntry logEntry)
        {
            // Check if we need to update the UI (don't update if filtered out)
            if (ShouldDisplayLog(logEntry))
            {
                try
                {
                    BeginInvoke(new Action(() => {
                        AppendFormattedLog(logEntry);
                    }));
                }
                catch (InvalidOperationException)
                {
                    // Handle case when control is disposed
                }
            }
        }

        private void AppendFormattedLog(LogEntry logEntry)
        {
            lock (_logUpdateLock)
            {
                // Store current position
                int startPosition = _logTextBox.TextLength;

                // Add the log entry text
                _logTextBox.AppendText(logEntry.ToString() + Environment.NewLine);

                // Color the text based on log level
                _logTextBox.Select(startPosition, logEntry.ToString().Length);
                _logTextBox.SelectionColor = _logColors[logEntry.Level];
                
                // Reset selection
                _logTextBox.SelectionStart = _logTextBox.TextLength;
                _logTextBox.SelectionLength = 0;

                // Auto-scroll if enabled
                if (_autoScrollCheckBox.Checked)
                {
                    _logTextBox.ScrollToCaret();
                }
            }
        }

        private bool ShouldDisplayLog(LogEntry logEntry)
        {
            // Check plugin filter
            if (_filterPluginComboBox.SelectedIndex > 0 && 
                _filterPluginComboBox.SelectedItem.ToString() != logEntry.PluginName)
                return false;

            // Check level filter
            if (_filterLevelComboBox.SelectedIndex > 0 &&
                _filterLevelComboBox.SelectedItem.ToString() != logEntry.Level.ToString())
                return false;

            return true;
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            RefreshLogDisplay();
        }

        private void RefreshLogDisplay()
        {
            _logTextBox.Clear();
            
            // Get filtered logs from the manager
            List<LogEntry> filteredLogs = _logManager.GetLogHistory();
            
            // Apply our filter
            filteredLogs = filteredLogs.FindAll(log => ShouldDisplayLog(log));
            
            // Display logs
            foreach (var log in filteredLogs)
            {
                AppendFormattedLog(log);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            _logTextBox.Clear();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Log files (*.log)|*.log|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveDialog.DefaultExt = "log";
                saveDialog.FileName = $"FlowVision_Log_{DateTime.Now:yyyyMMdd_HHmmss}";
                
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var writer = new System.IO.StreamWriter(saveDialog.FileName))
                    {
                        writer.Write(_logTextBox.Text);
                    }
                }
            }
        }

        private void LogTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Allow Ctrl+C for copy
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (_logTextBox.SelectionLength > 0)
                    Clipboard.SetText(_logTextBox.SelectedText);
            }
        }
    }
}
