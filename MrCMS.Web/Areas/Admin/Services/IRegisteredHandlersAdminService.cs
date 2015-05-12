using System.Collections.Generic;
using MrCMS.Website.Routing;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IRegisteredHandlersAdminService
    {
        List<IMrCMSRouteHandler> GetAllHandlers();
    }
}