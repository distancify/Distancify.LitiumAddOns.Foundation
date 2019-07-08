using System;

namespace Distancify.LitiumAddOns.Wrappers.CMS
{
    [Obsolete("Replaced by new domain model in Litium 7.")]
    public interface IWebsite : IEntity
    {   
        string DisplayName { get; set; }        
        string DomainName { get; set; }

        string GetString(string key);
        void SetString(string key, string value);
    }
}
