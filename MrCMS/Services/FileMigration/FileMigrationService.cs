using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website;
using Newtonsoft.Json;
using NHibernate;
using Ninject;

namespace MrCMS.Services.FileMigration
{
    public class FileMigrationService : IFileMigrationService
    {
        private readonly IEnumerable<IFileSystem> _allFileSystems;
        private readonly ICreateBatchRun _createBatchRun;
        private readonly FileSystemSettings _fileSystemSettings;
        private readonly ISession _session;
        private readonly Site _site;
        private readonly IStatelessSession _statelessSession;

        public FileMigrationService(IKernel kernel, FileSystemSettings fileSystemSettings, ISession session, Site site,
            IStatelessSession statelessSession, ICreateBatchRun createBatchRun)
        {
            _allFileSystems =
                TypeHelper.GetAllTypesAssignableFrom<IFileSystem>()
                    .Select(type => kernel.Get(type) as IFileSystem)
                    .ToList();
            _fileSystemSettings = fileSystemSettings;
            _session = session;
            _site = site;
            _statelessSession = statelessSession;
            _createBatchRun = createBatchRun;
        }

        public IFileSystem CurrentFileSystem
        {
            get
            {
                string storageType = _fileSystemSettings.StorageType;
                return _allFileSystems.FirstOrDefault(system => system.GetType().FullName == storageType);
            }
        }

        public void MigrateFiles()
        {
            IList<MediaFile> mediaFiles = _session.QueryOver<MediaFile>().Where(file => file.Site == _site).List();
            List<Guid> guids =
                mediaFiles.Where(
                    mediaFile =>
                        MediaFileExtensions.GetFileSystem(mediaFile.FileUrl, _allFileSystems) != CurrentFileSystem)
                    .Select(file => file.Guid).ToList();

            DateTime now = CurrentRequestData.Now;
            // we need to make sure that the site is loaded from the correct session
            var site = _statelessSession.Get<Site>(_site.Id);
            var batch = new Batch
            {
                BatchJobs = new List<BatchJob>(),
                BatchRuns = new List<BatchRun>(),
                Site = site,
                CreatedOn = now,
                UpdatedOn = now
            };
            _statelessSession.Transact(session => session.Insert(batch));
            _statelessSession.Transact(session =>
            {
                foreach (MigrateFilesBatchJob importNewsBatchJob
                    in guids.Chunk(10)
                        .Select(set => new MigrateFilesBatchJob
                        {
                            Batch = batch,
                            Data = JsonConvert.SerializeObject(set.ToHashSet()),
                            Site = site,
                            CreatedOn = now,
                            UpdatedOn = now
                        }))
                {
                    batch.BatchJobs.Add(importNewsBatchJob);
                    session.Insert(importNewsBatchJob);
                }
            });
            BatchRun batchRun = _createBatchRun.Create(batch);
            batch.BatchRuns.Add(batchRun);
            _statelessSession.Transact(session => session.Update(batch));
        }
    }
}