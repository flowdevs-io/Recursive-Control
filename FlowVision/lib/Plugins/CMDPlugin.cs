using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowVision.lib.Classes;
using Microsoft.SemanticKernel;

namespace FlowVision.lib.Plugins
{
    class CMDPlugin
    {

        [KernelFunction, Description("Executes a CMD command and returns the output.")]
        public async Task<string> ExecuteCommand([Description("Command Prompt Command")] string command)
        {
            // Log the plugin usage
            PluginLogger.LogPluginUsage("CMDPlugin", "ExecuteCommand", command);

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
