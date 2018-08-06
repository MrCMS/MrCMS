using System;
using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Models.Tabs
{
    public abstract class AdminTabGroup<T> : AdminTabBase<T>where T : SystemEntity
    {
        protected AdminTabGroup()
        {
            Children = new List<AdminTabBase<T>>();
        }

        public sealed override IEnumerable<IAdminTab> Children { get; protected set; }

        public sealed override Type ModelType => null;

        public void SetChildren(List<AdminTabBase<T>> children)
        {
            Children = children;
        }

    }
}