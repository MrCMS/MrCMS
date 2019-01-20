using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IMergeWebpageAdminService
    {
        IEnumerable<SelectListItem> GetValidParents(Webpage webpage);
        MergeWebpageResult Validate(MergeWebpageModel moveWebpageModel);
        MergeWebpageConfirmationModel GetConfirmationModel(MergeWebpageModel model);
        MergeWebpageResult Confirm(MergeWebpageModel model);
        MergeWebpageModel GetModel(Webpage webpage);
    }
}