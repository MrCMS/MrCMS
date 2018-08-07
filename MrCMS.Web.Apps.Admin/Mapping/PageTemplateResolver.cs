using System;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.WebpageEdit;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class PageTemplateResolver : IdToEntityResolver<LayoutTabViewModel,Webpage,PageTemplate>
    {
        public PageTemplateResolver(ISession session) : base(session)
        {
        }

        protected override int? GetId(LayoutTabViewModel source) => source.PageTemplateId;
    }
}