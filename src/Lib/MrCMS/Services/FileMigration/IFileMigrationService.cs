using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Services.FileMigration
{
    public interface IFileMigrationService
    {
        Task<FileMigrationResult> MigrateFiles();
    }
}