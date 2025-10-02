using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.AI;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Helper class to extract AITools from plugin instances
    /// </summary>
    public static class PluginToolExtractor
    {
        /// <summary>
        /// Extracts all public instance methods from a plugin and converts them to AITools
        /// </summary>
        public static List<AITool> ExtractTools(object plugin)
        {
            var tools = new List<AITool>();
            var pluginType = plugin.GetType();
            
            var methods = pluginType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var method in methods)
            {
                // Skip methods from Object base class and special methods (properties, etc.)
                if (method.DeclaringType == typeof(object) || method.IsSpecialName)
                    continue;
                
                // Skip methods declared in parent types (only get methods from the plugin itself)
                if (method.DeclaringType != pluginType)
                    continue;
                
                try
                {
                    var tool = AIFunctionFactory.Create(method, plugin);
                    tools.Add(tool);
                }
                catch (Exception)
                {
                    // Skip methods that can't be converted to tools
                    continue;
                }
            }
            
            return tools;
        }
    }
}
