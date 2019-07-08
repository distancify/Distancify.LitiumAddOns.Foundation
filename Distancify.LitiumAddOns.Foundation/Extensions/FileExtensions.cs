using Litium.Foundation.Modules.ExtensionMethods;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using File = Litium.Media.File;

namespace Distancify.LitiumAddOns.Extensions
{
    public static class FileExtensions
    {
        private static string[] _supportedImageExtensions = new[] { ".bmp", ".gif", ".jpg", ".jpeg", ".png", ".pngx", ".tif", ".tiff", ".ico", ".icon", ".svg" };

        public static bool IsImage(this File file)
        {
            return _supportedImageExtensions.Contains(Path.GetExtension(file.Name), StringComparer.OrdinalIgnoreCase);
        }

        public static string GetUrlToImage(this File file, Size size)
        {
            return file != null ? file.GetUrlToImage(size.Height, size.Width, size.Height, size.Width) : string.Empty;
        }
    }
}