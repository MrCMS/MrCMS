using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MrCMS.Data;
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
        private readonly IRepository<MediaFile> _mediaFileRepository;
        private readonly IRepository<MediaCategory> _mediaCategoryRepository;
        private readonly IRepository<ResizedImage> _resizedImageRepository;

        public FileService(IFileSystem fileSystem, IImageProcessor imageProcessor,
            MediaSettings mediaSettings, Site currentSite, IRepository<MediaFile> mediaFileRepository,
            IRepository<MediaCategory> mediaCategoryRepository,IRepository<ResizedImage> resizedImageRepository)
        {
            _fileSystem = fileSystem;
            _imageProcessor = imageProcessor;
            _mediaSettings = mediaSettings;
            _currentSite = currentSite;
            _mediaFileRepository = mediaFileRepository;
            _mediaCategoryRepository = mediaCategoryRepository;
            _resizedImageRepository = resizedImageRepository;
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
                    _mediaFileRepository.Query()
                        .Where(x => x.MediaCategory.Id == mediaFile.MediaCategory.Id)
                        .Max(x => (int?) x.DisplayOrder);
                mediaFile.DisplayOrder = (max.HasValue ? (int) max + 1 : 1);
            }

            if (mediaFile.IsImage())
            {
                _imageProcessor.EnforceMaxSize(ref stream, mediaFile, _mediaSettings);
                _imageProcessor.SetFileDimensions(mediaFile, stream);
            }

            string fileLocation = GetFileLocation(fileName, mediaCategory);

            mediaFile.FileUrl = _fileSystem.SaveFile(stream, fileLocation, contentType);


            _mediaFileRepository.Transact(repository =>
            {
                repository.Add(mediaFile);
                if (mediaCategory != null)
                {
                    mediaCategory.Files.Add(mediaFile);
                    _mediaCategoryRepository.Update(mediaCategory);
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

            _mediaFileRepository.Delete(mediaFile);
        }

        public void SaveFile(MediaFile mediaFile)
        {
            _mediaFileRepository.Add(mediaFile);
        }

        public string GetFileLocation(MediaFile mediaFile, Size imageSize)
        {
            return GetUrl(mediaFile, imageSize);
        }

        public string GetFileLocation(Crop crop, Size imageSize)
        {
            return GetUrl(crop, imageSize);
        }

        public FilesPagedResult GetFilesPaged(int? categoryId, bool imagesOnly, int page = 1)
        {
            var queryOver =
                _mediaFileRepository.Query().Where(file => file.Site.Id == _currentSite.Id);

            if (categoryId.HasValue)
                queryOver = queryOver.Where(file => file.MediaCategory.Id == categoryId);

            if (imagesOnly)
                queryOver.Where(file => MediaFileExtensions.ImageExtensions.Contains(file.FileExtension));

            IPagedList<MediaFile> mediaFiles = queryOver.OrderByDescending(file => file.CreatedOn)
                .Paged(page, SessionHelper.DefaultPageSize);
            return new FilesPagedResult(mediaFiles, mediaFiles.GetMetaData(), categoryId, imagesOnly);
        }

        public MediaFile GetFileByUrl(string value)
        {
            return
                _mediaFileRepository.Query()
                    .FirstOrDefault(file => file.FileUrl == value);
        }

        public string GetFileUrl(string value)
        {
            MediaFile mediaFile = GetFileByUrl(value);
            if (mediaFile != null)
                return mediaFile.FileUrl;

            string[] split = value.Split('-');
            int id = Convert.ToInt32(split[0]);
            var file = _mediaFileRepository.Get(id);
            ImageSize imageSize =
                file.GetSizes()
                    .FirstOrDefault(size => size.Size == new Size(Convert.ToInt32(split[1]), Convert.ToInt32(split[2])));
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
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension) || extension.Length < 1)
                return false;
            return _mediaSettings.AllowedFileTypeList.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        public void DeleteFileSoft(MediaFile mediaFile)
        {
            if (mediaFile.MediaCategory != null)
                mediaFile.MediaCategory.Files.Remove(mediaFile);
            _mediaFileRepository.Delete(mediaFile);
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

        public virtual string GetUrl(MediaFile file, Size size)
        {
            if (!file.IsImage())
                return file.FileUrl;

            //check to see if the image already exists, if it does simply return it
            string requestedImageFileUrl = ImageProcessor.RequestedImageFileUrl(file, size);

            // if we've cached the file existing then we're fine
            IList<ResizedImage> resizedImages =
                _resizedImageRepository.Query().Where(image => image.MediaFile.Id == file.Id).ToList();
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
        }

        public virtual string GetUrl(Crop crop, Size size)
        {
            //check to see if the image already exists, if it does simply return it
            string requestedImageFileUrl = ImageProcessor.RequestedResizedCropFileUrl(crop, size);

            // if we've cached the file existing then we're fine
            IList<ResizedImage> resizedImages =
                _resizedImageRepository.Query().Where(image => image.Crop.Id == crop.Id).ToList();
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
        }

        private void CacheResizedImage(MediaFile file, string requestedImageFileUrl)
        {
            var resizedImage = new ResizedImage {Url = requestedImageFileUrl, MediaFile = file};
            file.ResizedImages.Add(resizedImage);
            _resizedImageRepository.Add(resizedImage);
        }

        private void CacheResizedImage(Crop crop, string requestedImageFileUrl)
        {
            var resizedImage = new ResizedImage {Url = requestedImageFileUrl, Crop = crop};
            crop.ResizedImages.Add(resizedImage);
            _resizedImageRepository.Add(resizedImage);
        }

        public List<ImageSize> GetImageSizes()
        {
            return _mediaSettings.ImageSizes.ToList();
        }
    }
}