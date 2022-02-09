using System;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Models.WebpageEdit
{
    public class RedirectSEOTabViewModel
    {
        public int Id { get; set; }
        public string UrlSegment { get; set; }
        public DateTime? PublishOn { get; set; }
        public bool Published { get; set; }
        
        public Webpage.WebpagePublishStatus PublishStatus
        {
            get
            {
                Webpage.WebpagePublishStatus status = Published
                    ? Webpage.WebpagePublishStatus.Published
                    : PublishOn.HasValue
                        ? Webpage.WebpagePublishStatus.Scheduled
                        : Webpage.WebpagePublishStatus.Unpublished;
                return status;
            }
        }
    }
}