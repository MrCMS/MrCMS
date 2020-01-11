using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class WebpageAdminService : IWebpageAdminService
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IMapper _mapper;
        private readonly IGetDocumentsByParent<Webpage> _getDocumentsByParent;
        private readonly SiteSettings _siteSettings;

        public WebpageAdminService(IRepository<Webpage> webpageRepository,
            IMapper mapper,
            IGetDocumentsByParent<Webpage> getDocumentsByParent,
            SiteSettings siteSettings)
        {
            _webpageRepository = webpageRepository;
            _mapper = mapper;
            _getDocumentsByParent = getDocumentsByParent;
            _siteSettings = siteSettings;
        }


        public Webpage GetWebpage(int? id)
        {
            return id.HasValue ? _webpageRepository.LoadSync(id.Value) : null;
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


        public Webpage Add(AddWebpageModel model, object additionalPropertyModel)
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

            _webpageRepository.Add(instance);

            return instance;
        }

        public async Task<Webpage> Update(UpdateWebpageViewModel viewModel)
        {
            var webpage = GetWebpage(viewModel.Id);

            _mapper.Map(viewModel, webpage);

            foreach (var model in viewModel.Models)
                _mapper.Map(model, webpage);

            await _webpageRepository.Update(webpage);

            return webpage;
        }

        public async Task<Webpage> Delete(int id)
        {
            var webpage = GetWebpage(id);
            await _webpageRepository.Delete(webpage);
            return webpage;
        }

        public List<SortItem> GetSortItems(int? id)
        {
            var parent = GetWebpage(id);
            return _getDocumentsByParent.GetDocuments(parent)
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
            var webpage = GetWebpage(id);
            if (webpage == null)
                return;
            webpage.Published = false;
            webpage.PublishOn = null;
            await _webpageRepository.Update(webpage);
        }

        public string GetServerDate()
        {
            var now = DateTime.UtcNow;
            return now.Add(_siteSettings.TimeZoneInfo.GetUtcOffset(now)).ToString();
        }
    }
}