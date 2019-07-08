using System;
using System.Linq;
using Distancify.LitiumAddOns.Wrappers.CMS;
using Litium.Globalization;
using Litium.Products;
using Litium.Workflows;

namespace Distancify.LitiumAddOns.PIM.WorkflowAutomations
{
    public class UnpublishProductAutomation : WorkflowAutomation
    {
        private readonly VariantService _variantService;
        private readonly ChannelService _channelService;

        public override string Name => "UnpublishProduct";

        public UnpublishProductAutomation(VariantService variantService,
            ChannelService channelService)
        {
            _variantService = variantService;
            _channelService = channelService;
        }

        public override void Execute(Workflow.Task task, Guid variantSystemId)
        {
            var channelId = task.Description;
            var variant = _variantService.Get(variantSystemId);
            
            var channel = _channelService.Get(channelId);
            if (channel != null && variant.ChannelLinks.Any(l => l.ChannelSystemId.Equals(channel.SystemId)))
            {
                variant = variant.MakeWritableClone();
                variant.ChannelLinks.Remove(variant.ChannelLinks.First(l => l.ChannelSystemId.Equals(channel.SystemId)));
                _variantService.Update(variant);
            }
        }
    }
}
