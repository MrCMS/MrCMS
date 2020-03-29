using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Settings;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class GetLayoutOptions : IGetLayoutOptions
    {
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IConfigurationProvider _configurationProvider;

        public GetLayoutOptions(IRepository<Layout> layoutRepository, IConfigurationProvider configurationProvider)
        {
            _layoutRepository = layoutRepository;
            _configurationProvider = configurationProvider;
        }

        public async Task<List<SelectListItem>> Get()
        {
            var layouts = await _layoutRepository.Readonly().OrderBy(layout => layout.DisplayOrder).ToListAsync();
            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            var systemDefaultLayout = await _layoutRepository.GetData(siteSettings.DefaultLayoutId);
            var selectListItems = new List<SelectListItem>
            {
                new SelectListItem {Text = string.Format("System Default ({0})", systemDefaultLayout.Name), Value = ""}
            };

            AddItems(layouts.FindAll(layout => layout.Parent == null), layouts, ref selectListItems);

            return selectListItems;
        }

        private void AddItems(List<Layout> layouts, List<Layout> allLayouts, ref List<SelectListItem> selectListItems, int depth = 0)
        {
            foreach (var layout in layouts)
            {
                selectListItems.Add(new SelectListItem
                {
                    Value = layout.Id.ToString(),
                    Text = new string('-', depth * 2) + layout.Name
                });
                AddItems(allLayouts.FindAll(l => l.Parent == layout), allLayouts, ref selectListItems, depth + 1);
            }
        }
    }
}