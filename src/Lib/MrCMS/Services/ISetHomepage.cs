using System.Threading.Tasks;

namespace MrCMS.Services
{
    public interface ISetHomepage
    {
        Task<int> Set();
    }
}