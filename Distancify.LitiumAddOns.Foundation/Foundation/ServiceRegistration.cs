using System.Reflection;
using Distancify.LitiumAddOns.Foundation.Foundation.Email;
using Distancify.LitiumAddOns.Foundation.Tasks.Synchronization;
using Litium.Owin.InversionOfControl;

namespace Distancify.LitiumAddOns.Foundation.Foundation
{
    public class ServiceRegistration : IComponentInstaller
    {
        public void Install(IIoCContainer container, Assembly[] assemblies)
        {
            container.For<IEmailSender>().ImplementedBy<EmailSender>().RegisterAsScoped();
            container.For<ITaskSynchronizer>().ImplementedBy<TaskSynchronizer>().RegisterAsSingleton();
        }
    }
}
