using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using FlowVision.lib.Interfaces;
using FlowVision.lib.Controls;
using System.ComponentModel;

namespace FlowVision.lib.Classes
{
    public class LLMPluginInteractionManager
    {
        private static readonly Lazy<LLMPluginInteractionManager> _instance = 
            new Lazy<LLMPluginInteractionManager>(() => new LLMPluginInteractionManager());
        
        public static LLMPluginInteractionManager Instance => _instance.Value;

        private LogViewer _logViewer;
        private ConcurrentQueue<LogEntry> _logQueue = new ConcurrentQueue<LogEntry>();
        private Timer _logProcessTimer;
        private bool _isProcessingLogs = false;
        private readonly object _lockObject = new object();
        private readonly int _maxLogEntries = 1000;
        private readonly List<LogEntry> _logHistory = new List<LogEntry>();

        // Event for real-time notification when a new log is added
        public event EventHandler<LogEntryEventArgs> LogAdded;

        private LLMPluginInteractionManager()
        {
            // Initialize timer to process logs every 100ms
            _logProcessTimer = new Timer(ProcessLogs, null, 100, 100);
        }

        public void SetLogViewer(LogViewer logViewer)
        {
            _logViewer = logViewer;
        }

        public void LogPluginActivity(string pluginName, string message, LogLevel level)
        {
            var logEntry = new LogEntry
            {
                Timestamp = DateTime.Now,
                PluginName = pluginName,
                Message = message,
                Level = level
            };

            _logQueue.Enqueue(logEntry);
            _logHistory.Add(logEntry);

            // Keep log history within limits
            if (_logHistory.Count > _maxLogEntries)
            {
                _logHistory.RemoveAt(0);
            }

            // Raise event for real-time monitoring
            OnLogAdded(new LogEntryEventArgs(logEntry));
        }

        public List<LogEntry> GetLogHistory()
        {
            return new List<LogEntry>(_logHistory);
        }

        public List<LogEntry> FilterLogs(string pluginName = null, LogLevel? level = null, 
            DateTime? startTime = null, DateTime? endTime = null)
        {
            IEnumerable<LogEntry> filteredLogs = _logHistory;

            if (!string.IsNullOrEmpty(pluginName))
                filteredLogs = filteredLogs.Where(log => log.PluginName == pluginName);

            if (level.HasValue)
                filteredLogs = filteredLogs.Where(log => log.Level == level.Value);

            if (startTime.HasValue)
                filteredLogs = filteredLogs.Where(log => log.Timestamp >= startTime.Value);

            if (endTime.HasValue)
                filteredLogs = filteredLogs.Where(log => log.Timestamp <= endTime.Value);

            return filteredLogs.ToList();
        }

        // Process logs in batches for better performance
        private void ProcessLogs(object state)
        {
            if (_isProcessingLogs || _logViewer == null) return;

            lock (_lockObject)
            {
                _isProcessingLogs = true;
                int processedCount = 0;
                try
                {
                    while (_logQueue.TryDequeue(out LogEntry logEntry) && processedCount < 50)
                    {
                        _logViewer.AppendLogEntry(logEntry);
                        processedCount++;
                    }
                }
                finally
                {
                    _isProcessingLogs = false;
                }
            }
        }

        protected virtual void OnLogAdded(LogEntryEventArgs e)
        {
            LogAdded?.Invoke(this, e);
        }

        // Clean up resources
        public void Dispose()
        {
            _logProcessTimer?.Dispose();
        }
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string PluginName { get; set; }
        public string Message { get; set; }
        public LogLevel Level { get; set; }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss.fff}] [{Level}] [{PluginName}] {Message}";
        }
    }

    public class LogEntryEventArgs : EventArgs
    {
        public LogEntry LogEntry { get; }

        public LogEntryEventArgs(LogEntry logEntry)
        {
            LogEntry = logEntry;
        }
    }

    // Base implementation of IPluginLogger that plugins can use
    public class PluginLoggerBase : IPluginLogger
    {
        public string PluginName { get; private set; }

        public PluginLoggerBase(string pluginName)
        {
            PluginName = pluginName;
        }

        public void LogInfo(string message)
        {
            LLMPluginInteractionManager.Instance.LogPluginActivity(PluginName, message, LogLevel.Info);
        }

        public void LogError(string message)
        {
            LLMPluginInteractionManager.Instance.LogPluginActivity(PluginName, message, LogLevel.Error);
        }

        public void LogWarning(string message)
        {
            LLMPluginInteractionManager.Instance.LogPluginActivity(PluginName, message, LogLevel.Warning);
        }
    }
}
