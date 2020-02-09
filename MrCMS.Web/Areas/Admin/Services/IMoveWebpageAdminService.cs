using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IMoveWebpageAdminService
    {
        IEnumerable<SelectListItem> GetValidParents(Webpage webpage);
        MoveWebpageResult Validate(MoveWebpageModel moveWebpageModel);
        MoveWebpageConfirmationModel GetConfirmationModel(MoveWebpageModel model);
        MoveWebpageResult Confirm(MoveWebpageModel model);
        MoveWebpageModel GetModel(Webpage webpage);
    }
}