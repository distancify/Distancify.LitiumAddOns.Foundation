using Litium.Foundation.Modules.ECommerce.Deliveries;
using Litium.Runtime.DependencyInjection;
using System;

namespace Distancify.LitiumAddOns.Services
{
    [Service(ServiceType = typeof(IDeliveryMethodService), Lifetime = DependencyLifetime.Singleton)]
    public interface IDeliveryMethodService
    {
        DeliveryMethod Get(Guid deliveryMethodId);
    }
}
