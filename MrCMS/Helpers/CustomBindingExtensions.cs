using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class CustomBindingExtensions
    {
        public static void ApplyCustomBinding<T>(this T entity, ControllerContext context) where T : SystemEntity
        {
            MrCMSApplication.Get<ICustomBindingService>().ApplyCustomBinding(entity, context);
        }
    }
}