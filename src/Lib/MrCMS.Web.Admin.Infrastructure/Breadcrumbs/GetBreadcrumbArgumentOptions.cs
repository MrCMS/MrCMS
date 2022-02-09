using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public class GetBreadcrumbArgumentOptions : IGetBreadcrumbArgumentOptions
    {
        public void AppendArguments(ViewResult viewResult, IDictionary<string, object> arguments)
        {
            if (viewResult == null)
                return;

            if (viewResult.Model != null)
                arguments["view-model"] = viewResult.Model;

            foreach (var key in viewResult.ViewData.Keys)
                arguments[key] = viewResult.ViewData[key];
        }
    }
}