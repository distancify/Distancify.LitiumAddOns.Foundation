using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Deliveries;
using System;

namespace Distancify.LitiumAddOns.Services
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
