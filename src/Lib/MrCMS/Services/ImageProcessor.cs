using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ImageMagick;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Media;
using MrCMS.Settings;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class ImageProcessor : IImageProcessor
    {
        //match w300-h500.something or w300.something Group 0 = whole, Group 1 = With, Group 2 = height
        public const string Pattern = @"_w([0-9]+)_?h?([0-9]+)?.";
        private readonly IFileSystemFactory _fileSystemFactory;
        private readonly ISession _session;

        public ImageProcessor(ISession session, IFileSystemFactory fileSystemFactory)
        {
            _session = session;
            _fileSystemFactory = fileSystemFactory;
        }

        public async Task<(string title, string description)> GetImageMetaData(string imageUrl)
        {
            var originalImageUrl = GetOriginalImageUrl(imageUrl);
            var mediaFile =
                await _session
                    .Query<MediaFile>()
                    .Where(file => file.FileUrl == originalImageUrl)
                    .Select(x => new { x.Title, x.Description }).FirstOrDefaultAsync();

            return mediaFile != null ? (mediaFile.Title, mediaFile.Description) : (null, null);

        }

        public async Task<MediaFile> GetImage(string imageUrl)
        {
            using var disabler = new SiteFilterDisabler(_session);
            var originalImageUrl = GetOriginalImageUrl(imageUrl);
            var fileByLocation =
                await _session
                    .Query<MediaFile>()
                    .WithOptions(x => x.SetCacheable(true))
                    .FirstOrDefaultAsync(file => file.FileUrl == originalImageUrl);

            if (fileByLocation != null)
                return fileByLocation;

            var crop = await GetCrop(imageUrl);
            return crop?.MediaFile;
        }

        public async Task<Crop> GetCrop(string imageUrl)
        {
            var originalImageUrl = GetOriginalImageUrl(imageUrl);
            var crop =
                await _session.Query<Crop>().WithOptions(x => x.SetCacheable(true))
                    .FirstOrDefaultAsync(file => file.Url == originalImageUrl);
            return crop;
        }

        public void SetFileDimensions(MediaFile file, Stream stream)
        {
            stream.Position = 0;
            var b = new MagickImageInfo(stream);
            file.Width = b.Width;
            file.Height = b.Height;
            file.ContentLength = Convert.ToInt32(stream.Length);
        }


        public async Task SaveResizedImage(MediaFile file, Size size, byte[] fileBytes, string fileUrl)
        {
            Size newSize = CalculateDimensions(file.Size, size);
            await SaveFile(fileBytes, fileUrl, newSize, file.ContentType);
        }

        public async Task SaveCrop(MediaFile file, CropType cropType, Rectangle cropInfo, byte[] fileBytes,
            string fileUrl)
        {
            await SaveFile(fileBytes, fileUrl, cropType.Size, file.ContentType, cropInfo);
        }

        public void EnforceMaxSize(ref Stream stream, MediaFile file, MediaSettings mediaSettings)
        {
            if (!mediaSettings.EnforceMaxImageSize)
            {
                return;
            }

            var imageInfo = new MagickImageInfo(stream);
            file.Width = imageInfo.Width;
            file.Height = imageInfo.Height;
            file.ContentLength = stream.Length;
            if (!RequiresResize(file.Size, mediaSettings.MaxSize))
                return;


            stream.Position = 0;
            var outputStream = new MemoryStream();
            using (var collection = new MagickImageCollection(stream))
            {
                collection.Coalesce();
                foreach (var image in collection)
                {
                    MagickGeometry geometry = new MagickGeometry
                    {
                        Width = mediaSettings.MaxImageSizeWidth,
                        Height = mediaSettings.MaxImageSizeHeight
                    };
                    image.Resize(geometry);
                    image.Strip();
                }

                collection.OptimizePlus();
                collection.Write(outputStream);
                outputStream.Position = 0;
            }

            Stream originalStream = stream;
            stream = outputStream;
            originalStream.Dispose();
        }


        private async Task SaveFile(byte[] fileBytes, string fileUrl, Size newSize, string contentType,
            Rectangle? cropRectangle = null)
        {
            await using var inputStream = new MemoryStream();
            await using var outputStream = new MemoryStream();

            inputStream.Write(fileBytes, 0, fileBytes.Length);
            inputStream.Position = 0;

            using var collection = new MagickImageCollection(inputStream);
            collection.Coalesce();
            foreach (var image in collection)
            {
                MagickGeometry geometry = new MagickGeometry
                {
                    Width = newSize.Width,
                    Height = newSize.Height
                };
                image.Resize(geometry);
                if (cropRectangle.HasValue)
                {
                    Rectangle rectangle = cropRectangle.Value;
                    image.Crop(
                        new MagickGeometry
                        {
                            X = rectangle.X,
                            Y = rectangle.Y,
                            Width = rectangle.Width,
                            Height = rectangle.Height
                        });
                }

                image.Strip();
            }

            collection.OptimizePlus();
            await collection.WriteAsync(outputStream);
            outputStream.Position = 0;
            var fileSystem = _fileSystemFactory.GetForCurrentSite();
            await fileSystem.SaveFile(outputStream, fileUrl, contentType);
        }

        public async Task SaveResizedCrop(Crop crop, Size size, byte[] fileBytes, string fileUrl)
        {
            Size newSize = CalculateDimensions(crop.Size, size);
            await SaveFile(fileBytes, fileUrl, newSize, crop.ContentType);
        }

        public string GetOriginalImageUrl(string imageUrl)
        {
            var match = Regex.Match(imageUrl, Pattern, RegexOptions.Compiled);
            if (match.Success)
            {
                return imageUrl.Replace(match.Groups[0].Value, ".");
            }
            /*if (IsResized(imageUrl))
            {
                string resizePart = GetResizePart(imageUrl);
                int lastIndexOf = imageUrl.LastIndexOf(resizePart, StringComparison.OrdinalIgnoreCase);
                imageUrl = imageUrl.Remove(lastIndexOf - 1, resizePart.Length + 1);
            }*/

            return imageUrl;
        }


        public static Size? GetRequestedSize(string imageUrl)
        {
            var matches = Regex.Match(imageUrl, Pattern, RegexOptions.Compiled);
            if (matches.Length == 0) return null;

            int width = Convert.ToInt16(matches.Groups[1].Value);
            int height = 0;
            if (matches.Length == 3)
            {
                height = Convert.ToInt16(matches.Groups[2].Value);
            }

            return new Size(width, height);
        }


        public static Size CalculateDimensions(Size originalSize, Size targetSize)
        {
            // If the target image is bigger than the source
            if (!RequiresResize(originalSize, targetSize) || targetSize == Size.Empty)
            {
                return originalSize;
            }

            // What ratio should we resize it by
            double? widthRatio =
                targetSize.Width == 0 ? (double?) null : originalSize.Width / (double) targetSize.Width;
            double? heightRatio = targetSize.Height == 0
                ? (double?) null
                : originalSize.Height / (double) targetSize.Height;
            var ratio = widthRatio.GetValueOrDefault() > heightRatio.GetValueOrDefault()
                ? originalSize.Width / (double) targetSize.Width
                : originalSize.Height / (double) targetSize.Height;

            double width = Math.Ceiling(originalSize.Width / ratio);
            width = targetSize.Width != 0 && width > targetSize.Width ? targetSize.Width : width;
            double resizeWidth = width;

            double height = Math.Ceiling(originalSize.Height / ratio);
            height = targetSize.Height != 0 && height > targetSize.Height ? targetSize.Height : height;
            double resizeHeight = height;

            return new Size((int) resizeWidth, (int) resizeHeight);
        }

        public static bool RequiresResize(Size originalSize, Size targetSize)
        {
            return (targetSize.Width != 0 && targetSize.Width < originalSize.Width) ||
                   (targetSize.Height != 0 && targetSize.Height < originalSize.Height);
        }

        /// <summary>
        ///     Returns the name and full path of the requested file
        /// </summary>
        public static string RequestedFileUrl(string fileUrl, Size imageSize, Size targetSize)
        {
            if (imageSize == targetSize || !RequiresResize(imageSize, targetSize))
                return fileUrl;

            var fileExtension = Path.GetExtension(fileUrl);

            string temp = fileUrl.Replace(fileExtension, "");
            if (targetSize.Width != 0)
                temp += "_w" + targetSize.Width;
            if (targetSize.Height != 0)
                temp += "_h" + targetSize.Height;

            return temp + fileExtension;
        }
        //
        // /// <summary>
        // ///     Returns the name and full path of the requested crop
        // /// </summary>
        // public static string RequestedResizedCropFileUrl(Crop crop, Size size)
        // {
        //     string fileLocation = crop.Url;
        //
        //     if (crop.Size == size || !RequiresResize(crop.Size, size))
        //         return fileLocation;
        //
        //     string temp = fileLocation.Replace(crop.FileExtension, "");
        //     if (size.Width != 0)
        //         temp += "_w" + size.Width;
        //     if (size.Height != 0)
        //         temp += "_h" + size.Height;
        //
        //     return temp + crop.FileExtension;
        // }

        /// <summary>
        ///     Returns the name and full path of the requested crop
        /// </summary>
        public static string RequestedCropUrl(MediaFile file, CropType cropType)
        {
            string fileLocation = file.FileUrl;

            string temp = fileLocation.Replace(file.FileExtension, "");
            temp += "_c" + cropType.Id;

            return temp + file.FileExtension;
        }
    }
}