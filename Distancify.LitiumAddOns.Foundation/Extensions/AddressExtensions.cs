using System.Collections.Generic;
using System.Linq;

namespace Distancify.LitiumAddOns.Foundation.Extensions
{
    public static class AddressExtensions
    {
        public static IList<string> GetAsListOfStrings(this Litium.Foundation.Modules.ECommerce.Addresses.Address address)
        {
            var lines = new[] {
                GetFirstLine(),
                address.CareOf,
                address.Address1,
                address.Address2,
                $"{address.Zip} {address.City}",
                address.Country
            };

            return lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

            string GetFirstLine()
                => string.IsNullOrWhiteSpace(address.OrganizationName) ?
                    $"{address.FirstName} {address.LastName}" :
                    address.OrganizationName;
        }
    }
}