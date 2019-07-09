using Litium.Foundation.Modules.ExtensionMethods;
using System;
using System.IO;
using System.Linq;
using System.Drawing;
using File = Litium.Media.File;

namespace Distancify.LitiumAddOns.Foundation.Extensions
{
    public static class FileExtensions
    {
        private static readonly string[] SupportedImageExtensions = { ".bmp", ".gif", ".jpg", ".jpeg", ".png", ".pngx", ".tif", ".tiff", ".ico", ".icon", ".svg" };

        public static bool IsImage(this File file)
        {
            return SupportedImageExtensions.Contains(Path.GetExtension(file.Name), StringComparer.OrdinalIgnoreCase);
        }

        public static string GetUrlToImage(this File file, Size size)
        {
            return file != null ? file.GetUrlToImage(size.Height, size.Width, size.Height, size.Width) : string.Empty;
        }
    }
}