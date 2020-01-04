using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Infrastructure.Models.Tabs;


namespace MrCMS.Web.Apps.Admin.Services
{
    public class GetEditTabsService : IGetEditTabsService
    {
        public List<AdminTabBase<T>> GetEditTabs<T>(IHtmlHelper html, T entity) where T : SystemEntity
        {
            return GetEditTabs(html.ViewContext.HttpContext.RequestServices, entity);
        }

        public List<AdminTabBase<T>> GetEditTabs<T>(IServiceProvider serviceProvider, T entity) where T : SystemEntity
        {
            var tabsToShow =
                TypeHelper.GetAllConcreteTypesAssignableFrom<AdminTabBase<T>>()
                    .Select(serviceProvider.GetService)
                    .OfType<AdminTabBase<T>>()
                    .Where(@base => @base.ShouldShow(serviceProvider, entity))
                    .ToList();

            var rootTabs = tabsToShow.Where(@base => @base.ParentType == null).OrderBy(@base => @base.Order).ToList();
            foreach (var tab in rootTabs)
            {
                AssignChildren(tab, tabsToShow);
            }

            return rootTabs;

        }

        public List<AdminTabBase<T>> GetEditTabs<T>(IHtmlHelper html, int id) where T : SystemEntity
        {
            return GetEditTabs<T>(html.ViewContext.HttpContext.RequestServices, id);
        }

        public List<AdminTabBase<T>> GetEditTabs<T>(IServiceProvider serviceProvider, int id) where T : SystemEntity
        {
            var entity = serviceProvider.GetRequiredService<ISession>().Get<T>(id);
            return GetEditTabs(serviceProvider, entity);
        }

        public List<object> GetEditTabs(IHtmlHelper html, Type type, int id)
        {
            var methodInfo = GetType().GetMethodExt(nameof(GetEditTabs), typeof(IHtmlHelper), typeof(int));

            var genericMethod = methodInfo.MakeGenericMethod(type);
            var editTabs = genericMethod.Invoke(this, new object[] { html, id });
            if (!(editTabs is IEnumerable enumerable))
                return new List<object>();
            return enumerable.Cast<object>().ToList();
        }

        public List<object> GetEditTabs(IServiceProvider serviceProvider, Type type, int id)
        {
            var methodInfo = GetType().GetMethodExt(nameof(GetEditTabs), typeof(IServiceProvider), typeof(int));

            var genericMethod = methodInfo.MakeGenericMethod(type);
            var editTabs = genericMethod.Invoke(this, new object[] { serviceProvider, id });
            if (!(editTabs is IEnumerable enumerable))
                return new List<object>();
            return enumerable.Cast<object>().ToList();
        }

        private void AssignChildren<T>(AdminTabBase<T> tab, List<AdminTabBase<T>> allTabs) where T : SystemEntity
        {
            if (!(tab is AdminTabGroup<T> tabGroup))
            {
                return;
            }
            var children =
                allTabs.Where(x => x.ParentType == tabGroup.GetType()).OrderBy(@base => @base.Order).ToList();
            tabGroup.SetChildren(children);
            foreach (var tabBase in children)
            {
                AssignChildren(tabBase, allTabs);
            }
        }
    }
}