using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Settings;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Web.Admin.Services
{
    public class GetLayoutOptions : IGetLayoutOptions
    {
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;

        public GetLayoutOptions(ISession session, SiteSettings siteSettings)
        {
            _session = session;
            _siteSettings = siteSettings;
        }

        public async Task<List<SelectListItem>> Get()
        {
            var layouts = await
                _session.Query<Layout>()
                    .Fetch(x => x.Parent)
                    .OrderBy(layout => layout.DisplayOrder)
                    // .WithOptions(options => options.SetCacheable(true))
                    .ToListAsync();
            var systemDefaultLayout = await _session.GetAsync<Layout>(_siteSettings.DefaultLayoutId);
            var selectListItems = new List<SelectListItem>
            {
                new() {Text = $"System Default ({systemDefaultLayout.Name})", Value = ""}
            };

            AddItems(layouts.FindAll(layout => layout.Parent == null), layouts, ref selectListItems);

            return selectListItems;
        }

        private void AddItems(List<Layout> layouts, List<Layout> allLayouts, ref List<SelectListItem> selectListItems,
            int depth = 0)
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