using Litium.Runtime.DependencyInjection;
using Litium.Web.Administration.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Litium.Data.Queryable.Conditions;
using Litium.Products;
using System.Text.RegularExpressions;
using Distancify.LitiumAddOns.Conditions.Category;
using Litium.Data.Queryable;
using Litium.Runtime.AutoMapper;
using AutoMapper;
using Litium.Data.Queryable.Internal;
using System.Globalization;
using Litium.FieldFramework;
using Distancify.LitiumAddOns.Extensions;
using FilterOperator = Litium.Web.Administration.Filtering.FilterOperator;

namespace Distancify.LitiumAddOns.Conditions
{   
    public class CategoryFilterService : IFilterService<BaseProduct>
    {
        private readonly CategoryService _categoryService;
        private readonly AssortmentService _assortmentService;
        private readonly Regex _fieldIdParser = new Regex($"^{CategoryConditionConstants.FieldPrefix}(?<id>[a-z0-9-]+$)");

        public CategoryFilterService(AssortmentService assortmentService, CategoryService categoryService)
        {
            _assortmentService = assortmentService;
            _categoryService = categoryService;
        }

        public TDescriptor ApplyFilter<TDescriptor>([NotNull] TDescriptor filterDescriptor, [NotNull] FilterArgs filterArgs) where TDescriptor : IFilterDescriptorExpression
        {
            object obj = filterArgs.Value;
            if (Guid.TryParse(obj != null ? obj.ToString() : null, out var result))
                filterDescriptor.AddExpression(new CategoryQueryExpressionInfo()
                {
                    Operator = filterArgs.Operator,
                    SystemId = result
                });
            else
                filterDescriptor.AddExpression(new CategoryQueryExpressionInfo()
                {
                    Operator = filterArgs.Operator
                });
            return filterDescriptor;
        }

        public FilterCondition CreateFilterCondition([NotNull] FilterArgs filterArgs)
        {
            return new Category.CategoryFilterCondition
            {
                FieldId = filterArgs.Field,
                Operator = filterArgs.Operator,
                CategorySystemId = Guid.Parse(filterArgs.Value as string)
            };
        }

        public FilterModel GetFilterItems()
        {
            return new FilterModel()
            {
                Filters = _assortmentService.GetAll()
                   .Select(r => new FilterItem
                   {
                       Field = CategoryConditionConstants.FieldPrefix + r.SystemId,
                       Title = r.Localizations.CurrentCulture.Name
                   }).ToList()
            };
        }

        public IEnumerable<FilterOperator> GetFilterOperators([NotNull] string fieldId, string culture)
        {
            var assortmentId = GetAssortmentId(fieldId);

            Func<string, string, Guid, string[], IList<FilterOperator>> getChildCategories = null;
            getChildCategories = (string fieldTitle, string @operator, Guid parent, string[] parents) =>
            {
                return _categoryService
                    .GetChildCategories(parent, assortmentId)
                    .OrderBy(x => x.SortIndex)
                    .SelectMany(x =>
                    {
                        var path = parents.Union(new[] { x.Fields.GetValueWithFallback<string>(SystemFieldDefinitionConstants.Name, CultureInfo.CurrentCulture) });
                        var result = new List<FilterOperator>();
                        result.Add(new FilterOperator
                        {
                            Field = fieldId,
                            FieldTitle = fieldTitle,
                            Operator = @operator,
                            OperatorTitle = string.Join(" / ", path),
                            Culture = "*",
                            Value = x.SystemId.ToString()
                        });
                        result.AddRange(getChildCategories(fieldTitle, @operator, x.SystemId, path.ToArray()));
                        return result;
                    })
                    .ToList();
            };

            return new List<FilterOperator>
            {
                new FilterOperator
                {
                    Field = fieldId,
                    FieldTitle = "In Category",
                    OperatorTitle = "In Category",
                    Culture = "*",
                    List = getChildCategories("In Category", CategoryConditionConstants.OperatorIn, Guid.Empty, new string[0])
                },
                new FilterOperator
                {
                    Field = fieldId,
                    FieldTitle = "Not in Category",
                    OperatorTitle = "Not in Category",
                    Culture = "*",
                    List = getChildCategories("Not in Category", CategoryConditionConstants.OperatorNotIn, Guid.Empty, new string[0])
                }
            };
        }

        public bool ServiceMatch([NotNull] string fieldId)
        {
            return fieldId.StartsWith(CategoryConditionConstants.FieldPrefix);
        }

        private Guid GetAssortmentId(string fieldId)
        {
            var m = _fieldIdParser.Match(fieldId);
            if (!m.Success) return Guid.Empty;
            return Guid.Parse(m.Groups["id"].Value);
        }
    }
}
