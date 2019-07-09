using System;
using System.Linq;
using Distancify.SerilogExtensions;
using Litium.Globalization;
using Litium.Runtime.DependencyInjection;
using Litium.Studio.Builders.Attributes;
using Litium.Studio.Builders.CMS;
using Litium.Studio.Builders.ReflectionExtensions;
using Litium.Websites;

namespace Distancify.LitiumAddOns.Localization
{
    [Service(ServiceType = typeof(WebsiteTextsDefinition), Lifetime = DependencyLifetime.Transient)]
    public abstract class WebsiteTextsDefinition
    {
        public void Setup(Guid websiteId, bool overwriteExisting)
        {
            var websiteService = Litium.IoC.Resolve<WebsiteService>();

            var website = websiteService.Get(websiteId);
            if (website == null)
            {
                return;
            }

            var dirty = false;
            website = website.MakeWritableClone();

            var channelService = Litium.IoC.Resolve<ChannelService>();
            var channel = channelService.GetAll().FirstOrDefault(c => c.WebsiteSystemId.HasValue && c.WebsiteSystemId.Equals(website.SystemId));
            if (channel == null)
            {
                return;
            }

            var languageService = Litium.IoC.Resolve<LanguageService>();
            var language = languageService.Get(channel.WebsiteLanguageSystemId.Value);

            var culture = language.CultureInfo.Name;
            var typeUnproxied = this.GetTypeUnproxied();

            try
            {
                foreach (var property in typeUnproxied.GetProperties())
                {
                    var key = property.Name;
                    var translations = Enumerable.Cast<TranslationAttribute>(typeUnproxied.GetProperty(property.Name).GetCustomAttributes(typeof(TranslationAttribute), true));
                    var translationAttribute = translations.FirstOrDefault(x => culture.Equals(x.Culture, StringComparison.InvariantCultureIgnoreCase)) ?? translations.FirstOrDefault(x => x.Culture == null);

                    if (translationAttribute != null)
                    {
                        var stringAttribute = PropertyInfoExtensions.GetCustomAttributes<StringAttribute>(typeUnproxied.GetProperty(property.Name)).FirstOrDefault();
                        if (stringAttribute != null && !string.IsNullOrEmpty(stringAttribute.Prefix))
                            key = stringAttribute.Prefix + key;

                        var overwrite = overwriteExisting || translationAttribute.Overwrite;
                        var currentText = website.Texts[key, culture];

                        if (currentText == null || (overwrite && !translationAttribute.Text.Equals(currentText)))
                        {
                            website.Texts.AddOrUpdateValue(key, culture, translationAttribute.Text);
                            dirty = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Log().Error(ex, "Unable to set up website texts");
            }
            finally
            {
                if (dirty)
                {
                    websiteService.Update(website);
                }
            }
        }
    }
}
