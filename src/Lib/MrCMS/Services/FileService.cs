using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Settings;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class FileService : IFileService
    {
        private readonly IFileSystemFactory _fileSystemFactory;
        private readonly IImageProcessor _imageProcessor;
        private readonly MediaSettings _mediaSettings;
        private readonly ICurrentSiteLocator _siteLocator;
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;
        private IFileSystem _currentFileSystem;

        public FileService(ISession session, IFileSystemFactory fileSystemFactory, IImageProcessor imageProcessor,
            MediaSettings mediaSettings, ICurrentSiteLocator siteLocator, SiteSettings siteSettings)
        {
            _session = session;
            _fileSystemFactory = fileSystemFactory;
            _imageProcessor = imageProcessor;
            _mediaSettings = mediaSettings;
            _siteLocator = siteLocator;
            _siteSettings = siteSettings;
        }

        private IFileSystem GetFileSystem()
        {
            return _currentFileSystem ??= _fileSystemFactory.GetForCurrentSite();
        }

        public async Task<MediaFile> AddFile(Stream stream, string fileName, string contentType, long contentLength,
            MediaCategory mediaCategory)
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
                var max =
                    _session.Query<MediaFile>()
                        .Where(x => x.MediaCategory.Id == mediaFile.MediaCategory.Id)
                        .Max(x => (int?)x.DisplayOrder);
                mediaFile.DisplayOrder = max + 1 ?? 1;
            }

            if (mediaFile.IsImage())
            {
                _imageProcessor.EnforceMaxSize(ref stream, mediaFile, _mediaSettings);
            }

            var fileLocation = await GetFileLocation(fileName, mediaCategory);

            var fileSystem = GetFileSystem();
            mediaFile.FileUrl = await fileSystem.SaveFile(stream, fileLocation, contentType);


            await _session.TransactAsync(async session =>
            {
                await session.SaveAsync(mediaFile);
                if (mediaCategory != null)
                {
                    mediaCategory.Files.Add(mediaFile);
                    await session.SaveOrUpdateAsync(mediaCategory);
                }
            });

            await stream.DisposeAsync();
            return mediaFile;
        }

        public async Task DeleteFile(int id)
        {
            var file = _session.Get<MediaFile>(id);
            if (file == null)
                return;

            // remove file from the file list for its category, to prevent missing item exception
            file.MediaCategory?.Files.Remove(file);


            var fileSystem = GetFileSystem();
            foreach (var resizedImage in
                file.ResizedImages)
            {
                if (await fileSystem.Exists(resizedImage.Url))
                    await fileSystem.Delete(resizedImage.Url);
            }

            await fileSystem.Delete(file.FileUrl);

            await _session.TransactAsync((session, ct) => session.DeleteAsync(file, ct));
        }

        public async Task SaveFile(MediaFile mediaFile)
        {
            await _session.TransactAsync(session => session.SaveOrUpdateAsync(mediaFile));
        }

        public async Task<string> GetFileLocation(MediaFile mediaFile, Size imageSize, bool getCdn = false)
        {
            return await GetUrl(mediaFile, imageSize, getCdn);
        }

        public async Task<string> GetFileLocation(Crop crop, Size imageSize, bool getCdn = false)
        {
            return await GetUrl(crop, imageSize, getCdn);
        }

        public FilesPagedResult GetFilesPaged(int? categoryId, bool imagesOnly, int page = 1)
        {
            var currentSite = _siteLocator.GetCurrentSite();
            var queryOver = _session.QueryOver<MediaFile>().Where(file => file.Site == currentSite);

            if (categoryId.HasValue)
                queryOver = queryOver.Where(file => file.MediaCategory.Id == categoryId);

            if (imagesOnly)
                queryOver.Where(file => file.FileExtension.IsIn(MediaFileExtensions.ImageExtensions));

            var mediaFiles = queryOver.OrderBy(file => file.CreatedOn)
                .Desc.Paged(page, _siteSettings.DefaultPageSize);
            return new FilesPagedResult(mediaFiles, mediaFiles.GetMetaData(), categoryId, imagesOnly);
        }

        public async Task<MediaFile> GetFileByUrl(string url)
        {
            return
                await _session.QueryOver<MediaFile>()
                    .Where(file => file.FileUrl == url)
                    .Take(1)
                    .Cacheable()
                    .SingleOrDefaultAsync();
        }

        public async Task<MediaFile> GetFile(string value)
        {
            var mediaFile = await GetFileByUrl(value);
            if (mediaFile != null)
                return mediaFile;

            var split = value.Split('-');
            var id = Convert.ToInt32(split[0]);
            return await _session.GetAsync<MediaFile>(id);
        }

        public async Task<string> GetFileUrl(MediaFile mediaFile, string value)
        {
            if (mediaFile == null)
                return null;
            if (string.Equals(mediaFile.FileUrl, value, StringComparison.OrdinalIgnoreCase))
                return mediaFile.FileUrl;

            var split = value.Split('-');
            var id = Convert.ToInt32(split[0]);
            var file = _session.Get<MediaFile>(id);
            var imageSize =
                file.GetSizes(_mediaSettings)
                    .FirstOrDefault(size =>
                        size.Size == new Size(Convert.ToInt32(split[1]), Convert.ToInt32(split[2])));
            return await GetFileLocation(file, imageSize.Size, false);
        }

        public async Task RemoveFolder(MediaCategory mediaCategory)
        {
            var currentSite = _siteLocator.GetCurrentSite();
            var folderLocation = $"{currentSite.Id}/{mediaCategory.UrlSegment}/";

            var fileSystem = GetFileSystem();
            await fileSystem.Delete(folderLocation);
        }

        public async Task CreateFolder(MediaCategory mediaCategory)
        {
            var currentSite = _siteLocator.GetCurrentSite();
            var folderLocation = $"{currentSite.Id}/{mediaCategory.UrlSegment}/";

            var fileSystem = GetFileSystem();
            await fileSystem.CreateDirectory(folderLocation);
        }

        public bool IsValidFileType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension) || extension.Length < 1)
                return false;
            return FileTypeUploadSettings.AllowedFileTypeList.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        public async Task DeleteFileSoft(int id)
        {
            var file = _session.Get<MediaFile>(id);
            if (file == null)
                return;
            file.MediaCategory?.Files.Remove(file);
            await _session.TransactAsync(session => session.DeleteAsync(file));
        }

        private async Task<string> GetFileLocation(string fileName, MediaCategory mediaCategory = null)
        {
            var fileNameOriginal = fileName.GetTidyFileName();

            var urlSegment = "root";
            if (mediaCategory != null)
                urlSegment = mediaCategory.UrlSegment;

            var currentSite = _siteLocator.GetCurrentSite();
            var folderLocation = $"{currentSite.Id}/{urlSegment}/";

            //check for duplicates
            var i = 1;
            var fileSystem = GetFileSystem();
            while (await fileSystem.Exists(folderLocation + fileName))
            {
                fileName = fileNameOriginal.Replace(Path.GetExtension(fileName), "") + i + Path.GetExtension(fileName);
                i++;
            }

            var fileLocation = $"{currentSite.Id}/{urlSegment}/{fileName}";
            return fileLocation;
        }

        protected virtual async Task<byte[]> LoadFile(string filePath)
        {
            var fileSystem = GetFileSystem();
            if (!await fileSystem.Exists(filePath))
                return new byte[0];
            return await fileSystem.ReadAllBytes(filePath);
        }

        public virtual async Task<string> GetUrl(MediaFile file, Size size, bool getCdnUrl)
        {
            return await GetCdnUrl(async () =>
            {
                var fileUrl = HttpUtility.UrlDecode(file.FileUrl);
                if (!file.IsImage())
                    return fileUrl;

                //check to see if the image already exists, if it does simply return it
                var requestedImageFileUrl = ImageProcessor.RequestedFileUrl(fileUrl, file.Size, size);
                if (requestedImageFileUrl == fileUrl)
                    return fileUrl;

                // if we've cached the file existing then we're fine
                IList<ResizedImage> resizedImages =
                    _session.Query<ResizedImage>().Where(image => image.MediaFile.Id == file.Id)
                        .WithOptions(x => x.SetCacheable(true)).ToList();
                if (resizedImages.Any(image => image.Url == requestedImageFileUrl))
                    return requestedImageFileUrl;

                // if it exists but isn't cached, we should add it to the cache
                var fileSystem = GetFileSystem();
                if (await fileSystem.Exists(requestedImageFileUrl))
                {
                    await CacheResizedImage(file, requestedImageFileUrl);
                    return requestedImageFileUrl;
                }

                //if we have got this far the image doesn't exist yet so we need to create the image at the requested size
                var fileBytes = await LoadFile(fileUrl);
                if (fileBytes.Length == 0)
                    return "";

                await _imageProcessor.SaveResizedImage(file, size, fileBytes, requestedImageFileUrl);

                // we also need to cache the resized image, to save making a request to find it
                await CacheResizedImage(file, requestedImageFileUrl);

                return requestedImageFileUrl;
            }, getCdnUrl);
        }

        private async Task<string> GetCdnUrl(Func<Task<string>> func, bool getUrl)
        {
            var url = await func();
            var fileSystem = GetFileSystem();
            var cdnInfo = await fileSystem.GetCdnInfo();
            if (!getUrl || cdnInfo == null || string.IsNullOrWhiteSpace(url))
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
                var filePath = HttpUtility.UrlDecode(crop.Url);
                var requestedImageFileUrl = ImageProcessor.RequestedFileUrl(filePath, crop.Size, size);

                // if we've cached the file existing then we're fine
                var resizedImages =
                    _session.QueryOver<ResizedImage>().Where(image => image.Crop.Id == crop.Id).Cacheable().List();
                if (resizedImages.Any(image => image.Url == requestedImageFileUrl))
                    return requestedImageFileUrl;

                // if it exists but isn't cached, we should add it to the cache
                var fileSystem = GetFileSystem();
                if (await fileSystem.Exists(requestedImageFileUrl))
                {
                    await CacheResizedImage(crop, requestedImageFileUrl);
                    return requestedImageFileUrl;
                }

                //if we have got this far the image doesn't exist yet so we need to create the image at the requested size
                var fileBytes = await LoadFile(filePath);
                if (fileBytes.Length == 0)
                    return "";

                await _imageProcessor.SaveResizedCrop(crop, size, fileBytes, requestedImageFileUrl);

                // we also need to cache the resized image, to save making a request to find it
                await CacheResizedImage(crop, requestedImageFileUrl);

                return requestedImageFileUrl;
            }, getCdnUrl);
        }

        private async Task CacheResizedImage(MediaFile file, string requestedImageFileUrl)
        {
            var resizedImage = new ResizedImage { Url = requestedImageFileUrl, MediaFile = file };
            file.ResizedImages.Add(resizedImage);
            await _session.TransactAsync(session => session.SaveAsync(resizedImage));
        }

        private async Task CacheResizedImage(Crop crop, string requestedImageFileUrl)
        {
            var resizedImage = new ResizedImage { Url = requestedImageFileUrl, Crop = crop };
            crop.ResizedImages.Add(resizedImage);
            await _session.TransactAsync(session => session.SaveAsync(resizedImage));
        }
    }
}