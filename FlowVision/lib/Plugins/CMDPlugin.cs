using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Classes;
using Microsoft.SemanticKernel;

namespace FlowVision.lib.Plugins
{
    class CMDPlugin
    {
        private static readonly string[] BrowserProcesses = { "msedge", "chrome", "firefox", "iexplore", "opera" };
        private static readonly string[] NonWaitingCommands = { "start ", "explorer " };

        [KernelFunction, Description("Executes a CMD command and returns the output.")]
        public async Task<string> ExecuteCommand([Description("Command Prompt Command")] string command)
        {
            // Log the plugin usage
            PluginLogger.LogPluginUsage("CMDPlugin", "ExecuteCommand", command);

            // Detect if this is likely launching an external application
            bool shouldWaitForExit = !ShouldSkipWaiting(command);
            
            try
            {
                var startInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    
                    // Use tasks to read output with timeout
                    var outputTask = process.StandardOutput.ReadToEndAsync();
                    var errorTask = process.StandardError.ReadToEndAsync();
                    
                    if (shouldWaitForExit)
                    {
                        // Wait for process with reasonable timeout
                        if (!process.WaitForExit(10000))  // 10 second timeout
                        {
                            return $"Command started but may still be running: {command}";
                        }
                    }
                    else
                    {
                        // For non-waiting commands, give a short time to collect initial output
                        await Task.Delay(500);
                        return $"Command launched: {command}";
                    }

                    // Complete reading output
                    string output = await outputTask;
                    string errors = await errorTask;

                    if (!string.IsNullOrWhiteSpace(errors))
                    {
                        return $"Error: {errors}";
                    }

                    return output.Trim();
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }

        [KernelFunction, Description("Launches an external application without waiting for it to exit.")]
        public string LaunchApplication([Description("Application to launch")] string application, 
                                       [Description("Arguments for the application")] string arguments = "")
        {
            // Log the plugin usage
            PluginLogger.LogPluginUsage("CMDPlugin", "LaunchApplication", $"{application} {arguments}");

            try
            {
                var startInfo = new ProcessStartInfo()
                {
                    FileName = application,
                    Arguments = arguments,
                    UseShellExecute = true,
                    CreateNoWindow = false
                };

                Process.Start(startInfo);
                return $"Successfully launched: {application}";
            }
            catch (Exception ex)
            {
                return $"Failed to launch {application}: {ex.Message}";
            }
        }

        private bool ShouldSkipWaiting(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return false;

            // Check if command contains any browser names (case-insensitive)
            if (BrowserProcesses.Any(browser =>
                command.IndexOf(browser, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                return true;
            }

            // Check for commands that typically launch external processes
            if (NonWaitingCommands.Any(cmd =>
                command.StartsWith(cmd, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            // Check for URLs or web addresses
            if (command.IndexOf("http://", StringComparison.OrdinalIgnoreCase) >= 0 ||
                command.IndexOf("https://", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }
    }
}
