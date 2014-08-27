namespace MrCMS.Installation
{
    public interface IFileSystemAccessService
    {
        InstallationResult EnsureAccessToFileSystem();
        void EmptyAppData();
    }
}