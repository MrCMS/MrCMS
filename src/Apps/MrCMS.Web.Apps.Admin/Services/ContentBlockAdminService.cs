using AutoMapper;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Admin.Models.ContentBlocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Apps.Admin.Services
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

            return GetAdditionalPropertyModel(block.Unproxy().GetType());
        }

        public int? Add(AddContentBlockViewModel addModel, object additionalPropertyModel)
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

            block.DisplayOrder = GetDisplayOrder(block.Webpage?.Id ?? 0);
            block.Webpage?.ContentBlocks.Add(block);
            _repository.Add(block);

            return block.Webpage?.Id;
        }

        public UpdateContentBlockViewModel GetUpdateModel(int id)
        {
            var contentBlock = GetEntity(id);

            return _mapper.Map<UpdateContentBlockViewModel>(contentBlock);
        }


        public int? Update(UpdateContentBlockViewModel updateModel, object additionalPropertyModel)
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

            _repository.Update(block);

            return block.Webpage?.Id;
        }

        public int? Delete(int id)
        {
            var block = GetEntity(id);
            if (block == null)
            {
                return null;
            }

            var webpageId = block.Webpage?.Id;
            block.Webpage?.ContentBlocks.Remove(block);
            _repository.Delete(block);
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

        public void Sort(IList<SortItem> sortItems)
        {
            _sortService.Sort<ContentBlock>(sortItems);
        }

        public ContentBlock GetEntity(int id)
        {
            return _repository.Get(id).Unproxy();
        }
    }
}