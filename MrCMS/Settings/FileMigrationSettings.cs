using System;
using System.Collections.Generic;
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
            get { return FilesToMigrate.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries); }
        }

        public IEnumerable<string> MigratedFilesList
        {
            get { return MigratedFiles.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries); }
        }

        public IEnumerable<string> ExistingFiles
        {
            get
            {
                foreach (string file in FilesToMigrateList)
                    yield return file;
                foreach (string file in MigratedFilesList)
                    yield return file;
            }
        }

        public override bool RenderInSettings
        {
            get { return false; }
        }

        public void AddFileToMigrate(string file)
        {
            FilesToMigrate += (Environment.NewLine + file);
        }

        public void MoveFileToMigrated(string file)
        {
            List<string> list = FilesToMigrateList.ToList();
            list.Remove(file);
            FilesToMigrate = string.Join(Environment.NewLine, list);
            MigratedFiles += (Environment.NewLine + file);
        }
    }
}