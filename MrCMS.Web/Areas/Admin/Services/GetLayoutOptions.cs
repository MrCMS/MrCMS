using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
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

        public List<SelectListItem> Get()
        {
            var layouts =
                _session.QueryOver<Layout>().OrderBy(layout => layout.DisplayOrder).Asc.Cacheable().List().ToList();
            var systemDefaultLayout = _session.Get<Layout>(_siteSettings.DefaultLayoutId);
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