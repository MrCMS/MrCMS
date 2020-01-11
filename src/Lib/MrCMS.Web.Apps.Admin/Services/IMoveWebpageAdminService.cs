using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IMoveWebpageAdminService
    {
        IEnumerable<SelectListItem> GetValidParents(Webpage webpage);
        MoveWebpageResult Validate(MoveWebpageModel moveWebpageModel);
        Task<MoveWebpageConfirmationModel> GetConfirmationModel(MoveWebpageModel model);
        Task<MoveWebpageResult> Confirm(MoveWebpageModel model);
        MoveWebpageModel GetModel(Webpage webpage);
    }
}