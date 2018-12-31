using MrCMS.Batching.Entities;

namespace MrCMS.Services.FileMigration
{
    public interface IFileMigrationService
    {
        FileMigrationResult MigrateFiles();
    }
}