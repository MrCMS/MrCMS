using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Settings;
using NHibernate;
using MrCMS.Helpers;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class FileService : IFileService
    {
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;
        private readonly IFileSystem _fileSystem;

        public FileService(ISession session, SiteSettings siteSettings, IFileSystem fileSystem)
        {
            _session = session;
            _siteSettings = siteSettings;
            _fileSystem = fileSystem;
        }

        public ViewDataUploadFilesResult AddFile(Stream stream, string fileName, string contentType, int contentLength, MediaCategory mediaCategory)
        {
            if (mediaCategory == null) throw new ArgumentNullException("mediaCategory");

            fileName = GetFileSeName(fileName);
            var fileNameOriginal = GetFileSeName(fileName);

            string folderLocation = string.Format("{0}/{1}/", _siteSettings.MediaDirectory, mediaCategory.UrlSegment);

            //check for duplicates
            int i = 1;
            while (File.Exists(MappedApplicationPath + folderLocation + fileName))
            {
                fileName = fileNameOriginal.Replace(Path.GetExtension(fileName), "") + i + Path.GetExtension(fileName);
                i++;
            }

            string fileLocation = string.Format("{0}/{1}/{2}", _siteSettings.MediaDirectory, mediaCategory.UrlSegment, fileName);

            var mediaFile = new MediaFile
                                {
                                    FileName = fileName,
                                    ContentType = contentType,
                                    ContentLength = contentLength,
                                    MediaCategory = mediaCategory,
                                    FileExtension = Path.GetExtension(fileName),
                                    FileLocation = fileLocation
                                };

            if (mediaFile.IsImage)
            {
                var b = new Bitmap(stream);

                mediaFile.Width = b.Size.Width;
                mediaFile.Height = b.Size.Height;
            }

            _fileSystem.SaveFile(stream, mediaFile.FileLocation);

            mediaCategory.Files.Add(mediaFile);
            _session.Transact(session =>
                                  {
                                      session.SaveOrUpdate(mediaFile);
                                      session.SaveOrUpdate(mediaCategory);
                                  });
            return new ViewDataUploadFilesResult(mediaFile, GetImageUrl(mediaFile, GetImageSizes().Find(size => size.Name == "Thumbnail").Size));
        }

        /// <summary>
        /// Get file se name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Result</returns>
        public virtual string GetFileSeName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return name;

            var extention = Path.GetExtension(name);

            name = name.Replace(extention, "");

            const string okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
            name = name.Trim().ToLowerInvariant();

            var sb = new StringBuilder();
            foreach (char c in name.ToCharArray())
            {
                string c2 = c.ToString();
                if (okChars.Contains(c2))
                    sb.Append(c2);
            }
            string name2 = sb.ToString();
            name2 = name2.Replace(" ", "_");
            name2 = name2.Replace("-", "_");
            while (name2.Contains("__"))
                name2 = name2.Replace("__", "_");
            return name2.ToLowerInvariant() + extention.ToLower();
        }

        public virtual byte[] LoadPictureFromFile(MediaFile file)
        {
            var location = MappedApplicationPath + file.FileLocation;

            if (!File.Exists(location))
                return new byte[0];
            return File.ReadAllBytes(location);
        }

        public string MappedApplicationPath
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(OverrideApplicationPath))
                    return OverrideApplicationPath;
                string APP_PATH = HttpContext.Current.Request.ApplicationPath.ToLower();
                if (APP_PATH == "/")      //a site
                    APP_PATH = "/";
                else if (!APP_PATH.EndsWith(@"/")) //a virtual
                    APP_PATH += @"/";

                string it = HttpContext.Current.Server.MapPath(APP_PATH);
                if (!it.EndsWith(@"\"))
                    it += @"\";
                return it;
            }
        }

        public string OverrideApplicationPath { get; set; }

        protected virtual string GetFileExtensionFromMimeType(string mimeType)
        {
            if (mimeType == null)
                return null;

            string[] parts = mimeType.Split('/');
            string lastPart = parts[parts.Length - 1];
            switch (lastPart)
            {
                case "pjpeg":
                    lastPart = "jpg";
                    break;
                case "x-png":
                    lastPart = "png";
                    break;
                case "x-icon":
                    lastPart = "ico";
                    break;
            }
            return lastPart;
        }

        public virtual string GetImageUrl(MediaFile file, Size size)
        {
            //fileName = someimage_50.jpg
            if (!file.IsImage)
                return ""; // blank image?

            //check to see if the image already exists, if it does simply return it
            var requestedFileLocation = RequestedImageFileLocation(file, size);

            if (File.Exists(MappedApplicationPath + requestedFileLocation))
                return requestedFileLocation;

            //if we have got this far the image doesn't exist yet so we need to create the image at the requested size
            var fileBytes = LoadPictureFromFile(file);
            if (fileBytes.Length == 0)
                return "";

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
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        g.DrawImage(b, 0, 0, newSize.Width, newSize.Height);
                        var ep = new EncoderParameters();
                        ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                        ImageCodecInfo ici = GetImageCodecInfoFromExtension(file.FileExtension)
                                             ?? GetImageCodecInfoFromMimeType("image/jpeg");
                        newBitMap.Save(MappedApplicationPath + requestedFileLocation, ici, ep);
                    }
                }
            }

            return requestedFileLocation;
        }

        /// <summary>
        /// Returns the name and full path of the requested file
        /// </summary>
        private string RequestedImageFileLocation(MediaFile file, Size size)
        {
            if (file.Size == size)
                return file.FileLocation;

            var fileLocation = file.FileLocation;

            var temp = fileLocation.Replace(file.FileExtension, "");
            return size.Height >= size.Width
                       ? temp.Insert(temp.Length, "_h" + size.Height + file.FileExtension)
                       : temp.Insert(temp.Length, "_w" + size.Width + file.FileExtension);
        }

        public static Size CalculateDimensions(Size originalSize, Size targetSize)
        {
            // If the target image is bigger than the source
            if (!RequiresResize(originalSize, targetSize))
            {
                return originalSize;
            }

            double ratio = 0;

            // What ratio should we resize it by
            var widthRatio = originalSize.Width / (double)targetSize.Width;
            var heightRatio = originalSize.Height / (double)targetSize.Height;
            ratio = widthRatio > heightRatio
                        ? originalSize.Width / (double)targetSize.Width
                        : originalSize.Height / (double)targetSize.Height;

            var resizeWidth = Math.Floor(originalSize.Width / ratio);

            var resizeHeight = Math.Floor(originalSize.Height / ratio);

            return new Size((int)resizeWidth, (int)resizeHeight);
        }

        public static bool RequiresResize(Size originalSize, Size targetSize)
        {
            return targetSize.Width <= originalSize.Width || targetSize.Height <= originalSize.Height;
        }

        /// <summary>
        /// Returns the first ImageCodecInfo instance with the specified mime type.
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns>ImageCodecInfo</returns>
        private ImageCodecInfo GetImageCodecInfoFromMimeType(string mimeType)
        {
            var info = ImageCodecInfo.GetImageEncoders();
            foreach (var ici in info)
                if (ici.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase))
                    return ici;
            return null;
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

        public ViewDataUploadFilesResult[] GetFiles(int mediaCategoryId)
        {
            return
                _session.QueryOver<MediaFile>().Where(file => file.MediaCategory.Id == mediaCategoryId).List().Select(
                    file => new ViewDataUploadFilesResult(file, GetImageUrl(file, GetImageSizes().Find(size => size.Name == "Thumbnail").Size))).ToArray();
        }

        public MediaFile GetFile(int id)
        {
            return _session.Get<MediaFile>(id);
        }

        public void DeleteFile(int id)
        {
            var mediaFile = GetFile(id);

            // remove file from the file list for its category, to prevent missing item exception
            mediaFile.MediaCategory.Files.Remove(mediaFile);

            _fileSystem.Delete(mediaFile.FileLocation);
            foreach (var imageSize in GetImageSizes())
            {
                var imageUrl = GetImageUrl(mediaFile, imageSize.Size);
                var file = MappedApplicationPath + imageUrl;
                if (File.Exists(file))
                    _fileSystem.Delete(imageUrl);
            }

            _session.Transact(session => session.Delete(mediaFile));
        }

        public void SaveFile(MediaFile mediaFile)
        {
            _session.Transact(session => session.SaveOrUpdate(mediaFile));
        }

        public List<ImageSize> GetImageSizes()
        {
            return ImageSizes;
        }

        public static List<ImageSize> ImageSizes
        {
            get
            {
                return new List<ImageSize>
                           {
                               new ImageSize {Size = new Size(480, 640), Name = "Large - Portrait"},
                               new ImageSize {Size = new Size(640, 480), Name = "Large - Landscape"},
                               new ImageSize {Size = new Size(240, 320), Name = "Medium - Portrait"},
                               new ImageSize {Size = new Size(320, 240), Name = "Medium - Landscape"},
                               new ImageSize {Size = new Size(75, 100), Name = "Small - Portrait"},
                               new ImageSize {Size = new Size(100, 75), Name = "Small - Landscape"},
                               new ImageSize {Size = new Size(64, 64), Name = "Thumbnail"}
                           };
            }
        }

        public string GetFileLocation(MediaFile mediaFile, ImageSize imageSize)
        {
            return GetImageUrl(mediaFile, imageSize.Size);
        }

        public FilesPagedResult GetFilesPaged(int? categoryId, bool imagesOnly, int page = 1, int pageSize = 10)
        {
            var queryOver = _session.QueryOver<MediaFile>();

            if (categoryId.HasValue)
                queryOver = queryOver.Where(file => file.MediaCategory.Id == categoryId);

            if (imagesOnly)
                queryOver.Where(file => file.FileExtension.IsIn(MediaFile.ImageExtensions));

            var mediaFiles = queryOver.OrderBy(file => file.CreatedOn).Desc.Paged(page, pageSize);
            return new FilesPagedResult(mediaFiles, mediaFiles.GetMetaData(), categoryId, imagesOnly);
        }

        public MediaFile GetFileByLocation(string value)
        {
            return _session.QueryOver<MediaFile>().Where(file => file.FileLocation == value).Take(1).Cacheable().SingleOrDefault();
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
                var lastIndexOf = imageUrl.LastIndexOf(resizePart);
                imageUrl = imageUrl.Remove(lastIndexOf - 1, resizePart.Length + 1);
            }
            var fileByLocation = GetFileByLocation(imageUrl);
            return fileByLocation ?? null;
        }

        private bool IsResized(string imageUrl)
        {
            var resizePart = GetResizePart(imageUrl);
            if (resizePart == null) return false;

            int val;
            return new List<char> {'w', 'h'}.Contains(resizePart[0]) && int.TryParse(resizePart.Substring(1), out val);
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
    }

    public class FilesPagedResult : StaticPagedList<MediaFile>
    {
        public int? CategoryId { get; set; }

        public bool ImagesOnly { get; set; }

        public FilesPagedResult(IEnumerable<MediaFile> subset, IPagedList metaData, int? categoryId, bool imagesOnly)
            : base(subset, metaData)
        {
            CategoryId = categoryId;
            ImagesOnly = imagesOnly;
        }
    }
}