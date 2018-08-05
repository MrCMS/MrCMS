using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities;
using MrCMS.Helpers;
using NHibernate;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace MrCMS.Website
{
    public class SystemEntityBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType.IsImplementationOf(typeof(SystemEntity)))
            {
                return new BinderTypeModelBinder(typeof(SystemEntityBinder));
                //return GetModelBinder(context.Metadata, context.Services);
            }

            return null;
        }

        //public static IModelBinder GetModelBinder(ModelMetadata initialMetadata, IServiceProvider serviceProvider)
        //{
        //    var modelType = GetModelType(initialMetadata, serviceProvider);
        //    if (modelType == null)
        //        return null;

        //    var metadataProvider = serviceProvider.GetRequiredService<IModelMetadataProvider>();
        //    var metadata = metadataProvider.GetMetadataForType(modelType);
        //    var dictionary = new Dictionary<ModelMetadata, IModelBinder>();
        //    var modelBinderFactory = serviceProvider.GetRequiredService<IModelBinderFactory>();
        //    foreach (var property in metadata.Properties.Where(x => !x.IsCollectionType))
        //    {
        //        var modelBinderFactoryContext = new ModelBinderFactoryContext
        //        {
        //            BindingInfo = BindingInfo.GetBindingInfo(Enumerable.Empty<object>(), property),
        //            Metadata = property
        //        };

        //        dictionary.Add(property, modelBinderFactory.CreateBinder(modelBinderFactoryContext));
        //    }

        //    var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        //    return new SystemEntityBinder(dictionary, loggerFactory);
        //}

        //private static Type GetModelType(ModelMetadata metadata, IServiceProvider serviceProvider)
        //{
        //    if (!metadata.ModelType.IsAbstract) return metadata.ModelType;

        //    var contextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        //    var sessionFactory = serviceProvider.GetRequiredService<ISessionFactory>();
        //    if (int.TryParse(contextAccessor.HttpContext.GetRouteValue("id")?.ToString() ?? string.Empty, out var id)
        //        && sessionFactory.OpenFilteredSession(serviceProvider).Get(metadata.ModelType, id) is SystemEntity entity)
        //        return entity.Unproxy().GetType();

        //    return null;
        //}
    }
}