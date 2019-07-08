using Litium.Foundation.Modules.ECommerce.Orders;
using System;

namespace Distancify.LitiumAddOns.Models
{
    public class OrderModel
    {
        public string Id { get; set; }
        public Guid SystemId { get; set; }

        public virtual void MapFrom(Order order)
        {
            Id = order.ExternalOrderID;
            SystemId = order.ID;
        }
    }
}
