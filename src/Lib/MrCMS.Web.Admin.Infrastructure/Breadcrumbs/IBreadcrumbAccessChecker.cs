using System.Threading.Tasks;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public interface IBreadcrumbAccessChecker
    {
        bool CanAccess(Breadcrumb breadcrumb);
    }
}