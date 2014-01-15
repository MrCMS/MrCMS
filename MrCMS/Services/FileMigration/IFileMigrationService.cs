using System.Collections.Generic;
using System.IO;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services.FileMigration
{
    public interface IFileMigrationService
    {
        void MigrateFilesToAzure(int numberOfFiles = 100);
        int FilesToMigrate();
    }

    public class FileMigrationService : IFileMigrationService
    {
        private readonly AzureFileSystem _azureFileSystem;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly FileMigrationSettings _fileMigrationSettings;
        private readonly FileSystem _fileSystem;
        private readonly FileSystemSettings _fileSystemSettings;
        private readonly ImageProcessor _imageProcessor;
        private readonly ISession _session;
        private readonly Site _site;
        private Dictionary<MediaFile, string> _filesToUpdate;

        public FileMigrationService(Site site, ISession session, FileSystem fileSystem, AzureFileSystem azureFileSystem,
                                    FileSystemSettings fileSystemSettings, IConfigurationProvider configurationProvider,
                                    FileMigrationSettings fileMigrationSettings)
        {
            _site = site;
            _session = session;
            _fileSystem = fileSystem;
            _azureFileSystem = azureFileSystem;
            _fileSystemSettings = fileSystemSettings;
            _configurationProvider = configurationProvider;
            _fileMigrationSettings = fileMigrationSettings;
            _imageProcessor = new ImageProcessor(_session, _fileSystem, null);
        }

        public void MigrateFilesToAzure(int numberOfFiles = 100)
        {
            if (_fileSystemSettings.StorageType != typeof(AzureFileSystem).FullName)
                return;

            PrimeFileMigrationSettings();

            _session.Transact(session =>
                                  {
                                      _filesToUpdate = new Dictionary<MediaFile, string>();
                                      foreach (string file in _fileMigrationSettings.FilesToMigrateList.Take(numberOfFiles))
                                          MoveFile(file);

                                      UpdateUrls();
                                  });
        }

        public int FilesToMigrate()
        {
            if (_fileSystemSettings.StorageType != typeof(AzureFileSystem).FullName)
                return 0;

            PrimeFileMigrationSettings();

            return _fileMigrationSettings.FilesToMigrateList.Count();
        }

        private void UpdateUrls()
        {
            foreach (var pair in _filesToUpdate)
            {
                pair.Key.FileUrl = pair.Value;
                var resizedImages = pair.Key.ResizedImages.ToList();
                foreach (var resizedImage in resizedImages)
                {
                    pair.Key.ResizedImages.Remove(resizedImage);
                    _session.Delete(resizedImage);
                }
                _session.Update(pair.Key);
            }
        }

        private void MoveFile(string file)
        {
            MediaFile fileByUrl = _imageProcessor.GetImage(file);

            using (var memoryStream = new MemoryStream())
            {
                if (fileByUrl != null)
                {
                    _fileSystem.WriteToStream(file, memoryStream);
                    memoryStream.Position = 0;
                    string result = _azureFileSystem.SaveFile(memoryStream, file.Substring(1), fileByUrl.ContentType);
                    if (fileByUrl.FileUrl == file)
                        _filesToUpdate[fileByUrl] = result;
                }
            }
            _fileMigrationSettings.MoveFileToMigrated(file);
            _configurationProvider.SaveSettings(_fileMigrationSettings);
        }

        private void PrimeFileMigrationSettings()
        {
            IEnumerable<string> files = _fileSystem.GetFiles(string.Format("{0}", _site.Id));

            var existingFiles = new HashSet<string>(_fileMigrationSettings.ExistingFiles);
            foreach (string file in files.Where(s => !existingFiles.Contains(s)))
                _fileMigrationSettings.AddFileToMigrate(file);

            _configurationProvider.SaveSettings(_fileMigrationSettings);
        }
    }
}