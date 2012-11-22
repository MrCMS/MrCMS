using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Website
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
            var bindModel = base.BindModel(controllerContext, bindingContext);
            if (bindModel is BaseEntity)
            {
                bindingContext.ModelMetadata =
                        ModelMetadataProviders.Current.GetMetadataForType(
                            () => CreateModel(controllerContext, bindingContext, bindModel.GetType()), bindModel.GetType());
                bindingContext.ModelMetadata.Model = bindModel;
                bindModel = base.BindModel(controllerContext, bindingContext);
                (bindModel as BaseEntity).CustomBinding(controllerContext,Session);
            }
            return bindModel;
        }

        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            if (propertyDescriptor.PropertyType.IsSubclassOf(typeof(BaseEntity)))
            {
                var id = controllerContext.HttpContext.Request[bindingContext.ModelName + ".Id"];
                int idVal;
                return int.TryParse(id, out idVal)
                           ? Session.Get(propertyDescriptor.PropertyType,
                                                               idVal)
                           : null;
            }
            if (typeof(IEnumerable<BaseEntity>).IsAssignableFrom(propertyDescriptor.PropertyType))
            {
                var baseValue = base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);

                var retrievedValue = baseValue as IEnumerable<BaseEntity>;
                
                var baseEntities = retrievedValue as BaseEntity[] ?? retrievedValue.ToArray();
                for (int i = 0; i < baseEntities.Count(); i++)
                {
                    var deletedKey = propertyDescriptor.Name + "[" + i + "].Deleted";

                    var isDeleted =
                        Convert.ToString(controllerContext.RouteData.Values[deletedKey] ??
                                         controllerContext.HttpContext.Request[deletedKey]).Contains("true");

                    if (isDeleted)
                    {
                        var baseEntity = baseEntities.ElementAt(i);

                        foreach (var property in baseEntity.GetType().GetProperties().Where(info => info.PropertyType.IsSubclassOf(typeof(BaseEntity))))
                        {
                            var parent = property.GetValue(baseEntity, null) as BaseEntity;

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
            if (typeof(BaseEntity).IsAssignableFrom(modelType))
            {
                var subItem = string.Format("{0}.Id", bindingContext.ModelName);

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
                id =
                       Convert.ToString(controllerContext.RouteData.Values[baseId] ??
                                        controllerContext.HttpContext.Request[baseId]);


                if (int.TryParse(id, out intId))
                {
                    var obj = Session.Get(modelType, intId);
                    return obj ?? Activator.CreateInstance(modelType);
                }
            }
            return base.CreateModel(controllerContext, bindingContext, modelType);
        }
    }
}