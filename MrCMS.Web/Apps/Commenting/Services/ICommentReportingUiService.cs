using MrCMS.Web.Apps.Commenting.Models;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public interface ICommentReportingUiService
    {
        ReportResponse Report(ReportModel reportModel);
    }
}