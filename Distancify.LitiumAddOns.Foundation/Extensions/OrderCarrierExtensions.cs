using System.Linq;
using Litium.Foundation.Modules.ECommerce.Carriers;

namespace Distancify.LitiumAddOns.Foundation.Extensions
{
    public static class OrderCarrierExtensions
    {
        public static void SetAdditionalOrderInfoValue(this OrderCarrier orderCarrier, string key, string value)
        {
            var additionalInfoCarrier = orderCarrier.GetAdditionalOrderInfoCarrier(key);

            if (additionalInfoCarrier == null)
            {
                orderCarrier.AdditionalOrderInfo.Add(new AdditionalOrderInfoCarrier(key, orderCarrier.ID, value));
            }
            else if (additionalInfoCarrier.Value != value)
            {
                additionalInfoCarrier.Value = value;
            }
        }

        private static AdditionalOrderInfoCarrier GetAdditionalOrderInfoCarrier(this OrderCarrier orderCarrier, string key)
        {
            return orderCarrier.AdditionalOrderInfo.FirstOrDefault(x => x.Key == key && !x.CarrierState.IsMarkedForDeleting);
        }

        public static void SetAdditionalOrderInfoValueIfNotNullOrEmpty(this OrderCarrier orderCarrier, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                orderCarrier.SetAdditionalOrderInfoValue(key, value);
            }
        }
    }
}
