using Litium;
using Litium.Application.Web.Routing;
using Litium.Foundation.Modules.CMS;
using Litium.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.LitiumAddOns.Foundation.CMS.PermanentRedirects
{
    public class PermanentRedirectsService : IPermanentRedirectsService
    {
        private readonly RoutingHelperService _routingHelperService;

        public PermanentRedirectsService(RoutingHelperService routingHelperService)
        {
            _routingHelperService = routingHelperService;
        }

        public bool HasPermanentRedirect(string oldUrl)
        {
            return _routingHelperService.TryGetUrlRedirect(oldUrl, out _, out _);
        }
    }
}
