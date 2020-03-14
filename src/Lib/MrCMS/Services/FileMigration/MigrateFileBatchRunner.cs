using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Batching;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Settings;
using Newtonsoft.Json;

namespace MrCMS.Services.FileMigration
{
    public class MigrateFileBatchRunner : BaseBatchJobExecutor<MigrateFilesBatchJob>
    {
        private readonly IEnumerable<IFileSystem> _fileSystems;
        private readonly IRepository<MediaFile> _mediaFileRepository;
        private readonly IRepository<ResizedImage> _resizedImageRepository;
        private readonly IConfigurationProvider _configurationProvider;

        public MigrateFileBatchRunner(
            IRepository<MediaFile> mediaFileRepository,
            IRepository<ResizedImage> resizedImageRepository,
            IServiceProvider serviceProvider, IConfigurationProvider configurationProvider)
        {
            _fileSystems =
                TypeHelper.GetAllTypesAssignableFrom<IFileSystem>()
                    .Select(type => serviceProvider.GetService(type) as IFileSystem)
                    .ToList();
            ;
            _mediaFileRepository = mediaFileRepository;
            _resizedImageRepository = resizedImageRepository;
            _configurationProvider = configurationProvider;
        }

        public async Task<IFileSystem> GetCurrentFileSystem()
        {
            var fileSystemSettings = await _configurationProvider.GetSiteSettings<FileSystemSettings>();
            var storageType = fileSystemSettings.StorageType;
            return _fileSystems.FirstOrDefault(system => system.GetType().FullName == storageType);
        }

        private string GetNewFilePath(MediaFile file)
        {
            var fileUrl = file.FileUrl;
            var id = file.Site.Id;
            var indexOf = file.FileUrl.IndexOf(string.Format("/{0}/", id), StringComparison.OrdinalIgnoreCase);
            var newFilePath = fileUrl.Substring(indexOf + 1);
            return newFilePath;
        }

        protected override async Task<BatchJobExecutionResult> OnExecuteAsync(MigrateFilesBatchJob batchJob,
            CancellationToken token)
        {
            var guids = JsonConvert.DeserializeObject<HashSet<Guid>>(batchJob.Data).ToList();

            var mediaFiles = await _mediaFileRepository.Query().Where(x => guids.Contains(x.Guid)).ToListAsync();

            foreach (var mediaFile in mediaFiles)
            {
                var from = await MediaFileExtensions.GetFileSystem(mediaFile, _fileSystems);
                var to = await GetCurrentFileSystem();
                if (from.GetType() == to.GetType())
                    continue;

                await _mediaFileRepository.Transact(async (repo, ct) =>
                 {
                     // remove resized images (they will be regenerated on the to system)
                     foreach (var resizedImage in await _resizedImageRepository.Query().Where(x => x.MediaFileId == mediaFile.Id).ToListAsync())
                     {
                         // check for resized file having same url as the original - 
                         // do not delete from disc yet in that case, or else it will cause an error when copying
                         if (resizedImage.Url != mediaFile.FileUrl)
                         {
                             await from.Delete(resizedImage.Url);
                         }
                         mediaFile.ResizedImages.Remove(resizedImage);
                         await _resizedImageRepository.Delete(resizedImage, ct);
                     }

                     var existingUrl = mediaFile.FileUrl;
                     using (var readStream = await @from.GetReadStream(existingUrl))
                     {
                         mediaFile.FileUrl = await to.SaveFile(readStream, GetNewFilePath(mediaFile),
                             mediaFile.ContentType);
                     }
                     await from.Delete(existingUrl);

                     await repo.Update(mediaFile, ct);
                 }, token);
            }
            return BatchJobExecutionResult.Success();
        }
    }
}