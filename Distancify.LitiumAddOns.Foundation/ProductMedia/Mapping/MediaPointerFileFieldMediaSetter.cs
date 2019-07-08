using System;
using Litium.FieldFramework;
using Litium.Media;
using Litium.Runtime.DependencyInjection;

namespace Distancify.LitiumAddOns.ProductMedia.Mapping
{
    [Service(
        ServiceType = typeof(IFieldSetter<File>),
        Lifetime = DependencyLifetime.Singleton)]
    public class MediaPointerFileFieldMediaSetter : IFieldSetter<File>
    {
        public void Set(FieldContainer fields, IFieldDefinition field, File media)
        {
            fields.AddOrUpdateValue(field.Id, media.SystemId);
        }

        public bool CanSet(IFieldDefinition field)
        {
            return field.FieldType.Equals(SystemFieldTypeConstants.MediaPointerFile, StringComparison.Ordinal);
        }
    }
}
