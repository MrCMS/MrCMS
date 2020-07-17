using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public class MediaCategoryAdminService : IMediaCategoryAdminService
    {
        private readonly IRepository<MediaCategory> _mediaCategoryRepository;
        private readonly IGetDocumentsByParent<MediaCategory> _getDocumentsByParent;
        private readonly IUrlValidationService _urlValidationService;
        private readonly IMapper _mapper;

        public MediaCategoryAdminService(IRepository<MediaCategory> mediaCategoryRepository,
            IGetDocumentsByParent<MediaCategory> getDocumentsByParent, 
            IUrlValidationService urlValidationService,
            IMapper mapper)
        {
            _mediaCategoryRepository = mediaCategoryRepository;
            _getDocumentsByParent = getDocumentsByParent;
            _urlValidationService = urlValidationService;
            _mapper = mapper;
        }

        public AddMediaCategoryModel GetNewCategoryModel(int? id)
        {
            return new AddMediaCategoryModel
            {
                ParentId = id
            };
        }

        public MediaCategory GetCategory(int? id)
        {
            return id.HasValue ? _mediaCategoryRepository.Get(id.Value) : null;
        }

        public MediaCategory Add(AddMediaCategoryModel model)
        {
            var mediaCategory = _mapper.Map<MediaCategory>(model);
            _mediaCategoryRepository.Add(mediaCategory);
            return mediaCategory;
        }

        public MediaCategory Update(UpdateMediaCategoryModel model)
        {
            var category = GetCategory(model.Id);
            _mapper.Map(model, category);
            _mediaCategoryRepository.Update(category);
            return category;
        }

        public MediaCategory Delete(int id)
        {
            var mediaCategory = _mediaCategoryRepository.Get(id);
            _mediaCategoryRepository.Delete(mediaCategory);
            return mediaCategory;
        }

        public List<SortItem> GetSortItems(int id)
        {
            return
                _getDocumentsByParent.GetDocuments(GetCategory(id))
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

        public UpdateMediaCategoryModel GetEditModel(int id)
        {
            return _mapper.Map<UpdateMediaCategoryModel>(GetCategory(id));
        }
    }
}