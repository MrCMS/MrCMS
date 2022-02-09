using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IMergeWebpageAdminService
    {
        Task<List<SelectListItem>> GetValidParents(Webpage webpage);
        Task<MergeWebpageResult> Validate(MergeWebpageModel moveWebpageModel);
        Task<MergeWebpageConfirmationModel> GetConfirmationModel(MergeWebpageModel model);
        Task<MergeWebpageResult> Confirm(MergeWebpageModel model);
        MergeWebpageModel GetModel(Webpage webpage);
    }
}