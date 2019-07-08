using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Litium.Workflows;

namespace Distancify.LitiumAddOns.PIM.WorkflowAutomations
{
    public abstract class WorkflowAutomation : IWorkflowAutomation
    {
        public abstract string Name { get; }
        
        public virtual bool CanProcess(Workflow.Task task)
        {
            return !String.IsNullOrEmpty(task.Name) &&
                task.Name.Trim().StartsWith($"[{this.Name}]", StringComparison.OrdinalIgnoreCase);
        }

        public abstract void Execute(Workflow.Task task, Guid variantSystemId);
    }
}
