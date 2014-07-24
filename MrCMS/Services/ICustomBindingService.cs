using System.Web.Mvc;
using MrCMS.Entities;

namespace MrCMS.Services
{
    public interface ICustomBindingService
    {
        void ApplyCustomBinding<T>(T entity, ControllerContext controllerContext) where T : SystemEntity;
    }
}