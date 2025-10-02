using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FlowVision.lib.Classes;
using System.ComponentModel;

namespace FlowVision.lib.Plugins
{
    /// <summary>
    /// Simple HTTP server that forwards JSON commands to an assigned handler.
    /// </summary>
    public class RemoteControlPlugin : IDisposable
    {
        private static HttpListener _listener;
        private static bool _running;
        private static int _port;
        private static Func<string, Task<string>> _commandHandler;

        /// <summary>
        /// Assign the delegate used to execute incoming commands.
        /// </summary>
        public static void SetCommandHandler(Func<string, Task<string>> handler)
        {
            _commandHandler = handler;
        }

        /// <summary>
        /// Start listening on the configured port if not already running.
        /// </summary>
        public static void StartServer(int port)
        {
            if (_running)
            {
                return;
            }

            _port = port;
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://*:{port}/");
            _listener.Start();
            _running = true;
            Task.Run(ListenLoop);
        }

        /// <summary>
        /// Returns the status of the remote control server.
        /// </summary>
        [Description("Get the status of the remote control server")]
        public static string GetStatus() => _running ? $"Listening on {_port}" : "Stopped";

        private static async Task ListenLoop()
        {
            while (_running)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    if (context.Request.HttpMethod != "POST")
                    {
                        context.Response.StatusCode = 404;
                        context.Response.Close();
                        continue;
                    }

                    string body;
                    using (var reader = new StreamReader(context.Request.InputStream))
                    {
                        body = await reader.ReadToEndAsync();
                    }

                    string command = null;
                    try
                    {
                        var doc = JsonDocument.Parse(body);
                        if (doc.RootElement.TryGetProperty("command", out var cmdEl))
                        {
                            command = cmdEl.GetString();
                        }
                    }
                    catch { }

                    string result = string.Empty;
                    if (!string.IsNullOrEmpty(command) && _commandHandler != null)
                    {
                        PluginLogger.LogPluginUsage("RemoteControlPlugin", "Command", command);
                        result = await _commandHandler(command);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        result = "Invalid command";
                    }

                    var writer = new StreamWriter(context.Response.OutputStream);
                    await writer.WriteAsync(result ?? string.Empty);
                    context.Response.Close();
                }
                catch (Exception ex)
                {
                    PluginLogger.LogError("RemoteControl", "Listener", ex.Message);
                }
            }
        }

        public static void StopServer()
        {
            _running = false;
            try { _listener?.Stop(); } catch { }
            _listener = null;
        }

        public void Dispose()
        {
            StopServer();
        }
    }
}
