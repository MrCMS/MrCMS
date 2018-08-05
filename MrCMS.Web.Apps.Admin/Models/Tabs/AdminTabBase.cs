using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.WebpageEdit;

namespace MrCMS.Web.Apps.Admin.Models.Tabs
{
    public abstract class AdminTabBase<T> where T : SystemEntity
    {
        public abstract int Order { get; }
        public abstract string Name(IHtmlHelper helper, T entity);

        public abstract bool ShouldShow(IHtmlHelper helper, T entity);

        public abstract Type ParentType { get; }
    }
}