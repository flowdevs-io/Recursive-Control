using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Classes;

namespace FlowVision.lib.Plugins
{
    internal class PowerShellPlugin
    {
        [Description("Executes a PowerShell command and returns the output.")]
        public async Task<string> ExecuteCommand([Description("Powershell Command")] string command)
        {
            // Log the plugin usage
            PluginLogger.LogPluginUsage("PowerShellPlugin", "ExecuteCommand", command);

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
    }
}