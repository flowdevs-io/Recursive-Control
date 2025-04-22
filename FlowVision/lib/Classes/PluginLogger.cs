using System;
using System.IO;
using System.Text;
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

        // Initialize logger with a text box for displaying messages
        public static void Initialize(RichTextBox outputTextBox)
        {
            _outputTextBox = outputTextBox;
            
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
            UpdateUI($"Plugin usage: {pluginName}" + 
                     (string.IsNullOrEmpty(methodName) ? "" : $".{methodName}()") +
                     (string.IsNullOrEmpty(parameters) ? "" : $" with {parameters}"));
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
        private static void UpdateUI(string message)
        {
            if (_outputTextBox != null && !_outputTextBox.IsDisposed)
            {
                try
                {
                    if (_outputTextBox.InvokeRequired)
                    {
                        _outputTextBox.Invoke(new Action<string>(UpdateUI), message);
                        return;
                    }

                    _outputTextBox.AppendText($"[Logger] {message}\n");
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
