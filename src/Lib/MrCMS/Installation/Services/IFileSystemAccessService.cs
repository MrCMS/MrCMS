using MrCMS.Installation.Models;

namespace MrCMS.Installation.Services
{
    public interface IFileSystemAccessService
    {
        InstallationResult EnsureAccessToFileSystem();
        void EmptyAppData();
    }
}