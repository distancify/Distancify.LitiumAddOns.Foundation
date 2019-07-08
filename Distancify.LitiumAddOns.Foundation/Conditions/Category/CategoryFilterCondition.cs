using AutoMapper;
using Litium.Data.Queryable.Conditions;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Web.Administration.Filtering;
using Litium.Workflows;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.LitiumAddOns.Conditions.Category
{
    public class CategoryFilterCondition : FilterCondition
    {
        public string FieldId { get; set; }
        public string Operator { get; set; }
        public Guid CategorySystemId { get; set; }

        internal class Mapper : IAutoMapperConfiguration
        {
            public void Configure(IMapperConfigurationExpression cfg)
            {
                cfg.CreateMap<CategoryFilterCondition, ConditionModel>()
                    .ForMember(r => r.Value, m => m.MapFrom(p => p.CategorySystemId))
                    .ForMember(x => x.Field, m => m.MapFrom(p => p.FieldId))
                    .ForMember(x => x.FieldTitle, m => m.MapFrom<ConditionServiceResolver<CategoryFilterCondition>>());
            }
        }

        
    }
}
