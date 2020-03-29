using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Models.ContentBlocks;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IContentBlockAdminService
    {
        object GetAdditionalPropertyModel(string blockType);
        object GetAdditionalPropertyModel(int id);
        Task<int?> AddAsync(AddContentBlockViewModel addModel, object additionalPropertyModel);
        UpdateContentBlockViewModel GetUpdateModel(int id);
        ContentBlock GetEntity(int id);
        Task<int?> Update(UpdateContentBlockViewModel updateModel, object additionalPropertyModel);
        Task<int?> Delete(int id);

        IList<ContentBlock> GetBlocks(int webpageId);

        IList<SortItem> GetSortItems(int webpageId);
        Task Sort(IList<SortItem> sortItems);
    }
}