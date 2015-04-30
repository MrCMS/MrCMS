using System.Collections.Generic;
using System.Linq;
using MrCMS.Website.Routing;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IRegisteredHandlersAdminService
    {
        List<IMrCMSRouteHandler> GetAllHandlers();
    }

    public class RegisteredHandlersAdminService : IRegisteredHandlersAdminService
    {
        private readonly IEnumerable<IMrCMSRouteHandler> _handlers;

        public RegisteredHandlersAdminService(IEnumerable<IMrCMSRouteHandler> handlers)
        {
            _handlers = handlers;
        }

        public List<IMrCMSRouteHandler> GetAllHandlers()
        {
            return _handlers.OrderByDescending(handler => handler.Priority).ToList();
        }
    }
}