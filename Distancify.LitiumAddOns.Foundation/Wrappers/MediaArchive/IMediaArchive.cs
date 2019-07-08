using System;
using System.Collections.Generic;
using Litium.Media;

namespace Distancify.LitiumAddOns.Wrappers.MediaArchive
{
    public interface IMediaArchive
    {   
        File GetFile(Guid fileId);
        Folder GetFolder(string path, bool createIfMissing);
        IEnumerable<File> GetFiles(Folder folder, bool includeSubFolders);
        void MoveFile(Guid fileId, Folder targetFolder);
        void SaveChanges(File file);
    }
}
