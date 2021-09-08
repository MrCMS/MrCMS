using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities;
using MrCMS.Web.Admin.Infrastructure.Models.Tabs;

namespace MrCMS.Web.Admin.Services
{
    public interface IGetEditTabsService
    {
        Task<List<AdminTabBase<T>>> GetEditTabs<T>(IHtmlHelper html, T entity) where T : SystemEntity;
        Task<List<AdminTabBase<T>>> GetEditTabs<T>(IServiceProvider serviceProvider, T entity) where T : SystemEntity;
        Task<List<AdminTabBase<T>>> GetEditTabs<T>(IHtmlHelper html, int id) where T : SystemEntity;
        Task<List<AdminTabBase<T>>> GetEditTabs<T>(IServiceProvider serviceProvider, int id) where T : SystemEntity;
        Task<List<object>> GetEditTabs(IHtmlHelper html, Type type, int id);
        Task<List<object>> GetEditTabs(IServiceProvider serviceProvider, Type type, int id);
    }
}