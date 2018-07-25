using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MrCMS.Entities;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Website
{
    public class SystemEntityBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType.IsImplementationOf(typeof(SystemEntity)))
            {
                return new BinderTypeModelBinder(typeof(SystemEntityBinder<>).MakeGenericType(context.Metadata.ModelType));
            }

            return null;
        }
    }
    public class SystemEntityBinder<T> : IModelBinder where T : SystemEntity
    {
        private readonly ISession _session;
        public SystemEntityBinder(ISession session)
        {
            _session = session;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // Specify a default argument name if none is set by ModelBinderAttribute
            var modelName = bindingContext.BinderModelName;
            if (string.IsNullOrEmpty(modelName))
            {
                modelName = "id";
            }

            // Try to fetch the value of the argument by name
            var valueProviderResult =
                bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName,
                valueProviderResult);

            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            int id = 0;
            if (!int.TryParse(value, out id))
            {
                // Non-integer arguments result in model state errors
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    "Id must be an integer.");
                return Task.CompletedTask;
            }

            // Model will be null if not found, including for 
            // out of range id values (0, -3, etc.)
            var model = _session.Get(typeof(T), id);
            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}