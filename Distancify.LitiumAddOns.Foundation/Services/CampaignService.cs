using System;
using Distancify.LitiumAddOns.Wrappers.ECommerce;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Campaigns;

namespace Distancify.LitiumAddOns.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ModuleECommerce _moduleECommerce;

        public CampaignService(ModuleECommerce moduleECommerce)
        {
            _moduleECommerce = moduleECommerce;
        }

        public ICampaign Get(Guid campaignSystemId)
            => new LitiumCampaign(_moduleECommerce.Campaigns.GetCampaign(campaignSystemId, _moduleECommerce.AdminToken));
    }
}
