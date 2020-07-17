using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public interface IGetBreadcrumbArgumentOptions
    {
        void AppendArguments(ViewResult viewResult, IDictionary<string, object> arguments);
    }
}