using System.Linq;
using Litium.Foundation.Modules.ECommerce.Orders;

namespace Distancify.LitiumAddOns.Extensions
{
    public static class OrderExtensions
    {
        public static string GetAdditionalOrderInfoValue(this Order order, string key)
        {
            return order.AdditionalOrderInfo.SingleOrDefault(aoi => aoi.AdditionalOrderInfoKey.Equals(key))?.AdditionalOrderInfoValue;
        }
    }
}
