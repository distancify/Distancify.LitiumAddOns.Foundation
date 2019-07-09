using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Products;

namespace Distancify.LitiumAddOns.Foundation.Extensions
{
    public static class PriceListExtensions
    {
        public static bool Update(this PriceList priceList, Guid variantSystemId, decimal price, bool includeVat, bool variantShouldBeInPriceList)
        {
            var includeVatUpdated = priceList.UpdateIncludeVat(includeVat);
            var productListUpdated = variantShouldBeInPriceList ? priceList.Add(variantSystemId, price) : priceList.Remove(variantSystemId);

            return productListUpdated || includeVatUpdated;
        }

        public static bool UpdateIncludeVat(this PriceList priceList, bool includeVat)
        {
            if (priceList.IncludeVat != includeVat)
            {
                priceList.IncludeVat = includeVat;

                return true;
            }
            else return false;
        }

        public static bool Add(this PriceList priceList, Guid variantSystemId, decimal price)
        {
            if (priceList.Items is null)
            {
                priceList.Items = new List<PriceListItem>()
                {
                    new PriceListItem(variantSystemId)
                    {
                        Price = price
                    }
                };

                return true;
            }
            else
            {
                var priceListItem = priceList.Items.Where(item => item.VariantSystemId.Equals(variantSystemId)).FirstOrDefault();

                if (priceListItem is PriceListItem)
                {
                    if (priceListItem.Price != price)
                    {
                        priceListItem.Price = price;

                        return true;
                    }
                }
                else
                {
                    priceList.Items.Add(new PriceListItem(variantSystemId)
                    {
                        Price = price
                    });

                    return true;
                }
            }

            return false;
        }

        public static bool Remove(this PriceList priceList, Guid variantSystemId)
        {
            var priceListItem = priceList.Items?.Where(item => item.VariantSystemId.Equals(variantSystemId)).FirstOrDefault();

            if (priceListItem is PriceListItem)
            {
                return priceList.Items.Remove(priceListItem);
            }
            else return false;
        }
    }
}