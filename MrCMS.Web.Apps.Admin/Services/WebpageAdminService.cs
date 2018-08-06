using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class WebpageAdminService : IWebpageAdminService
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IMapper _mapper;
        private readonly IGetDocumentsByParent<Webpage> _getDocumentsByParent;

        public WebpageAdminService(IRepository<Webpage> webpageRepository,
            IMapper mapper,
            IGetDocumentsByParent<Webpage> getDocumentsByParent)
        {
            _webpageRepository = webpageRepository;
            _mapper = mapper;
            _getDocumentsByParent = getDocumentsByParent;
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

            _mapper.Map(model, instance);

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

            _webpageRepository.Update(webpage);

            return webpage;
        }

        public void Delete(Webpage webpage)
        {
            _webpageRepository.Delete(webpage);
        }

        public List<SortItem> GetSortItems(Webpage parent)
        {
            return _getDocumentsByParent.GetDocuments(parent)
                .Select(
                    arg => new SortItem { Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name })
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
                webpage.PublishOn = DateTime.UtcNow; // todo: check date
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
    }
}