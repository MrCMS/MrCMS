using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities;

namespace MrCMS.Web.Admin.Infrastructure.Models.Tabs
{
    public abstract class AdminTabBase<T> : IAdminTab where T : SystemEntity
    {
        public abstract int Order { get; }

        public abstract Type ParentType { get; }
        public abstract Type ModelType { get; }
        public abstract IEnumerable<IAdminTab> Children { get; protected set; } 
        public abstract Task<string> Name(IServiceProvider serviceProvider, T entity);

        public abstract Task<bool> ShouldShow(IServiceProvider serviceProvider, T entity);
    }
}