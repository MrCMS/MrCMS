using System.Collections.Generic;
using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class WebpageAdminService : IWebpageAdminService
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IGetDocumentsByParent<Webpage> _getDocumentsByParent;

        public WebpageAdminService(IRepository<Webpage> webpageRepository, IGetDocumentsByParent<Webpage> getDocumentsByParent)
        {
            _webpageRepository = webpageRepository;
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

        public void Update(Webpage webpage)
        {
            _webpageRepository.Update(webpage);
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
                webpage.PublishOn = CurrentRequestData.Now;
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