using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SemanticKernel;

namespace FlowVision.lib.Plugins
{
    class CMDPlugin
    {

        [KernelFunction, Description("Executes a CMD command and returns the output.")]
        public async Task<string> ExecuteCommand([Description("Command Prompt Command")] string command)
        {
            AppendLog($"Executing CMD command: {command}");
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
                        AppendLog($"Error: {errors}");
                        return $"Error: {errors}";
                    }

                    AppendLog("CMD command executed successfully.");
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
