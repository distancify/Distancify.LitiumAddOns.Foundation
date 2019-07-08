using Distancify.LitiumAddOns.Wrappers.ECommerce;
using Litium.Runtime.DependencyInjection;
using System;

namespace Distancify.LitiumAddOns.Services
{
    [Service(ServiceType = typeof(ICampaignService), Lifetime = DependencyLifetime.Singleton)]
    public interface ICampaignService
    {
        ICampaign Get(Guid campaignSystemId);
    }
}
