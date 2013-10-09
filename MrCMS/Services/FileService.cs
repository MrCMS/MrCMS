using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Settings;
using NHibernate;
using MrCMS.Helpers;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class FileService : IFileService
    {
        private readonly ISession _session;
        private readonly IFileSystem _fileSystem;
        private readonly IImageProcessor _imageProcessor;
        private readonly MediaSettings _mediaSettings;
        private readonly Site _currentSite;
        private readonly SiteSettings _siteSettings;

        public FileService(ISession session, IFileSystem fileSystem, IImageProcessor imageProcessor, MediaSettings mediaSettings, Site currentSite, SiteSettings siteSettings)
        {
            _session = session;
            _fileSystem = fileSystem;
            _imageProcessor = imageProcessor;
            _mediaSettings = mediaSettings;
            _currentSite = currentSite;
            _siteSettings = siteSettings;
        }

        public ViewDataUploadFilesResult AddFile(Stream stream, string fileName, string contentType, long contentLength, MediaCategory mediaCategory)
        {
            if (mediaCategory == null) throw new ArgumentNullException("mediaCategory");

            fileName = Path.GetFileName(fileName);

            fileName = GetFileSeName(fileName);
            var fileNameOriginal = GetFileSeName(fileName);

            string folderLocation = string.Format("{0}/{1}/", _currentSite.Id, mediaCategory.UrlSegment);

            //check for duplicates
            int i = 1;
            while (_fileSystem.Exists(folderLocation + fileName))
            {
                fileName = fileNameOriginal.Replace(Path.GetExtension(fileName), "") + i + Path.GetExtension(fileName);
                i++;
            }

            string fileLocation = string.Format("{0}/{1}/{2}", _currentSite.Id, mediaCategory.UrlSegment, fileName);

            var mediaFile = new MediaFile
                                {
                                    FileName = fileName,
                                    ContentType = contentType,
                                    ContentLength = contentLength,
                                    MediaCategory = mediaCategory,
                                    FileExtension = Path.GetExtension(fileName),
                                    DisplayOrder = mediaCategory.Files.Count
                                };

            if (mediaFile.IsImage)
            {
                _imageProcessor.EnforceMaxSize(ref stream, mediaFile, _mediaSettings);
                _imageProcessor.SetFileDimensions(mediaFile, stream);
            }

            mediaFile.FileUrl = _fileSystem.SaveFile(stream, fileLocation, contentType);

            mediaCategory.Files.Add(mediaFile);
            _session.Transact(session =>
                                  {
                                      session.SaveOrUpdate(mediaFile);
                                      session.SaveOrUpdate(mediaCategory);
                                  });
            return GetUploadFilesResult(mediaFile);
        }

        private ViewDataUploadFilesResult GetUploadFilesResult(MediaFile mediaFile)
        {
            return new ViewDataUploadFilesResult(mediaFile, GetUrl(mediaFile, GetImageSizes().Find(size => size.Name == "Thumbnail").Size));
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

            var extension = Path.GetExtension(name);

            if (!string.IsNullOrWhiteSpace(extension))
                name = name.Replace(extension, "");

            name = name.Replace("&", " and ");

            var name2 = RemoveInvalidUrlCharacters(name);
            name2 = name2.Replace(" ", "-");
            name2 = name2.Replace("_", "-");
            while (name2.Contains("--"))
                name2 = name2.Replace("--", "-");
            return extension != null
                       ? name2.ToLowerInvariant() + extension.ToLower()
                       : name2.ToLowerInvariant();
        }

        public static string RemoveInvalidUrlCharacters(string name)
        {
            const string okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
            name = name.Trim().ToLowerInvariant();

            var sb = new StringBuilder();
            foreach (char c in name)
            {
                string c2 = c.ToString();
                if (okChars.Contains(c2))
                    sb.Append(c2);
            }
            string name2 = sb.ToString();
            return name2;
        }

        public virtual byte[] LoadFile(MediaFile file)
        {
            if (!_fileSystem.Exists(file.FileUrl))
                return new byte[0];
            return _fileSystem.ReadAllBytes(file.FileUrl);
        }

        public virtual string GetUrl(MediaFile file, Size size)
        {
            if (!file.IsImage)
                return file.FileUrl;

            //check to see if the image already exists, if it does simply return it
            var requestedImageFileUrl = ImageProcessor.RequestedImageFileUrl(file, size);

            // if we've cached the file existing then we're fine
            if (file.ResizedImages.Any(image => image.Url == requestedImageFileUrl))
                return requestedImageFileUrl;

            // if it exists but isn't cached, we should add it to the cache
            if (_fileSystem.Exists(requestedImageFileUrl))
            {
                CacheResizedImage(file, requestedImageFileUrl);
                return requestedImageFileUrl;
            }

            //if we have got this far the image doesn't exist yet so we need to create the image at the requested size
            var fileBytes = LoadFile(file);
            if (fileBytes.Length == 0)
                return "";

            _imageProcessor.SaveResizedImage(file, size, fileBytes, requestedImageFileUrl);

            // we also need to cache the resized image, to save making a request to find it
            CacheResizedImage(file, requestedImageFileUrl);

            return requestedImageFileUrl;
        }

        private void CacheResizedImage(MediaFile file, string requestedImageFileUrl)
        {
            var resizedImage = new ResizedImage { Url = requestedImageFileUrl, MediaFile = file };
            file.ResizedImages.Add(resizedImage);
            _session.Transact(session => session.Save(resizedImage));
        }

        public ViewDataUploadFilesResult[] GetFiles(MediaCategory mediaCategory)
        {
            return
                _session.QueryOver<MediaFile>()
                        .Where(file => file.MediaCategory == mediaCategory)
                        .OrderBy(file => file.DisplayOrder)
                        .Asc.Cacheable()
                        .List()
                        .Select(GetUploadFilesResult).ToArray();
        }

        public MediaFile GetFile(int id)
        {
            return _session.Get<MediaFile>(id);
        }

        public void DeleteFile(MediaFile mediaFile)
        {
            // remove file from the file list for its category, to prevent missing item exception
            if (mediaFile.MediaCategory != null)
                mediaFile.MediaCategory.Files.Remove(mediaFile);

            _fileSystem.Delete(mediaFile.FileUrl);
            foreach (var imageUrl in
                GetImageSizes()
                    .Select(imageSize => GetUrl(mediaFile, imageSize.Size))
                    .Where(path => _fileSystem.Exists(path)))
            {
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
            return _mediaSettings.ImageSizes.ToList();
        }

        public string GetFileLocation(MediaFile mediaFile, Size imageSize)
        {
            return GetUrl(mediaFile, imageSize);
        }

        public FilesPagedResult GetFilesPaged(int? categoryId, bool imagesOnly, int page = 1)
        {
            var queryOver = _session.QueryOver<MediaFile>().Where(file => file.Site == _currentSite);

            if (categoryId.HasValue)
                queryOver = queryOver.Where(file => file.MediaCategory.Id == categoryId);

            if (imagesOnly)
                queryOver.Where(file => file.FileExtension.IsIn(MediaFile.ImageExtensions));

            var mediaFiles = queryOver.OrderBy(file => file.CreatedOn).Desc.Paged(page, _siteSettings.DefaultPageSize);
            return new FilesPagedResult(mediaFiles, mediaFiles.GetMetaData(), categoryId, imagesOnly);
        }

        public MediaFile GetFileByUrl(string value)
        {
            return _session.QueryOver<MediaFile>().Where(file => file.FileUrl == value).Take(1).Cacheable().SingleOrDefault();
        }

        public string GetFileUrl(string value)
        {
            var mediaFile = GetFileByUrl(value);
            if (mediaFile != null)
                return mediaFile.FileUrl;

            var split = value.Split('-');
            var id = Convert.ToInt32(split[0]);
            var file = GetFile(id);
            var imageSize =
                file.Sizes.FirstOrDefault(size => size.Size == new Size(Convert.ToInt32(split[1]), Convert.ToInt32(split[2])));
            return GetFileLocation(file, imageSize.Size);
        }

        public void RemoveFolder(MediaCategory mediaCategory)
        {
            string folderLocation = string.Format("{0}/{1}/", _currentSite.Id,
                                                  mediaCategory.UrlSegment);

            _fileSystem.Delete(folderLocation);
        }

        public void CreateFolder(MediaCategory mediaCategory)
        {
            string folderLocation = string.Format("{0}/{1}/", _currentSite.Id,
                                                  mediaCategory.UrlSegment);

            _fileSystem.CreateDirectory(folderLocation);

        }

        public void SetOrders(List<SortItem> items)
        {
            _session.Transact(session => items.ForEach(item =>
                                                           {
                                                               var mediaFile = session.Get<MediaFile>(item.Id);
                                                               mediaFile.DisplayOrder = item.Order;
                                                               session.Update(mediaFile);
                                                           }));
        }
    }
}