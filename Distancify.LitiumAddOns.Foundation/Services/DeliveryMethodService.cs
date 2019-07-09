using System;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Deliveries;

namespace Distancify.LitiumAddOns.Foundation.Services
{
    public class DeliveryMethodService : IDeliveryMethodService
    {
        private readonly ModuleECommerce _moduleECommerce;

        public DeliveryMethodService(ModuleECommerce moduleECommerce)
        {
            _moduleECommerce = moduleECommerce;
        }

        public DeliveryMethod Get(Guid deliveryMethodId)
            => _moduleECommerce.DeliveryMethods.Get(deliveryMethodId, _moduleECommerce.AdminToken);
    }
}
