using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Distancify.Migrations.Litium.Seeds;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Globalization;

namespace Distancify.LitiumAddOns.Foundation.Extensions
{
    public static class FieldDefinitionExtensions
    {   
        public static string GetTranslation(this IFieldDefinition field, string key, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var options = field.Option as TextOption;
            var option = options?.Items.FirstOrDefault(x => x.Value == key);
            if (option == null)
            {
                return key;
            }

            string translation;
            if (culture != null && option.Name.TryGetValue(culture.Name, out translation) && !string.IsNullOrEmpty(translation))
            {
                return translation;
            }

            var languageService = Litium.IoC.Resolve<LanguageService>();
            var currentLanguage = languageService?.Get(culture.Name);

            if (currentLanguage?.FallbackLanguages != null)
            {
                foreach (var fallbackLanguageLink in currentLanguage.FallbackLanguages)
                {
                    var fallbackLanguage = languageService.Get(fallbackLanguageLink.FallbackLanguageSystemId);

                    if (option.Name.TryGetValue(fallbackLanguage.CultureInfo.Name, out translation) && !string.IsNullOrEmpty(translation))
                    {
                        return translation;
                    }
                }
            }

            if (option.Name.TryGetValue(CultureInfo.CurrentCulture.Name, out translation) && !string.IsNullOrEmpty(translation))
            {
                return translation;
            }

            if (option.Name.TryGetValue(CultureInfo.CurrentUICulture.Name, out translation) && !string.IsNullOrEmpty(translation))
            {
                return translation;
            }

            if (option.Name.TryGetValue(Cultures.en_US, out translation) && !string.IsNullOrEmpty(translation))
            {
                return translation;
            }

            throw new KeyNotFoundException($"Unable to find key `{key ?? string.Empty}` for field {field.Id}");
        }
    }
}
