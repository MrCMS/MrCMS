using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IWebpageUIService
    {
        ActionResult GetContent(Controller controller, Webpage webpage, Func<ActionResult> func,
            IQueryCollection queryData = null);

        T GetPage<T>(int id) where T : Webpage;
    }
}