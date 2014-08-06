using System.Web.Mvc;
using MrCMS.Entities;

namespace MrCMS.Services
{
    public abstract class CustomBinderBase
    {
        public abstract void ApplyCustomBinding(SystemEntity entity, ControllerContext controllerContext);
    }

    public abstract class CustomBinderBase<T> : CustomBinderBase where T : SystemEntity
    {
        public abstract void ApplyCustomBinding(T entity, ControllerContext controllerContext);

        public override void ApplyCustomBinding(SystemEntity entity, ControllerContext controllerContext)
        {
            ApplyCustomBinding(entity as T, controllerContext);
        }
    }
}