using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Products;

namespace Distancify.LitiumAddOns.Foundation.Extensions
{
    public static class BaseProductServiceExtensions
    {
        public static bool AddVariantToCategory(this BaseProductService productService, Guid categorySystemId, BaseProduct baseProduct, Guid variantSystemId)
        {
            var categoryLink = baseProduct.CategoryLinks.FirstOrDefault(category => category.CategorySystemId.Equals(categorySystemId));

            if (categoryLink == null)
            {
                categoryLink = new BaseProductToCategoryLink(categorySystemId)
                {
                    ActiveVariantSystemIds = new HashSet<Guid>()
                };
                baseProduct.CategoryLinks.Add(categoryLink);
            }

            if (!categoryLink.ActiveVariantSystemIds.Contains(variantSystemId))
            {
                categoryLink.ActiveVariantSystemIds.Add(variantSystemId);

                return true;
            }

            return false;
        }

        public static bool RemoveVariantFromCategory(this BaseProductService productService, Guid categorySystemId, BaseProduct baseProduct, Guid variantSystemId)
        {
            var categoryLink = baseProduct.CategoryLinks.FirstOrDefault(category => category.CategorySystemId.Equals(categorySystemId));

            if (categoryLink is BaseProductToCategoryLink && categoryLink.ActiveVariantSystemIds.Contains(variantSystemId))
            {
                if (categoryLink.ActiveVariantSystemIds.Count == 1)
                {
                    baseProduct.CategoryLinks.Remove(categoryLink);
                }
                else
                {
                    categoryLink.ActiveVariantSystemIds.Remove(variantSystemId);
                }

                return true;
            }

            return false;
        }
    }
}
