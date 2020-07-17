using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Helpers
{
    public static class FileHelper
    {
        public static ViewDataUploadFilesResult GetUploadFilesResult(this MediaFile mediaFile)
        {
            return new ViewDataUploadFilesResult(mediaFile);
        }
    }
}