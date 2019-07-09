using System.Globalization;
using Litium.FieldFramework;
using Litium.Globalization;

namespace Distancify.LitiumAddOns.Foundation.Extensions
{
    public static class FieldFrameworkExtensions
    {
        public static T GetValueWithFallback<T>(this IFieldFramework fieldFramework, string fieldName, CultureInfo currentCulture)
        {   
            currentCulture = currentCulture ?? CultureInfo.CurrentCulture;

            var fieldValue = fieldFramework.GetValue<T>(fieldName, currentCulture);
            if (ValueIsSet(fieldValue))
            {
                return fieldValue;
            }

            var languageService = Litium.IoC.Resolve<LanguageService>();
            var currentLanguage = languageService?.Get(currentCulture.Name);

            if (currentLanguage?.FallbackLanguages != null)
            {
                foreach (var fallbackLanguageLink in currentLanguage.FallbackLanguages)
                {
                    var fallbackLanguage = languageService.Get(fallbackLanguageLink.FallbackLanguageSystemId);
                    var fallbackFieldValue = fieldFramework.GetValue<T>(fieldName, fallbackLanguage.CultureInfo);

                    if (ValueIsSet(fallbackFieldValue))
                    { 
                        return fallbackFieldValue;
                    }
                }
            }

            var uiCultureFallbackValue = fieldFramework.GetValue<T>(fieldName, CultureInfo.CurrentUICulture);
            if(ValueIsSet(uiCultureFallbackValue))
            {
                return uiCultureFallbackValue;
            }

            return default;

            bool ValueIsSet(T valueToCheck)
            {
                if (valueToCheck != null)
                {
                    return true;
                }

                if (valueToCheck is string && !string.IsNullOrEmpty(valueToCheck as string))
                {
                    return true;
                }

                return false;
            }
        }
    }
}
