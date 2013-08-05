using System;
using System.Web.Mvc;
using MrCMS.Settings;

namespace MrCMS.Website.Binders
{
    public class FileSystemSettingsModelBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return MrCMSApplication.Get<FileSystemSettings>();
        }
    }
}