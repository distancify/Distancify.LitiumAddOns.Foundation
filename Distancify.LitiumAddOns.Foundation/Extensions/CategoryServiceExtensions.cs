using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.LitiumAddOns.Extensions
{
    public static class CategoryServiceExtensions
    {
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
    }
}