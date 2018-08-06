using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Models;

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

        public AddPageModel GetAddModel(int? id)
        {
            return new AddPageModel
            {
                Parent = id.HasValue ? _webpageRepository.Get(id.Value) : null
            };
        }

        public void Add(Webpage webpage)
        {
            _webpageRepository.Add(webpage);
        }

        public Webpage Update(UpdateWebpageViewModel viewModel)
        {
            var webpage = _webpageRepository.Get(viewModel.Id);

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

        public void PublishNow(Webpage webpage)
        {
            if (webpage.PublishOn == null)
            {
                webpage.Published = true;
                webpage.PublishOn = DateTime.UtcNow; // todo: check date
                _webpageRepository.Update(webpage);
            }
        }

        public void Unpublish(Webpage webpage)
        {
            webpage.Published = false;
            webpage.PublishOn = null;
            _webpageRepository.Update(webpage);
        }
    }
}