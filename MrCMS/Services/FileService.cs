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
        private readonly CurrentSite _currentSite;
        private const string _mediaDirectory = "content/upload";

        public FileService(ISession session, IFileSystem fileSystem, IImageProcessor imageProcessor, MediaSettings mediaSettings, CurrentSite currentSite)
        {
            _session = session;
            _fileSystem = fileSystem;
            _imageProcessor = imageProcessor;
            _mediaSettings = mediaSettings;
            _currentSite = currentSite;
        }

        public ViewDataUploadFilesResult AddFile(Stream stream, string fileName, string contentType, int contentLength, MediaCategory mediaCategory)
        {
            if (mediaCategory == null) throw new ArgumentNullException("mediaCategory");

            fileName = Path.GetFileName(fileName);

            fileName = GetFileSeName(fileName);
            var fileNameOriginal = GetFileSeName(fileName);

            string folderLocation = string.Format("{0}/{1}/{2}", MediaDirectory, _currentSite.Site.Id, mediaCategory.UrlSegment);

            //check for duplicates
            int i = 1;
            while (_fileSystem.Exists(_fileSystem.ApplicationPath + folderLocation + fileName))
            {
                fileName = fileNameOriginal.Replace(_fileSystem.GetExtension(fileName), "") + i + _fileSystem.GetExtension(fileName);
                i++;
            }

            string fileLocation = string.Format("{0}/{1}/{2}/{3}", MediaDirectory, _currentSite.Site.Id, mediaCategory.UrlSegment, fileName);

            var mediaFile = new MediaFile
                                {
                                    FileName = fileName,
                                    ContentType = contentType,
                                    ContentLength = contentLength,
                                    MediaCategory = mediaCategory,
                                    FileExtension = _fileSystem.GetExtension(fileName),
                                    FileLocation = fileLocation,
                                    DisplayOrder = mediaCategory.Files.Count
                                };

            if (mediaFile.IsImage)
            {
                _imageProcessor.EnforceMaxSize(ref stream, mediaFile, _mediaSettings);
                _imageProcessor.SetFileDimensions(mediaFile, stream);
            }

            _fileSystem.SaveFile(stream, mediaFile.FileLocation);

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

            var extension = _fileSystem.GetExtension(name);

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
            var location = _fileSystem.ApplicationPath + file.FileLocation;

            if (!_fileSystem.Exists(location))
                return new byte[0];
            return _fileSystem.ReadAllBytes(location);
        }


        public virtual string GetUrl(MediaFile file, Size size)
        {
            if (!file.IsImage)
                return file.FileLocation;

            //check to see if the image already exists, if it does simply return it
            var requestedFileLocation = ImageProcessor.RequestedImageFileLocation(file, size);

            if (_fileSystem.Exists(_fileSystem.ApplicationPath + requestedFileLocation))
                return requestedFileLocation;

            //if we have got this far the image doesn't exist yet so we need to create the image at the requested size
            var fileBytes = LoadFile(file);
            if (fileBytes.Length == 0)
                return "";

            _imageProcessor.SaveResizedImage(file, size, fileBytes, _fileSystem.ApplicationPath + requestedFileLocation);

            return requestedFileLocation;
        }

        public ViewDataUploadFilesResult[] GetFiles(MediaCategory mediaCategory)
        {
            return
                _session.QueryOver<MediaFile>()
                        .Where(file => file.MediaCategory == mediaCategory)
                        .OrderBy(file => file.DisplayOrder)
                        .Asc.Cacheable()
                        .List()
                        .Select(
                            GetUploadFilesResult).ToArray();
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

            _fileSystem.Delete(mediaFile.FileLocation);
            foreach (var imageUrl in
                GetImageSizes()
                    .Select(imageSize => GetUrl(mediaFile, imageSize.Size))
                    .Select(imageUrl => new { imageUrl, file = _fileSystem.ApplicationPath + imageUrl })
                    .Where(@t => _fileSystem.Exists(@t.file))
                    .Select(@t => @t.imageUrl))
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

        public FilesPagedResult GetFilesPaged(int? categoryId, bool imagesOnly, int page = 1, int pageSize = 10)
        {
            var queryOver = _session.QueryOver<MediaFile>().Where(file => file.Site == _currentSite.Site);

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

        public string GetFileUrl(string value)
        {
            var mediaFile = GetFileByLocation(value);
            if (mediaFile != null)
                return "/" + mediaFile.FileLocation;

            var split = value.Split('-');
            var id = Convert.ToInt32(split[0]);
            var file = GetFile(id);
            var imageSize =
                file.Sizes.FirstOrDefault(size => size.Size == new Size(Convert.ToInt32(split[1]), Convert.ToInt32(split[2])));
            return "/" + GetFileLocation(file, imageSize.Size);
        }

        public string MediaDirectory { get { return _mediaDirectory; } }

        public void RemoveFolder(MediaCategory mediaCategory)
        {
            string folderLocation = string.Format("{0}/{1}/{2}/", MediaDirectory, _currentSite.Site.Id,
                                                  mediaCategory.UrlSegment);

            _fileSystem.Delete(folderLocation);
        }

        public void CreateFolder(MediaCategory mediaCategory)
        {
            string folderLocation = string.Format("{0}/{1}/{2}/", MediaDirectory, _currentSite.Site.Id,
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