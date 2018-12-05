using MrCMS.Installation.Models;

namespace MrCMS.Installation.Services
{
    public interface IInitializeDatabase
    {
        void Initialize(InstallModel model);
    }
}