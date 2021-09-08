using System.Threading.Tasks;
using MrCMS.Installation.Models;

namespace MrCMS.Installation.Services
{
    public interface IInitializeDatabase
    {
        Task Initialize(InstallModel model);
    }
}