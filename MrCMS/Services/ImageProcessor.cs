using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using NHibernate;

namespace MrCMS.Services
{
    public class ImageProcessor : IImageProcessor
    {
        private readonly ISession _session;

        public ImageProcessor(ISession session)
        {
            _session = session;
        }

        public MediaFile GetImage(string imageUrl)
        {
            if (imageUrl.StartsWith("/"))
            {
                imageUrl = imageUrl.Substring(1);
            }
            if (IsResized(imageUrl))
            {
                var resizePart = GetResizePart(imageUrl);
                var lastIndexOf = imageUrl.LastIndexOf(resizePart, StringComparison.Ordinal);
                imageUrl = imageUrl.Remove(lastIndexOf - 1, resizePart.Length + 1);
            }
            var fileByLocation =
                _session.QueryOver<MediaFile>()
                        .Where(file => file.FileLocation == imageUrl)
                        .Take(1)
                        .Cacheable()
                        .SingleOrDefault();

            return fileByLocation;
        }

        private bool IsResized(string imageUrl)
        {
            var resizePart = GetResizePart(imageUrl);
            if (resizePart == null) return false;

            int val;
            return new List<char> { 'w', 'h' }.Contains(resizePart[0]) && Int32.TryParse(resizePart.Substring(1), out val);
        }

        private static string GetResizePart(string imageUrl)
        {
            if (imageUrl.LastIndexOf('_') == -1 || imageUrl.LastIndexOf('.') == -1)
                return null;

            var startIndex = imageUrl.LastIndexOf('_') + 1;
            var length = imageUrl.LastIndexOf('.') - startIndex;
            if (length < 2) return null;
            var resizePart = imageUrl.Substring(startIndex, length);
            return resizePart;
        }

        public void SetFileDimensions(MediaFile mediaFile, Stream stream)
        {
            using (var b = new Bitmap(stream))
            {
                mediaFile.Width = b.Size.Width;
                mediaFile.Height = b.Size.Height;
            }
        }

        public void SaveResizedImage(MediaFile file, Size size, byte[] fileBytes, string filePath)
        {
            using (var stream = new MemoryStream(fileBytes))
            {
                using (var b = new Bitmap(stream))
                {
                    var newSize = CalculateDimensions(b.Size, size);

                    if (newSize.Width < 1)
                        newSize.Width = 1;
                    if (newSize.Height < 1)
                        newSize.Height = 1;

                    using (var newBitMap = new Bitmap(newSize.Width, newSize.Height))
                    {
                        var g = Graphics.FromImage(newBitMap);
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.DrawImage(b, 0, 0, newSize.Width, newSize.Height);
                        var ep = new EncoderParameters();
                        ep.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                        ImageCodecInfo ici = GetImageCodecInfoFromExtension(file.FileExtension)
                                             ?? GetImageCodecInfoFromMimeType("image/jpeg");
                        newBitMap.Save(filePath, ici, ep);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the first ImageCodecInfo instance with the specified mime type.
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns>ImageCodecInfo</returns>
        private ImageCodecInfo GetImageCodecInfoFromMimeType(string mimeType)
        {
            var info = ImageCodecInfo.GetImageEncoders();
            return info.FirstOrDefault(ici => ici.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns the first ImageCodecInfo instance with the specified extension.
        /// </summary>
        /// <param name="fileExt">File extension</param>
        /// <returns>ImageCodecInfo</returns>
        private ImageCodecInfo GetImageCodecInfoFromExtension(string fileExt)
        {
            fileExt = fileExt.TrimStart(".".ToCharArray()).ToLower().Trim();
            switch (fileExt)
            {
                case "jpg":
                case "jpeg":
                    return GetImageCodecInfoFromMimeType("image/jpeg");
                case "png":
                    return GetImageCodecInfoFromMimeType("image/png");
                case "gif":
                    //use png codec for gif to preserve transparency
                    //return GetImageCodecInfoFromMimeType("image/gif");
                    return GetImageCodecInfoFromMimeType("image/png");
                default:
                    return GetImageCodecInfoFromMimeType("image/jpeg");
            }
        }


        public static Size CalculateDimensions(Size originalSize, Size targetSize)
        {
            // If the target image is bigger than the source
            if (!RequiresResize(originalSize, targetSize)|| targetSize== Size.Empty)
            {
                return originalSize;
            }

            double ratio = 0;

            // What ratio should we resize it by
            double? widthRatio = targetSize.Width == 0 ? (double?) null : originalSize.Width/(double) targetSize.Width;
            double? heightRatio = targetSize.Height == 0
                                      ? (double?) null
                                      : originalSize.Height/(double) targetSize.Height;
            ratio = widthRatio.GetValueOrDefault() > heightRatio.GetValueOrDefault()
                        ? originalSize.Width / (double)targetSize.Width
                        : originalSize.Height / (double)targetSize.Height;

            var width = Math.Ceiling(originalSize.Width / ratio);
            width = targetSize.Width != 0 && width > targetSize.Width ? targetSize.Width : width;
            var resizeWidth = width;

            var height = Math.Ceiling(originalSize.Height / ratio);
            height = targetSize.Height != 0 && height > targetSize.Height ? targetSize.Height : height;
            var resizeHeight = height;

            return new Size((int)resizeWidth, (int)resizeHeight);
        }

        public static bool RequiresResize(Size originalSize, Size targetSize)
        {
            return (targetSize.Width != 0 && targetSize.Width < originalSize.Width) ||
                   (targetSize.Height != 0 && targetSize.Height < originalSize.Height);
        }

        /// <summary>
        /// Returns the name and full path of the requested file
        /// </summary>
        public static string RequestedImageFileLocation(MediaFile file, Size size)
        {
            if (file.Size == size)
                return file.FileLocation;

            var fileLocation = file.FileLocation;

            var temp = fileLocation.Replace(file.FileExtension, "");
            if(size.Width != 0)
                temp += "_w" + size.Width;
            if (size.Height != 0)
                temp += "_h" + size.Height;

            return temp + file.FileExtension;
        }
    }
}