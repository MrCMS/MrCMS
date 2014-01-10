using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.Settings
{
    public class FileMigrationSettings : SiteSettingsBase
    {
        public FileMigrationSettings()
        {
            FilesToMigrate = "";
            MigratedFiles = "";
        }
        public string FilesToMigrate { get; set; }
        public string MigratedFiles { get; set; }

        public IEnumerable<string> FilesToMigrateList
        {
            get { return FilesToMigrate.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries); }
        }
        public IEnumerable<string> MigratedFilesList
        {
            get { return MigratedFiles.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries); }
        }

        public IEnumerable<string> ExistingFiles
        {
            get
            {
                foreach (var file in FilesToMigrateList)
                    yield return file;
                foreach (var file in MigratedFilesList)
                    yield return file;
            }
        }

        public void AddFileToMigrate(string file)
        {
            FilesToMigrate += (Environment.NewLine + file);
        }

        public void MoveFileToMigrated(string file)
        {
            var list = FilesToMigrateList.ToList();
            list.Remove(file);
            FilesToMigrate = string.Join(Environment.NewLine, list);
            MigratedFiles += (Environment.NewLine + file);
        }
    }
}