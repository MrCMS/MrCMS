using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Core.Widgets;
using MrCMS.Website;
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
                    CreatedOn = CurrentRequestData.Now,
                    Layout = baseLayout,
                },
                new LayoutArea
                {
                    AreaName = "Header Left",
                    CreatedOn = CurrentRequestData.Now,
                    Layout = baseLayout,
                },
                new LayoutArea
                {
                    AreaName = "Header Right",
                    CreatedOn = CurrentRequestData.Now,
                    Layout = baseLayout,
                },
                new LayoutArea
                {
                    AreaName = "Before Content",
                    CreatedOn = CurrentRequestData.Now,
                    Layout = baseLayout,
                },
                new LayoutArea
                {
                    AreaName = "After Content",
                    CreatedOn = CurrentRequestData.Now,
                    Layout = baseLayout,
                },
                new LayoutArea
                {
                    AreaName = "Footer",
                    CreatedOn = CurrentRequestData.Now,
                    Layout = baseLayout,
                }
            };
            return layoutAreas;
        }

        private IEnumerable<Widget> AddWidgets(List<LayoutArea> layoutAreas)
        {
            yield return new Navigation
            {
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
                Text = string.Format("<p>© Mr CMS {0}</p>", CurrentRequestData.Now.Year),
                LayoutArea = layoutAreas.Single(x => x.AreaName == "Footer")
            };
            yield return new TextWidget
            {
                Name = "Mr CMS Logo",
                Text =
                    @"<a class=""navbar-brand"" href=""/""><img src=""/Apps/Core/Content/Images/mrcms-hat.gif"" style=""width: 40px; height: auto;"" />Mr CMS</a>",
                LayoutArea = layoutAreas.Single(x => x.AreaName == "Header Left")
            };
        }
    }
}