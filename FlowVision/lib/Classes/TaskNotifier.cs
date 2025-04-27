using System;
using System.Threading.Tasks;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Helper class to manage task notifications and loading indicators
    /// </summary>
    public class TaskNotifier : IDisposable
    {
        private readonly string _taskName;
        private readonly string _taskDescription;
        private bool _disposed = false;

        /// <summary>
        /// Creates a new task notification with loading indicator
        /// </summary>
        /// <param name="taskName">The name of the task being performed</param>
        /// <param name="taskDescription">Optional description of what the task does</param>
        public TaskNotifier(string taskName, string taskDescription = null)
        {
            _taskName = taskName;
            _taskDescription = taskDescription;
            
            // Display the initial notification
            PluginLogger.NotifyTaskStart(_taskName, _taskDescription);
            
            // Start the loading indicator
            PluginLogger.StartLoadingIndicator(_taskName);
        }
        
        /// <summary>
        /// Completes the task notification and stops the loading indicator
        /// </summary>
        /// <param name="success">Whether the task completed successfully</param>
        public void Complete(bool success = true)
        {
            if (!_disposed)
            {
                PluginLogger.NotifyTaskComplete(_taskName, success);
                PluginLogger.StopLoadingIndicator();
                _disposed = true;
            }
        }


        /// <summary>
        /// Helper method to wrap an async void task with notifications
        /// </summary>
        /// <param name="taskName">Name of the task</param>
        /// <param name="description">Description of the task</param>
        /// <param name="task">The task to execute</param>
        public static async Task RunWithNotificationAsync(string taskName, string description, Func<Task> task)
        {
            var notifier = new TaskNotifier(taskName, description);
            try
            {
                await task();
                notifier.Complete(true);
            }
            catch
            {
                notifier.Complete(false);
                throw;
            }
            finally
            {
                notifier.Dispose();
            }
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                // Make sure we stop the loading indicator if this object is disposed 
                // without calling Complete
                PluginLogger.StopLoadingIndicator();
                _disposed = true;
            }
        }
    }
}
