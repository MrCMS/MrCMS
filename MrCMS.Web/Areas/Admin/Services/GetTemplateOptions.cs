using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class GetTemplateOptions : BaseAssignWebpageAdminViewData<Webpage>
    {
        private readonly ISession _session;

        public GetTemplateOptions(ISession session)
        {
            _session = session;
        }

        public override void AssignViewData(Webpage webpage, ViewDataDictionary viewData)
        {
            if (webpage == null)
                return;
            var typeName = webpage.GetType().FullName;
            var templates = _session.QueryOver<PageTemplate>().Where(template => template.PageType == typeName)
                .OrderBy(template => template.Name).Asc.Cacheable().List();

            viewData["template-options"] = templates.BuildSelectItemList(template => template.Name,
                template => template.Id.ToString(),
                emptyItem: new SelectListItem {Text = "Default template", Value = "0"});
        }
    }
}