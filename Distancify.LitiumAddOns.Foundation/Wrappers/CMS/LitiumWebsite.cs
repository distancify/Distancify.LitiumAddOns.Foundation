using System;
using System.Linq;
using Litium.Foundation.Modules.CMS.Carriers;
using Litium.Foundation.Modules.CMS.WebSites;
using Litium.Foundation.Security;

namespace Distancify.LitiumAddOns.Wrappers.CMS
{
    [Obsolete("Replaced by new domain model in Litium 7.")]
    public class LitiumWebsite
    {
        public LitiumWebsite(WebSite entity, SecurityToken securityToken)            
        {
        }

        public LitiumWebsite(SecurityToken securityToken)
        {   
        }

        public Guid Id
        {
            get { throw new NotImplementedException(); }
        }

        public string DisplayName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string DomainName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        protected WebSiteCarrier CreateCarrier()
        {
            throw new NotImplementedException();
        }

        public string GetString(string key)
        {
            throw new NotImplementedException();
        }

        public void SetString(string key, string value)
        {
            throw new NotImplementedException();
        }
    }
}