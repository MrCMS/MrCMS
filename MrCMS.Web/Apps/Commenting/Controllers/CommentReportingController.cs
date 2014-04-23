using System.Web.Mvc;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;

namespace MrCMS.Web.Apps.Commenting.Controllers
{
    public class CommentReportingController : BaseCommentUiController
    {
        private readonly ICommentReportingUiService _commentReportingUiService;

        public CommentReportingController(ICommentReportingUiService commentReportingUiService)
        {
            _commentReportingUiService = commentReportingUiService;
        }

        [HttpPost]
        public ActionResult Report(ReportModel reportModel)
        {
            var response = _commentReportingUiService.Report(reportModel);
            return RedirectToPage(response);
        }
    }
}