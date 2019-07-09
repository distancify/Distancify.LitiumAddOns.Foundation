using Litium.FieldFramework;
using Litium.Products;
using Litium.Workflows;
using System;
using System.Collections.Immutable;
using System.Globalization;
using Distancify.LitiumAddOns.Foundation.Extensions;

namespace Distancify.LitiumAddOns.Conditions.Category
{
    public class CategoryFilterConditionService : ConditionService<ProductArea, CategoryFilterCondition>
    {
        private readonly CategoryService categoryService;

        public CategoryFilterConditionService(CategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        private ImmutableList<string> GetPath(Guid categoryId, ImmutableList<string> path)
        {
            if (categoryService.Get(categoryId) is Litium.Products.Category category)
            {
                return GetPath(
                    category.ParentCategorySystemId,
                    path.Insert(0, category.Fields.GetValueWithFallback<string>(SystemFieldDefinitionConstants.Name, CultureInfo.CurrentCulture)));
            }
            return path;
        }

        public override string GetTitle(CategoryFilterCondition condition)
        {
            var path = GetPath(condition.CategorySystemId, ImmutableList<string>.Empty);
            return (condition.Operator == CategoryConditionConstants.OperatorIn ? "In " : "Not in ") + string.Join(" / ", path);

        }
    }
}
