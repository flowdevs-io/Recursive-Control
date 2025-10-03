using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace FlowVision.lib.Classes.UI
{
    /// <summary>
    /// Visual component showing step-by-step execution progress
    /// </summary>
    public class ExecutionVisualizer : UserControl
    {
        private FlowLayoutPanel stepsPanel;
        private Label titleLabel;
        private ProgressBar overallProgress;
        private Label statusLabel;
        private List<ExecutionStep> steps = new List<ExecutionStep>();
        private int currentStep = 0;
        private int totalSteps = 0;

        public ExecutionVisualizer()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Title
            titleLabel = new Label
            {
                Text = "Execution Progress",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            // Status label
            statusLabel = new Label
            {
                Text = "Ready",
                Font = new Font("Segoe UI", 9F),
                AutoSize = true,
                Location = new Point(10, 40),
                ForeColor = Color.Gray
            };

            // Overall progress bar
            overallProgress = new ProgressBar
            {
                Location = new Point(10, 65),
                Size = new Size(this.Width - 20, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Style = ProgressBarStyle.Continuous
            };

            // Steps panel
            stepsPanel = new FlowLayoutPanel
            {
                Location = new Point(10, 95),
                Size = new Size(this.Width - 20, this.Height - 105),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            this.Controls.AddRange(new Control[] { titleLabel, statusLabel, overallProgress, stepsPanel });
            this.Size = new Size(400, 500);
            this.BackColor = Color.White;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void StartExecution(int totalSteps)
        {
            this.totalSteps = totalSteps;
            this.currentStep = 0;
            steps.Clear();
            stepsPanel.Controls.Clear();
            overallProgress.Maximum = totalSteps;
            overallProgress.Value = 0;
            statusLabel.Text = $"Starting execution: {totalSteps} steps planned";
            statusLabel.ForeColor = Color.Blue;
        }

        public void AddStep(string description, string icon = "⏳")
        {
            var step = new ExecutionStep
            {
                Description = description,
                Status = StepStatus.Pending,
                Icon = icon,
                Index = steps.Count + 1
            };
            steps.Add(step);

            var stepControl = CreateStepControl(step);
            stepsPanel.Controls.Add(stepControl);
            
            // Auto-scroll to bottom
            stepsPanel.ScrollControlIntoView(stepControl);
        }

        public void UpdateStep(int stepIndex, StepStatus status, string result = null)
        {
            if (stepIndex < 0 || stepIndex >= steps.Count)
                return;

            var step = steps[stepIndex];
            step.Status = status;
            step.Result = result;

            switch (status)
            {
                case StepStatus.InProgress:
                    step.Icon = "⚙️";
                    currentStep = stepIndex;
                    statusLabel.Text = $"Step {stepIndex + 1}/{totalSteps}: {step.Description}";
                    statusLabel.ForeColor = Color.Blue;
                    break;
                case StepStatus.Completed:
                    step.Icon = "✅";
                    overallProgress.Value = Math.Min(stepIndex + 1, totalSteps);
                    statusLabel.Text = $"Completed step {stepIndex + 1}/{totalSteps}";
                    statusLabel.ForeColor = Color.Green;
                    break;
                case StepStatus.Failed:
                    step.Icon = "❌";
                    statusLabel.Text = $"Step {stepIndex + 1} failed: {result}";
                    statusLabel.ForeColor = Color.Red;
                    break;
                case StepStatus.Skipped:
                    step.Icon = "⏭️";
                    break;
            }

            // Update the visual control
            if (stepIndex < stepsPanel.Controls.Count)
            {
                var control = stepsPanel.Controls[stepIndex];
                UpdateStepControl(control, step);
            }
        }

        public void CompleteExecution(bool success)
        {
            if (success)
            {
                statusLabel.Text = $"✅ Execution completed successfully ({steps.Count} steps)";
                statusLabel.ForeColor = Color.Green;
                overallProgress.Value = overallProgress.Maximum;
            }
            else
            {
                statusLabel.Text = $"❌ Execution failed at step {currentStep + 1}";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private Control CreateStepControl(ExecutionStep step)
        {
            var panel = new Panel
            {
                Size = new Size(stepsPanel.Width - 25, 60),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5),
                Margin = new Padding(0, 0, 0, 5),
                BackColor = Color.WhiteSmoke,
                Tag = step
            };

            var iconLabel = new Label
            {
                Text = step.Icon,
                Font = new Font("Segoe UI", 14F),
                Size = new Size(30, 30),
                Location = new Point(5, 15),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var indexLabel = new Label
            {
                Text = $"#{step.Index}",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Size = new Size(30, 15),
                Location = new Point(5, 5),
                ForeColor = Color.Gray
            };

            var descLabel = new Label
            {
                Text = step.Description,
                Font = new Font("Segoe UI", 9F),
                Size = new Size(panel.Width - 45, 40),
                Location = new Point(40, 10),
                AutoEllipsis = true
            };

            panel.Controls.AddRange(new Control[] { iconLabel, indexLabel, descLabel });
            return panel;
        }

        private void UpdateStepControl(Control control, ExecutionStep step)
        {
            if (control is Panel panel && panel.Tag == step)
            {
                // Update icon
                var iconLabel = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Size.Width == 30 && l.Location.X == 5 && l.Location.Y == 15);
                if (iconLabel != null)
                {
                    iconLabel.Text = step.Icon;
                }

                // Update background color based on status
                switch (step.Status)
                {
                    case StepStatus.InProgress:
                        panel.BackColor = Color.LightBlue;
                        break;
                    case StepStatus.Completed:
                        panel.BackColor = Color.LightGreen;
                        break;
                    case StepStatus.Failed:
                        panel.BackColor = Color.LightCoral;
                        break;
                    case StepStatus.Skipped:
                        panel.BackColor = Color.LightGray;
                        break;
                }
            }
        }

        public void Clear()
        {
            steps.Clear();
            stepsPanel.Controls.Clear();
            overallProgress.Value = 0;
            statusLabel.Text = "Ready";
            statusLabel.ForeColor = Color.Gray;
            currentStep = 0;
            totalSteps = 0;
        }
    }

    public class ExecutionStep
    {
        public int Index { get; set; }
        public string Description { get; set; }
        public StepStatus Status { get; set; }
        public string Icon { get; set; }
        public string Result { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;
    }

    public enum StepStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed,
        Skipped
    }
}
