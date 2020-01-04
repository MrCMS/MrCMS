using System.Threading.Tasks;
using MrCMS.Installation.Models;

namespace MrCMS.Installation.Services
{
    public interface ICreateInitialUser
    {
        Task Create(InstallModel model);
    }
}