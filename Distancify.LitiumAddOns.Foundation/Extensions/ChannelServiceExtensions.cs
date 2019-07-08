using Litium.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.LitiumAddOns.Extensions
{
    public static class ChannelServiceExtensions
    {
        public static IEnumerable<Channel> GetChannelsForCurrency(this ChannelService channelService, CountryService countryService, Guid currencySystemId)
        {
            var countriesSystemIds = countryService.GetAll().Where(c => c.CurrencySystemId == currencySystemId).Select(c => c.SystemId);

            return channelService.GetAll().Where(c => c.CountryLinks.Any(l => countriesSystemIds.Contains(l.CountrySystemId)));
        }
    }
}
