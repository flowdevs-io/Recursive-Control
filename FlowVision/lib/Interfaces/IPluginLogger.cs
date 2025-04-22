using System;

namespace FlowVision.lib.Interfaces
{
    /// <summary>
    /// Interface for standardized logging across plugins
    /// </summary>
    public interface IPluginLogger
    {
        /// <summary>
        /// Log an informational message
        /// </summary>
        void LogInfo(string message);
        
        /// <summary>
        /// Log an error message
        /// </summary>
        void LogError(string message);
        
        /// <summary>
        /// Log a warning message
        /// </summary>
        void LogWarning(string message);
        
        /// <summary>
        /// Plugin name for identification in logs
        /// </summary>
        string PluginName { get; }
    }
}
