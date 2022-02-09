using System.Threading.Tasks;

namespace MrCMS.Services.FileMigration
{
    public interface IFileMigrationService
    {
        Task<FileMigrationResult> MigrateFiles();
    }
}