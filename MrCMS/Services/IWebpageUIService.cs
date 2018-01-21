using System;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Services
{
    public interface IWebpageUIService
    {
        ActionResult GetContent(Controller controller, Webpage webpage, Func<IHtmlHelper, MvcHtmlString> func,
            object queryData = null);
    }
}