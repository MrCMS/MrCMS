using System;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Services.Content;

public interface IContentBlockItemAdminService
{
    Task<BlockItem> GetBlockItem(int blockId, Guid itemId);
    Task<object> GetUpdateModel(int blockId, Guid itemId);
    Task UpdateBlockItem(int blockId, Guid itemId, object model);
}