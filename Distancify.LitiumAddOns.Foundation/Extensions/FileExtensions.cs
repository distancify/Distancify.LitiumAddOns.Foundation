using System;
using System.IO;
using System.Linq;
using File = Litium.Media.File;

namespace Distancify.LitiumAddOns.Extensions
{
    public static class FileExtensions
    {
        private static readonly string[] SupportedImageExtensions = { ".bmp", ".gif", ".jpg", ".jpeg", ".png", ".pngx", ".tif", ".tiff", ".ico", ".icon", ".svg" };

        public static bool IsImage(this File file)
        {
            return SupportedImageExtensions.Contains(Path.GetExtension(file.Name), StringComparer.OrdinalIgnoreCase);
        }
    }
}