using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities;
using MrCMS.Mapping;

namespace MrCMS.Web.Admin.Infrastructure.Models.Tabs
{
    public abstract class AdminTab<T> : AdminTabBase<T> where T : SystemEntity
    {
        public abstract string TabHtmlId { get; }
        public abstract Task RenderTabPane(IHtmlHelper helper, ISessionAwareMapper mapper, T entity);
        public sealed override IEnumerable<IAdminTab> Children { get; protected set; } = new List<IAdminTab>();
    }
}