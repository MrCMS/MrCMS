using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Apps;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Models;

using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class PageTemplateAdminService : IPageTemplateAdminService
    {
        private readonly IRepository<PageTemplate> _pageTemplateRepository;
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IGetUrlGeneratorOptions _getUrlGeneratorOptions;
        private readonly IMapper _mapper;
        private readonly MrCMSAppContext _appContext;

        public PageTemplateAdminService(IRepository<PageTemplate> pageTemplateRepository, IRepository<Layout> layoutRepository, IGetUrlGeneratorOptions getUrlGeneratorOptions, IMapper mapper, MrCMSAppContext appContext)
        {
            _pageTemplateRepository = pageTemplateRepository;
            _layoutRepository = layoutRepository;
            _getUrlGeneratorOptions = getUrlGeneratorOptions;
            _mapper = mapper;
            _appContext = appContext;
        }

        public IPagedList<PageTemplate> Search(PageTemplateSearchQuery query)
        {
            var queryOver = _pageTemplateRepository.Query();

            return queryOver.ToPagedList(query.Page);
        }

        public async Task Add(AddPageTemplateModel model)
        {
            var template = _mapper.Map<PageTemplate>(model);
            await _pageTemplateRepository.Add(template);
        }

        public async Task<UpdatePageTemplateModel> GetEditModel(int id)
        {
            var template = await GetTemplate(id);
            return _mapper.Map<UpdatePageTemplateModel>(template);
        }

        public async Task Update(UpdatePageTemplateModel model)
        {
            var template = await GetTemplate(model.Id);
            _mapper.Map(model, template);
            await _pageTemplateRepository.Update(template);
        }

        private async Task<PageTemplate> GetTemplate(int id)
        {
            return await _pageTemplateRepository.Load(id);
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
                _layoutRepository.Query().Where(x => x.Hidden == false).ToList();

            return layouts.BuildSelectItemList(layout => layout.Name,
                layout => layout.Id.ToString(CultureInfo.InvariantCulture),
                emptyItemText: "Please select...");
        }

        public async Task<List<SelectListItem>> GetUrlGeneratorOptions(int templateId)
        {
            var template = await GetTemplate(templateId);
            return GetUrlGeneratorOptions(template?.PageType);
        }
        public List<SelectListItem> GetUrlGeneratorOptions(string pageType)
        {
            var type = TypeHelper.GetTypeByName(pageType);
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