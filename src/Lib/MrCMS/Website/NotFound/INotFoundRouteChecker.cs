using System.Threading.Tasks;

namespace MrCMS.Website.NotFound
{
    public interface INotFoundRouteChecker
    {
        Task<NotFoundCheckResult> Check(string path, string query);
    }
}