using System;
using System.Web;
using Litium.Runtime.DependencyInjection;

namespace Distancify.LitiumAddOns.Foundation.Services
{
    [Service(ServiceType = typeof(ICMSService), Lifetime = DependencyLifetime.Singleton)]
    public interface ICMSService
    {
        T GetWebsiteModel<T>(Guid websiteGuid) where T : Models.WebsiteModel, new();
        bool IsLoggedIn();
        void SetTemporaryRedirect(string currentUrl, string pageTemplateID, Guid channelSystemId, Guid websiteSystemId, HttpResponse httpResponse);
    }
}
