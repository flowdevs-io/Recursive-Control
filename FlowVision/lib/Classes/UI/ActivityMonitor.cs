using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace FlowVision.lib.Classes.UI
{
    /// <summary>
    /// Displays real-time activity and system status
    /// </summary>
    public class ActivityMonitor : UserControl
    {
        private Panel statusPanel;
        private Label aiStatusLabel;
        private Label ocrStatusLabel;
        private Label browserStatusLabel;
        private RichTextBox activityLog;
        private List<ActivityEntry> activities = new List<ActivityEntry>();
        private const int MaxActivities = 100;

        public ActivityMonitor()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Status panel
            statusPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(this.Width, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.FromArgb(240, 240, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            // AI Status
            aiStatusLabel = CreateStatusLabel("🤖 AI: Ready", new Point(10, 10));
            
            // OCR Status
            ocrStatusLabel = CreateStatusLabel("👁️ ONNX: Ready", new Point(10, 35));
            
            // Browser Status
            browserStatusLabel = CreateStatusLabel("🌐 Browser: Inactive", new Point(10, 60));

            statusPanel.Controls.AddRange(new Control[] { aiStatusLabel, ocrStatusLabel, browserStatusLabel });

            // Activity log
            activityLog = new RichTextBox
            {
                Location = new Point(0, 85),
                Size = new Size(this.Width, this.Height - 85),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Consolas", 9F),
                BorderStyle = BorderStyle.FixedSingle
            };

            this.Controls.AddRange(new Control[] { statusPanel, activityLog });
            this.Size = new Size(300, 500);

            this.ResumeLayout(false);
        }

        private Label CreateStatusLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Location = location,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };
        }

        public void UpdateAIStatus(string status, Color color)
        {
            if (aiStatusLabel.InvokeRequired)
            {
                aiStatusLabel.Invoke(new Action(() => UpdateAIStatus(status, color)));
                return;
            }

            aiStatusLabel.Text = $"🤖 AI: {status}";
            aiStatusLabel.ForeColor = color;
        }

        public void UpdateONNXStatus(string status, Color color)
        {
            if (ocrStatusLabel.InvokeRequired)
            {
                ocrStatusLabel.Invoke(new Action(() => UpdateONNXStatus(status, color)));
                return;
            }

            ocrStatusLabel.Text = $"👁️ ONNX: {status}";
            ocrStatusLabel.ForeColor = color;
        }

        public void UpdateBrowserStatus(string status, Color color)
        {
            if (browserStatusLabel.InvokeRequired)
            {
                browserStatusLabel.Invoke(new Action(() => UpdateBrowserStatus(status, color)));
                return;
            }

            browserStatusLabel.Text = $"🌐 Browser: {status}";
            browserStatusLabel.ForeColor = color;
        }

        public void LogActivity(string category, string message, ActivityLevel level = ActivityLevel.Info)
        {
            if (activityLog.InvokeRequired)
            {
                activityLog.Invoke(new Action(() => LogActivity(category, message, level)));
                return;
            }

            var activity = new ActivityEntry
            {
                Timestamp = DateTime.Now,
                Category = category,
                Message = message,
                Level = level
            };

            activities.Add(activity);

            // Keep only last MaxActivities entries
            if (activities.Count > MaxActivities)
            {
                activities.RemoveAt(0);
            }

            // Format and append to log
            string icon = GetLevelIcon(level);
            Color color = GetLevelColor(level);
            string timestamp = activity.Timestamp.ToString("HH:mm:ss");
            
            activityLog.SelectionStart = activityLog.TextLength;
            activityLog.SelectionLength = 0;
            
            // Add timestamp
            activityLog.SelectionColor = Color.Gray;
            activityLog.AppendText($"[{timestamp}] ");
            
            // Add icon and category
            activityLog.SelectionColor = color;
            activityLog.AppendText($"{icon} {category}: ");
            
            // Add message
            activityLog.SelectionColor = Color.Black;
            activityLog.AppendText($"{message}\n");
            
            // Auto-scroll to bottom
            activityLog.SelectionStart = activityLog.Text.Length;
            activityLog.ScrollToCaret();
        }

        private string GetLevelIcon(ActivityLevel level)
        {
            return level switch
            {
                ActivityLevel.Debug => "🔍",
                ActivityLevel.Info => "ℹ️",
                ActivityLevel.Success => "✅",
                ActivityLevel.Warning => "⚠️",
                ActivityLevel.Error => "❌",
                _ => "•"
            };
        }

        private Color GetLevelColor(ActivityLevel level)
        {
            return level switch
            {
                ActivityLevel.Debug => Color.Gray,
                ActivityLevel.Info => Color.Blue,
                ActivityLevel.Success => Color.Green,
                ActivityLevel.Warning => Color.Orange,
                ActivityLevel.Error => Color.Red,
                _ => Color.Black
            };
        }

        public void Clear()
        {
            activities.Clear();
            activityLog.Clear();
        }

        public void ExportLog(string filePath = null)
        {
            if (filePath == null)
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    saveDialog.DefaultExt = "txt";
                    saveDialog.FileName = $"activity-log-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        filePath = saveDialog.FileName;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            System.IO.File.WriteAllText(filePath, activityLog.Text);
            MessageBox.Show($"Activity log exported to:\n{filePath}", "Export Complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    public class ActivityEntry
    {
        public DateTime Timestamp { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public ActivityLevel Level { get; set; }
    }

    public enum ActivityLevel
    {
        Debug,
        Info,
        Success,
        Warning,
        Error
    }
}
