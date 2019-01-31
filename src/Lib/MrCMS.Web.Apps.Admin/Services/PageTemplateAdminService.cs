using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Models;
using NHibernate;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class PageTemplateAdminService : IPageTemplateAdminService
    {
        private readonly IGetUrlGeneratorOptions _getUrlGeneratorOptions;
        private readonly IMapper _mapper;
        private readonly MrCMSAppContext _appContext;
        private readonly ISession _session;

        public PageTemplateAdminService(ISession session, IGetUrlGeneratorOptions getUrlGeneratorOptions, IMapper mapper, MrCMSAppContext appContext)
        {
            _session = session;
            _getUrlGeneratorOptions = getUrlGeneratorOptions;
            _mapper = mapper;
            _appContext = appContext;
        }

        public IPagedList<PageTemplate> Search(PageTemplateSearchQuery query)
        {
            IQueryOver<PageTemplate, PageTemplate> queryOver = _session.QueryOver<PageTemplate>();

            return queryOver.Paged(query.Page);
        }

        public void Add(AddPageTemplateModel model)
        {
            var template = _mapper.Map<PageTemplate>(model);
            _session.Transact(session => session.Save(template));
        }

        public UpdatePageTemplateModel GetEditModel(int id)
        {
            var template = GetTemplate(id);
            return _mapper.Map<UpdatePageTemplateModel>(template);
        }

        public void Update(UpdatePageTemplateModel model)
        {
            var template = GetTemplate(model.Id);
            _mapper.Map(model, template);
            _session.Transact(session => session.Update(template));
        }

        private PageTemplate GetTemplate(int id)
        {
            return _session.Get<PageTemplate>(id);
        }

        public List<SelectListItem> GetPageTypeOptions()
        {
            List<SelectListItem> selectListItems = GetNewList();
            selectListItems.AddRange(
                TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                    .Select(type => new { typeName = type.FullName, displayName = type.Name.BreakUpString(), app = _appContext.Types.ContainsKey(type) ? _appContext.Types[type].Name : "System" })
                    .OrderBy(x => x.app)
                    .ThenBy(x => x.displayName)
                    .Select(info => new SelectListItem
                    {
                        Text = string.Format("{0} ({1})", info.displayName, info.app),
                        Value = info.typeName
                    }));
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

        public List<SelectListItem> GetUrlGeneratorOptions(string typeName)
        {
            var type = TypeHelper.GetTypeByName(typeName);
            List<SelectListItem> urlGeneratorOptions = _getUrlGeneratorOptions.Get(type);
            urlGeneratorOptions.Insert(0, new SelectListItem { Text = "Please select...", Value = "" });
            return urlGeneratorOptions;
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