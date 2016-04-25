using System;
using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class GetTaskInfoFromTypeModelBinder : MrCMSDefaultModelBinder
    {
        private readonly ITaskAdminService _taskAdminService;

        public GetTaskInfoFromTypeModelBinder(ITaskAdminService taskAdminService, IKernel kernel) : base(kernel)
        {
            _taskAdminService = taskAdminService;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            var valueFromContext = GetValueFromContext(controllerContext, "type");
            var taskInfo = _taskAdminService.GetTaskUpdateData(valueFromContext);
            return taskInfo;
        }
    }
}