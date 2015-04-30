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
using MrCMS.Paging;
using MrCMS.Services.FileMigration;
using MrCMS.Settings;
using NHibernate;
using MrCMS.Helpers;
using NHibernate.Criterion;
using NHibernate.Linq;

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

        public MediaFile AddFile(Stream stream, string fileName, string contentType, long contentLength, MediaCategory mediaCategory = null)
        {

            fileName = Path.GetFileName(fileName);

            fileName = fileName.GetTidyFileName();

            var mediaFile = new MediaFile
            {
                FileName = fileName,
                ContentType = contentType,
                ContentLength = contentLength,
                FileExtension = Path.GetExtension(fileName),
            };
            if (mediaCategory != null)
            {
                mediaFile.MediaCategory = mediaCategory;
                int? max = _session.Query<MediaFile>().Where(x => x.MediaCategory.Id == mediaFile.MediaCategory.Id).Max(x => (int?)x.DisplayOrder);
                mediaFile.DisplayOrder = (max.HasValue ? (int)max + 1 : 1);
            }

            if (mediaFile.IsImage())
            {
                if (mediaFile.IsJpeg())
                {
                    _imageProcessor.EnforceMaxSize(ref stream, mediaFile, _mediaSettings);
                }
                _imageProcessor.SetFileDimensions(mediaFile, stream);
            }

            var fileLocation = GetFileLocation(fileName, mediaCategory);

            mediaFile.FileUrl = _fileSystem.SaveFile(stream, fileLocation, contentType);


            _session.Transact(session =>
            {
                session.Save(mediaFile);
                if (mediaCategory != null)
                {
                    mediaCategory.Files.Add(mediaFile);
                    session.SaveOrUpdate(mediaCategory);
                }
            });

            stream.Dispose();
            return mediaFile;
        }

        private string GetFileLocation(string fileName, MediaCategory mediaCategory = null)
        {
            var fileNameOriginal = fileName.GetTidyFileName();

            string urlSegment = "root";
            if (mediaCategory != null)
                urlSegment = mediaCategory.UrlSegment;

            string folderLocation = string.Format("{0}/{1}/", _currentSite.Id, urlSegment);

            //check for duplicates
            int i = 1;
            while (_fileSystem.Exists(folderLocation + fileName))
            {
                fileName = fileNameOriginal.Replace(Path.GetExtension(fileName), "") + i + Path.GetExtension(fileName);
                i++;
            }

            string fileLocation = string.Format("{0}/{1}/{2}", _currentSite.Id, urlSegment, fileName);
            return fileLocation;
        }

        public virtual byte[] LoadFile(MediaFile file)
        {
            if (!_fileSystem.Exists(file.FileUrl))
                return new byte[0];
            return _fileSystem.ReadAllBytes(file.FileUrl);
        }

        public virtual string GetUrl(MediaFile file, Size size)
        {
            if (!file.IsImage())
                return file.FileUrl;

            //check to see if the image already exists, if it does simply return it
            var requestedImageFileUrl = ImageProcessor.RequestedImageFileUrl(file, size);

            // if we've cached the file existing then we're fine
            var resizedImages = _session.QueryOver<ResizedImage>().Where(image => image.MediaFile.Id == file.Id).Cacheable().List();
            if (resizedImages.Any(image => image.Url == requestedImageFileUrl))
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

        public void DeleteFile(MediaFile mediaFile)
        {
            // remove file from the file list for its category, to prevent missing item exception
            if (mediaFile.MediaCategory != null)
                mediaFile.MediaCategory.Files.Remove(mediaFile);

            foreach (var resizedImage in
                mediaFile.ResizedImages
                    .Where(path => _fileSystem.Exists(path.Url)))
            {
                _fileSystem.Delete(resizedImage.Url);
            }
            _fileSystem.Delete(mediaFile.FileUrl);

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
                queryOver.Where(file => file.FileExtension.IsIn(MediaFileExtensions.ImageExtensions));

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
            var file = _session.Get<MediaFile>(id);
            var imageSize =
                file.GetSizes().FirstOrDefault(size => size.Size == new Size(Convert.ToInt32(split[1]), Convert.ToInt32(split[2])));
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

        public bool IsValidFileType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension) || extension.Length < 1)
                return false;
            return _mediaSettings.AllowedFileTypeList.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        public void DeleteFileSoft(MediaFile mediaFile)
        {
            if (mediaFile.MediaCategory != null)
                mediaFile.MediaCategory.Files.Remove(mediaFile);
            _session.Transact(session => session.Delete(mediaFile));
        }
    }
}