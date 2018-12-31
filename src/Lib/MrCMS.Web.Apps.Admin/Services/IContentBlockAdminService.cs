using System.Collections;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models.ContentBlocks;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IContentBlockAdminService
    {
        object GetAdditionalPropertyModel(string blockType);
        object GetAdditionalPropertyModel(int id);
        int? Add(AddContentBlockViewModel addModel, object additionalPropertyModel);
        UpdateContentBlockViewModel GetUpdateModel(int id);
        ContentBlock GetEntity(int id);
        int? Update(UpdateContentBlockViewModel updateModel, object additionalPropertyModel);
        int? Delete(int id);

        IList<ContentBlock> GetBlocks(int webpageId);

        IList<SortItem> GetSortItems(int webpageId);
        void Sort(IList<SortItem> sortItems);
    }
}