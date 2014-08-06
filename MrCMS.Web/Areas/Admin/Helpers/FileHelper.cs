using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class FileHelper
    {
        public static ViewDataUploadFilesResult GetUploadFilesResult(this MediaFile mediaFile)
        {
            return new ViewDataUploadFilesResult(mediaFile);
        }
    }
}
