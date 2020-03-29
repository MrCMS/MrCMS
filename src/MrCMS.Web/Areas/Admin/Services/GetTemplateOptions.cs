using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class GetTemplateOptions : BaseAssignWebpageAdminViewData<Webpage>
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IRepository<PageTemplate> _pageTemplateRepository;

        public GetTemplateOptions(IRepository<Webpage> webpageRepository, IRepository<PageTemplate> pageTemplateRepository)
        {
            _webpageRepository = webpageRepository;
            _pageTemplateRepository = pageTemplateRepository;
        }

        public override async Task AssignViewData(Webpage webpage, ViewDataDictionary viewData)
        {
            if (webpage == null)
                return;
            var typeName = webpage.GetType().FullName;
            var templates = await _pageTemplateRepository.Readonly().Where(template => template.PageType == typeName)
                .OrderBy(template => template.Name).ToListAsync();

            templates = templates.FindAll(template =>
            {
                if (!template.SingleUse)
                    return true;
                return !_webpageRepository.Readonly().Any(page => page.PageTemplate.Id == template.Id && page.Id != webpage.Id);
            });

            viewData["template-options"] = templates.BuildSelectItemList(template => template.Name,
                template => template.Id.ToString(),
                emptyItem: new SelectListItem {Text = "Default template", Value = ""});
        }
    }
}