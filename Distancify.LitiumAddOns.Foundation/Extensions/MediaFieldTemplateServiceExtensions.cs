using Litium.FieldFramework;
using Litium.Media;

namespace Distancify.LitiumAddOns.Foundation.Extensions
{
    public static class MediaFieldTemplateServiceExtensions
    {
        public static FileFieldTemplate GetDefaultMediaFileTemplate(this FieldTemplateService fieldTemplateService)
        {
            return fieldTemplateService.FindFileTemplate("jpg");
        }
    }
}
