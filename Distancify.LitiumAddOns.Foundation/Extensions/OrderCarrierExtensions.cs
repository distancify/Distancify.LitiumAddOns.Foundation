using System.Linq;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Payments;

namespace Distancify.LitiumAddOns.Extensions
{
    public static class OrderCarrierExtensions
    {
        public static string GetAdditionalOrderInfoValue(this OrderCarrier orderCarrier, string key)
        {
            return orderCarrier.GetAdditionalOrderInfoCarrier(key)?.Value;
        }

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

        public static void RemoveAdditionalOrderInfoValue(this OrderCarrier orderCarrier, string key)
        {
            var additionalInfoCarrier = orderCarrier.GetAdditionalOrderInfoCarrier(key);

            if (additionalInfoCarrier != null)
            {
                orderCarrier.AdditionalOrderInfo.Remove(additionalInfoCarrier);
            }
        }

        private static AdditionalOrderInfoCarrier GetAdditionalOrderInfoCarrier(this OrderCarrier orderCarrier, string key)
        {
            return orderCarrier.AdditionalOrderInfo.FirstOrDefault(x => x.Key == key && !x.CarrierState.IsMarkedForDeleting);
        }

        public static bool PaymentStatusEquals(this OrderCarrier orderCarrier, PaymentStatus paymentStatus)
        {
            var paymentInfoList = orderCarrier.PaymentInfo.Where(pic => !pic.CarrierState.IsMarkedForDeleting);

            if (paymentInfoList.Count() == 0)
            {
                return false;
            }

            var paymentStatusAsShort = (short) paymentStatus;

            foreach (var paymentInfo in paymentInfoList)
            {
                if (paymentInfo.PaymentStatus != paymentStatusAsShort)
                {
                    return false;
                }
            }

            return true;
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
