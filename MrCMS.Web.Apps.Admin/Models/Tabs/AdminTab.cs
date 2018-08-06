using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Admin.Models.Tabs
{
    public abstract class AdminTab<T> : AdminTabBase<T> where T : SystemEntity
    {
        public abstract string TabHtmlId { get; }
        public abstract Task RenderTabPane(IHtmlHelper helper, IMapper mapper, T entity);
        public sealed override IEnumerable<IAdminTab> Children { get; protected set; } = new List<IAdminTab>();
    }
}