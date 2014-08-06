using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class InPageAdminService : IInPageAdminService
    {
        private readonly ISession _session;

        public InPageAdminService(ISession session)
        {
            _session = session;
        }

        public SaveResult SaveBodyContent(UpdatePropertyData updatePropertyData)
        {
            HashSet<Type> types = TypeHelper.GetAllConcreteTypesAssignableFrom<SystemEntity>();
            Type entityType = types.FirstOrDefault(t => t.Name == updatePropertyData.Type);
            if (entityType == null)
                return new SaveResult(false, string.Format("Could not find entity type '{0}'", updatePropertyData.Type));
            object entity = _session.Get(entityType, updatePropertyData.Id);
            if (entity == null)
                return new SaveResult(false,
                    string.Format("Could not find entity of type '{0}' with id {1}", updatePropertyData.Type,
                        updatePropertyData.Id));
            PropertyInfo propertyInfo =
                entityType.GetProperties().FirstOrDefault(info => info.Name == updatePropertyData.Property);
            if (propertyInfo == null)
                return new SaveResult(false,
                    string.Format("Could not find entity of type '{0}' with id {1}", updatePropertyData.Type,
                        updatePropertyData.Id));
            if (string.IsNullOrWhiteSpace(updatePropertyData.Content) &&
                propertyInfo.GetCustomAttributes(typeof (RequiredAttribute), false).Any())
            {
                return new SaveResult(false,
                    string.Format("Could not edit '{0}' as it is required", updatePropertyData.Property));
            }

            propertyInfo.SetValue(entity, updatePropertyData.Content, null);
            _session.Transact(session => session.SaveOrUpdate(entity));

            return new SaveResult();
        }

        public string GetUnformattedBodyContent(GetPropertyData getPropertyData)
        {
            return GetContent(getPropertyData).Content;
        }

        public string GetFormattedBodyContent(GetPropertyData getPropertyData, Controller controller)
        {
            ContentInfo content = GetContent(getPropertyData);

            object entity = content.Entity;
            if (entity is Webpage)
                CurrentRequestData.CurrentPage = entity as Webpage;
            HtmlHelper htmlHelper = MrCMSHtmlHelper.GetHtmlHelper(controller);
            return htmlHelper.ParseShortcodes(content.Content).ToHtmlString();
        }

        private ContentInfo GetContent(GetPropertyData getPropertyData)
        {
            HashSet<Type> types = TypeHelper.GetAllConcreteTypesAssignableFrom<SystemEntity>();
            Type entityType = types.FirstOrDefault(t => t.Name == getPropertyData.Type);
            if (entityType == null)
                return new ContentInfo();
            object entity = _session.Get(entityType, getPropertyData.Id);
            if (entity == null)
                return new ContentInfo();
            PropertyInfo propertyInfo =
                entityType.GetProperties().FirstOrDefault(info => info.Name == getPropertyData.Property);
            if (propertyInfo == null)
                return new ContentInfo();
            return new ContentInfo
            {
                Content = Convert.ToString(propertyInfo.GetValue(entity, null)),
                Entity = entity
            };
        }

        private struct ContentInfo
        {
            public string Content { get; set; }
            public object Entity { get; set; }
        }
    }
}