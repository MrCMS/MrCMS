using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Controllers;
using MrCMS.Web.Admin.Models.Content;

namespace MrCMS.Web.Admin.Services.Content;

public interface IContentBlockAdminService
{
    Task<IReadOnlyList<ContentBlockOption>> GetContentRowOptions();
    Task<AddContentBlockModel> GetAddModel(int id);
    Task<ContentBlock> AddBlock(AddContentBlockModel model);
    Task<IContentBlock> GetBlock(int id);
    Task UpdateBlock(int id, object model);
    Task RemoveBlock(int id);
    Task AddChild(int id);
    Task<object> GetUpdateModel(int id);
}