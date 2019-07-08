using System;
using System.Collections.Generic;
using Litium.FieldFramework;
using Litium.Products;

namespace Distancify.LitiumAddOns.Extensions
{
    public static class VariantServiceExtensions
    {
        public static IEnumerable<Variant> GetAllVariants(this VariantService variantService, FieldTemplateService fieldTemplateService, BaseProductService baseProductService, string templateId)
        {
            var fieldTemplate = fieldTemplateService.Get<ProductFieldTemplate>(templateId);
            var baseProducts = baseProductService.GetByTemplate(fieldTemplate.SystemId);

            foreach (var baseProduct in baseProducts)
            {
                var variants = variantService.GetByBaseProduct(baseProduct.SystemId);

                foreach (var variant in variants)
                {
                    yield return variant;
                }
            }
        }
    }
}
