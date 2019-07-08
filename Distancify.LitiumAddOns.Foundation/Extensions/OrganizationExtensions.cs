using Litium.Customers;
using System;

namespace Distancify.LitiumAddOns.Extensions
{
    public static class OrganizationExtensions
    {
        [Obsolete("Use `Organization.Fields.GetValue` instead", true)]
        public static string GetCustomStringFieldValue(this Organization organization, string fieldName)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Use `Organization.Fields.GetValue` instead", true)]
        public static double GetCustomNumberFieldValue(this Organization organization, string fieldName)
        {
            throw new NotImplementedException();
        }
    }
}