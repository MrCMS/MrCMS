using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Core.Widgets;
using NHibernate;

namespace MrCMS.Web.Apps.Core.Services.Installation
{
    public class SetupCoreLayouts : ISetupCoreLayouts
    {
        private readonly ISession _session;
        private readonly IConfigurationProvider _configurationProvider;

        public SetupCoreLayouts(ISession session, IConfigurationProvider configurationProvider)
        {
            _session = session;
            _configurationProvider = configurationProvider;
        }

        public void Setup()
        {
            Layout baseLayout = null;
            _session.Transact(session =>
            {
                baseLayout = new Layout
                {
                    Name = "Base Layout",
                    UrlSegment = "_BaseLayout",
                    LayoutAreas = new List<LayoutArea>()
                };

                session.Save(baseLayout);
            });

            var siteSettings = _configurationProvider.GetSiteSettings<SiteSettings>();
            siteSettings.DefaultLayoutId = baseLayout.Id;
            _configurationProvider.SaveSettings(siteSettings);

            _session.Transact(session =>
            {

                List<LayoutArea> layoutAreas = GetDefaultAreas(baseLayout);
                layoutAreas.ForEach(area => session.Save(area));

                var layoutTwoColumn = new Layout
                {
                    Parent = baseLayout,
                    Name = "Two Column"
                };

                session.Save(layoutTwoColumn);

                var layoutAreasTwoColumn = new List<LayoutArea>
                {
                    new LayoutArea
                    {
                        AreaName = "Right Column",
                        Layout = layoutTwoColumn
                    }
                };
                foreach (LayoutArea layoutArea in layoutAreasTwoColumn)
                    session.Save(layoutArea);

                AddWidgets(layoutAreas).ForEach(widget => session.Save(widget));
            });
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

        private IEnumerable<Widget> AddWidgets(List<LayoutArea> layoutAreas)
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