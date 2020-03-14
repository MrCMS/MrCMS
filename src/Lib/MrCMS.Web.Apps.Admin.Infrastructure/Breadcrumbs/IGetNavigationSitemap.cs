using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public interface IGetNavigationSitemap
    {
        Task<Sitemap> GetNavigation();
    }
}