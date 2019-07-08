using System;
using System.Linq;
using Distancify.LitiumAddOns.Wrappers.CMS;
using Litium.Globalization;
using Litium.Products;
using Litium.Workflows;

namespace Distancify.LitiumAddOns.PIM.WorkflowAutomations
{
    public class PublishProductAutomation : WorkflowAutomation
    {
        private readonly VariantService _variantService;
        private readonly ChannelService _channelService;

        public override string Name => "PublishProduct";

        public PublishProductAutomation(VariantService variantService,
            ChannelService channelService)
        {
            _variantService = variantService;
            _channelService = channelService;
        }

        public override void Execute(Workflow.Task task, Guid variantSystemId)
        {
            var channelId = task.Description;
            var variant = _variantService.Get(variantSystemId);
            var channelToPublish = _channelService.Get(channelId);

            if (channelToPublish != null && 
                !variant.ChannelLinks.Any(l=>l.ChannelSystemId.Equals(channelToPublish.SystemId)))
            {
                variant = variant.MakeWritableClone();
                variant.ChannelLinks.Add(new VariantToChannelLink(channelToPublish.SystemId));
                _variantService.Update(variant);
            }
        }
    }    
}