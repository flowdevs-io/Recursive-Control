using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Coordinates communication between the user, planner and executor agents
    /// </summary>
    public class AgentCoordinator
    {
        // Message templates for structured communication between agents
        private const string PLAN_REQUEST = "PLAN_REQUEST: {0}";
        private const string PLAN_RESPONSE = "PLAN_RESPONSE: {0}";
        private const string EXECUTION_REQUEST = "EXECUTION_REQUEST: {0}";
        private const string EXECUTION_RESPONSE = "EXECUTION_RESPONSE: {0}";
        private const string STATUS_UPDATE = "STATUS_UPDATE: {0}";
        private const string TASK_COMPLETE = "TASK_COMPLETE: {0}";
        private const string USER_REQUEST = "USER_REQUEST: {0}";
        private const string USER_RESPONSE = "USER_RESPONSE: {0}";
        private const string COORDINATION_REQUEST = "COORDINATION_REQUEST: {0}";
        private const string COORDINATION_RESPONSE = "COORDINATION_RESPONSE: {0}";
        
        private List<AgentMessage> messageHistory;
        
        public AgentCoordinator()
        {
            messageHistory = new List<AgentMessage>();
        }
        
        public void AddMessage(AgentRole sender, AgentRole recipient, string messageType, string content)
        {
            var message = new AgentMessage
            {
                Sender = sender,
                Recipient = recipient,
                MessageType = messageType,
                Content = content,
                Timestamp = DateTime.Now
            };
            
            messageHistory.Add(message);
        }
        
        public List<AgentMessage> GetMessageHistory()
        {
            return messageHistory;
        }
        
        public List<AgentMessage> GetMessagesForAgent(AgentRole agentRole)
        {
            return messageHistory.FindAll(m => m.Recipient == agentRole || m.Recipient == AgentRole.All);
        }
        
        public string FormatPlanRequest(string userRequest)
        {
            return string.Format(PLAN_REQUEST, userRequest);
        }
        
        public string FormatPlanResponse(string plan)
        {
            return string.Format(PLAN_RESPONSE, plan);
        }
        
        public string FormatExecutionRequest(string step)
        {
            return string.Format(EXECUTION_REQUEST, step);
        }
        
        public string FormatExecutionResponse(string result)
        {
            return string.Format(EXECUTION_RESPONSE, result);
        }
        
        public string FormatStatusUpdate(string status)
        {
            return string.Format(STATUS_UPDATE, status);
        }
        
        public string FormatTaskComplete(string summary)
        {
            return string.Format(TASK_COMPLETE, summary);
        }
        
        public string FormatUserRequest(string request)
        {
            return string.Format(USER_REQUEST, request);
        }
        
        public string FormatUserResponse(string response)
        {
            return string.Format(USER_RESPONSE, response);
        }
        
        public string FormatCoordinationRequest(string request)
        {
            return string.Format(COORDINATION_REQUEST, request);
        }
        
        public string FormatCoordinationResponse(string response)
        {
            return string.Format(COORDINATION_RESPONSE, response);
        }
        
        public void Clear()
        {
            messageHistory.Clear();
        }
    }
    
    public enum AgentRole
    {
        User,
        Coordinator,
        Planner,
        Executor,
        All
    }
    
    public class AgentMessage
    {
        public AgentRole Sender { get; set; }
        public AgentRole Recipient { get; set; }
        public string MessageType { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
