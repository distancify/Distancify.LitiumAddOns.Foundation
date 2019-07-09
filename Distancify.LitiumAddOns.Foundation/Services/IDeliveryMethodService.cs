using System;
using Litium.Foundation.Modules.ECommerce.Deliveries;
using Litium.Runtime.DependencyInjection;

namespace Distancify.LitiumAddOns.Foundation.Services
{
    [Service(ServiceType = typeof(IDeliveryMethodService), Lifetime = DependencyLifetime.Singleton)]
    public interface IDeliveryMethodService
    {
        DeliveryMethod Get(Guid deliveryMethodId);
    }
}
