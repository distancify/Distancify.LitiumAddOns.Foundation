using System.Configuration;
using System.Reflection;
using Distancify.LitiumAddOns.Foundation.Email;
using Distancify.LitiumAddOns.MediaMapper;
using Distancify.LitiumAddOns.MediaMapper.Services;
using Distancify.LitiumAddOns.PIM.WorkflowAutomations;
using Distancify.LitiumAddOns.Tasks.Synchronization;
using Litium.Owin.InversionOfControl;


namespace Distancify.LitiumAddOns.Foundation
{
    public class ServiceRegistration : IComponentInstaller
    {
        public void Install(IIoCContainer container, Assembly[] assemblies)
        {
            container.For<IAutomatedUserSetup>().ImplementedBy<AutomatedUserSetup>().RegisterAsScoped();
            container.For<IWorkflowAutomation>().RegisterAsScoped();
            container.For<IAutomationRunner>().ImplementedBy<ProductAutomationRunner>().RegisterAsScoped();

            container.For<IEmailSender>().ImplementedBy<EmailSender>().RegisterAsScoped();

            container.For<ITaskSynchronizer>().ImplementedBy<TaskSynchronizer>().RegisterAsSingleton();
        }
    }
}
