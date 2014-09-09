using System;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website.Controllers;

namespace MrCMS.Website.Routing
{
    public interface IHandleStandardMrCMSPageExecution
    {
        void Handle(RequestContext context, Webpage webpage, Action<MrCMSUIController> beforeExecute = null);
    }
}