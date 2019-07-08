using Litium.Foundation.Modules.ECommerce.Campaigns;

namespace Distancify.LitiumAddOns.Wrappers.ECommerce
{
    public class LitiumCampaign : ICampaign
    {
        private Campaign _campaign;

        public LitiumCampaign(Campaign campaign)
        {
            _campaign = campaign;
        }

        public string Description => _campaign.Description;
    }
}
