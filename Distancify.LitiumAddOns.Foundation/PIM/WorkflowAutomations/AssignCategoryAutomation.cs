using System;
using System.Linq;
using Litium.Workflows;
using Litium.Products;
using Distancify.LitiumAddOns.Conditions.Category;
using Distancify.SerilogExtensions;

namespace Distancify.LitiumAddOns.PIM.WorkflowAutomations
{
    public class AssignCategoryAutomation : WorkflowAutomation
    {
        private readonly CategoryService _categoryService;
        private readonly VariantService _variantService;

        public AssignCategoryAutomation(VariantService variantService,
            CategoryService categoryService)
        {
            _variantService = variantService;
            _categoryService = categoryService;
        }

        public override string Name => "AssignCategory";

        protected virtual bool SetMainCategory => false;
        
        private void AddToCategory(Guid variantSystemId, Guid categoryId)
        {
            if (_categoryService.Get(categoryId)?.MakeWritableClone() is Category category &&
                _variantService.Get(variantSystemId) is Variant variant)
            {
                var link = category.ProductLinks.FirstOrDefault(r => r.BaseProductSystemId == variant.BaseProductSystemId);
                if (link == null)
                {
                    link = new CategoryToProductLink(variant.BaseProductSystemId);
                    category.ProductLinks.Add(link);
                }
                if (!link.ActiveVariantSystemIds.Contains(variant.SystemId))
                {
                    link.ActiveVariantSystemIds.Add(variant.SystemId);
                }
                link.MainCategory = SetMainCategory;

                _categoryService.Update(category);
            }
            else
            {
                this.Log().Warning("Could not add variant to category - category {CategoryId} or variant {VariantId} not found", categoryId, variantSystemId);
            }
        }

        private void RemoveFromCategory(Guid variantSystemId, Guid categoryId)
        {
            if (_categoryService.Get(categoryId)?.MakeWritableClone() is Category category &&
                _variantService.Get(variantSystemId) is Variant variant)
            {
                var link = category.ProductLinks.FirstOrDefault(r => r.BaseProductSystemId == variant.BaseProductSystemId);
                if (link != null)
                {
                    category.ProductLinks.Remove(link);
                }

                _categoryService.Update(category);
            }
            else
            {
                this.Log().Warning("Could not remove variant from category - category {CategoryId} or variant {VariantId} not found", categoryId, variantSystemId);
            }
        }

        public override void Execute(Workflow.Task task, Guid variantSystemId)
        {
            var categoryFilterConditions = task.Conditions.Select(r => r.Data).OfType<CategoryFilterCondition>();

            foreach (var c in categoryFilterConditions)
            {
                if (c.Operator == CategoryConditionConstants.OperatorNotIn)
                {
                    AddToCategory(variantSystemId, c.CategorySystemId);
                }
                else if (c.Operator == CategoryConditionConstants.OperatorIn)
                {
                    RemoveFromCategory(variantSystemId, c.CategorySystemId);
                }
            }

            
        }
    }
}
