using Litium.Runtime.DependencyInjection;

namespace Distancify.LitiumAddOns.ProductMedia.Mapping
{
    [Service(
        ServiceType = typeof(IMediaMapper),
        Lifetime = DependencyLifetime.Singleton)]
    public interface IMediaMapper
    {
        void Map();
    }
}
