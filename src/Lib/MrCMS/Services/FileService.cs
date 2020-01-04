using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Settings;
using X.PagedList;

namespace MrCMS.Services
{
    public class FileService : IFileService
    {
        private readonly Site _currentSite;
        private readonly IRepository<MediaFile> _repository;
        private readonly IRepository<ResizedImage> _resizedImageRepository;
        private readonly IFileSystem _fileSystem;
        private readonly IImageProcessor _imageProcessor;
        private readonly MediaSettings _mediaSettings;
        private readonly SiteSettings _siteSettings;

        public FileService(
            IRepository<MediaFile> repository,
            IRepository<ResizedImage> resizedImageRepository,
            IFileSystem fileSystem, IImageProcessor imageProcessor,
            MediaSettings mediaSettings, Site currentSite, SiteSettings siteSettings)
        {
            _repository = repository;
            _resizedImageRepository = resizedImageRepository;
            _fileSystem = fileSystem;
            _imageProcessor = imageProcessor;
            _mediaSettings = mediaSettings;
            _currentSite = currentSite;
            _siteSettings = siteSettings;
        }

        public async Task<MediaFile> AddFile(Stream stream, string fileName, string contentType, long contentLength,
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
                await _repository.Readonly()
                        .Where(x => x.MediaCategory.Id == mediaFile.MediaCategory.Id)
                        .MaxAsync(x => (int?)x.DisplayOrder);
                mediaFile.DisplayOrder = max + 1 ?? 1;
            }

            if (mediaFile.IsImage())
            {
                _imageProcessor.EnforceMaxSize(ref stream, mediaFile, _mediaSettings);
            }

            string fileLocation = GetFileLocation(fileName, mediaCategory);

            mediaFile.FileUrl = _fileSystem.SaveFile(stream, fileLocation, contentType);


            await _repository.Transact(async (repo, ct) =>
             {
                 await repo.Add(mediaFile, ct);
                 mediaCategory?.Files.Add(mediaFile);
             });

            stream.Dispose();
            return mediaFile;
        }

        public async Task DeleteFile(MediaFile mediaFile)
        {
            // remove file from the file list for its category, to prevent missing item exception
            mediaFile.MediaCategory?.Files.Remove(mediaFile);

            foreach (ResizedImage resizedImage in
                mediaFile.ResizedImages
                    .Where(path => _fileSystem.Exists(path.Url)))
            {
                _fileSystem.Delete(resizedImage.Url);
            }
            _fileSystem.Delete(mediaFile.FileUrl);

            await _repository.Delete(mediaFile);
        }

        public async Task AddFIle(MediaFile mediaFile)
        {
            await _repository.Add(mediaFile);
        }
        public async Task UpdateFile(MediaFile mediaFile)
        {
            await _repository.Update(mediaFile);
        }

        public async Task<string> GetFileLocation(MediaFile mediaFile, Size imageSize, bool getCdn = false)
        {
            return await GetUrl(mediaFile, imageSize, getCdn);
        }

        public async Task<string> GetFileLocation(Crop crop, Size imageSize, bool getCdn = false)
        {
            return await GetUrl(crop, imageSize, getCdn);
        }

        public async Task<FilesPagedResult> GetFilesPaged(int? categoryId, bool imagesOnly, int page = 1)
        {
            var queryOver = _repository.Query().Where(file => file.Site == _currentSite);

            if (categoryId.HasValue)
                queryOver = queryOver.Where(file => file.MediaCategory.Id == categoryId);

            if (imagesOnly)
                queryOver = queryOver.Where(file => MediaFileExtensions.ImageExtensions.Contains(file.FileExtension));

            IPagedList<MediaFile> mediaFiles = await queryOver.OrderByDescending(file => file.CreatedOn)
                .ToPagedListAsync(page, _siteSettings.DefaultPageSize);
            return new FilesPagedResult(mediaFiles, mediaFiles.GetMetaData(), categoryId, imagesOnly);
        }

        public MediaFile GetFileByUrl(string url)
        {
            return
                _repository
                    .Query()
                    .FirstOrDefault(file => file.FileUrl == url);
        }

        public MediaFile GetFile(string value)
        {
            MediaFile mediaFile = GetFileByUrl(value);
            if (mediaFile != null)
                return mediaFile;

            string[] split = value.Split('-');
            int id = Convert.ToInt32(split[0]);
            return _repository.LoadSync(id);
        }

        public async Task<string> GetFileUrl(MediaFile mediaFile, string value)
        {
            if (mediaFile == null)
                return null;
            if (string.Equals(mediaFile.FileUrl, value, StringComparison.OrdinalIgnoreCase))
                return mediaFile.FileUrl;

            string[] split = value.Split('-');
            int id = Convert.ToInt32(split[0]);
            var file = _repository.GetDataSync(id);
            ImageSize imageSize =
                file.GetSizes(_mediaSettings)
                    .FirstOrDefault(size => size.Size == new Size(Convert.ToInt32(split[1]), Convert.ToInt32(split[2])));
            return await GetFileLocation(file, imageSize.Size, false);
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

        public async Task DeleteFileSoft(MediaFile mediaFile)
        {
            if (mediaFile.MediaCategory != null)
                mediaFile.MediaCategory.Files.Remove(mediaFile);
            await _repository.Delete(mediaFile);
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

        public virtual async Task<string> GetUrl(MediaFile file, Size size, bool getCdnUrl)
        {
            return await GetCdnUrl(async () =>
            {
                if (!file.IsImage())
                    return file.FileUrl;

                //check to see if the image already exists, if it does simply return it
                string requestedImageFileUrl = ImageProcessor.RequestedImageFileUrl(file, size);

                if (requestedImageFileUrl == file.FileUrl)
                    return file.FileUrl;

                // if we've cached the file existing then we're fine
                IList<ResizedImage> resizedImages =
                    _resizedImageRepository.Readonly().Where(image => image.MediaFile.Id == file.Id).ToList();
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
                await CacheResizedImage(file, requestedImageFileUrl);

                return requestedImageFileUrl;
            }, getCdnUrl);
        }

        private async Task<string> GetCdnUrl(Func<Task<string>> func, bool getUrl)
        {
            var url = await func();
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

        public virtual async Task<string> GetUrl(Crop crop, Size size, bool getCdnUrl)
        {
            return await GetCdnUrl(async () =>
            {
                //check to see if the image already exists, if it does simply return it
                string requestedImageFileUrl = ImageProcessor.RequestedResizedCropFileUrl(crop, size);

                // if we've cached the file existing then we're fine
                IList<ResizedImage> resizedImages =
                    _resizedImageRepository.Readonly().Where(image => image.Crop.Id == crop.Id).ToList();
                if (resizedImages.Any(image => image.Url == requestedImageFileUrl))
                    return requestedImageFileUrl;

                // if it exists but isn't cached, we should add it to the cache
                if (_fileSystem.Exists(requestedImageFileUrl))
                {
                    await CacheResizedImage(crop, requestedImageFileUrl);
                    return requestedImageFileUrl;
                }

                //if we have got this far the image doesn't exist yet so we need to create the image at the requested size
                byte[] fileBytes = LoadFile(crop.Url);
                if (fileBytes.Length == 0)
                    return "";

                _imageProcessor.SaveResizedCrop(crop, size, fileBytes, requestedImageFileUrl);

                // we also need to cache the resized image, to save making a request to find it
                await CacheResizedImage(crop, requestedImageFileUrl);

                return requestedImageFileUrl;
            }, getCdnUrl);
        }

        private async Task CacheResizedImage(MediaFile file, string requestedImageFileUrl)
        {
            var resizedImage = new ResizedImage { Url = requestedImageFileUrl, MediaFile = file };
            file.ResizedImages.Add(resizedImage);
            await _resizedImageRepository.Add(resizedImage);
        }

        private async Task CacheResizedImage(Crop crop, string requestedImageFileUrl)
        {
            var resizedImage = new ResizedImage { Url = requestedImageFileUrl, Crop = crop };
            crop.ResizedImages.Add(resizedImage);
            await _resizedImageRepository.Add(resizedImage);
        }
    }
}