using System;
using System.Collections.Generic;
using System.Linq;
using Distancify.LitiumAddOns.Wrappers.MediaArchive;
using Litium.FieldFramework;
using Litium.Runtime.DependencyInjection;
using Litium.Media;

namespace Distancify.LitiumAddOns.ProductMedia.Mapping
{
    [Service(
        ServiceType = typeof(IFieldSetter<File>),
        Lifetime = DependencyLifetime.Singleton)]
    public class MediaPointerImageArrayFieldMediaSetter : IFieldSetter<File>
    {
        private readonly IMediaArchive _mediaArchive;

        public MediaPointerImageArrayFieldMediaSetter(IMediaArchive mediaArchive)
        {
            _mediaArchive = mediaArchive;
        }

        /// <summary>
        /// Override to provide custom sorting rules to images
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        protected virtual IEnumerable<File> Sort(IEnumerable<File> files)
        {
            return files;
        } 

        public void Set(FieldContainer fields, IFieldDefinition field, File media)
        {
            var images = fields.GetValue<List<Guid>>(field.Id)?
                .Select(r => _mediaArchive.GetFile(r))
                .Where(r => r != null)
                .ToList() ?? new List<File>();

            images.RemoveAll(r => string.Equals(r.Name, media.Name, StringComparison.OrdinalIgnoreCase));
            images.Add(_mediaArchive.GetFile(media.SystemId));

            images = Sort(images).ToList();

            fields.AddOrUpdateValue(field.Id, images.Select(r => r.SystemId).ToList());
        }

        public bool CanSet(IFieldDefinition field)
        {
            return field.FieldType.Equals(SystemFieldTypeConstants.MediaPointerImage + "Array", StringComparison.Ordinal);
        }
    }
}
