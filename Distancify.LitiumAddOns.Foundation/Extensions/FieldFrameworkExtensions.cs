using System.Globalization;
using Litium;
using Litium.FieldFramework;
using Litium.Globalization;

namespace Distancify.LitiumAddOns.Foundation.Extensions
{
    public static class FieldFrameworkExtensions
    {
        public static T GetValueWithFallback<T>(this IFieldFramework fieldFramework, string fieldName, CultureInfo mainCulture)
        {
            return fieldFramework.GetValueWithFallback<T>(fieldName, mainCulture, IoC.Resolve<LanguageService>());
        }

        public static T GetValueWithFallback<T>(this IFieldFramework fieldFramework, string fieldName, CultureInfo mainCulture, LanguageService languageService)
        {
            mainCulture = mainCulture ?? CultureInfo.CurrentCulture;

            var fieldValue = fieldFramework.GetValue<T>(fieldName, mainCulture);
            if (ValueIsSet(fieldValue))
            {
                return fieldValue;
            }

            var currentLanguage = languageService?.Get(mainCulture.Name);

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
            if (ValueIsSet(uiCultureFallbackValue))
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
