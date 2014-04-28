using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Tasks;
using MrCMS.Website;
using Newtonsoft.Json;
using NHibernate;
using Ninject;

namespace MrCMS.Services.FileMigration
{
    public class FileMigrationService : IFileMigrationService
    {
        private readonly IEnumerable<IFileSystem> _allFileSystems;
        private readonly FileSystemSettings _fileSystemSettings;
        private readonly ISession _session;
        private readonly Site _site;

        public FileMigrationService(IKernel kernel, FileSystemSettings fileSystemSettings, ISession session, Site site)
        {
            _allFileSystems =
                TypeHelper.GetAllTypesAssignableFrom<IFileSystem>()
                    .Select(type => kernel.Get(type) as IFileSystem)
                    .ToList();
            _fileSystemSettings = fileSystemSettings;
            _session = session;
            _site = site;
        }

        public IFileSystem CurrentFileSystem
        {
            get
            {
                var storageType = _fileSystemSettings.StorageType;
                return _allFileSystems.FirstOrDefault(system => system.GetType().FullName == storageType);
            }
        }

        public void MigrateFiles()
        {
            var mediaFiles = _session.QueryOver<MediaFile>().Where(file => file.Site == _site).List();
            var filesToMove =
                mediaFiles.Where(mediaFile => mediaFile.GetFileSystem(_allFileSystems) != CurrentFileSystem)
                    .Select(file => new MoveFileData
                                    {
                                        FileId = file.Id,
                                        From = file.GetFileSystem(_allFileSystems).GetType().FullName,
                                        To = CurrentFileSystem.GetType().FullName
                                    }).ToList();

            foreach (var queuedTask in filesToMove.Select(moveFileData => new QueuedTask
                                                                          {
                                                                              Data = JsonConvert.SerializeObject(moveFileData),
                                                                              Type = typeof(MoveFile).FullName,
                                                                              Status = TaskExecutionStatus.Pending
                                                                          }))
            {
                CurrentRequestData.QueuedTasks.Add(queuedTask);
            }
        }
    }
}