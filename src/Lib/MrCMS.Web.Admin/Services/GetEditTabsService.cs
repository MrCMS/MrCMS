using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Infrastructure.Models.Tabs;
using NHibernate;

namespace MrCMS.Web.Admin.Services
{
    public class GetEditTabsService : IGetEditTabsService
    {
        public async Task<List<AdminTabBase<T>>> GetEditTabs<T>(IHtmlHelper html, T entity) where T : SystemEntity
        {
            return await GetEditTabs(html.ViewContext.HttpContext.RequestServices, entity);
        }

        public async Task<List<AdminTabBase<T>>> GetEditTabs<T>(IServiceProvider serviceProvider, T entity)
            where T : SystemEntity
        {
            var adminTabBases = TypeHelper.GetAllConcreteTypesAssignableFrom<AdminTabBase<T>>()
                .Select(serviceProvider.GetService)
                .OfType<AdminTabBase<T>>().ToList();
            var tabsToShow = new List<AdminTabBase<T>>();
            foreach (var @base in adminTabBases)
                if (await @base.ShouldShow(serviceProvider, entity))
                    tabsToShow.Add(@base);

            var rootTabs = tabsToShow.Where(@base => @base.ParentType == null).OrderBy(@base => @base.Order).ToList();
            foreach (var tab in rootTabs)
            {
                AssignChildren(tab, tabsToShow);
            }

            return rootTabs;
        }

        public async Task<List<AdminTabBase<T>>> GetEditTabs<T>(IHtmlHelper html, int id) where T : SystemEntity
        {
            return await GetEditTabs<T>(html.ViewContext.HttpContext.RequestServices, id);
        }

        public async Task<List<AdminTabBase<T>>> GetEditTabs<T>(IServiceProvider serviceProvider, int id) where T : SystemEntity
        {
            var entity = serviceProvider.GetRequiredService<ISession>().Get<T>(id);
            return await GetEditTabs(serviceProvider, entity);
        }

        public async Task<List<object>> GetEditTabs(IHtmlHelper html, Type type, int id)
        {
            var methodInfo = GetType().GetMethodExt(nameof(GetEditTabs), typeof(IHtmlHelper), typeof(int));

            var genericMethod = methodInfo.MakeGenericMethod(type);
            var editTabs = await genericMethod.InvokeAsync(this, new object[] {html, id});
            if (!(editTabs is IEnumerable enumerable))
                return new List<object>();
            return enumerable.Cast<object>().ToList();
        }

        public async Task<List<object>> GetEditTabs(IServiceProvider serviceProvider, Type type, int id)
        {
            var methodInfo = GetType().GetMethodExt(nameof(GetEditTabs), typeof(IServiceProvider), typeof(int));

            var genericMethod = methodInfo.MakeGenericMethod(type);
            var editTabs = await genericMethod.InvokeAsync(this, new object[] {serviceProvider, id});
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