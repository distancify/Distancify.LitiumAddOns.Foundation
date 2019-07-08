using Litium.Application.Data.Queryable;
using Litium.Application.Products.Data;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Litium.Data.Queryable;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Distancify.LitiumAddOns.Conditions.Category
{
    public class VariantCategoryQueryExpressionInfoBuilder :
        QueryExpressionInfoBuilder<Variant, VariantEntity, CategoryQueryExpressionInfo>
    {
        public override Expression<Func<VariantEntity, bool>> Build(DbContext dbContext, QueryOptions<Variant> options, CategoryQueryExpressionInfo queryExpressionInfo)
        {
            if (queryExpressionInfo.Operator == CategoryConditionConstants.OperatorIn)
            {
                return (v) => v.BaseProduct.CategoryLinks
                    .Any(r => r.CategorySystemId == queryExpressionInfo.SystemId &&
                        r.ActiveVariants.Any(vc => vc.VariantSystemId == v.SystemId));
            }
            else if (queryExpressionInfo.Operator == CategoryConditionConstants.OperatorNotIn)
            {
                return (v) => v.BaseProduct.CategoryLinks
                    .All(r => r.CategorySystemId != queryExpressionInfo.SystemId ||
                        r.ActiveVariants.All(vc => vc.VariantSystemId != v.SystemId));
            }

            throw new ArgumentException(string.Format("No operator matching '{0}'.", (object)queryExpressionInfo.Operator));
        }
    }

    public class BaseProductCategoryQueryExpressionInfoBuilder :
        QueryExpressionInfoBuilder<BaseProduct, BaseProductEntity, CategoryQueryExpressionInfo>
    {
        public override Expression<Func<BaseProductEntity, bool>> Build(DbContext dbContext, QueryOptions<BaseProduct> options, CategoryQueryExpressionInfo queryExpressionInfo)
        {
            if (queryExpressionInfo.Operator == CategoryConditionConstants.OperatorIn)
            {
                return (p) => p.CategoryLinks
                    .Any(r => r.CategorySystemId == queryExpressionInfo.SystemId &&
                        r.ActiveVariants.Count() > 0);
            }
            else if (queryExpressionInfo.Operator == CategoryConditionConstants.OperatorNotIn)
            {
                return (p) => p.CategoryLinks
                    .All(r => r.CategorySystemId != queryExpressionInfo.SystemId || 
                        r.ActiveVariants.Count() < p.Variants.Count());
            }

            throw new ArgumentException(string.Format("No operator matching '{0}'.", (object)queryExpressionInfo.Operator));
        }
    }
}
