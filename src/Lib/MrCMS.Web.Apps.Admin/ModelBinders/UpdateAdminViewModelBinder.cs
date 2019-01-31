using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Admin.Infrastructure.Models.Tabs;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class UpdateAdminViewModelBinder : IModelBinder
    {
        private readonly Func<ModelMetadata, IModelBinder> _createBinder;

        public UpdateAdminViewModelBinder(Func<ModelMetadata, IModelBinder> createBinder)
        {
            _createBinder = createBinder;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var adminViewModelType =
                bindingContext.ModelType.GetBaseTypes(x =>
                    x.GetGenericTypeDefinition() == typeof(UpdateAdminViewModel<>));

            var type = adminViewModelType.FirstOrDefault();
            if (type == null)
                return;

            var modelType = type.GenericTypeArguments[0];
            var id = bindingContext.GetId();
            if (!id.HasValue)
                return;

            var instance = Activator.CreateInstance(bindingContext.ModelType);
            if (!(instance is IUpdateAdminViewModel model))
                return;

            var serviceProvider = bindingContext.HttpContext.RequestServices;
            var metadataProvider = serviceProvider.GetRequiredService<IModelMetadataProvider>();

            var properties = metadataProvider.GetMetadataForProperties(instance.GetType());

            foreach (var modelMetadata in properties.Where(x => !x.IsComplexType))
            {
                var value = await BindModelType(bindingContext, modelMetadata);
                modelMetadata.PropertySetter(model, value);
            }

            model.Models = new List<object>();

            var getEditTabsService = serviceProvider.GetRequiredService<IGetEditTabsService>();
            var tabs = getEditTabsService.GetEditTabs(serviceProvider, modelType, id.Value)
                .OfType<IAdminTab>()
                .ToList();


            foreach (var tab in tabs.OrderBy(x => x.Order))
            {
                var tabModel = await BindTabModel(bindingContext, metadataProvider, tab);
                foreach (var o in tabModel)
                {
                    model.Models.Add(o);
                }
            }

            // add implementation view models
            var entity = await serviceProvider.GetRequiredService<ISession>().GetAsync(modelType, id);

            var implementationModelTypes = TypeHelper.GetAllConcreteTypesAssignableFrom(
                typeof(IUpdatePropertiesViewModel<>).MakeGenericType(entity.GetType()));

            foreach (var viewModelType in implementationModelTypes)
            {
                var boundModel = await BindModelType(bindingContext, metadataProvider.GetMetadataForType(viewModelType));
                model.Models.Add(boundModel);
            }

            bindingContext.Result = ModelBindingResult.Success(model);
        }

        private async Task<IEnumerable<object>> BindTabModel(ModelBindingContext bindingContext, IModelMetadataProvider metadataProvider,
            IAdminTab tab)
        {
            var objects = new List<object>();
            if (tab.ModelType != null)
            {
                Type type = tab.ModelType;
                if (type != null)
                {
                    var model = await BindModelType(bindingContext, metadataProvider.GetMetadataForType(type));
                    objects.Add(model);
                }
            }

            foreach (var tabChild in tab.Children)
            {
                var children = await BindTabModel(bindingContext, metadataProvider, tabChild);
                objects.AddRange(children);
            }

            return objects;
        }

        private async Task<object> BindModelType(ModelBindingContext bindingContext, ModelMetadata metadata)
        {
            var modelBindingContext = new DefaultModelBindingContext
            {
                BinderModelName = metadata.BinderModelName,
                BindingSource = metadata.BindingSource,
                IsTopLevelObject = true,
                //Model = Activator.CreateInstance(metadata.ModelType),
                ModelMetadata = metadata,
                ModelName = metadata.Name ?? string.Empty,
                ModelState = new ModelStateDictionary(),
                ValueProvider = bindingContext.ValueProvider
            };

            var modelBinder = _createBinder(metadata);
            await modelBinder.BindModelAsync(modelBindingContext);
            var model = modelBindingContext.Result.Model;
            return model;
        }
    }
}