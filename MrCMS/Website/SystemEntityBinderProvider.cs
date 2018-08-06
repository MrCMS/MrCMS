using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.Website
{
    public class SystemEntityBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType.IsImplementationOf(typeof(SystemEntity)))
            {
                // this will just try and load the entity by id
                return new BinderTypeModelBinder(typeof(SystemEntityBinder));
            }

            return null;
        }
    }
}