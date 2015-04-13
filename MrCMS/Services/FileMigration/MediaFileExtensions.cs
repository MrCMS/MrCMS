using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Services.FileMigration
{
    public static class MediaFileExtensions
    {
        public static IFileSystem GetFileSystem(this MediaFile file, IEnumerable<IFileSystem> possibleFileSystems)
        {
            return possibleFileSystems.FirstOrDefault(system => system.Exists(file.FileUrl));
        }

        public static bool IsImage(this MediaFile file)
        {
            return file != null && ImageExtensions.Any(s => s.Equals(file.FileExtension, StringComparison.InvariantCultureIgnoreCase));
        }
        public static bool IsJpeg(this MediaFile file)
        {
            return file != null && JpegExtensions.Any(s => s.Equals(file.FileExtension, StringComparison.InvariantCultureIgnoreCase));
        }
        public static IEnumerable<ImageSize> GetSizes(this MediaFile file)
        {
            if (!IsImage(file)) 
                yield break;
            yield return new ImageSize("Original", file.Size);
            foreach (
                var imageSize in
                    MrCMSApplication.Get<MediaSettings>()
                        .ImageSizes.Where(size => ImageProcessor.RequiresResize(file.Size, size.Size)))
            {
                imageSize.ActualSize = ImageProcessor.CalculateDimensions(file.Size, imageSize.Size);
                yield return imageSize;
            }
        }


        public static readonly HashSet<string> JpegExtensions = new HashSet<string> { ".jpg", ".jpeg" }; 
        public static readonly List<string> ImageExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png" };
    }
}