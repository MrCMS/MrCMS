using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MrCMS.Common;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;
using IConfigurationProvider = MrCMS.Settings.IConfigurationProvider;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class WebpageAdminService : IWebpageAdminService
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IMapper _mapper;
        private readonly IGetDocumentsByParent<Webpage> _getDocumentsByParent;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IOptions<SystemConfigurationSettings> _systemConfigurationSettings;
        private readonly IGetDateTimeNow _getDateTimeNow;

        public WebpageAdminService(IRepository<Webpage> webpageRepository,
            IMapper mapper,
            IGetDocumentsByParent<Webpage> getDocumentsByParent,
            IConfigurationProvider configurationProvider, IOptions<SystemConfigurationSettings> systemConfigurationSettings, IGetDateTimeNow getDateTimeNow)
        {
            _webpageRepository = webpageRepository;
            _mapper = mapper;
            _getDocumentsByParent = getDocumentsByParent;
            _configurationProvider = configurationProvider;
            _systemConfigurationSettings = systemConfigurationSettings;
            _getDateTimeNow = getDateTimeNow;
        }


        public async Task<Webpage> GetWebpage(int? id)
        {
            return id.HasValue ? await _webpageRepository.Query()
                .Include(x=>x.DocumentTags)
                .ThenInclude(x=>x.Tag)
                .Where(x=>x.Id == id.Value)
                .SingleOrDefaultAsync() : null;
        }

        public AddWebpageModel GetAddModel(int? id)
        {
            return new AddWebpageModel
            {
                ParentId = id
            };
        }

        public object GetAdditionalPropertyModel(string type)
        {
            var documentType = TypeHelper.GetTypeByName(type);
            if (documentType == null)
                return null;
            var additionalPropertyType =
                TypeHelper.GetAllConcreteTypesAssignableFrom(
                    typeof(IAddPropertiesViewModel<>).MakeGenericType(documentType)).FirstOrDefault();
            if (additionalPropertyType == null)
                return null;

            return Activator.CreateInstance(additionalPropertyType);
        }


        public async Task<Webpage> Add(AddWebpageModel model, object additionalPropertyModel)
        {
            var type = TypeHelper.GetTypeByName(model.DocumentType);
            var instance = Activator.CreateInstance(type) as Webpage;
            var revealInNavigation = instance.GetMetadata().RevealInNavigation;
            _mapper.Map(model, instance);

            if (revealInNavigation)
                if (instance != null)
                    instance.RevealInNavigation = true;

            if (additionalPropertyModel != null)
                _mapper.Map(additionalPropertyModel, instance);

            await _webpageRepository.Add(instance);

            return instance;
        }

        public async Task<IResult<Webpage>> Update(UpdateWebpageViewModel viewModel)
        {
            var webpage = await GetWebpage(viewModel.Id);

            _mapper.Map(viewModel, webpage);

            foreach (var model in viewModel.Models)
                _mapper.Map(model, webpage, options => { });

            //if (webpage.DocumentTags.Any())
            return await _webpageRepository.Update(webpage);

        }

        public async Task<Webpage> Delete(int id)
        {
            var webpage = await GetWebpage(id);
            await _webpageRepository.Delete(webpage);
            return webpage;
        }

        public async Task<List<SortItem>> GetSortItems(int? id)
        {
            var parent = await GetWebpage(id);
            return (await _getDocumentsByParent.GetDocuments(parent))
                .Select(
                    arg => new SortItem { Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name })
                .OrderBy(x => x.Order)
                .ToList();
        }

        public async Task SetOrders(List<SortItem> items)
        {
            var pages = items.Select(item =>
            {
                var webpage = _webpageRepository.LoadSync(item.Id);
                webpage.DisplayOrder = item.Order;
                return webpage;
            }).ToList();

            await _webpageRepository.UpdateRange(pages);
        }

        public async Task PublishNow(int id)
        {
            var webpage = await _webpageRepository.Load(id);
            if (webpage == null)
                return;

            if (webpage.PublishOn == null)
            {
                webpage.Published = true;
                webpage.PublishOn = DateTime.UtcNow;
                await _webpageRepository.Update(webpage);
            }
        }

        public async Task Unpublish(int id)
        {
            var webpage = await GetWebpage(id);
            if (webpage == null)
                return;
            webpage.Published = false;
            webpage.PublishOn = null;
            await _webpageRepository.Update(webpage);
        }

        public async Task<string> GetServerDate()
        {
            var now = DateTime.UtcNow;
            return await Task.FromResult(now.Add(_systemConfigurationSettings.Value.TimeZoneInfo.GetUtcOffset(now)).ToString(CultureInfo.CurrentUICulture));
        }
    }
}