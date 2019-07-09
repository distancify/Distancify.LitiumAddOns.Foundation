using System;
using System.Collections.Generic;
using System.Linq;
using Litium;
using Litium.Common;
using Litium.FieldFramework;
using Litium.Web.Models.Websites;
using Litium.Websites;

namespace Distancify.LitiumAddOns.Foundation.Extensions
{
    public static class WebSiteExtensions
    {
        private static PageService _pageService;
        internal static PageService PageService
        {
            get
            {
                if(_pageService == null)
                {
                    _pageService = IoC.Resolve<PageService>();
                }

                return _pageService;
            }
        }

        private static FieldTemplateService _fieldTemplateService;
        internal static FieldTemplateService FieldTemplateService
        {
            get
            {
                if (_fieldTemplateService == null)
                {
                    _fieldTemplateService = IoC.Resolve<FieldTemplateService>();
                }

                return _fieldTemplateService;
            }
        }

        public static Page GetSinglePageInstance(this WebsiteModel webSite, string fieldTemplate)
        {
            var template = FieldTemplateService.Get<PageFieldTemplate>(fieldTemplate);

            return webSite.GetSinglePageInstance(template != null ? template.SystemId : Guid.Empty);
        }

        public static Page GetSinglePageInstance(this WebsiteModel webSite, Guid fieldTemplateSystemId)
        {   
            var pageInstances = webSite.GetPageInstances(fieldTemplateSystemId, 2).ToList();

            if (pageInstances.Count == 0)
            {
                return null;
            }

            if (pageInstances.Count > 1)
            {
                throw new Exception($"Page type {fieldTemplateSystemId} has multiple instances in the same website(ID: {webSite.SystemId}).");
            }

            return pageInstances[0];
        }

        public static IEnumerable<Page> GetPageInstances(this WebsiteModel webSite, Guid fieldTemplateSystemId, int? max = null)
        {
            var publishedPages = webSite.GetPublishedPages(fieldTemplateSystemId);

            if (max != null)
            {
                if (max < 1)
                {
                    throw new Exception("max must be positive.");
                }

                publishedPages = publishedPages.Take(max.Value);
            }

            return publishedPages;
        }

        public static IEnumerable<Page> GetPublishedPages(this WebsiteModel webSite, Guid fieldTemplateSystemId)
        {
            return webSite.GetChildPages().Where(p => p.FieldTemplateSystemId.Equals(fieldTemplateSystemId) && p.Status == ContentStatus.Published);
        }

        public static IEnumerable<Page> GetChildPages(this WebsiteModel website)
        {
            foreach(var page in website.GetChildPages(Guid.Empty))
            {
                yield return page;
            }
        }

        public static IEnumerable<Page> GetChildPages(this WebsiteModel website, Guid parentPageId)
        {
            foreach (var page in PageService.GetChildPages(parentPageId, website.SystemId))
            {
                yield return page;

                foreach (var child in website.GetChildPages(page.SystemId))
                {
                    yield return child;
                }
            }
        }
    }
}