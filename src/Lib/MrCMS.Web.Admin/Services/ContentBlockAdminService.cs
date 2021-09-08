using AutoMapper;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Admin.Infrastructure.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Web.Admin.Models.ContentBlocks;
using NHibernate.Linq;

namespace MrCMS.Web.Admin.Services
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


        private async Task<int> GetDisplayOrder(int webpageId)
        {
            return (await _repository.Query()
                .Where(x => x.Webpage.Id == webpageId)
                .OrderByDescending(x => x.DisplayOrder)
                .Select(x => x.DisplayOrder)
                .FirstOrDefaultAsync()) + 1; // + 1 because we want to set a value one higher than the highest
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

        private static readonly HashSet<Type> AddPropertiesTypes = TypeHelper.GetAllConcreteTypesAssignableFromGeneric(
            typeof(IAddPropertiesViewModel<>));

        private static object GetAdditionalPropertyModel(Type type)
        {
            var additionalPropertyType = AddPropertiesTypes.FirstOrDefault(x =>
                typeof(IAddPropertiesViewModel<>).MakeGenericType(type).IsAssignableFrom(x));
            if (additionalPropertyType == null)
            {
                return null;
            }

            return Activator.CreateInstance(additionalPropertyType);
        }

        public async Task<object> GetAdditionalPropertyModel(int id)
        {
            var block = await GetEntity(id);
            if (block == null)
            {
                return null;
            }

            return GetAdditionalPropertyModel(block.Unproxy().GetType());
        }

        public async Task<int?> Add(AddContentBlockViewModel addModel, object additionalPropertyModel)
        {
            var type = TypeHelper.GetTypeByName(addModel.BlockType);
            if (type == null)
            {
                return null;
            }

            var block = Activator.CreateInstance(type) as ContentBlock;
            if (block == null)
            {
                return null;
            }

            _mapper.Map(addModel, block);
            if (additionalPropertyModel != null)
            {
                _mapper.Map(additionalPropertyModel, block);
            }

            block.DisplayOrder = await GetDisplayOrder(block.Webpage?.Id ?? 0);
            block.Webpage?.ContentBlocks.Add(block);
            await _repository.Add(block);

            return block.Webpage?.Id;
        }

        public async Task<UpdateContentBlockViewModel> GetUpdateModel(int id)
        {
            var contentBlock = await GetEntity(id);

            return _mapper.Map<UpdateContentBlockViewModel>(contentBlock);
        }


        public async Task<int?> Update(UpdateContentBlockViewModel updateModel, object additionalPropertyModel)
        {
            var block = await GetEntity(updateModel.Id);
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
            var block = await GetEntity(id);
            if (block == null)
            {
                return null;
            }

            var webpageId = block.Webpage?.Id;
            block.Webpage?.ContentBlocks.Remove(block);
            await _repository.Delete(block);
            return webpageId;
        }

        public async Task<List<ContentBlock>> GetBlocks(int webpageId)
        {
            return await _repository.Query().Where(x => x.Webpage.Id == webpageId).OrderBy(x => x.DisplayOrder).ToListAsync();
        }

        public async Task<IList<SortItem>> GetSortItems(int webpageId)
        {
            return _sortService.GetSortItems(await GetBlocks(webpageId));
        }

        public async Task Sort(IList<SortItem> sortItems)
        {
            await _sortService.Sort<ContentBlock>(sortItems);
        }

        public async Task<ContentBlock> GetEntity(int id)
        {
            return await _repository.Get(id);
        }
    }
}