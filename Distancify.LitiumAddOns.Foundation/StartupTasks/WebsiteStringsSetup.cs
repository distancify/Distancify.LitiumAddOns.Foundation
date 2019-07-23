using Distancify.LitiumAddOns.Foundation.Localization;
using Litium;
using Litium.Foundation;
using Litium.Owin.Lifecycle;
using Litium.Websites;

namespace Distancify.LitiumAddOns.Foundation.StartupTasks
{
    public class WebsiteStringsSetup : IStartupTask
    {
        private readonly WebsiteService _websiteService;

        public WebsiteStringsSetup(WebsiteService websiteService)
        {
            _websiteService = websiteService;
        }

        public void Start()
        {
            using (Solution.Instance.SystemToken.Use())
            {
                foreach (var website in _websiteService.GetAll())
                {
                    foreach (var definition in IoC.ResolveAll<WebsiteTextsDefinition>())
                    {
                        definition.Setup(website.SystemId, false);
                    }
                }
            }
        }
    }
}
