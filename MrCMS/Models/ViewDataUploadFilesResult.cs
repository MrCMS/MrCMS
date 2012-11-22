using System.Drawing;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;

namespace MrCMS.Models
{
    public class ViewDataUploadFilesResult
    {
        public ViewDataUploadFilesResult(MediaFile file, string adminThumb)
        {
            Id = file.Id;
            name = file.FileName;
            url = "/" + file.FileLocation;
            thumbnail_url = file.IsImage ? "/" + adminThumb : null;
            delete_type = "POST";
            delete_url = string.Format("/admin/File/Delete/{0}", file.Id);
            size = file.ContentLength;
            Type = file.ContentType;
            Path = file.FileLocation;
            seo_url = string.Format("/admin/File/UpdateSEO/{0}", file.Id);
            title = file.Title ?? string.Empty;
            description = file.Description ?? string.Empty;
        }


        public int Id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_type { get; set; }
        public string delete_url { get; set; }
        public int size { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public string seo_url { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }
}