using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Models.Tabs
{
    public interface IAdminTab
    {
        int Order { get; }
        Type ParentType { get; }
        Type ModelType { get; }
        IEnumerable<IAdminTab> Children { get; }
    }
}