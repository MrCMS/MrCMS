using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MrCMS.Website
{
    public class GetControllerFromFilterContext : IGetControllerFromFilterContext
    {
        public Controller GetController(ActionExecutingContext context)
        {
            return context.Controller as Controller;
        }

        public Controller GetController(ActionExecutedContext context)
        {
            return context.Controller as Controller;
        }

        public Controller GetController(ResultExecutingContext context)
        {
            return context.Controller as Controller;
        }

        public Controller GetController(ResultExecutedContext context)
        {
            return context.Controller as Controller;
        }
    }
}