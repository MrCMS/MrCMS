using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Admin.Models.Tabs
{
    public abstract class AdminTabGroup<T> : AdminTabBase<T> where T : SystemEntity
    {
        protected AdminTabGroup()
        {
            Children = new List<AdminTabBase<T>>();
        }

        public List<AdminTabBase<T>> Children { get; private set; }

        public void SetChildren(List<AdminTabBase<T>> children)
        {
            Children = children;
        }

    }
}