using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
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
            return id.HasValue ? _mediaCategoryRepository.LoadSync(id.Value) : null;
        }

        public async Task<MediaCategory> Add(AddMediaCategoryModel model)
        {
            var mediaCategory = _mapper.Map<MediaCategory>(model);
            await _mediaCategoryRepository.Add(mediaCategory);
            return mediaCategory;
        }

        public async Task<MediaCategory> Update(UpdateMediaCategoryModel model)
        {
            var category = GetCategory(model.Id);
            _mapper.Map(model, category);
            await _mediaCategoryRepository.Update(category);
            return category;
        }

        public async Task<MediaCategory> Delete(int id)
        {
            var mediaCategory = await _mediaCategoryRepository.Load(id);
            await _mediaCategoryRepository.Delete(mediaCategory);
            return mediaCategory;
        }

        public async Task<List<SortItem>> GetSortItems(int id)
        {
            return
                (await _getDocumentsByParent.GetDocuments(GetCategory(id)))
                    .Select(
                        arg => new SortItem { Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name })
                    .OrderBy(x => x.Order)
                    .ToList();

        }

        public async Task SetOrders(List<SortItem> items)
        {
            var folders = items.Select(item =>
           {
               var mediaFile = _mediaCategoryRepository.LoadSync(item.Id);
               mediaFile.DisplayOrder = item.Order;
               return mediaFile;
           }).ToList();

            await _mediaCategoryRepository.UpdateRange(folders);
        }

        public async Task<bool> UrlIsValidForMediaCategory(string urlSegment, int? id)
        {
            return await _urlValidationService.UrlIsValidForMediaCategory(urlSegment, id);
        }

        public UpdateMediaCategoryModel GetEditModel(int id)
        {
            return _mapper.Map<UpdateMediaCategoryModel>(GetCategory(id));
        }
    }
}