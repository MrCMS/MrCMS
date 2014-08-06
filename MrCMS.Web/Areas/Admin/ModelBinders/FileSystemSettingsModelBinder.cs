using System;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class FileSystemSettingsModelBinder : MrCMSDefaultModelBinder
    {
        private readonly FileSystemSettings _fileSystemSettings;

        public FileSystemSettingsModelBinder(IKernel kernel, FileSystemSettings fileSystemSettings) : base(kernel)
        {
            _fileSystemSettings = fileSystemSettings;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                              Type modelType)
        {
            return _fileSystemSettings;
        }
    }
}