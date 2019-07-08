using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.LitiumAddOns.Extensions
{
    public static class PriceListServiceExtensions
    {
        public static void Create(this PriceListService priceListService, string priceListId, Guid currencySystemId, bool includeVat, decimal price, List<Guid> variantSystemIds)
        {
            var priceList = new PriceList(currencySystemId)
            {
                Id = priceListId,
                SystemId = Guid.NewGuid(),
                Active = true,
                IncludeVat = includeVat,
                Items = variantSystemIds.Distinct().Select(variantSystemId => new PriceListItem(variantSystemId)
                {
                    Price = price
                }).ToList()
            };

            priceListService.Create(priceList);
        }
    }
}