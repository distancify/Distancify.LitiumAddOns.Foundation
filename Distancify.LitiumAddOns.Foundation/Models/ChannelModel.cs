﻿using Litium.Globalization;
using System;

namespace Distancify.LitiumAddOns.Models
{
    public class ChannelModel
    {
        public string Id { get; set; }
        public Guid SystemId { get; set; }
        public Guid? WebsiteSystemId { get; set; }
        public bool ShowPricesWithVat { get; set; }

        public virtual void MapFrom(Channel channel)
        {
            Id = channel.Id;
            SystemId = channel.SystemId;
            WebsiteSystemId = channel.WebsiteSystemId;
            ShowPricesWithVat = channel.ShowPricesWithVat;
        }
    }
}