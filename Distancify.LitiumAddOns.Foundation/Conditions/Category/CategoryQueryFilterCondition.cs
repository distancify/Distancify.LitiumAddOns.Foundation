using System;
using Litium.Data.Queryable;
using Litium.Data.Queryable.Conditions;
using Litium.Data.Queryable.ExpressionInfos;
using Litium.Data.Queryable.Internal;

namespace Distancify.LitiumAddOns.Conditions.Category
{
    class CategoryQueryFilterCondition : QueryFilterCondition<CategoryFilterCondition>
    {
        private CategoryFilterCondition _filterCondition;

        protected override void InitCondition(CategoryFilterCondition filterCondition)
        {
            _filterCondition = filterCondition;
        }

        public override void ApplyFilter<T>(FilterDescriptor<T> filterDescriptor)
        {
            ((IFilterDescriptorExpression)filterDescriptor).AddExpression(new CategoryQueryExpressionInfo { SystemId = _filterCondition.CategorySystemId, Operator = _filterCondition.Operator });
        }
    }
}
