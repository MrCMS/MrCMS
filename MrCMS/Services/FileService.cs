using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Settings;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class FileService : IFileService
    {
        private readonly Site _currentSite;
        private readonly IFileSystem _fileSystem;
        private readonly IImageProcessor _imageProcessor;
        private readonly MediaSettings _mediaSettings;
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;

        public FileService(ISession session, IFileSystem fileSystem, IImageProcessor imageProcessor,
            MediaSettings mediaSettings, Site currentSite, SiteSettings siteSettings)
        {
            _session = session;
            _fileSystem = fileSystem;
            _imageProcessor = imageProcessor;
            _mediaSettings = mediaSettings;
            _currentSite = currentSite;
            _siteSettings = siteSettings;
        }

        public MediaFile AddFile(Stream stream, string fileName, string contentType, long contentLength,
            MediaCategory mediaCategory = null)
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
                int? max =
                    _session.Query<MediaFile>()
                        .Where(x => x.MediaCategory.Id == mediaFile.MediaCategory.Id)
                        .Max(x => (int?)x.DisplayOrder);
                mediaFile.DisplayOrder = (max.HasValue ? (int)max + 1 : 1);
            }

            if (mediaFile.IsImage())
            {
                _imageProcessor.EnforceMaxSize(ref stream, mediaFile, _mediaSettings);
                _imageProcessor.SetFileDimensions(mediaFile, stream);
            }

            string fileLocation = GetFileLocation(fileName, mediaCategory);

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

        public void DeleteFile(MediaFile mediaFile)
        {
            // remove file from the file list for its category, to prevent missing item exception
            if (mediaFile.MediaCategory != null)
                mediaFile.MediaCategory.Files.Remove(mediaFile);

            foreach (ResizedImage resizedImage in
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

        public string GetFileLocation(MediaFile mediaFile, Size imageSize, bool getCdn = false)
        {
            return GetUrl(mediaFile, imageSize, getCdn);
        }

        public string GetFileLocation(Crop crop, Size imageSize, bool getCdn = false)
        {
            return GetUrl(crop, imageSize, getCdn);
        }

        public FilesPagedResult GetFilesPaged(int? categoryId, bool imagesOnly, int page = 1)
        {
            IQueryOver<MediaFile, MediaFile> queryOver =
                _session.QueryOver<MediaFile>().Where(file => file.Site == _currentSite);

            if (categoryId.HasValue)
                queryOver = queryOver.Where(file => file.MediaCategory.Id == categoryId);

            if (imagesOnly)
                queryOver.Where(file => file.FileExtension.IsIn(MediaFileExtensions.ImageExtensions));

            IPagedList<MediaFile> mediaFiles = queryOver.OrderBy(file => file.CreatedOn)
                .Desc.Paged(page, _siteSettings.DefaultPageSize);
            return new FilesPagedResult(mediaFiles, mediaFiles.GetMetaData(), categoryId, imagesOnly);
        }

        public MediaFile GetFileByUrl(string value)
        {
            return
                _session.QueryOver<MediaFile>()
                    .Where(file => file.FileUrl == value)
                    .Take(1)
                    .Cacheable()
                    .SingleOrDefault();
        }

        public string GetFileUrl(string value)
        {
            MediaFile mediaFile = GetFileByUrl(value);
            if (mediaFile != null)
                return mediaFile.FileUrl;

            string[] split = value.Split('-');
            int id = Convert.ToInt32(split[0]);
            var file = _session.Get<MediaFile>(id);
            ImageSize imageSize =
                file.GetSizes()
                    .FirstOrDefault(size => size.Size == new Size(Convert.ToInt32(split[1]), Convert.ToInt32(split[2])));
            return GetFileLocation(file, imageSize.Size, false);
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
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension) || extension.Length < 1)
                return false;
            return FileTypeUploadSettings.AllowedFileTypeList.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        public void DeleteFileSoft(MediaFile mediaFile)
        {
            if (mediaFile.MediaCategory != null)
                mediaFile.MediaCategory.Files.Remove(mediaFile);
            _session.Transact(session => session.Delete(mediaFile));
        }

        private string GetFileLocation(string fileName, MediaCategory mediaCategory = null)
        {
            string fileNameOriginal = fileName.GetTidyFileName();

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

        protected virtual byte[] LoadFile(string filePath)
        {
            if (!_fileSystem.Exists(filePath))
                return new byte[0];
            return _fileSystem.ReadAllBytes(filePath);
        }

        public virtual string GetUrl(MediaFile file, Size size, bool getCdnUrl)
        {
            return GetCdnUrl(() =>
            {
                if (!file.IsImage())
                    return file.FileUrl;

                //check to see if the image already exists, if it does simply return it
                string requestedImageFileUrl = ImageProcessor.RequestedImageFileUrl(file, size);

                if (requestedImageFileUrl == file.FileUrl)
                    return file.FileUrl;

                // if we've cached the file existing then we're fine
                IList<ResizedImage> resizedImages =
                    _session.QueryOver<ResizedImage>().Where(image => image.MediaFile.Id == file.Id).Cacheable().List();
                if (resizedImages.Any(image => image.Url == requestedImageFileUrl))
                    return requestedImageFileUrl;

                // if it exists but isn't cached, we should add it to the cache
                if (_fileSystem.Exists(requestedImageFileUrl))
                {
                    CacheResizedImage(file, requestedImageFileUrl);
                    return requestedImageFileUrl;
                }

                //if we have got this far the image doesn't exist yet so we need to create the image at the requested size
                byte[] fileBytes = LoadFile(file.FileUrl);
                if (fileBytes.Length == 0)
                    return "";

                _imageProcessor.SaveResizedImage(file, size, fileBytes, requestedImageFileUrl);

                // we also need to cache the resized image, to save making a request to find it
                CacheResizedImage(file, requestedImageFileUrl);

                return requestedImageFileUrl;
            }, getCdnUrl);
        }

        private string GetCdnUrl(Func<string> func, bool getUrl)
        {
            var url = func();
            var cdnInfo = _fileSystem.CdnInfo;
            if (!getUrl || cdnInfo == null)
                return url;

            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            var uriBuilder = new UriBuilder
            {
                Scheme = cdnInfo.Scheme,
                Host = cdnInfo.Host,
                Path = uri.AbsolutePath
            };
            return uriBuilder.Uri.ToString();
        }

        public virtual string GetUrl(Crop crop, Size size, bool getCdnUrl)
        {
            return GetCdnUrl(() =>
            {
                //check to see if the image already exists, if it does simply return it
                string requestedImageFileUrl = ImageProcessor.RequestedResizedCropFileUrl(crop, size);

                // if we've cached the file existing then we're fine
                IList<ResizedImage> resizedImages =
                    _session.QueryOver<ResizedImage>().Where(image => image.Crop.Id == crop.Id).Cacheable().List();
                if (resizedImages.Any(image => image.Url == requestedImageFileUrl))
                    return requestedImageFileUrl;

                // if it exists but isn't cached, we should add it to the cache
                if (_fileSystem.Exists(requestedImageFileUrl))
                {
                    CacheResizedImage(crop, requestedImageFileUrl);
                    return requestedImageFileUrl;
                }

                //if we have got this far the image doesn't exist yet so we need to create the image at the requested size
                byte[] fileBytes = LoadFile(crop.Url);
                if (fileBytes.Length == 0)
                    return "";

                _imageProcessor.SaveResizedCrop(crop, size, fileBytes, requestedImageFileUrl);

                // we also need to cache the resized image, to save making a request to find it
                CacheResizedImage(crop, requestedImageFileUrl);

                return requestedImageFileUrl;
            }, getCdnUrl);
        }

        private void CacheResizedImage(MediaFile file, string requestedImageFileUrl)
        {
            var resizedImage = new ResizedImage { Url = requestedImageFileUrl, MediaFile = file };
            file.ResizedImages.Add(resizedImage);
            _session.Transact(session => session.Save(resizedImage));
        }

        private void CacheResizedImage(Crop crop, string requestedImageFileUrl)
        {
            var resizedImage = new ResizedImage { Url = requestedImageFileUrl, Crop = crop };
            crop.ResizedImages.Add(resizedImage);
            _session.Transact(session => session.Save(resizedImage));
        }
    }
}