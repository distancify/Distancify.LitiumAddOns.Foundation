using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Litium;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models;
using Litium.Web.Models.Websites;
using Litium.Websites;

namespace Distancify.LitiumAddOns.Foundation.Extensions
{
    public static class PageModelExtensions
    {
        private static PageService _pageService;
        internal static PageService PageService
        {
            get {
                if (_pageService == null)
                {
                    _pageService = IoC.Resolve<PageService>();
                }

                return _pageService;
            }
        }

        private static FieldTemplateService _fieldTemplateService;
        internal static FieldTemplateService FieldTemplateService
        {
            get {
                if (_fieldTemplateService == null)
                {
                    _fieldTemplateService = IoC.Resolve<FieldTemplateService>();
                }

                return _fieldTemplateService;
            }
        }

        private static FieldDefinitionService _fieldDefinitionService;
        internal static FieldDefinitionService FieldDefinitionService
        {
            get {
                if (_fieldDefinitionService == null)
                {
                    _fieldDefinitionService = IoC.Resolve<FieldDefinitionService>();
                }

                return _fieldDefinitionService;
            }
        }

        private static T GetPropertyValue<T>(PageModel pageModel, string propertyName)
        {
            var field = FieldDefinitionService.Get<WebsiteArea>(propertyName);

            if (field.MultiCulture)
            {
                return pageModel.Fields.GetValueWithFallback<T>(propertyName, CultureInfo.CurrentUICulture);
            }

            return pageModel.Fields.GetValue<T>(propertyName);
        }

        public static Guid GetPropertyGuidValue(this PageModel pageModel, string propertyName)
        {
            return GetPropertyValue<Guid>(pageModel, propertyName);
        }

        public static PointerItem GetPropertyPointerItemValue(this PageModel pageModel, string propertyName)
        {
            return GetPropertyValue<PointerItem>(pageModel, propertyName);
        }

        public static List<Guid> GetPropertyGuidValues(this PageModel pageModel, string propertyName)
        {
            return GetPropertyValue<List<Guid>>(pageModel, propertyName);
        }

        public static IList<PointerItem> GetPropertyPointerItemValues(this PageModel pageModel, string propertyName)
        {
            return GetPropertyValue<IList<PointerItem>>(pageModel, propertyName);
        }

        public static string GetPropertyStringValue(this PageModel pageModel, string propertyName)
        {
            return GetPropertyValue<string>(pageModel, propertyName);
        }

        public static int? GetPropertyIntegerValue(this PageModel pageModel, string propertyName)
        {
            return GetPropertyValue<int?>(pageModel, propertyName);
        }

        public static decimal? GetPropertyDecimalValue(this PageModel pageModel, string propertyName)
        {
            return GetPropertyValue<decimal?>(pageModel, propertyName);
        }

        public static DateTime? GetPropertyDateValue(this PageModel pageModel, string propertyName)
        {
            return GetPropertyValue<DateTime?>(pageModel, propertyName);
        }

        public static ImageModel GetPropertyImageValue(this PageModel pageModel, string propertyName)
        {
            return GetPropertyValue<Guid>(pageModel, propertyName).MapTo<ImageModel>();
        }

        public static Guid GetCategoryIdPropertyValue(this PageModel pageModel, string propertyName)
        {
            return pageModel.GetPropertyGuidValue(propertyName);
        }

        public static PointerPageItem GetPointerPageItemPropertyValue(this PageModel pageModel, string propertyName)
        {
            return GetPropertyValue<PointerPageItem>(pageModel, propertyName);
        }

        public static string GetImagePropertyUrl(this PageModel pageModel, string propertyName, int maxHeight, int maxWidth, int minHeight, int minWidth)
        {
            return pageModel.GetPropertyImageValue(propertyName)?.GetUrlToImage(new Size(minWidth, minHeight), new Size(maxWidth, maxHeight))?.Url ?? string.Empty;
        }

        public static IEnumerable<PageModel> GetPublishedChildrenOfType(this PageModel pageModel, Guid fieldTemplateSystemId)
        {
            return PageService.GetChildPages(pageModel.SystemId, pageModel.Page.WebsiteSystemId)
                .Where(PageHasCorrectFieldTemplateAndIsPublished)
                .Select(p => p.MapTo<PageModel>());

            bool PageHasCorrectFieldTemplateAndIsPublished(Page p)
                => p.FieldTemplateSystemId.Equals(fieldTemplateSystemId) &&
                   p.Status == Litium.Common.ContentStatus.Published;
        }

        public static string GetFieldTemplateId(this PageModel pageModel)
        {
            return FieldTemplateService.Get<PageFieldTemplate>(pageModel.Page.FieldTemplateSystemId)?.Id;
        }

        public static IEnumerable<Guid> GetParentIDs(this PageModel pageModel)
        {
            if (pageModel.Page.ParentPageSystemId.Equals(Guid.Empty))
            {
                yield break;
            }

            yield return pageModel.Page.ParentPageSystemId;

            foreach (var parentId in GetParentIDs(pageModel.Page.ParentPageSystemId.MapTo<PageModel>()))
            {
                yield return parentId;
            }
        }

    }
}