using System;

namespace Distancify.LitiumAddOns.Wrappers.MediaArchive
{
    [Obsolete("File metadata replaced by Fields and Field Templates in Litium 6", true)]
    public interface IFileMetadataProvider
    {
        string GetMetadata(Guid fileId, string key);
        void SetMetadata(Guid fileId, string key, string value);
    }
}