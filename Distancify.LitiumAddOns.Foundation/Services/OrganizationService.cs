using Distancify.LitiumAddOns.Tasks.Synchronization;
using Litium.Foundation.Modules.Relations;
using Litium.Foundation.Modules.Relations.Carriers;
using Litium.Foundation.Security;
using System;

namespace Distancify.LitiumAddOns.Services
{
    [Obsolete("Use the new Relations API introduced in Litium 6", true)]
    public class OrganizationService : IOrganizationService
    {   
        public bool EnsureOrganizationExists(string fieldTemplateName, string name, string customerNumber, string parentCustomerNumber = null)
        {
            throw new NotImplementedException();
        }
    }
}