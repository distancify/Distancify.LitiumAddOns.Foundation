using Distancify.LitiumAddOns.Foundation;
using Distancify.LitiumAddOns.PIM.WorkflowAutomations;

namespace Distancify.LitiumAddOns.Tasks
{
    public class WorkflowAutomationsTask : NonConcurrentTask
    {
        private readonly IAutomatedUserSetup _automatedUserSetup;
        private readonly IAutomationRunner _productAutomations;        

        public WorkflowAutomationsTask(IAutomatedUserSetup automatedUserSetup, 
            IAutomationRunner productAutomations)
        {
            _automatedUserSetup = automatedUserSetup;
            _productAutomations = productAutomations;
        }

        protected override void Run()
        {
            _automatedUserSetup.EnsureUserExists();

            _productAutomations.Run();
        }
    }
}