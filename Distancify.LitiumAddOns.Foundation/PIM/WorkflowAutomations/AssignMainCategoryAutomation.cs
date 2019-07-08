using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Litium.Products;

namespace Distancify.LitiumAddOns.PIM.WorkflowAutomations
{
    public class AssignMainCategoryAutomation : AssignCategoryAutomation
    {
        public AssignMainCategoryAutomation(VariantService variantService, CategoryService categoryService) : base(variantService, categoryService)
        {
        }

        public override string Name => "AssignMainCategory";

        protected override bool SetMainCategory => true;
    }
}
