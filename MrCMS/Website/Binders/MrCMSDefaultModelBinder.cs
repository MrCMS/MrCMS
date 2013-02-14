using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Website.Binders
{
    public class MrCMSDefaultModelBinder : DefaultModelBinder
    {
        private readonly Func<ISession> _session;

        public MrCMSDefaultModelBinder(Func<ISession> session)
        {
            _session = session;
        }

        protected ISession Session
        {
            get { return _session.Invoke(); }
        }
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext.Controller is MrCMSAdminController &&
                typeof (SystemEntity).IsAssignableFrom(bindingContext.ModelType) &&
                (CreateModel(controllerContext, bindingContext, bindingContext.ModelType) == null || ShouldReturnNull(controllerContext,bindingContext)))
                return null;

            var bindModel = base.BindModel(controllerContext, bindingContext);
            if (bindModel is SiteEntity)
            {
                var model = bindModel;
                bindingContext.ModelMetadata =
                        ModelMetadataProviders.Current.GetMetadataForType(
                            () => CreateModel(controllerContext, bindingContext, model.GetType()), bindModel.GetType());
                bindingContext.ModelMetadata.Model = bindModel;
                bindModel = base.BindModel(controllerContext, bindingContext);
                var baseEntity = bindModel as SiteEntity;
                if (baseEntity != null)
                {
                    baseEntity.CustomBinding(controllerContext, Session);
                }
            }
            return bindModel;
        }

        protected virtual bool ShouldReturnNull(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return false;
        }

        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            if (propertyDescriptor.PropertyType.IsSubclassOf(typeof(SiteEntity)))
            {
                var id = controllerContext.HttpContext.Request[bindingContext.ModelName + ".Id"];
                int idVal;
                return int.TryParse(id, out idVal)
                           ? Session.Get(propertyDescriptor.PropertyType,
                                                               idVal)
                           : null;
            }
            if (typeof(IEnumerable<SiteEntity>).IsAssignableFrom(propertyDescriptor.PropertyType))
            {
                var baseValue = base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);

                var retrievedValue = baseValue as IEnumerable<SiteEntity>;

                if (retrievedValue != null)
                {
                    var baseEntities = retrievedValue as SiteEntity[] ?? retrievedValue.ToArray();
                    for (int i = 0; i < baseEntities.Count(); i++)
                    {
                        var deletedKey = propertyDescriptor.Name + "[" + i + "].Deleted";

                        var isDeleted =
                            Convert.ToString(controllerContext.RouteData.Values[deletedKey] ??
                                             controllerContext.HttpContext.Request[deletedKey]).Contains("true");

                        if (isDeleted)
                        {
                            var baseEntity = baseEntities.ElementAt(i);

                            foreach (var property in baseEntity.GetType().GetProperties().Where(info => info.PropertyType.IsSubclassOf(typeof(SiteEntity))))
                            {
                                var parent = property.GetValue(baseEntity, null) as SiteEntity;

                                if (parent == null)
                                    continue;
                                var makeGenericType = typeof(IList<>).MakeGenericType(baseEntity.GetType());
                                var lists =
                                    parent.GetType().GetProperties().Where(
                                        info => makeGenericType.IsAssignableFrom(info.PropertyType));

                                foreach (var info in lists)
                                {
                                    var infoValue = info.GetValue(parent, null);
                                    var methodInfo = info.PropertyType.GetMethodExt("Remove", new[] { baseEntity.GetType() });

                                    methodInfo.Invoke(infoValue, new[] { baseEntity });
                                }

                                property.SetValue(baseEntity, null, null);
                            }
                            Session.Delete(baseEntity);
                        }
                    }
                }

                return baseValue;
            }

            var value = base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
            return value;
        }

        protected static string GetValueFromContext(ControllerContext controllerContext, string request)
        {
            return controllerContext.HttpContext.Request[request];
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            var modelFromSession = GetModelFromSession(controllerContext, bindingContext.ModelName, modelType);
            if (modelFromSession != null)
                return modelFromSession;
            if (modelType == typeof (Webpage))
                return null;
            var model = base.CreateModel(controllerContext, bindingContext, modelType);

            return model;
        }

        public object GetModelFromSession(ControllerContext controllerContext, string modelName, Type modelType)
        {
            if (typeof(SystemEntity).IsAssignableFrom(modelType))
            {
                var subItem = string.Format("{0}.Id", modelName);

                var id =
                    Convert.ToString(controllerContext.RouteData.Values[subItem] ??
                                     controllerContext.HttpContext.Request[subItem]);

                int intId;
                if (int.TryParse(id, out intId))
                {
                    var obj = Session.Get(modelType, intId);
                    return obj ?? Activator.CreateInstance(modelType);
                }

                const string baseId = "Id";
                id = Convert.ToString(controllerContext.RouteData.Values[baseId] ??
                                      controllerContext.HttpContext.Request[baseId]);

                if (int.TryParse(id, out intId))
                {
                    var obj = Session.Get(modelType, intId);
                    return obj ?? Activator.CreateInstance(modelType);
                }
            }
            return null;
        }
    }
}