using Litium.Data.Queryable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.LitiumAddOns.Conditions.Category
{
    public class CategoryQueryExpressionInfo : QueryExpressionInfo
    {
        public string Operator { get; set; }
        public Guid SystemId { get; set; }
    }
}
