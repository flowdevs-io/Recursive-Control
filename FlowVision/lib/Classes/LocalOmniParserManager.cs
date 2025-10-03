using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Manages the local OmniParser server - auto-starts, monitors, and restarts as needed
    /// </summary>
    public class LocalOmniParserManager
    {
        private static Process _serverProcess;
        private static readonly object _lock = new object();
        private static bool _isStarting = false;
        private static DateTime _lastStartAttempt = DateTime.MinValue;
        private static readonly TimeSpan StartupCooldown = TimeSpan.FromSeconds(10);

        // Default paths - can be configured
        private static string _omniParserPath = @"T:\OmniParser";
        private static string _serverScriptPath = @"omnitool\omniparserserver\omniparserserver.py";
        private static string _pythonExecutable = "python"; // Will try to find conda/python
        private static string _serverUrl = "http://127.0.0.1:8080";
        private static int _serverPort = 8080;

        /// <summary>
        /// Check if the OmniParser server is running and accessible
        /// </summary>
        public static async Task<bool> IsServerRunningAsync()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(2);
                    var response = await httpClient.GetAsync($"{_serverUrl}/probe/");
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Ensure the OmniParser server is running - auto-start if needed
        /// </summary>
        public static async Task<bool> EnsureServerRunningAsync()
        {
            // Check if already running
            if (await IsServerRunningAsync())
            {
                PluginLogger.LogInfo("LocalOmniParser", "EnsureServerRunning", "Server already running");
                return true;
            }

            // Prevent multiple simultaneous start attempts
            lock (_lock)
            {
                if (_isStarting)
                {
                    PluginLogger.LogInfo("LocalOmniParser", "EnsureServerRunning", "Server startup already in progress");
                    return false;
                }

                // Cooldown between start attempts
                if (DateTime.Now - _lastStartAttempt < StartupCooldown)
                {
                    PluginLogger.LogInfo("LocalOmniParser", "EnsureServerRunning", "Waiting for cooldown period");
                    return false;
                }

                _isStarting = true;
                _lastStartAttempt = DateTime.Now;
            }

            try
            {
                PluginLogger.NotifyTaskStart("OmniParser", "Starting local OmniParser server...");
                
                // Try to start the server
                bool started = await StartServerAsync();
                
                if (started)
                {
                    // Wait for server to be ready
                    for (int i = 0; i < 30; i++) // Wait up to 30 seconds
                    {
                        await Task.Delay(1000);
                        if (await IsServerRunningAsync())
                        {
                            PluginLogger.NotifyTaskComplete("OmniParser");
                            PluginLogger.LogInfo("LocalOmniParser", "EnsureServerRunning", 
                                $"Server started successfully in {i + 1} seconds");
                            return true;
                        }
                    }
                    
                    PluginLogger.LogError("LocalOmniParser", "EnsureServerRunning", 
                        "Server process started but not responding");
                }
                
                return false;
            }
            catch (Exception ex)
            {
                PluginLogger.LogError("LocalOmniParser", "EnsureServerRunning", 
                    $"Failed to start server: {ex.Message}");
                return false;
            }
            finally
            {
                _isStarting = false;
            }
        }

        /// <summary>
        /// Start the OmniParser server process
        /// </summary>
        private static async Task<bool> StartServerAsync()
        {
            try
            {
                // Check if path exists
                string fullPath = Path.Combine(_omniParserPath, _serverScriptPath);
                if (!File.Exists(fullPath))
                {
                    PluginLogger.LogError("LocalOmniParser", "StartServer", 
                        $"OmniParser server script not found at: {fullPath}");
                    return false;
                }

                // Build the command
                string workingDir = Path.Combine(_omniParserPath, "omnitool", "omniparserserver");
                string weightsPath = Path.Combine(_omniParserPath, "weights");
                
                // Prepare the Python command with proper paths
                string arguments = $"-m omniparserserver " +
                    $"--som_model_path \"{Path.Combine(weightsPath, "icon_detect", "model.pt")}\" " +
                    $"--caption_model_name florence2 " +
                    $"--caption_model_path \"{Path.Combine(weightsPath, "icon_caption_florence")}\" " +
                    $"--device cpu " + // Use CPU by default, user can modify
                    $"--BOX_TRESHOLD 0.05 " +
                    $"--port {_serverPort} " +
                    $"--host 0.0.0.0";

                PluginLogger.LogInfo("LocalOmniParser", "StartServer", $"Starting server: {_pythonExecutable} {arguments}");

                var startInfo = new ProcessStartInfo
                {
                    FileName = _pythonExecutable,
                    Arguments = arguments,
                    WorkingDirectory = workingDir,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = false
                };

                // Set up environment if using conda
                // Try to detect and activate conda environment if it exists
                string condaEnvPath = Path.Combine(_omniParserPath, ".conda");
                if (Directory.Exists(condaEnvPath))
                {
                    startInfo.EnvironmentVariables["PATH"] = 
                        Path.Combine(condaEnvPath, "Scripts") + ";" + 
                        Path.Combine(condaEnvPath, "Library", "bin") + ";" +
                        Environment.GetEnvironmentVariable("PATH");
                    startInfo.EnvironmentVariables["CONDA_PREFIX"] = condaEnvPath;
                }

                _serverProcess = new Process { StartInfo = startInfo };
                
                // Capture output for debugging
                _serverProcess.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        PluginLogger.LogInfo("OmniParser-Server", "Output", e.Data);
                    }
                };
                
                _serverProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        PluginLogger.LogError("OmniParser-Server", "Error", e.Data);
                    }
                };

                bool started = _serverProcess.Start();
                
                if (started)
                {
                    _serverProcess.BeginOutputReadLine();
                    _serverProcess.BeginErrorReadLine();
                    
                    PluginLogger.LogInfo("LocalOmniParser", "StartServer", 
                        $"Server process started with PID: {_serverProcess.Id}");
                }
                
                return started;
            }
            catch (Exception ex)
            {
                PluginLogger.LogError("LocalOmniParser", "StartServer", 
                    $"Exception starting server: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Stop the OmniParser server if it's running
        /// </summary>
        public static void StopServer()
        {
            try
            {
                if (_serverProcess != null && !_serverProcess.HasExited)
                {
                    PluginLogger.LogInfo("LocalOmniParser", "StopServer", "Stopping server...");
                    _serverProcess.Kill();
                    _serverProcess.WaitForExit(5000);
                    _serverProcess.Dispose();
                    _serverProcess = null;
                    PluginLogger.LogInfo("LocalOmniParser", "StopServer", "Server stopped");
                }
            }
            catch (Exception ex)
            {
                PluginLogger.LogError("LocalOmniParser", "StopServer", $"Error stopping server: {ex.Message}");
            }
        }

        /// <summary>
        /// Configure the OmniParser paths and settings
        /// </summary>
        public static void Configure(string omniParserPath = null, string pythonExe = null, 
            string serverUrl = null, int? port = null)
        {
            if (!string.IsNullOrEmpty(omniParserPath))
                _omniParserPath = omniParserPath;
            
            if (!string.IsNullOrEmpty(pythonExe))
                _pythonExecutable = pythonExe;
            
            if (!string.IsNullOrEmpty(serverUrl))
                _serverUrl = serverUrl;
            
            if (port.HasValue)
            {
                _serverPort = port.Value;
                _serverUrl = $"http://127.0.0.1:{_serverPort}";
            }
            
            PluginLogger.LogInfo("LocalOmniParser", "Configure", 
                $"Configured - Path: {_omniParserPath}, Python: {_pythonExecutable}, URL: {_serverUrl}");
        }

        /// <summary>
        /// Get the current server URL
        /// </summary>
        public static string GetServerUrl()
        {
            return _serverUrl;
        }

        /// <summary>
        /// Check if the OmniParser installation exists
        /// </summary>
        public static bool IsInstalled()
        {
            string fullPath = Path.Combine(_omniParserPath, _serverScriptPath);
            bool exists = File.Exists(fullPath);
            
            if (!exists)
            {
                PluginLogger.LogError("LocalOmniParser", "IsInstalled", 
                    $"OmniParser not found at: {fullPath}");
            }
            
            return exists;
        }

        /// <summary>
        /// Get installation status and diagnostics
        /// </summary>
        public static string GetDiagnostics()
        {
            string fullPath = Path.Combine(_omniParserPath, _serverScriptPath);
            string weightsPath = Path.Combine(_omniParserPath, "weights");
            
            return $"OmniParser Diagnostics:\n" +
                   $"  Installation Path: {_omniParserPath}\n" +
                   $"  Server Script: {(File.Exists(fullPath) ? "✓ Found" : "✗ Missing")}\n" +
                   $"  Weights Folder: {(Directory.Exists(weightsPath) ? "✓ Found" : "✗ Missing")}\n" +
                   $"  Python Executable: {_pythonExecutable}\n" +
                   $"  Server URL: {_serverUrl}\n" +
                   $"  Server Running: {(IsServerRunningAsync().Result ? "✓ Yes" : "✗ No")}\n" +
                   $"  Process Active: {(_serverProcess != null && !_serverProcess.HasExited ? "✓ Yes" : "✗ No")}";
        }
    }
}
