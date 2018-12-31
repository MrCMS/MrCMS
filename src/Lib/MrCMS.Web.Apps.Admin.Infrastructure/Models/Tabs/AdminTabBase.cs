using System;
using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Models.Tabs
{
    public abstract class AdminTabBase<T> : IAdminTab where T : SystemEntity
    {
        public abstract int Order { get; }

        public abstract Type ParentType { get; }
        public abstract Type ModelType { get; }
        public abstract IEnumerable<IAdminTab> Children { get; protected set; } 
        public abstract string Name(IServiceProvider serviceProvider, T entity);

        public abstract bool ShouldShow(IServiceProvider serviceProvider, T entity);
    }
}