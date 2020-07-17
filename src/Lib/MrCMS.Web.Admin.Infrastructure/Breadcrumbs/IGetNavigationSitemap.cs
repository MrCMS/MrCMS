using System.Threading.Tasks;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public interface IGetNavigationSitemap
    {
        Sitemap GetNavigation();
    }
}