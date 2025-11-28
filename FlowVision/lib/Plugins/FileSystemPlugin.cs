using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using FlowVision.lib.Classes;

namespace FlowVision.lib.Plugins
{
    internal class FileSystemPlugin
    {
        [Description("Gets the current working directory of the application")]
        public string GetCurrentDirectory()
        {
            PluginLogger.LogPluginUsage("FileSystemPlugin", "GetCurrentDirectory");
            return Directory.GetCurrentDirectory();
        }

        [Description("Lists files and directories in the specified path. Returns first 50 entries.")]
        public string ListDirectory(string path)
        {
            PluginLogger.LogPluginUsage("FileSystemPlugin", "ListDirectory", path);
            try
            {
                if (!Directory.Exists(path)) return $"Directory not found: {path}";

                var dirs = Directory.GetDirectories(path).Select(d => $"[DIR] {Path.GetFileName(d)}");
                var files = Directory.GetFiles(path).Select(f => Path.GetFileName(f));
                
                var all = dirs.Concat(files).Take(50);
                return string.Join("\n", all);
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        [Description("Checks if a file exists at the specified path")]
        public bool FileExists(string path)
        {
            PluginLogger.LogPluginUsage("FileSystemPlugin", "FileExists", path);
            return File.Exists(path);
        }

        [Description("Reads the content of a text file (max 2000 chars)")]
        public string ReadFile(string path)
        {
            PluginLogger.LogPluginUsage("FileSystemPlugin", "ReadFile", path);
            try
            {
                if (!File.Exists(path)) return "File not found";
                
                string content = File.ReadAllText(path);
                if (content.Length > 2000)
                {
                    return content.Substring(0, 2000) + "\n...[Truncated]...";
                }
                return content;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        [Description("Writes text content to a file. Overwrites if exists.")]
        public string WriteFile(string path, string content)
        {
            PluginLogger.LogPluginUsage("FileSystemPlugin", "WriteFile", path);
            try
            {
                File.WriteAllText(path, content);
                return $"Successfully wrote to {path}";
            }
            catch (Exception ex)
            {
                return $"Error writing file: {ex.Message}";
            }
        }
    }
}
