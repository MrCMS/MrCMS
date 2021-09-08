using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Web.Admin.Services
{
    public class GetTemplateOptions : BaseAssignWebpageAdminViewData<Webpage>
    {
        private readonly ISession _session;

        public GetTemplateOptions(ISession session)
        {
            _session = session;
        }

        public override async Task AssignViewData(Webpage webpage, ViewDataDictionary viewData)
        {
            if (webpage == null)
                return;
            var defaultSelection = new SelectListItem {Text = "Default template", Value = ""};
            var typeName = webpage.GetType().FullName;
            var pageTemplates = await _session.QueryOver<PageTemplate>()
                .Where(template => template.PageType == typeName)
                .OrderBy(template => template.Name).Asc.Cacheable().ListAsync();

            if (!pageTemplates.Any())
            {
                var selectListItems = new List<SelectListItem> {defaultSelection};

                viewData["template-options"] = selectListItems;
                return;
            }
            
            var templates = new List<PageTemplate>();

            foreach (var template in pageTemplates)
            {
                if (!template.SingleUse)
                    templates.Add(template);
                else if (!await _session.QueryOver<Webpage>()
                    .Where(page => page.PageTemplate.Id == template.Id && page.Id != webpage.Id).AnyAsync())
                {
                    templates.Add(template);
                }
            }

            var options = templates.BuildSelectItemList(template => template.Name,
                template => template.Id.ToString(),
                emptyItem: defaultSelection);

            viewData["template-options"] = options;
        }
    }
}