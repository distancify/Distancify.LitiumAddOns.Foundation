using Litium.Runtime.DependencyInjection;
using System;

namespace Distancify.LitiumAddOns.Services
{   
    [Obsolete("Use the new Relations API introduced in Litium 6", true)]
    public interface IOrganizationService
    {   
        bool EnsureOrganizationExists(string fieldTemplateName, string name, string customerNumber, string parentCustomerNumber = null);
    }
}