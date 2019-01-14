using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MrCMS.Website
{
    public interface IGetControllerFromFilterContext
    {
        Controller GetController(ActionExecutingContext context);
        Controller GetController(ActionExecutedContext context);
        Controller GetController(ResultExecutingContext context);
        Controller GetController(ResultExecutedContext context);
    }
}