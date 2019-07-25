using Litium.Runtime.DependencyInjection;
using System.IO;

namespace Distancify.LitiumAddOns.Foundation.Net
{
    [Service(ServiceType = typeof(IFtpService), Lifetime = DependencyLifetime.Transient)]
    public interface IFtpService
    {
        bool Put(string host, int port, string user, string password, string fileNameAndPath, Stream content, FtpConnectionType connectionType, bool ignoreSslErrors);
    }

    public enum FtpConnectionType
    {
        FTP,
        FTPS,
        SFTP
    }
}
