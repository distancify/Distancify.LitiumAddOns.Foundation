using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Products;

namespace Distancify.LitiumAddOns.Foundation.Extensions
{
    public static class CategoryServiceExtensions
    {
        public static List<Category> GetFlattenedCategoryTree(this CategoryService categoryService, Guid assortmentId)
        {
            return categoryService.GetFlattenedCategoryTree(Guid.Empty, assortmentId);
        }

        public static List<Category> GetFlattenedCategoryTree(this CategoryService categoryService, Guid categoryId, Guid assortmentId)
        {
            var categories = new List<Category>();
            var rootCategories = categoryService.GetChildCategories(categoryId, assortmentId).ToList();
            categories.AddRange(rootCategories);

            foreach (var category in rootCategories)
            {
                categories.AddRange(categoryService.GetFlattenedCategoryTree(category.SystemId, assortmentId));
            }

            return categories;
        }

        public static IList<Category> GetAllCategories(this CategoryService categoryService, List<Guid> assortmentIds)
        {
            return assortmentIds.SelectMany(assortmentId => categoryService.GetFlattenedCategoryTree(Guid.Empty, assortmentId))
                .ToList();
        }
    }
}
