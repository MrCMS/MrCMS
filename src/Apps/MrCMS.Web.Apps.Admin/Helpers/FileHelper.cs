using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Helpers
{
    public static class FileHelper
    {
        public static ViewDataUploadFilesResult GetUploadFilesResult(this MediaFile mediaFile)
        {
            return new ViewDataUploadFilesResult(mediaFile);
        }
    }
}