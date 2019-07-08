using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Data;
using Litium.FieldFramework;
using Litium.Media;
using Litium.Media.Queryable;

namespace Distancify.LitiumAddOns.Wrappers.MediaArchive
{
    public class MediaArchive : IMediaArchive
    {
        private const string DefaultFolderTemplate = "DefaultFolderTemplate";

        private readonly FolderService _folderService;
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly FileService _fileService;
        private readonly DataService _dataService;
        
        public MediaArchive(FolderService folderService,
            FieldTemplateService fieldTemplateService,
            FileService fileService,
            DataService dataService)
        {
            _folderService = folderService;
            _fieldTemplateService = fieldTemplateService;
            _fileService = fileService;
            _dataService = dataService;
        }

        public File GetFile(Guid fileId)
        {
            return _fileService.Get(fileId);
        }
        
        public IEnumerable<File> GetFiles(Folder folder, bool includeSubFolders)
        {
            if(folder == null)
            {
                yield break;
            }

            using (var query = _dataService.CreateQuery<File>())
            {
                query.Filter(descriptor => FileFilterDescriptorExtensions.FolderSystemId(descriptor, folder.SystemId));

                foreach(var fileId in query.ToSystemIdList())
                {
                    yield return _fileService.Get(fileId);
                }
            }

            if (includeSubFolders)
            {
                var childFolders = _folderService.GetChildFolders(folder.SystemId);
                foreach (var childFolder in childFolders)
                {
                    foreach (var file in GetFiles(childFolder, true))
                    {
                        yield return file;
                    }
                }
            }
        }

        public Folder GetFolder(string path, bool createIfMissing)
        {
            var parts = path.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            var current = (Folder)null;            

            // Iterate down the path to find/create the requested folder
            foreach (var folderName in parts)
            {
                var childFolders = _folderService.GetChildFolders(current != null ?  current.SystemId : Guid.Empty);
                if (childFolders.Any(f => string.Equals(f.Name, folderName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    current = childFolders.Single(r => r.Name.Equals(folderName, StringComparison.InvariantCultureIgnoreCase));
                }
                else if (createIfMissing)
                {
                    var fieldTemplateSystemId = current != null ?
                        current.FieldTemplateSystemId :
                        _fieldTemplateService.Get<FolderFieldTemplate>(DefaultFolderTemplate).SystemId;

                    var newFolder = new Folder(fieldTemplateSystemId, folderName)
                    {
                        ParentFolderSystemId = current != null ? current.SystemId : Guid.Empty,
                        SystemId = Guid.NewGuid()
                    };

                    _folderService.Create(newFolder);

                    current = newFolder;
                }
                else
                {
                    return null;
                }
            }

            return current;
        }

        public void MoveFile(Guid fileId, Folder targetFolder)
        {
            var file = _fileService.Get(fileId);

            if (ReferenceEquals(file, null)) throw new ArgumentNullException("fileId");
            if (ReferenceEquals(targetFolder, null)) throw new ArgumentNullException("folder");
            
            var existingFiles = GetFiles(targetFolder, false).Where(
                    r => r.Name.Equals(file.Name, StringComparison.InvariantCultureIgnoreCase) && !r.SystemId.Equals(file.SystemId)).ToList();
            
            foreach (var e in existingFiles)
            {
                _fileService.Delete(e);
            }

            file = file.MakeWritableClone();
            file.FolderSystemId = targetFolder.SystemId;
            _fileService.Update(file);
        }

        public void SaveChanges(File file)
        {
            _fileService.Update(file);
        }
    }
}