using System;
using System.Collections.Generic;
using Distancify.LitiumAddOns.Wrappers.Exceptions;
using Litium.Foundation.Modules.CMS;
using Litium.Foundation.Security;

namespace Distancify.LitiumAddOns.Wrappers.CMS
{
    [Obsolete("Replaced by new domain model in Litium 7.")]
    public class LitiumWebsiteRepository : IWebsiteRepository
    {
        /// <summary>
        ///     Creates a new LitiumWebsiteRepository with the specified security token.
        /// </summary>
        public LitiumWebsiteRepository(SecurityToken securityToken)
        {   
        }

        /// <summary>
        ///     Creates a new LitiumWebsiteRepository with current state's security token.
        /// </summary>
        public LitiumWebsiteRepository()
        {
        }

        public IWebsite GetByDisplayName(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Guid> GetWebsiteIds()
        {
            throw new NotImplementedException();
        }
        
        public IWebsite Find(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}