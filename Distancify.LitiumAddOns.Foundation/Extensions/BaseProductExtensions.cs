using System.Globalization;
using Litium.Products;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.FieldFramework;
using System.Collections.Generic;
using System;

namespace Distancify.LitiumAddOns.Extensions
{
    public static class BaseProductExtensions
    {
        public static List<string> GetTranslatedTextOptionValues(this BaseProduct baseProduct, string fieldName, CultureInfo cultureInfo)
        {
            var fieldValueAsObject = baseProduct.Fields.GetValue<object>(fieldName);
            if(fieldValueAsObject == null)
            {
                return new List<string>();
            }

            var fieldValues = fieldValueAsObject as IEnumerable<string> ?? new List<string> { fieldValueAsObject.ToString() };

            var translations = new List<string>();

            if (fieldValues != null)
            {
                foreach (var fieldValue in fieldValues)
                {
                    if(string.IsNullOrEmpty(fieldValue))
                    {
                        continue;
                    }

                    var translation = fieldName.GetFieldDefinitionForProducts().GetTranslation(fieldValue, cultureInfo);
                    translations.Add(translation);
                }
            }

            return translations;
        }

        [Obsolete("Use `GetTranslatedTextOptionValues` instead")]
        public static List<string> GetTextFieldValueTranslations(this BaseProduct baseProduct, string fieldName, CultureInfo cultureInfo)
        {
            var fieldValues = baseProduct.Fields.GetValue<IEnumerable<string>>(fieldName);

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

        [Obsolete("Use `GetTranslatedTextOptionValues` instead and select the first entry in the return list")]
        public static string GetTextFieldValueTranslation(this BaseProduct baseProduct, string fieldName, CultureInfo cultureInfo)
        {
            var fieldValue = baseProduct.Fields.GetValue<string>(fieldName);

            if (string.IsNullOrEmpty(fieldValue))
            {
                return string.Empty;
            }

            var translation = fieldName.GetFieldDefinitionForProducts().GetTranslation(fieldValue, cultureInfo);

            return translation;
        }

        public static string GetZeroTrimmedStringValueFromDecimalField(this BaseProduct baseProduct, string fieldName, CultureInfo cultureInfo, decimal multiplier = 1)
        {
            decimal fieldValue = baseProduct.Fields.GetValue<decimal>(fieldName);
            string result = (multiplier == 1 ? fieldValue : fieldValue * multiplier).ToString(cultureInfo);
            var decimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;

            if (result.Contains(decimalSeparator))
            {
                result = result.TrimEnd('0').TrimEnd(decimalSeparator);
            }

            return result;
        }

        public static string GetName(this BaseProduct baseProduct, CultureInfo cultureInfo)
        {
            return baseProduct.Fields.GetValueWithFallback<string>(SystemFieldDefinitionConstants.Name, cultureInfo);
        }
    }
}