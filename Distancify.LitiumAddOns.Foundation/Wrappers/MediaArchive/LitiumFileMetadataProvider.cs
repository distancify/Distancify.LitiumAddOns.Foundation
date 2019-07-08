using System;
using Litium.Foundation;
using Litium.Media;

namespace Distancify.LitiumAddOns.Wrappers.MediaArchive
{
    [Obsolete("File metadata replaced by Fields and Field Templates in Litium 6", true)]
    public class LitiumFileMetadataProvider : IFileMetadataProvider
    {   
        public string GetMetadata(Guid fileId, string metadataKey)
        {
            throw new NotImplementedException();
        }

        public void SetMetadata(Guid fileId, string key, string value)
        {
            throw new NotImplementedException();
        }
    }
}