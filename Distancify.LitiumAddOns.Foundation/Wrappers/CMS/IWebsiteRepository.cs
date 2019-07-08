using System;
using System.Collections.Generic;

namespace Distancify.LitiumAddOns.Wrappers.CMS
{
    [Obsolete("Replaced by new domain model in Litium 7.")]
    public interface IWebsiteRepository : IRepository<IWebsite>
    {   
        IWebsite GetByDisplayName(string name);        
        IEnumerable<Guid> GetWebsiteIds();
    }
}
