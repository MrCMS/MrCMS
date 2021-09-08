using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Models;
using MrCMS.Web.Admin.Models.ContentBlocks;

namespace MrCMS.Web.Admin.Services
{
    public interface IContentBlockAdminService
    {
        object GetAdditionalPropertyModel(string blockType);
        Task<object> GetAdditionalPropertyModel(int id);
        Task<int?> Add(AddContentBlockViewModel addModel, object additionalPropertyModel);
        Task<UpdateContentBlockViewModel> GetUpdateModel(int id);
        Task<ContentBlock> GetEntity(int id);
        Task<int?> Update(UpdateContentBlockViewModel updateModel, object additionalPropertyModel);
        Task<int?> Delete(int id);

        Task<List<ContentBlock>> GetBlocks(int webpageId);

        Task<IList<SortItem>> GetSortItems(int webpageId);
        Task Sort(IList<SortItem> sortItems);
    }
}