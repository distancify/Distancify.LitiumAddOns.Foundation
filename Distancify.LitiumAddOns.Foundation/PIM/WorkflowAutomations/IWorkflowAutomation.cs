using System;
using Litium.Workflows;

namespace Distancify.LitiumAddOns.PIM.WorkflowAutomations
{   
    public interface IWorkflowAutomation
    {
        string Name { get; }
        bool CanProcess(Workflow.Task task);
        void Execute(Workflow.Task task, Guid variantSystemId);
    }
}
