using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Infrastructure.Models.Tabs;
using MrCMS.Web.Apps.Admin.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IGetEditTabsService
    {
        List<AdminTabBase<T>> GetEditTabs<T>(IHtmlHelper html, T entity) where T : SystemEntity;
        List<AdminTabBase<T>> GetEditTabs<T>(IServiceProvider serviceProvider, T entity) where T : SystemEntity;
        List<AdminTabBase<T>> GetEditTabs<T>(IHtmlHelper html, int id) where T : SystemEntity;
        List<AdminTabBase<T>> GetEditTabs<T>(IServiceProvider serviceProvider, int id) where T : SystemEntity;
        List<object> GetEditTabs(IHtmlHelper html, Type type, int id);
        List<object> GetEditTabs(IServiceProvider serviceProvider, Type type, int id);
    }
}