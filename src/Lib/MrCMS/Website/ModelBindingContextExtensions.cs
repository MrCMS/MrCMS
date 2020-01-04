using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Helpers;


namespace MrCMS.Website
{
    public static class ModelBindingContextExtensions
    {
        public static int? GetId(this ModelBindingContext context)
        {
            var modelName = "id";
            // Try to fetch the value of the argument by name
            var valueProviderResult =
                context.ValueProvider.GetValue(modelName);

            //var modelType = context.ModelType;
            if (valueProviderResult == ValueProviderResult.None)
                return null;
            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(value))
                return null;

            if (!int.TryParse(value, out var id))
            {
                // Non-integer arguments result in model state errors
                context.ModelState.TryAddModelError(
                    context.ModelName,
                    "Id must be an integer.");
                return null;
            }

            return id;
        }
        public static async Task<object> GetEntityById(this ModelBindingContext context, Type type)
        {
            var modelName = "id";
            // Try to fetch the value of the argument by name
            var valueProviderResult =
                context.ValueProvider.GetValue(modelName);

            //var modelType = context.ModelType;
            if (valueProviderResult == ValueProviderResult.None)
                return null;
            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(value))
                return null;

            if (!int.TryParse(value, out var id))
            {
                // Non-integer arguments result in model state errors
                context.ModelState.TryAddModelError(
                    context.ModelName,
                    "Id must be an integer.");
                return null;
            }

            return await context.HttpContext.RequestServices.GetRequiredService<IDataReader>()
                       .GlobalGet(type, id) ??
                   GetDefault(type);
        }
        public static async Task<TEntity> GetEntityById<TEntity>(this ModelBindingContext context)
            where TEntity : SystemEntity
        {
            return (TEntity)await GetEntityById(context, typeof(TEntity));
        }

        private static object GetDefault(Type type)
        {
            return type.IsAbstract
                ? type.GetDefaultValue()
                : Activator.CreateInstance(type);
        }
    }
}