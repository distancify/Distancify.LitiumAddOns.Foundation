using System;
using System.Collections.Generic;
using Distancify.Migrations.Litium.Seeds;
using Litium.FieldFramework;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;

namespace Distancify.LitiumAddOns.CMS.Pages
{
    [Service(ServiceType = typeof(BaseTemplateDefinition))]
    public abstract class BaseTemplateDefinition
    {
        public const string GeneralFieldGroupID = "General";

        protected string GetTemplatePath(Type controllerType, string actionName)
        {
            return $"~/MVC:{controllerType.MapTo<string>()}:{actionName}";
        }

        protected FieldTemplateFieldGroup GetBareMinimumGeneralFieldTemplateFieldGroup()
        {
            return new FieldTemplateFieldGroup()
            {
                Id = GeneralFieldGroupID,
                Collapsed = false,
                Localizations =
                {
                    [Cultures.sv_SE] = {Name = "Allmänt"},
                    [Cultures.en_US] = {Name = "General"},
                },
                Fields =
                {
                    SystemFieldDefinitionConstants.Name,
                    SystemFieldDefinitionConstants.Url
                }
            };
        }

        public virtual IEnumerable<FieldDefinition> GetFieldDefinitions() => new List<FieldDefinition>();
        public abstract IEnumerable<FieldTemplate> GetTemplates();
    }
}
