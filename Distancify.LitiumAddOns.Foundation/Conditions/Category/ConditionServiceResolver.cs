using AutoMapper;
using JetBrains.Annotations;
using Litium.Data.Queryable.Conditions;
using Litium.Products;
using Litium.Web.Administration.Filtering;
using Litium.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.LitiumAddOns.Conditions.Category
{
    [UsedImplicitly]
    class ConditionServiceResolver<T> : IValueResolver<T, ConditionModel, string>
        where T : FilterCondition
    {
        private readonly ConditionService<ProductArea, T> _conditionService;

        public ConditionServiceResolver(ConditionService<ProductArea, T> conditionService)
        {
            _conditionService = conditionService;
        }

        public string Resolve(T source, ConditionModel destination, string destMember, ResolutionContext context)
        {
            return _conditionService.GetTitle(source);
        }
    }
}
