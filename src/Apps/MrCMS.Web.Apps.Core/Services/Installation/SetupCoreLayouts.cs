using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Services.Installation
{
    public class SetupCoreLayouts : ISetupCoreLayouts
    {
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IRepository<LayoutArea> _layoutAreaRepository;
        private readonly IRepository<Widget> _widgetRepository;
        private readonly IConfigurationProvider _configurationProvider;

        public SetupCoreLayouts(IRepository<Layout> layoutRepository,
            IRepository<LayoutArea> layoutAreaRepository,
            IRepository<Widget> widgetRepository,
            IConfigurationProvider configurationProvider)
        {
            _layoutRepository = layoutRepository;
            _layoutAreaRepository = layoutAreaRepository;
            _configurationProvider = configurationProvider;
            _widgetRepository = widgetRepository;
        }

        public async Task Setup()
        {
            var baseLayout = new Layout
            {
                Name = "Base Layout",
                UrlSegment = "_BaseLayout",
                LayoutAreas = new List<LayoutArea>()
            };

            await _layoutRepository.Add(baseLayout);

            var siteSettings = _configurationProvider.GetSiteSettings<SiteSettings>();
            siteSettings.DefaultLayoutId = baseLayout.Id;
            _configurationProvider.SaveSettings(siteSettings);

            List<LayoutArea> layoutAreas = GetDefaultAreas(baseLayout);
            await _layoutAreaRepository.AddRange(layoutAreas);


            var layoutTwoColumn = new Layout
            {
                Parent = baseLayout,
                Name = "Two Column"
            };

            await _layoutRepository.Add(layoutTwoColumn);

            var layoutAreasTwoColumn = new List<LayoutArea>
            {
                new LayoutArea
                {
                    AreaName = "Right Column",
                    Layout = layoutTwoColumn
                }
            };
            await _layoutAreaRepository.AddRange(layoutAreasTwoColumn);

            await _widgetRepository.AddRange(GetWidgets(layoutAreas).ToList());
        }

        private List<LayoutArea> GetDefaultAreas(Layout baseLayout)
        {
            var layoutAreas = new List<LayoutArea>
            {
                new LayoutArea
                {
                    AreaName = "Main Navigation",
                    CreatedOn = DateTime.UtcNow,
                    Layout = baseLayout,
                },
                new LayoutArea
                {
                    AreaName = "Header Left",
                    CreatedOn = DateTime.UtcNow,
                    Layout = baseLayout,
                },
                new LayoutArea
                {
                    AreaName = "Header Right",
                    CreatedOn = DateTime.UtcNow,
                    Layout = baseLayout,
                },
                new LayoutArea
                {
                    AreaName = "Before Content",
                    CreatedOn = DateTime.UtcNow,
                    Layout = baseLayout,
                },
                new LayoutArea
                {
                    AreaName = "After Content",
                    CreatedOn = DateTime.UtcNow,
                    Layout = baseLayout,
                },
                new LayoutArea
                {
                    AreaName = "Footer",
                    CreatedOn = DateTime.UtcNow,
                    Layout = baseLayout,
                }
            };
            return layoutAreas;
        }

        private IEnumerable<Widget> GetWidgets(List<LayoutArea> layoutAreas)
        {
            yield return new Navigation
            {
                Name = "Navigation",
                IncludeChildren = true,
                LayoutArea = layoutAreas.Single(x => x.AreaName == "Main Navigation")
            };

            yield return new UserLinks
            {
                Name = "User Links",
                LayoutArea = layoutAreas.Single(x => x.AreaName == "Header Right")
            };

            yield return new TextWidget
            {
                Name = "Footer text",
                Text = string.Format("<p>&copy; Mr CMS {0}</p>", DateTime.UtcNow.Year),
                LayoutArea = layoutAreas.Single(x => x.AreaName == "Footer")
            };
        }
    }
}