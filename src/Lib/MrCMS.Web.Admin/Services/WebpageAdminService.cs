using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Infrastructure.ModelBinding;

namespace MrCMS.Web.Admin.Services
{
    public class WebpageAdminService : IWebpageAdminService
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IMapper _mapper;
        private readonly IGetDocumentsByParent<Webpage> _getDocumentsByParent;
        private readonly IDocumentMetadataService _documentMetadataService;
        private readonly IOptions<SystemConfig> _config;

        public WebpageAdminService(IRepository<Webpage> webpageRepository,
            IMapper mapper,
            IGetDocumentsByParent<Webpage> getDocumentsByParent,
            IDocumentMetadataService documentMetadataService, IOptions<SystemConfig> config)
        {
            _webpageRepository = webpageRepository;
            _mapper = mapper;
            _getDocumentsByParent = getDocumentsByParent;
            _documentMetadataService = documentMetadataService;
            _config = config;
        }


        public Webpage GetWebpage(int? id)
        {
            return id.HasValue ? _webpageRepository.Get(id.Value) : null;
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
            var revealInNavigation = _documentMetadataService.GetMetadata(instance).RevealInNavigation;
            _mapper.Map(model, instance);

            if (revealInNavigation)
                if (instance != null)
                    instance.RevealInNavigation = true;

            if (additionalPropertyModel != null)
                _mapper.Map(additionalPropertyModel, instance);

            _webpageRepository.Add(instance);

            return instance;
        }

        public Webpage Update(UpdateWebpageViewModel viewModel)
        {
            var webpage = _webpageRepository.Get(viewModel.Id);

            _mapper.Map(viewModel, webpage);

            foreach (var model in viewModel.Models)
                _mapper.Map(model, webpage);

            var otherSections = webpage.GetType().GetProperty("FeaturedArticle")?.GetValue(webpage);
            
            _webpageRepository.Update(webpage);

            return webpage;
        }

        public Webpage Delete(int id)
        {
            var webpage = GetWebpage(id);
            _webpageRepository.Delete(webpage);
            return webpage;
        }

        public List<SortItem> GetSortItems(int? id)
        {
            var parent = GetWebpage(id);
            return _getDocumentsByParent.GetDocuments(parent)
                .Select(
                    arg => new SortItem {Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name})
                .OrderBy(x => x.Order)
                .ToList();
        }

        public void SetOrders(List<SortItem> items)
        {
            _webpageRepository.Transact(repository => items.ForEach(item =>
            {
                var mediaFile = repository.Get(item.Id);
                mediaFile.DisplayOrder = item.Order;
                repository.Update(mediaFile);
            }));
        }

        public void PublishNow(int id)
        {
            var webpage = _webpageRepository.Get(id);
            if (webpage == null)
                return;

            if (webpage.PublishOn == null)
            {
                webpage.Published = true;
                webpage.PublishOn = DateTime.UtcNow;
                _webpageRepository.Update(webpage);
            }
        }

        public void Unpublish(int id)
        {
            var webpage = _webpageRepository.Get(id);
            if (webpage == null)
                return;
            webpage.Published = false;
            webpage.PublishOn = null;
            _webpageRepository.Update(webpage);
        }

        public string GetServerDate()
        {
            var now = DateTime.UtcNow;
            return now.Add(_config.Value.TimeZoneInfo.GetUtcOffset(now)).ToString();
        }
    }
}