using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class PageTemplateAdminService : IPageTemplateAdminService
    {
        private readonly ISession _session;

        public PageTemplateAdminService(ISession session)
        {
            _session = session;
        }

        public IPagedList<PageTemplate> Search(PageTemplateSearchQuery query)
        {
            IQueryOver<PageTemplate, PageTemplate> queryOver = _session.QueryOver<PageTemplate>();

            return queryOver.Paged(query.Page);
        }

        public void Add(PageTemplate template)
        {
            _session.Transact(session => session.Save(template));
        }

        public void Update(PageTemplate template)
        {
            _session.Transact(session => session.Update(template));
        }

        public List<SelectListItem> GetPageTypeOptions()
        {
            List<SelectListItem> selectListItems = GetNewList();
            selectListItems.AddRange(from key in MrCMSApp.AppWebpages.Keys.OrderBy(type => type.FullName)
                let appName = MrCMSApp.AppWebpages[key]
                select
                    new SelectListItem
                    {
                        Text = string.Format("{0} ({1})", key.GetMetadata().Name, appName),
                        Value = key.FullName
                    });
            return selectListItems;
        }

        public List<SelectListItem> GetLayoutOptions()
        {
            IEnumerable<Layout> layouts =
                _session.QueryOver<Layout>().Where(x => x.Hidden == false).Cacheable().List();

            return layouts.BuildSelectItemList(layout => layout.Name,
                layout => layout.Id.ToString(CultureInfo.InvariantCulture),
                emptyItemText: "Please select...");
        }

        public List<SelectListItem> GetUrlGeneratorOptions(Type type)
        {
            List<SelectListItem> selectListItems = GetNewList();
            if (type != null)
            {
                Type defaultGenerator = typeof (DefaultWebpageUrlGenerator);
                selectListItems.Add(new SelectListItem {Text = "Default", Value = defaultGenerator.FullName});
                if (WebpageUrlService.WebpageGenerators.ContainsKey(type.FullName))
                {
                    selectListItems.AddRange(
                        WebpageUrlService.WebpageGenerators[type.FullName].Select(
                            generatorType =>
                                new SelectListItem
                                {
                                    Text = generatorType.Name.BreakUpString(),
                                    Value = generatorType.FullName
                                }));
                }
            }
            return selectListItems;
        }

        private static List<SelectListItem> GetNewList()
        {
            var selectListItems = new List<SelectListItem>
            {
                new SelectListItem {Text = "Please select...", Value = ""}
            };
            return selectListItems;
        }
    }
}