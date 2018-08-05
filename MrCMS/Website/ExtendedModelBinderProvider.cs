using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace MrCMS.Website
{
    public class ExtendedModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.BindingInfo.BinderType != null && typeof(IExtendedModelBinder).IsAssignableFrom(context.BindingInfo.BinderType))
            {
                var internalBinder = context.CreateBinder(context.Metadata);
                return ActivatorUtilities.CreateInstance(context.Services, context.BindingInfo.BinderType, internalBinder) as IModelBinder;
            }

            return null;
        }
    }
}