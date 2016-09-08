using System.Collections.Generic;
using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Services;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class MediaCategoryAdminService : IMediaCategoryAdminService
    {
        private readonly IRepository<MediaCategory> _mediaCategoryRepository;
        private readonly IGetDocumentsByParent<MediaCategory> _getDocumentsByParent;
        private readonly IUrlValidationService _urlValidationService;

        public MediaCategoryAdminService(IRepository<MediaCategory> mediaCategoryRepository, IGetDocumentsByParent<MediaCategory> getDocumentsByParent,IUrlValidationService urlValidationService)
        {
            _mediaCategoryRepository = mediaCategoryRepository;
            _getDocumentsByParent = getDocumentsByParent;
            _urlValidationService = urlValidationService;
        }

        public MediaCategory GetNewCategoryModel(int? id)
        {
            return new MediaCategory
            {
                Parent = id.HasValue ? _mediaCategoryRepository.Get(id.Value) : null
            }; ;
        }

        public void Add(MediaCategory mediaCategory)
        {
            _mediaCategoryRepository.Add(mediaCategory);
        }

        public void Update(MediaCategory mediaCategory)
        {
            _mediaCategoryRepository.Update(mediaCategory);
        }

        public void Delete(MediaCategory mediaCategory)
        {
            _mediaCategoryRepository.Delete(mediaCategory);
        }

        public List<SortItem> GetSortItems(MediaCategory parent)
        {
            return
                _getDocumentsByParent.GetDocuments(parent)
                    .Select(
                        arg => new SortItem { Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name })
                    .OrderBy(x => x.Order)
                    .ToList();

        }

        public void SetOrders(List<SortItem> items)
        {
            _mediaCategoryRepository.Transact(repository => items.ForEach(item =>
            {
                var mediaFile = repository.Get(item.Id);
                mediaFile.DisplayOrder = item.Order;
                repository.Update(mediaFile);
            }));
        }

        public bool UrlIsValidForMediaCategory(string urlSegment, int? id)
        {
            return _urlValidationService.UrlIsValidForMediaCategory(urlSegment, id);
        }
    }
}