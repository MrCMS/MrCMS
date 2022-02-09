using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models.Content;

namespace MrCMS.Web.Admin.Services.Content;

public interface IContentBlockItemAdminService
{
    Task<BlockItem> GetBlockItem(int blockId, Guid itemId);
    Task<object> GetUpdateModel(int blockId, Guid itemId);
    Task UpdateBlockItem(int blockId, Guid itemId, object model);
    Task RemoveBlockItem(int blockId, Guid itemId);
    Task SetBlockItemOrders(int blockId, List<BlockItemSortModel> blockItemSortModel);
}