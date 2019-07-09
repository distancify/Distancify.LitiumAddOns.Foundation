using System.Collections.Generic;
using System.Globalization;
using Litium.FieldFramework;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Products;

namespace Distancify.LitiumAddOns.Foundation.Extensions
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

        public static string GetName(this BaseProduct baseProduct, CultureInfo cultureInfo)
        {
            return baseProduct.Fields.GetValueWithFallback<string>(SystemFieldDefinitionConstants.Name, cultureInfo);
        }
    }
}