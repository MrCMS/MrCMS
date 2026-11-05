using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Media;
using MrCMS.Settings;
using NHibernate;
using NHibernate.Linq;
using SkiaSharp;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;

namespace MrCMS.Services
{
    public class ImageProcessor : IImageProcessor
    {
        //match w300-h500.something or w300.something Group 0 = whole, Group 1 = With, Group 2 = height
        public const string Pattern = @"_w([0-9]+)_?h?([0-9]+)?.";
        // matches ImageSharp's default JPEG quality, which this replaced
        private const int JpegQuality = 75;
        private readonly IFileSystemFactory _fileSystemFactory;
        private readonly ISession _session;

        public ImageProcessor(ISession session, IFileSystemFactory fileSystemFactory)
        {
            _session = session;
            _fileSystemFactory = fileSystemFactory;
        }

        public async Task<MediaFile> GetImage(string imageUrl)
        {
            using var disabler = new SiteFilterDisabler(_session);
            var originalImageUrl = GetOriginalImageUrl(imageUrl);

            var fileByLocation =
                await _session
                    .Query<MediaFile>()
                    .FirstOrDefaultAsync(file => file.FileUrl == originalImageUrl);

            return fileByLocation;
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

        public void SetFileDimensions(MediaFile file, Stream stream)
        {
            stream.Position = 0;
            using var codec = OpenCodec(stream);
            file.Width = codec.Info.Width;
            file.Height = codec.Info.Height;
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
            stream.Position = 0;
            using var codec = OpenCodec(stream);
            file.Width = codec.Info.Width;
            file.Height = codec.Info.Height;
            file.ContentLength = stream.Length;

            if (!mediaSettings.EnforceMaxImageSize)
                return;
            if (!RequiresResize(file.Size, mediaSettings.MaxSize))
                return;
            // formats we cannot re-encode (e.g. animated GIF) are kept as uploaded
            if (!TryGetEncodeFormat(codec.EncodedFormat, out var format))
                return;

            using var original = DecodeBitmap(codec);
            var target = FitWithin(original.Width, original.Height, mediaSettings.MaxImageSizeWidth,
                mediaSettings.MaxImageSizeHeight);
            using var resized = Resize(original, target);

            var output = Encode(resized, format);
            file.Width = resized.Width;
            file.Height = resized.Height;
            file.ContentLength = output.Length;

            Stream originalStream = stream;
            stream = output;
            originalStream.Dispose();
        }


        private async Task SaveFile(byte[] fileBytes, string fileUrl, Size newSize, string contentType,
            Rectangle? cropRectangle = null)
        {
            var fileSystem = _fileSystemFactory.GetForCurrentSite();
            using var codec = SKCodec.Create(new MemoryStream(fileBytes));
            if (codec == null || !TryGetEncodeFormat(codec.EncodedFormat, out var format))
            {
                // cannot be processed (e.g. animated GIF) - serve the original rather than fail
                await using var originalStream = new MemoryStream(fileBytes);
                await fileSystem.SaveFile(originalStream, fileUrl, contentType);
                return;
            }

            using var original = DecodeBitmap(codec);
            SKBitmap result;
            if (cropRectangle.HasValue)
            {
                using var resized = Resize(original, new SKSizeI(newSize.Width, newSize.Height));
                result = Crop(resized, cropRectangle.Value);
            }
            else
            {
                var target = FitWithin(original.Width, original.Height, newSize.Width, newSize.Height);
                result = Resize(original, target);
            }

            using (result)
            {
                await using var outputStream = Encode(result, format);
                await fileSystem.SaveFile(outputStream, fileUrl, contentType);
            }
        }

        private static SKCodec OpenCodec(Stream stream)
        {
            // SKCodec takes ownership of the stream it is given, so give it a copy
            var copy = new MemoryStream();
            stream.CopyTo(copy);
            stream.Position = 0;
            copy.Position = 0;
            return SKCodec.Create(copy) ?? throw new InvalidOperationException("The file is not a supported image.");
        }

        private static SKBitmap DecodeBitmap(SKCodec codec)
        {
            var info = codec.Info.WithColorType(SKImageInfo.PlatformColorType).WithAlphaType(SKAlphaType.Premul);
            var bitmap = new SKBitmap(info);
            var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels());
            if (result != SKCodecResult.Success && result != SKCodecResult.IncompleteInput)
            {
                bitmap.Dispose();
                throw new InvalidOperationException($"Could not decode image: {result}");
            }

            return bitmap;
        }

        private static bool TryGetEncodeFormat(SKEncodedImageFormat format, out SKEncodedImageFormat encodeFormat)
        {
            encodeFormat = format;
            return format is SKEncodedImageFormat.Jpeg or SKEncodedImageFormat.Png or SKEncodedImageFormat.Webp;
        }

        /// <summary>
        /// Largest size that fits within the bounds (0 = unconstrained) keeping the aspect ratio, never upscaling.
        /// </summary>
        private static SKSizeI FitWithin(int width, int height, int maxWidth, int maxHeight)
        {
            var scale = 1d;
            if (maxWidth > 0)
                scale = Math.Min(scale, maxWidth / (double)width);
            if (maxHeight > 0)
                scale = Math.Min(scale, maxHeight / (double)height);
            return new SKSizeI(Math.Max(1, (int)Math.Round(width * scale)), Math.Max(1, (int)Math.Round(height * scale)));
        }

        private static SKBitmap Resize(SKBitmap source, SKSizeI size)
        {
            if (size.Width <= 0 || size.Height <= 0 || (size.Width == source.Width && size.Height == source.Height))
                return source.Copy();
            return source.Resize(new SKImageInfo(size.Width, size.Height, source.ColorType, source.AlphaType),
                       new SKSamplingOptions(SKCubicResampler.Mitchell))
                   ?? throw new InvalidOperationException("Could not resize image.");
        }

        private static SKBitmap Crop(SKBitmap source, Rectangle crop)
        {
            var bounds = SKRectI.Create(crop.X, crop.Y, crop.Width, crop.Height);
            bounds.Intersect(SKRectI.Create(0, 0, source.Width, source.Height));
            var subset = new SKBitmap();
            if (bounds.IsEmpty || !source.ExtractSubset(subset, bounds))
            {
                subset.Dispose();
                throw new InvalidOperationException("Crop area is outside the image.");
            }

            // copy so the result does not depend on the source's pixel memory
            using (subset)
                return subset.Copy();
        }

        private static MemoryStream Encode(SKBitmap bitmap, SKEncodedImageFormat format)
        {
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(format, format == SKEncodedImageFormat.Png ? 100 : JpegQuality);
            var output = new MemoryStream();
            data.SaveTo(output);
            output.Position = 0;
            return output;
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
                targetSize.Width == 0 ? (double?)null : originalSize.Width / (double)targetSize.Width;
            double? heightRatio = targetSize.Height == 0
                ? (double?)null
                : originalSize.Height / (double)targetSize.Height;
            var ratio = widthRatio.GetValueOrDefault() > heightRatio.GetValueOrDefault()
                ? originalSize.Width / (double)targetSize.Width
                : originalSize.Height / (double)targetSize.Height;

            double width = Math.Ceiling(originalSize.Width / ratio);
            width = targetSize.Width != 0 && width > targetSize.Width ? targetSize.Width : width;
            double resizeWidth = width;

            double height = Math.Ceiling(originalSize.Height / ratio);
            height = targetSize.Height != 0 && height > targetSize.Height ? targetSize.Height : height;
            double resizeHeight = height;

            return new Size((int)resizeWidth, (int)resizeHeight);
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