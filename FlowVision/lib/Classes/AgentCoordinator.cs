using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Coordinates and tracks messages between different agents in a multi-agent system.
    /// Maintains a record of interactions between different agent roles.
    /// </summary>
    public class AgentCoordinator
    {
        private List<AgentMessage> _messageHistory;

        /// <summary>
        /// A read-only view of the message history for debugging and tracking
        /// </summary>
        public IReadOnlyList<AgentMessage> MessageHistory => _messageHistory.AsReadOnly();

        public AgentCoordinator()
        {
            _messageHistory = new List<AgentMessage>();
        }

        /// <summary>
        /// Add a message to the coordination history
        /// </summary>
        /// <param name="fromRole">The agent role that sent the message</param>
        /// <param name="toRole">The agent role that received the message</param>
        /// <param name="messageType">The type/purpose of the message</param>
        /// <param name="content">The actual message content</param>
        public void AddMessage(AgentRole fromRole, AgentRole toRole, string messageType, string content)
        {
            var message = new AgentMessage
            {
                Timestamp = DateTime.Now,
                FromRole = fromRole,
                ToRole = toRole,
                MessageType = messageType,
                Content = content
            };

            _messageHistory.Add(message);
        }

        /// <summary>
        /// Retrieve all messages between specific agent roles
        /// </summary>
        /// <param name="fromRole">Source role (or null for any source)</param>
        /// <param name="toRole">Target role (or null for any target)</param>
        /// <returns>List of messages matching the criteria</returns>
        public List<AgentMessage> GetMessages(AgentRole? fromRole = null, AgentRole? toRole = null)
        {
            return _messageHistory.Where(m =>
                (!fromRole.HasValue || m.FromRole == fromRole.Value) &&
                (!toRole.HasValue || m.ToRole == toRole.Value)
            ).ToList();
        }

        /// <summary>
        /// Get the most recent message of a specific type
        /// </summary>
        /// <param name="messageType">Type of message to retrieve</param>
        /// <returns>The most recent message of that type, or null if none exists</returns>
        public AgentMessage GetLatestMessageOfType(string messageType)
        {
            return _messageHistory
                .Where(m => m.MessageType == messageType)
                .OrderByDescending(m => m.Timestamp)
                .FirstOrDefault();
        }

        /// <summary>
        /// Clears the message history
        /// </summary>
        public void Clear()
        {
            _messageHistory.Clear();
        }
    }

    /// <summary>
    /// Represents a message passed between agents in the multi-agent system
    /// </summary>
    public class AgentMessage
    {
        public DateTime Timestamp { get; set; }
        public AgentRole FromRole { get; set; }
        public AgentRole ToRole { get; set; }
        public string MessageType { get; set; }
        public string Content { get; set; }
    }
}
