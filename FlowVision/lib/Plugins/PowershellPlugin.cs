using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SemanticKernel;

namespace FlowVision.lib.Plugins
{
    internal class PowerShellPlugin
    {
        [KernelFunction, Description("Executes a PowerShell command and returns the output.")]
        public async Task<string> ExecuteCommand([Description("Powershell Command")] string command)
        {
            AppendLog($"Executing PowerShell command: {command}");
            try
            {
                var startInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string errors = await process.StandardError.ReadToEndAsync();
                    process.WaitForExit();

                    if (!string.IsNullOrWhiteSpace(errors))
                    {
                        AppendLog($"Error: {errors}");
                        return $"Error: {errors}";
                    }

                    AppendLog("Command executed successfully.");
                    AppendLog($"Output: {output.Trim()}");
                    return output.Trim();
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Exception: {ex.Message}");
                return $"Exception: {ex.Message}";
            }
        }

        /// <summary>
        /// Appends a message to the UI control safely.
        /// </summary>
        /// <param name="message">Message to log.</param>
        private void AppendLog(string message)
        {
        }
    }
}