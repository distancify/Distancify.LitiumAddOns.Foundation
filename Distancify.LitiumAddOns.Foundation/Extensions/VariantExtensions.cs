using System;
using System.Globalization;
using Litium;
using Litium.Products;
using Litium.FieldFramework;
using System.Collections.Generic;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Products.StockStatusCalculator;
using System.Web;
using Litium.Foundation.Security;

namespace Distancify.LitiumAddOns.Extensions
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

        public static string GetTextFieldValueTranslation(this Variant variant, string fieldName, CultureInfo cultureInfo)
        {
            var fieldValue = variant.Fields.GetValue<string>(fieldName);

            if (string.IsNullOrEmpty(fieldValue))
            {
                return string.Empty;
            }

            var translation = fieldName.GetFieldDefinitionForProducts().GetTranslation(fieldValue, cultureInfo);

            return translation;
        }

        public static List<string> GetTextFieldValueTranslations(this Variant variant, string fieldName, CultureInfo cultureInfo)
        {
            var fieldValues = variant.Fields.GetValue<IEnumerable<string>>(fieldName);

            var translations = new List<string>();

            if (fieldValues != null)
            {
                foreach (var fieldValue in fieldValues)
                {
                    var translation = fieldName.GetFieldDefinitionForProducts().GetTranslation(fieldValue, cultureInfo);
                    translations.Add(translation);
                }
            }

            return translations;
        }

        public static string GetDescription(this Variant variant, CultureInfo cultureInfo, bool useBaseProductAsFallBack = false, bool useCommonMark = false)
        {
            return variant.GetTextFieldValue(SystemFieldDefinitionConstants.Description, cultureInfo, useBaseProductAsFallBack, useCommonMark);
        }

        public static string GetTextFieldValue(this Variant variant, string fieldName, CultureInfo cultureInfo, bool useBaseProductAsFallBack = false, bool useCommonMark = false)
        {
            var value = variant.Fields.GetValueWithFallback<string>(fieldName, cultureInfo);

            if (string.IsNullOrEmpty(value) && useBaseProductAsFallBack)
            {
                value = variant.GetBaseProduct().Fields.GetValueWithFallback<string>(fieldName, cultureInfo);
            }

            if (!string.IsNullOrEmpty(value) && useCommonMark)
            {
                return CommonMark.CommonMarkConverter.Convert(value);
            }
            else return value;
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
