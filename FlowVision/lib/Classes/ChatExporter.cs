using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Exports chat history with tool calls for debugging and troubleshooting
    /// </summary>
    public static class ChatExporter
    {
        public class ExportedMessage
        {
            public string Timestamp { get; set; }
            public string Author { get; set; }
            public string Content { get; set; }
            public List<ToolCall> ToolCalls { get; set; }
            public string Duration { get; set; }
        }

        public class ToolCall
        {
            public string Plugin { get; set; }
            public string Method { get; set; }
            public string Parameters { get; set; }
            public string Result { get; set; }
            public string Timestamp { get; set; }
        }

        /// <summary>
        /// Export chat history to JSON format with tool calls
        /// </summary>
        public static void ExportToJson(List<LocalChatMessage> chatHistory, string filePath = null)
        {
            try
            {
                if (filePath == null)
                {
                    using (SaveFileDialog saveDialog = new SaveFileDialog())
                    {
                        saveDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                        saveDialog.DefaultExt = "json";
                        saveDialog.FileName = $"chat-export-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.json";

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

                var exportData = new
                {
                    ExportTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    MessageCount = chatHistory.Count,
                    Messages = chatHistory.Select(m => new
                    {
                        m.Timestamp,
                        m.Author,
                        m.Content
                    })
                };

                string json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(filePath, json);

                MessageBox.Show($"Chat exported successfully to:\n{filePath}", "Export Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting chat: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Export chat history to Markdown format
        /// </summary>
        public static void ExportToMarkdown(List<LocalChatMessage> chatHistory, string filePath = null)
        {
            try
            {
                if (filePath == null)
                {
                    using (SaveFileDialog saveDialog = new SaveFileDialog())
                    {
                        saveDialog.Filter = "Markdown files (*.md)|*.md|All files (*.*)|*.*";
                        saveDialog.DefaultExt = "md";
                        saveDialog.FileName = $"chat-export-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.md";

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

                var sb = new StringBuilder();
                sb.AppendLine($"# Chat Export - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine();
                sb.AppendLine($"**Total Messages:** {chatHistory.Count}");
                sb.AppendLine();
                sb.AppendLine("---");
                sb.AppendLine();

                foreach (var message in chatHistory)
                {
                    sb.AppendLine($"## {message.Author}");
                    sb.AppendLine($"*{message.Timestamp}*");
                    sb.AppendLine();
                    sb.AppendLine(message.Content);
                    sb.AppendLine();
                    sb.AppendLine("---");
                    sb.AppendLine();
                }

                File.WriteAllText(filePath, sb.ToString());

                MessageBox.Show($"Chat exported successfully to:\n{filePath}", "Export Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting chat: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Export with detailed tool call logs for debugging
        /// </summary>
        public static void ExportWithToolCalls(List<LocalChatMessage> chatHistory, string logFilePath = null)
        {
            try
            {
                string mainFilePath = null;
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Markdown files (*.md)|*.md|All files (*.*)|*.*";
                    saveDialog.DefaultExt = "md";
                    saveDialog.FileName = $"chat-debug-export-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.md";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        mainFilePath = saveDialog.FileName;
                    }
                    else
                    {
                        return;
                    }
                }

                var sb = new StringBuilder();
                sb.AppendLine($"# Debugging Chat Export");
                sb.AppendLine($"**Export Time:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"**Total Messages:** {chatHistory.Count}");
                sb.AppendLine();

                // Try to include plugin logs if available
                string pluginLogPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "FlowVision", "plugin_usage.log");

                if (File.Exists(pluginLogPath) && logFilePath == null)
                {
                    logFilePath = pluginLogPath;
                }

                sb.AppendLine("## Chat Messages");
                sb.AppendLine();

                foreach (var message in chatHistory)
                {
                    sb.AppendLine($"### {message.Author} - {message.Timestamp}");
                    sb.AppendLine();
                    sb.AppendLine("```");
                    sb.AppendLine(message.Content);
                    sb.AppendLine("```");
                    sb.AppendLine();
                }

                // Append tool call logs if available
                if (logFilePath != null && File.Exists(logFilePath))
                {
                    sb.AppendLine("---");
                    sb.AppendLine();
                    sb.AppendLine("## Plugin Usage Log");
                    sb.AppendLine();
                    sb.AppendLine("```");
                    
                    // Get last 1000 lines of log
                    var logLines = File.ReadAllLines(logFilePath);
                    var recentLines = logLines.Skip(Math.Max(0, logLines.Length - 1000)).ToArray();
                    sb.AppendLine(string.Join(Environment.NewLine, recentLines));
                    
                    sb.AppendLine("```");
                }

                File.WriteAllText(mainFilePath, sb.ToString());

                MessageBox.Show($"Debug export created successfully:\n{mainFilePath}\n\n" +
                               "This includes chat history and plugin usage logs for troubleshooting.",
                               "Debug Export Complete",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating debug export: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Quick export to clipboard in readable format
        /// </summary>
        public static void CopyToClipboard(List<LocalChatMessage> chatHistory)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine($"=== Chat Export - {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
                sb.AppendLine();

                foreach (var message in chatHistory)
                {
                    sb.AppendLine($"[{message.Timestamp}] {message.Author}:");
                    sb.AppendLine(message.Content);
                    sb.AppendLine();
                }

                Clipboard.SetText(sb.ToString());

                MessageBox.Show("Chat history copied to clipboard!", "Copied",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying to clipboard: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
