using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Areas.Admin.Models.ContentBlocks;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class ContentBlockAdminService : IContentBlockAdminService
    {
        private readonly IRepository<ContentBlock> _repository;
        private readonly IMapper _mapper;
        private readonly ISortService _sortService;

        public ContentBlockAdminService(IRepository<ContentBlock> repository, IMapper mapper, ISortService sortService)
        {
            _repository = repository;
            _mapper = mapper;
            _sortService = sortService;
        }


        private int GetDisplayOrder(int webpageId)
        {
            return _repository.Query()
                       .Where(x => x.Webpage.Id == webpageId)
                       .OrderByDescending(x => x.DisplayOrder)
                       .Select(x => x.DisplayOrder)
                       .FirstOrDefault() + 1; // + 1 because we want to set a value one higher than the highest
        }

        public object GetAdditionalPropertyModel(string blockType)
        {
            var type = TypeHelper.GetTypeByName(blockType);
            if (type == null)
            {
                return null;
            }

            return GetAdditionalPropertyModel(type);
        }

        private static object GetAdditionalPropertyModel(Type type)
        {
            var additionalPropertyType = TypeHelper.GetAllConcreteTypesAssignableFrom(
                typeof(IAddPropertiesViewModel<>).MakeGenericType(type)).FirstOrDefault();
            if (additionalPropertyType == null)
            {
                return null;
            }

            return Activator.CreateInstance(additionalPropertyType);
        }

        public object GetAdditionalPropertyModel(int id)
        {
            var block = GetEntity(id);
            if (block == null)
            {
                return null;
            }

            return GetAdditionalPropertyModel(block.GetType());
        }

        public async Task<int?> AddAsync(AddContentBlockViewModel addModel, object additionalPropertyModel)
        {
            var type = TypeHelper.GetTypeByName(addModel.BlockType);
            if (type == null)
            {
                return null;
            }

            if (!(Activator.CreateInstance(type) is ContentBlock block))
            {
                return null;
            }

            _mapper.Map(addModel, block);
            if (additionalPropertyModel != null)
            {
                _mapper.Map(additionalPropertyModel, block);
            }

            block.DisplayOrder = GetDisplayOrder(block.WebpageId);
            await _repository.Add(block);

            return block.WebpageId;
        }

        public UpdateContentBlockViewModel GetUpdateModel(int id)
        {
            var contentBlock = GetEntity(id);

            return _mapper.Map<UpdateContentBlockViewModel>(contentBlock);
        }


        public async Task<int?> Update(UpdateContentBlockViewModel updateModel, object additionalPropertyModel)
        {
            var block = GetEntity(updateModel.Id);
            if (block == null)
            {
                return null;
            }

            _mapper.Map(updateModel, block);
            if (additionalPropertyModel != null)
            {
                _mapper.Map(additionalPropertyModel, block);
            }

            await _repository.Update(block);

            return block.Webpage?.Id;
        }

        public async Task<int?> Delete(int id)
        {
            var block = GetEntity(id);
            if (block == null)
            {
                return null;
            }

            var webpageId = block.WebpageId;
            block.Webpage?.ContentBlocks.Remove(block);
            await _repository.Delete(block);
            return webpageId;
        }

        public IList<ContentBlock> GetBlocks(int webpageId)
        {
            return _repository.Query().Where(x => x.Webpage.Id == webpageId).OrderBy(x => x.DisplayOrder).ToList();
        }

        public IList<SortItem> GetSortItems(int webpageId)
        {
            return _sortService.GetSortItems(GetBlocks(webpageId).ToList());
        }

        public async Task Sort(IList<SortItem> sortItems)
        {
       await     _sortService.Sort<ContentBlock>(sortItems);
        }

        public ContentBlock GetEntity(int id)
        {
            return _repository.LoadSync(id);
        }
    }
}