using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FlowVision.lib.Plugins;
using Microsoft.SemanticKernel;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Utility class to generate descriptions of available tools for the system prompt
    /// </summary>
    public class ToolDescriptionGenerator
    {
        /// <summary>
        /// Gets a formatted description of all enabled tools based on the current configuration
        /// </summary>
        /// <param name="toolConfig">The current tool configuration</param>
        /// <returns>A formatted string describing available tools</returns>
        public static string GetToolDescriptions(ToolConfig toolConfig)
        {
            var availableTools = new List<Type>();

            // Add tools based on configuration
            if (toolConfig.EnableCMDPlugin) 
                availableTools.Add(typeof(CMDPlugin));
                
            if (toolConfig.EnablePowerShellPlugin) 
                availableTools.Add(typeof(PowerShellPlugin));
                
            if (toolConfig.EnableScreenCapturePlugin) 
                availableTools.Add(typeof(ScreenCaptureOmniParserPlugin));
                
            if (toolConfig.EnableKeyboardPlugin) 
                availableTools.Add(typeof(KeyboardPlugin));
                
            if (toolConfig.EnableMousePlugin) 
                availableTools.Add(typeof(MousePlugin));
                
            if (toolConfig.EnableWindowSelectionPlugin) 
                availableTools.Add(typeof(WindowSelectionPlugin));
                
            // Add Playwright plugin if enabled
            if (toolConfig.EnablePlaywrightPlugin)
                availableTools.Add(typeof(PlaywrightPlugin));

            if (availableTools.Count == 0)
                return "No tools are currently available.";

            return FormatToolDescriptions(availableTools);
        }

        /// <summary>
        /// Formats the list of tool types into a readable description for the LLM
        /// </summary>
        /// <param name="toolTypes">List of tool type classes</param>
        /// <returns>A formatted string with tool descriptions</returns>
        private static string FormatToolDescriptions(List<Type> toolTypes)
        {
            var sb = new StringBuilder();
            sb.AppendLine("You have access to the following tools:");
            sb.AppendLine();

            foreach (var toolType in toolTypes)
            {
                // Get the tool name (class name without "Plugin" suffix)
                string toolName = toolType.Name;
                if (toolName.EndsWith("Plugin"))
                    toolName = toolName.Substring(0, toolName.Length - 6);

                sb.AppendLine($"## {toolName}");

                // Get methods that are potential kernel functions (public instance methods)
                var methods = toolType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => !m.IsSpecialName) // Filter out property accessors
                    .Where(m => m.DeclaringType != typeof(object)) // Filter out Object methods
                    .ToList();

                foreach (var method in methods)
                {
                    // Get function name
                    string functionName = method.Name;
                    
                    // Get parameters
                    var parameters = method.GetParameters();
                    string paramList = string.Join(", ", parameters.Select(p => $"{p.Name}: {p.ParameterType.Name}"));
                    
                    // Get return type
                    string returnType = method.ReturnType.Name;
                    
                    // Format function description
                    sb.AppendLine($"- `{functionName}({paramList}) -> {returnType}`");
                    
                    // Add XML documentation if available (simplified for now)
                    sb.AppendLine($"  Use this to {GetFriendlyDescription(toolType, functionName)}");
                }
                
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a friendly description for a tool function based on its name and type
        /// </summary>
        private static string GetFriendlyDescription(Type toolType, string methodName)
        {
            // This could be expanded to use XML documentation comments or attributes
            // For now we'll use simple heuristics based on the method name
            
            if (toolType == typeof(CMDPlugin))
            {
                if (methodName == "ExecuteCommand")
                    return "execute a Windows command prompt (CMD) command";
            }
            else if (toolType == typeof(PowerShellPlugin))
            {
                if (methodName == "ExecuteScript")
                    return "execute a PowerShell script or command";
            }
            else if (toolType == typeof(ScreenCaptureOmniParserPlugin))
            {
                if (methodName == "CaptureScreen")
                    return "take a screenshot of the current screen";
                if (methodName == "CaptureWindow")
                    return "take a screenshot of a specific application window";
            }
            else if (toolType == typeof(KeyboardPlugin))
            {
                if (methodName == "SendKeys")
                    return "simulate keyboard input";
                if (methodName == "TypeText")
                    return "type text using the keyboard";
            }
            else if (toolType == typeof(MousePlugin))
            {
                if (methodName == "ClickAtPosition")
                    return "perform a mouse click at specific coordinates";
                if (methodName == "MoveToPosition")
                    return "move the mouse cursor to specific coordinates";
            }
            else if (toolType == typeof(WindowSelectionPlugin))
            {
                if (methodName == "GetOpenWindows")
                    return "get a list of all open windows";
                if (methodName == "FocusWindow")
                    return "switch focus to a specific application window";
            }
            else if (toolType == typeof(PlaywrightPlugin))
            {
                // Playwright plugin method descriptions
                switch (methodName)
                {
                    case "IsBrowserActive":
                        return "check if a browser instance is already running";
                    case "GetBrowserStatus":
                        return "get detailed information about the current browser status";
                    case "LaunchBrowser":
                        return "launch a new browser instance or use the existing one";
                    case "NavigateTo":
                        return "navigate to a specified URL in the browser";
                    case "SetSessionId":
                        return "set the active session ID for browser operations";
                    case "EnableSessionPersistence":
                        return "enable or disable browser session persistence";
                    case "SaveSession":
                        return "save the current browser session for future use";
                    case "TakeScreenshot":
                        return "take a screenshot of the current browser page";
                    case "ClickElement":
                        return "click on an element identified by a CSS selector";
                    case "TypeText":
                        return "type text into an input field identified by a CSS selector";
                    case "ListSessions":
                        return "list all available saved browser sessions";
                    case "DeleteSession":
                        return "delete a saved browser session";
                    case "CloseBrowser":
                        return "close the active browser and release all resources";
                    case "GetPageContent":
                        return "get the full HTML content of the current page";
                    case "GetElementText":
                        return "get the text content of an element by CSS selector";
                    case "ExecuteScript":
                        return "execute a JavaScript snippet in the current browser page";
                }
            }

            // Generic fallback based on method name
            return methodName.ToLower().Replace("execute", "run").Replace("get", "retrieve");
        }
    }
}
