using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Batching;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Settings;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using Ninject;

namespace MrCMS.Services.FileMigration
{
    public class MigrateFileBatchRunner : BaseBatchJobExecutor<MigrateFilesBatchJob>
    {
        private readonly ISession _session;
        private readonly IEnumerable<IFileSystem> _fileSystems;
        private readonly FileSystemSettings _fileSystemSettings;
        public MigrateFileBatchRunner(ISession session,
            ISetBatchJobExecutionStatus setBatchJobJobExecutionStatus, IKernel kernel, FileSystemSettings fileSystemSettings)
            : base(setBatchJobJobExecutionStatus)
        {
            _session = session;
            _fileSystems =
                TypeHelper.GetAllTypesAssignableFrom<IFileSystem>()
                    .Select(type => kernel.Get(type) as IFileSystem)
                    .ToList(); ;
            _fileSystemSettings = fileSystemSettings;
        }
        public IFileSystem CurrentFileSystem
        {
            get
            {
                var storageType = _fileSystemSettings.StorageType;
                return _fileSystems.FirstOrDefault(system => system.GetType().FullName == storageType);
            }
        }

        protected override BatchJobExecutionResult OnExecute(MigrateFilesBatchJob batchJob)
        {
            var guids = JsonConvert.DeserializeObject<HashSet<Guid>>(batchJob.Data).ToList();

            var mediaFiles = _session.QueryOver<MediaFile>().Where(x => x.Guid.IsIn(guids)).List();


            foreach (var mediaFile in mediaFiles)
            {
                var from = MediaFileExtensions.GetFileSystem(mediaFile.FileUrl, _fileSystems);
                var to = CurrentFileSystem;
                if (from.GetType() == to.GetType())
                    continue;

                _session.Transact(session =>
                {
                    // remove resized images (they will be regenerated on the to system)
                    foreach (var resizedImage in mediaFile.ResizedImages.ToList())
                    {
                        // check for resized file having same url as the original - 
                        // do not delete from disc yet in that case, or else it will cause an error when copying
                        if (resizedImage.Url != mediaFile.FileUrl)
                        {
                            from.Delete(resizedImage.Url);
                        }
                        mediaFile.ResizedImages.Remove(resizedImage);
                        session.Delete(resizedImage);
                    }

                    var existingUrl = mediaFile.FileUrl;
                    using (var readStream = @from.GetReadStream(existingUrl))
                    {
                        mediaFile.FileUrl = to.SaveFile(readStream, GetNewFilePath(mediaFile),
                            mediaFile.ContentType);
                    }
                    from.Delete(existingUrl);

                    session.Update(mediaFile);
                });
            }


            return BatchJobExecutionResult.Success();
        }

        private string GetNewFilePath(MediaFile file)
        {
            var fileUrl = file.FileUrl;
            var id = file.Site.Id;
            var indexOf = file.FileUrl.IndexOf(string.Format("/{0}/", id), StringComparison.OrdinalIgnoreCase);
            var newFilePath = fileUrl.Substring(indexOf + 1);
            return newFilePath;
        }

        protected override Task<BatchJobExecutionResult> OnExecuteAsync(MigrateFilesBatchJob batchJob)
        {
            throw new NotImplementedException();
        }
    }
}