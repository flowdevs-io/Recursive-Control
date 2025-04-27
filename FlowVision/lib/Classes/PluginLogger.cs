using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowVision.lib.Classes
{
    public class PluginLogger
    {
        private static readonly object LogLock = new object();
        private static string LogFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FlowVision", 
            "plugin_usage.log");
            
        private static RichTextBox _outputTextBox;
        private static CancellationTokenSource _loadingIndicatorCts;
        // Add delegate for direct UI updates
        private static Action<string, string, bool> _addMessageDelegate;

        // Initialize logger with a text box for displaying messages
        public static void Initialize(RichTextBox outputTextBox, Action<string, string, bool> addMessageDelegate = null)
        {
            _outputTextBox = outputTextBox;
            _addMessageDelegate = addMessageDelegate;
            
            // Ensure log directory exists
            string logDir = Path.GetDirectoryName(LogFilePath);
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
        }

        // Log plugin usage with plugin name and optional method name and parameters
        public static void LogPluginUsage(string pluginName, string methodName = null, string parameters = null)
        {
            if (!ToolConfig.LoadConfig("toolsconfig").EnablePluginLogging)
                return;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logMessage = $"[{timestamp}] Plugin: {pluginName}";
            
            if (!string.IsNullOrEmpty(methodName))
                logMessage += $", Method: {methodName}";
                
            if (!string.IsNullOrEmpty(parameters))
                logMessage += $", Params: {parameters}";
            
            // Write to file
            WriteToLogFile(logMessage);
            
            // Update UI if possible
            string uiMessage = $"Plugin usage: {pluginName}" + 
                     (string.IsNullOrEmpty(methodName) ? "" : $".{methodName}()") +
                     (string.IsNullOrEmpty(parameters) ? "" : $" with {parameters}");
            
            UpdateUI(uiMessage);
        }

        // Show a notification message before executing a task
        public static void NotifyTaskStart(string taskName, string description = null)
        {
            string message = $"Starting task: {taskName}";
            if (!string.IsNullOrEmpty(description))
                message += $" - {description}";
                
            UpdateUI(message, isTaskNotification: true);
            WriteToLogFile($"TASK START: {message}");
        }

        // Start a loading indicator animation for a running task
        public static void StartLoadingIndicator(string taskName)
        {
            StopLoadingIndicator(); // Ensure any previous indicators are stopped
            
            _loadingIndicatorCts = new CancellationTokenSource();
            var token = _loadingIndicatorCts.Token;
            
            // Start the animation in a background task
            Task.Run(async () => {
                string[] frames = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
                int frameIndex = 0;
                
                while (!token.IsCancellationRequested)
                {
                    string indicator = frames[frameIndex % frames.Length];
                    string loadingMessage = $"{indicator} Processing {taskName}...";
                    UpdateLoadingIndicator(loadingMessage);
                    
                    await Task.Delay(100, token).ContinueWith(t => { }, TaskContinuationOptions.OnlyOnCanceled);
                    if (token.IsCancellationRequested) break;
                    
                    frameIndex++;
                }
            }, token);
        }

        // Stop the loading indicator animation
        public static void StopLoadingIndicator()
        {
            if (_loadingIndicatorCts != null && !_loadingIndicatorCts.IsCancellationRequested)
            {
                _loadingIndicatorCts.Cancel();
                _loadingIndicatorCts.Dispose();
                _loadingIndicatorCts = null;
                
                // Clear the loading indicator from UI
                UpdateLoadingIndicator(string.Empty);
            }
        }

        // Notify that a task has completed
        public static void NotifyTaskComplete(string taskName, bool success = true)
        {
            StopLoadingIndicator();
            
            string message = success
                ? $"✓ Task completed: {taskName}"
                : $"✗ Task failed: {taskName}";
                
            UpdateUI(message, isTaskNotification: true);
            WriteToLogFile($"TASK {(success ? "COMPLETE" : "FAILED")}: {taskName}");
        }

        // Write a message to the log file
        private static void WriteToLogFile(string message)
        {
            lock (LogLock)
            {
                try
                {
                    File.AppendAllText(LogFilePath, message + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    // Handle any logging errors silently to avoid disrupting the main application
                    Console.WriteLine($"Error writing to log file: {ex.Message}");
                }
            }
        }

        // Update the UI with the plugin usage information
        private static void UpdateUI(string message, bool isTaskNotification = false)
        {
            // First try to update via the AddMessage delegate (for direct UI updates)
            if (_addMessageDelegate != null)
            {
                try
                {
                    string prefix = isTaskNotification ? "[Task]" : "[Logger]";
                    _addMessageDelegate("System", $"{prefix} {message}", true);
                    return; // If successful, return early
                }
                catch (Exception ex)
                {
                    // If the delegate fails, fall back to the text box approach
                    Console.WriteLine($"Error using message delegate: {ex.Message}");
                }
            }
            
            // Fall back to the original text box approach
            if (_outputTextBox != null && !_outputTextBox.IsDisposed)
            {
                try
                {
                    if (_outputTextBox.InvokeRequired)
                    {
                        _outputTextBox.Invoke(new Action<string, bool>(UpdateUI), message, isTaskNotification);
                        return;
                    }

                    string prefix = isTaskNotification ? "[Task] " : "[Logger] ";
                    _outputTextBox.AppendText($"{prefix}{message}\n");
                    _outputTextBox.ScrollToCaret();
                }
                catch (Exception)
                {
                    // Ignore UI update errors to prevent application disruption
                }
            }
        }

        // Update the UI with the current loading indicator frame
        private static void UpdateLoadingIndicator(string loadingMessage)
        {
            // Try to update via the AddMessage delegate first
            if (_addMessageDelegate != null && !string.IsNullOrEmpty(loadingMessage))
            {
                try
                {
                    _addMessageDelegate("System", $"[Task] {loadingMessage}", true);
                    return; // If successful, return early
                }
                catch (Exception)
                {
                    // If the delegate fails, fall back to the text box approach
                }
            }
            
            // Fall back to the original text box approach
            if (_outputTextBox != null && !_outputTextBox.IsDisposed)
            {
                try
                {
                    if (_outputTextBox.InvokeRequired)
                    {
                        _outputTextBox.Invoke(new Action<string>(UpdateLoadingIndicator), loadingMessage);
                        return;
                    }

                    // Find the last line that might be a loading indicator
                    string text = _outputTextBox.Text;
                    int lastLineStart = text.LastIndexOf('\n');
                    
                    if (lastLineStart >= 0 && text.Length > lastLineStart + 1)
                    {
                        string lastLine = text.Substring(lastLineStart + 1);
                        if (lastLine.Contains("Processing") && lastLine.Contains("..."))
                        {
                            // Replace the previous loading indicator line
                            _outputTextBox.Select(lastLineStart + 1, lastLine.Length);
                            _outputTextBox.SelectedText = string.IsNullOrEmpty(loadingMessage) ? "" : $"[Task] {loadingMessage}";
                        }
                        else if (!string.IsNullOrEmpty(loadingMessage))
                        {
                            // Add a new loading indicator line
                            _outputTextBox.AppendText($"[Task] {loadingMessage}\n");
                        }
                    }
                    else if (!string.IsNullOrEmpty(loadingMessage))
                    {
                        // Add a new loading indicator line if this is the first line
                        _outputTextBox.AppendText($"[Task] {loadingMessage}\n");
                    }
                    
                    _outputTextBox.ScrollToCaret();
                }
                catch (Exception)
                {
                    // Ignore UI update errors to prevent application disruption
                }
            }
        }

        // Get all logged plugin usage (for reporting)
        public static string GetAllLogs()
        {
            lock (LogLock)
            {
                try
                {
                    if (File.Exists(LogFilePath))
                    {
                        return File.ReadAllText(LogFilePath);
                    }
                    return "No log file found.";
                }
                catch (Exception ex)
                {
                    return $"Error reading log file: {ex.Message}";
                }
            }
        }

        // Clear all logs
        public static void ClearLogs()
        {
            lock (LogLock)
            {
                try
                {
                    if (File.Exists(LogFilePath))
                    {
                        File.WriteAllText(LogFilePath, "");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error clearing log file: {ex.Message}");
                }
            }
        }
    }
}
