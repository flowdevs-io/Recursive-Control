using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Manages Playwright browser sessions for persistence between application runs
    /// </summary>
    public static class PlaywrightSessionManager
    {
        private static readonly string SessionDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FlowVision", "PlaywrightSessions");

        /// <summary>
        /// Saves the browser storage state for a session
        /// </summary>
        /// <param name="sessionId">Unique identifier for the session</param>
        /// <param name="storageState">The storage state JSON string</param>
        public static void SaveStorageState(string sessionId, string storageState)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));
            }

            if (string.IsNullOrEmpty(storageState))
            {
                throw new ArgumentException("Storage state cannot be empty", nameof(storageState));
            }

            // Ensure directory exists
            Directory.CreateDirectory(SessionDirectory);

            // Sanitize session ID to be a valid filename
            string safeSessionId = SanitizeFileName(sessionId);
            
            // Save storage state to file
            string filePath = GetSessionFilePath(safeSessionId);
            File.WriteAllText(filePath, storageState);
        }

        /// <summary>
        /// Gets the storage state for a specific session
        /// </summary>
        /// <param name="sessionId">Unique identifier for the session</param>
        /// <returns>The storage state JSON string, or null if not found</returns>
        public static string GetStorageState(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));
            }

            string safeSessionId = SanitizeFileName(sessionId);
            string filePath = GetSessionFilePath(safeSessionId);

            if (!File.Exists(filePath))
            {
                return null;
            }

            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// Gets a list of all available session IDs
        /// </summary>
        /// <returns>List of session IDs</returns>
        public static List<string> GetAllSessions()
        {
            var sessions = new List<string>();

            if (!Directory.Exists(SessionDirectory))
            {
                return sessions;
            }

            foreach (var file in Directory.GetFiles(SessionDirectory, "*.json"))
            {
                // Extract session ID from filename (remove extension)
                string sessionId = Path.GetFileNameWithoutExtension(file);
                sessions.Add(sessionId);
            }

            return sessions;
        }

        /// <summary>
        /// Deletes a session
        /// </summary>
        /// <param name="sessionId">Unique identifier for the session</param>
        /// <returns>True if deleted, false if not found</returns>
        public static bool DeleteSession(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));
            }

            string safeSessionId = SanitizeFileName(sessionId);
            string filePath = GetSessionFilePath(safeSessionId);

            if (!File.Exists(filePath))
            {
                return false;
            }

            File.Delete(filePath);
            return true;
        }

        /// <summary>
        /// Validates a session ID exists
        /// </summary>
        /// <param name="sessionId">Unique identifier for the session</param>
        /// <returns>True if the session exists</returns>
        public static bool SessionExists(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return false;
            }

            string safeSessionId = SanitizeFileName(sessionId);
            string filePath = GetSessionFilePath(safeSessionId);

            return File.Exists(filePath);
        }

        /// <summary>
        /// Gets the file path for a session
        /// </summary>
        /// <param name="sessionId">Unique identifier for the session</param>
        /// <returns>Full path to the session file</returns>
        private static string GetSessionFilePath(string sessionId)
        {
            return Path.Combine(SessionDirectory, $"{sessionId}.json");
        }

        /// <summary>
        /// Sanitizes a string to be a valid filename
        /// </summary>
        /// <param name="fileName">Input filename</param>
        /// <returns>Sanitized filename</returns>
        private static string SanitizeFileName(string fileName)
        {
            // Remove invalid characters
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }

            return fileName;
        }
    }
}
