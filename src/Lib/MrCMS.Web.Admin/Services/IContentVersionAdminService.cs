using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models.Content;

namespace MrCMS.Web.Admin.Services;

public interface IContentVersionAdminService
{
    Task<IReadOnlyList<ContentVersion>> GetVersions(int webpageId);
    Task<ContentVersion> AddInitialContentVersion(AddInitialContentVersionModel model);
    Task<ContentVersionModel> GetEditModel(int id);
    // Task<AddContentSectionModel> GetAddSectionModel(int id);
}