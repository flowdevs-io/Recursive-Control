using System;
using System.Collections.Generic;
using System.Text;
using FlowVision.lib.Plugins;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Generates dynamic descriptions of enabled plugins and tools based on the current configuration
    /// </summary>
    public static class ToolDescriptionGenerator
    {
        /// <summary>
        /// Creates a formatted description of all enabled tools in the provided configuration
        /// </summary>
        /// <param name="toolConfig">The tool configuration containing enabled/disabled status of plugins</param>
        /// <returns>A formatted string describing all enabled tools and their capabilities</returns>
        public static string GetToolDescriptions(ToolConfig toolConfig)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Available Tools and Plugins");
            sb.AppendLine("You have access to the following tools:");
            sb.AppendLine();

            var enabledTools = new List<string>();

            // Add descriptions for each enabled plugin
            if (toolConfig.EnableCMDPlugin)
            {
                enabledTools.Add(GetCMDPluginDescription());
            }

            if (toolConfig.EnablePowerShellPlugin)
            {
                enabledTools.Add(GetPowerShellPluginDescription());
            }

            if (toolConfig.EnableScreenCapturePlugin)
            {
                enabledTools.Add(GetScreenCapturePluginDescription());
            }

            if (toolConfig.EnableKeyboardPlugin)
            {
                enabledTools.Add(GetKeyboardPluginDescription());
            }

            if (toolConfig.EnableMousePlugin)
            {
                enabledTools.Add(GetMousePluginDescription());
            }

            if (toolConfig.EnableWindowSelectionPlugin)
            {
                enabledTools.Add(GetWindowSelectionPluginDescription());
            }

            if (toolConfig.EnablePlaywrightPlugin)
            {
                enabledTools.Add(GetPlaywrightPluginDescription());
            }

            if (toolConfig.EnableRemoteControl)
            {
                enabledTools.Add(GetRemoteControlPluginDescription());
            }

            // Join all descriptions with double line breaks
            sb.Append(string.Join("\n\n", enabledTools));

            if (enabledTools.Count == 0)
            {
                sb.AppendLine("No tools are currently enabled in the configuration.");
            }
            else
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("You can call these tools to help answer the user's questions or perform tasks. " +
                              "Make sure to use the exact tool names and parameters as specified above.");
            }

            return sb.ToString();
        }

        private static string GetCMDPluginDescription()
        {
            return "## CMDPlugin\n" +
                  "- **ExecuteCommand(string command)**: Executes a Windows command prompt (CMD) command and returns the output. " +
                  "Use this for file operations, system information queries, and basic system administration on Windows.";
        }

        private static string GetPowerShellPluginDescription()
        {
            return "## PowerShellPlugin\n" +
                  "- **ExecuteScript(string script)**: Executes a PowerShell script and returns the output. " +
                  "Use this for more advanced system administration tasks, registry operations, and accessing Windows APIs.";
        }

        private static string GetScreenCapturePluginDescription()
        {
            return "## ScreenCaptureOmniParserPlugin\n" +
                  "- **CaptureScreen()**: Captures the current screen and returns a textual description of the visible content. " +
                  "- **CaptureScreenWithOCR()**: Captures the screen and performs OCR to extract visible text. " +
                  "- **CaptureScreenWithAnalysis()**: Captures the screen and provides AI-powered analysis of the visual content.";
        }

        private static string GetKeyboardPluginDescription()
        {
            return "## KeyboardPlugin\n" +
                  "- **SendKeys(string keys)**: Sends keystrokes to the active application. " +
                  "- **SendHotkey(string modifiers, string key)**: Sends modifier key combinations (e.g., Ctrl+C). " +
                  "- **TypeText(string text)**: Types text into the active application.";
        }

        private static string GetMousePluginDescription()
        {
            return "## MousePlugin\n" +
                  "- **MouseMove(int x, int y)**: Moves the mouse cursor to the specified screen coordinates. " +
                  "- **MouseClick(int x, int y)**: Performs a mouse click at the specified coordinates. " +
                  "- **MouseRightClick(int x, int y)**: Performs a right-click at the specified coordinates.";
        }

        private static string GetWindowSelectionPluginDescription()
        {
            return "## WindowSelectionPlugin\n" +
                  "- **ListWindows()**: Lists all open windows with their titles and handles. " +
                  "- **SwitchToWindow(string windowTitle)**: Brings the specified window to the foreground. " +
                  "- **CloseWindow(string windowTitle)**: Closes the specified window. " + 
                  "- **MaximizeWindow(string windowTitle)**: Maximizes the specified window.";
        }

        private static string GetPlaywrightPluginDescription()
        {
            return "## PlaywrightPlugin\n" +
                  "- **LaunchBrowser()**: Launches a new browser instance for web automation. " +
                  "- **NavigateToUrl(string url, string waitUntil='load')**: " +
                  "Navigates to the URL with optional wait strategy. " +
                  "- **ExecuteScript(string script)**: Runs JavaScript in the browser context. " +
                  "- **CaptureScreenshot()**: Takes a screenshot of the current page. " +
                  "- **GetPageContent()**: Gets the HTML content of the current page. " +
                  "- **CloseBrowser()**: Closes the browser.";
        }

        private static string GetRemoteControlPluginDescription()
        {
            return "## RemoteControlPlugin\n" +
                  "- **StartServer(int port)**: Starts a remote control server on the specified port. " +
                  "- **StopServer()**: Stops the remote control server. " +
                  "- **SendCommand(string command)**: Sends a command to connected clients.";
        }
    }
}
