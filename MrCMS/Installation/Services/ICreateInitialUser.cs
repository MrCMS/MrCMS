using MrCMS.Installation.Models;

namespace MrCMS.Installation.Services
{
    public interface ICreateInitialUser
    {
        void Create(InstallModel model);
    }
}