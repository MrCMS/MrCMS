using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class UpdateAdminViewModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType.IsImplementationOf(typeof(UpdateAdminViewModel<>)))
                return new UpdateAdminViewModelBinder(context.CreateBinder);

            return null;
        }
    }
}