using Litium.Runtime.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.LitiumAddOns.Foundation.CMS.PermanentRedirects
{
    [Service(ServiceType = typeof(IPermanentRedirectsService), Lifetime = DependencyLifetime.Singleton)]
    public interface IPermanentRedirectsService
    {
        bool HasPermanentRedirect(string oldUrl);
    }
}
