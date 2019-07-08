using System;
using Litium.FieldFramework;
using Litium.Media;

namespace Distancify.LitiumAddOns.Extensions
{
    public static class MediaFieldTemplateServiceExtensions
    {
        public static FileFieldTemplate GetDefaultMediaFileTemplate(this FieldTemplateService fieldTemplateService)
        {
            return fieldTemplateService.FindFileTemplate("jpg");
        }
    }
}
