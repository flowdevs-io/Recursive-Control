using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowVision.lib.Classes
{
    /// <summary>
    /// Represents the different roles in a multi-agent workflow
    /// </summary>
    public enum AgentRole
    {
        User,
        Coordinator,
        Planner,
        Executor
    }
}
