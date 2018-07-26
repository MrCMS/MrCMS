using System.Security.Principal;
using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface INotificationHubService
    {
        User GetUser(IPrincipal user);
    }
}