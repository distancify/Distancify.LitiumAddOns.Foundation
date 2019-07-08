using System;
using System.Net;
using System.Web;
using Distancify.LitiumAddOns.Extensions;
using Litium;
using Litium.FieldFramework;
using Litium.Foundation.Security;
using Litium.Runtime.AutoMapper;
using Litium.Web;
using Litium.Web.Models.Websites;
using Litium.Websites;

namespace Distancify.LitiumAddOns.Foundation.Services
{
    public class CMSService : ICMSService
    {
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly PageService _pageService;
        private readonly UrlService _urlService;
        private readonly WebsiteService _websiteService;

        public CMSService(FieldTemplateService fieldTemplateService, PageService pageService, UrlService urlService, WebsiteService websiteService)
        {
            _fieldTemplateService = fieldTemplateService;
            _pageService = pageService;
            _urlService = urlService;
            _websiteService = websiteService;
        }

        public T GetWebsiteModel<T>(Guid websiteGuid) where T : Models.WebsiteModel, new()
        {
            var websiteModel = new T();
            var website = _websiteService.Get(websiteGuid);
            websiteModel.MapFrom(website);

            return websiteModel;
        }

        public bool IsLoggedIn()
        {
            return !IoC.Resolve<SecurityToken>().IsAnonymousUser;
        }

        public void SetTemporaryRedirect(string currentUrl, string pageTemplateID, Guid channelSystemId, Guid websiteSystemId, HttpResponse httpResponse)
        {
            var page = _websiteService.Get(websiteSystemId).MapTo<WebsiteModel>().GetSinglePageInstance(pageTemplateID);
            var url = GetUrl(page, channelSystemId);

            if (!url.Equals(currentUrl))
            {
                httpResponse.Redirect(url, false);
                httpResponse.StatusCode = (int)HttpStatusCode.TemporaryRedirect;
                httpResponse.End();
            }
        }

        private string GetUrl(Page page, Guid channelSystemId)
        {
            return _urlService.GetUrl(page, new PageUrlArgs(channelSystemId));
        }
    }
}
