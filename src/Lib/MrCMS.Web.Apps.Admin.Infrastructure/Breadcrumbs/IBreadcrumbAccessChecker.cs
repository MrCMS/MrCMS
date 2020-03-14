using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public interface IBreadcrumbAccessChecker
    {
        Task<bool> CanAccess(Breadcrumb breadcrumb);
    }
}