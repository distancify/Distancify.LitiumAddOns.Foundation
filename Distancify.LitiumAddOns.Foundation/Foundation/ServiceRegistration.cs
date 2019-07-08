using System.Reflection;
using Distancify.LitiumAddOns.Foundation.Email;
using Distancify.LitiumAddOns.Foundation.InversionOfControl;
using Distancify.LitiumAddOns.PIM.WorkflowAutomations;
using Distancify.LitiumAddOns.ProductMedia.Mapping;
using Distancify.LitiumAddOns.Tasks.Synchronization;
using Distancify.LitiumAddOns.Wrappers.CMS;
using Distancify.LitiumAddOns.Wrappers.MediaArchive;
using Litium.Foundation.Modules.MediaArchive;
using Litium.Owin.InversionOfControl;

namespace Distancify.LitiumAddOns.Foundation
{
    public class ServiceRegistration : IComponentInstaller
    {
        public void Install(IIoCContainer container, Assembly[] assemblies)
        {
            container.For<IAutomatedUserSetup>().ImplementedBy<AutomatedUserSetup>().RegisterAsScoped();

            container.For<IMediaArchive>().ImplementedBy<MediaArchive>().RegisterAsScoped();
            container.For<IMediaMapper>().ImplementedBy<MediaMapper>().RegisterAsScoped();

            container.For<IWorkflowAutomation>().RegisterAsScoped();
            container.For<IAutomationRunner>().ImplementedBy<ProductAutomationRunner>().RegisterAsScoped();

            container.For<IEmailSender>().ImplementedBy<EmailSender>().RegisterAsScoped();

            container.For<ITaskSynchronizer>().ImplementedBy<TaskSynchronizer>().RegisterAsSingleton();
        }
    }
}
