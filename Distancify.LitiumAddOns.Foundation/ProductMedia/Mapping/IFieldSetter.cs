using Litium.FieldFramework;

namespace Distancify.LitiumAddOns.ProductMedia.Mapping
{
    public interface IFieldSetter<T>
    {
        bool CanSet(IFieldDefinition field);
        void Set(FieldContainer fields, IFieldDefinition field, T value);
    }
}
