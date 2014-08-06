using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IUserUIPermissionsService
    {
        bool IsCurrentUserAllowed(Webpage webpage);
    }
}