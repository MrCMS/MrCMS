using System.Threading.Tasks;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public interface IBreadcrumbAccessChecker
    {
        Task<bool> CanAccess(Breadcrumb breadcrumb);
    }
}