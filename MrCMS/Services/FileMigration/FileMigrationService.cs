using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Batching;
using MrCMS.Batching.Services;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Settings;
using Newtonsoft.Json;
using NHibernate;
using Ninject;

namespace MrCMS.Services.FileMigration
{
    public class FileMigrationService : IFileMigrationService
    {
        private readonly Dictionary<string, IFileSystem> _allFileSystems;
        private readonly ICreateBatch _createBatch;
        private readonly FileSystemSettings _fileSystemSettings;
        private readonly IKernel _kernel;
        private readonly ISession _session;
        private readonly UrlHelper _urlHelper;

        public FileMigrationService(IKernel kernel, FileSystemSettings fileSystemSettings, ISession session,
            ICreateBatch createBatch, UrlHelper urlHelper)
        {
            _allFileSystems =
                TypeHelper.GetAllTypesAssignableFrom<IFileSystem>()
                    .Select(type => kernel.Get(type) as IFileSystem)
                    .ToDictionary(system => system.GetType().FullName);
            _fileSystemSettings = fileSystemSettings;
            _session = session;
            _createBatch = createBatch;
            _kernel = kernel;
            _urlHelper = urlHelper;
        }

        public IFileSystem CurrentFileSystem
        {
            get
            {
                string storageType = _fileSystemSettings.StorageType;
                return _allFileSystems[storageType];
            }
        }

        public FileMigrationResult MigrateFiles()
        {
            IList<MediaFile> mediaFiles = _session.QueryOver<MediaFile>().List();

            List<Guid> guids =
                mediaFiles.Where(
                    mediaFile =>
                        MediaFileExtensions.GetFileSystem(mediaFile.FileUrl, _allFileSystems.Values) !=
                        CurrentFileSystem)
                    .Select(file => file.Guid).ToList();

            if (!guids.Any())
            {
                return new FileMigrationResult
                {
                    MigrationRequired = false,
                    Message = "Migration not required"
                };
            }

            BatchCreationResult result = _createBatch.Create(guids.Chunk(10)
                .Select(set => new MigrateFilesBatchJob
                {
                    Data = JsonConvert.SerializeObject(set.ToHashSet()),
                }));

            return new FileMigrationResult
            {
                MigrationRequired = true,
                Message = string.Format(
                    "Batch created. Click <a target=\"_blank\" href=\"{0}\">here</a> to view and start.".AsResource(
                        _kernel),
                    _urlHelper.Action("Show", "BatchRun", new {id = result.InitialBatchRun.Id}))
            };
        }
    }

    public class FileMigrationResult
    {
        public bool MigrationRequired { get; set; }
        public string Message { get; set; }
    }
}