using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public abstract class ChildCollectionBreadcrumb<TParent> : Breadcrumb<TParent> where TParent : Breadcrumb
    {
        public override string Controller { get; }
        public override string Action { get; }

        public override IDictionary<string, object> ParentActionArguments => ActionArguments;

        public override string Url(IUrlHelper url) => null;
    }
}