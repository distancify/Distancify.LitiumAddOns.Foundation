using System;
using System.Globalization;
using System.Web;
using Litium;
using Litium.FieldFramework;
using Litium.Foundation.Security;
using Litium.Products;
using Litium.Products.StockStatusCalculator;

namespace Distancify.LitiumAddOns.Foundation.Extensions
{
    public static class VariantExtensions
    {
        private static IStockStatusCalculator _stockStatusCalculator;
        internal static IStockStatusCalculator StockStatusCalculator
        {
            get
            {
                if(_stockStatusCalculator == null)
                {
                    _stockStatusCalculator = IoC.Resolve<IStockStatusCalculator>();
                }

                return _stockStatusCalculator;
            }
        }

        public static string GetName(this Variant variant, CultureInfo cultureInfo)
        {
            return variant.Fields.GetValueWithFallback<string>(SystemFieldDefinitionConstants.Name, cultureInfo);
        }

        public static decimal GetStockQuantity(this Variant variant, Guid websiteSystemId, Guid countrySystemId)
        {
            var stockStatus = variant.GetStockStatus(websiteSystemId, countrySystemId);

            return stockStatus?.InStockQuantity != null ? stockStatus.InStockQuantity.Value : 0;
        }

        public static StockStatusCalculatorResult GetStockStatus(this Variant variant, Guid websiteSystemId, Guid countrySystemId)
        {
            var calculatorArgs = new StockStatusCalculatorArgs
            {
                Date = HttpContext.Current?.Timestamp ?? DateTime.UtcNow,
                UserSystemId = SecurityToken.CurrentSecurityToken.UserID,
                WebSiteSystemId = websiteSystemId,
                CountrySystemId = countrySystemId
            };

            var calculatorItemArgs = new StockStatusCalculatorItemArgs
            {
                VariantSystemId = variant.SystemId
            };

            if (StockStatusCalculator.GetStockStatuses(calculatorArgs, calculatorItemArgs).TryGetValue(variant.SystemId, out StockStatusCalculatorResult calculatorResult))
            {
                return calculatorResult;
            }

            return null;
        }
    }
}
