using System;
using Litium.Websites;

namespace Distancify.LitiumAddOns.Foundation.Models
{
    public class WebsiteModel
    {
        public string Id { get; set; }
        public Guid SystemId { get; set; }

        public virtual void MapFrom(Website website)
        {
            Id = website.Id;
            SystemId = website.SystemId;
        }
    }
}
