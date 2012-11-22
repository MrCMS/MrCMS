using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.AddOns.Pages
{
    [MrCMSMapClass]
    public class GalleryItem : BaseDocumentItemEntity
    {
        public virtual string Preview { get; set; }
        public virtual string FullImage { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual Gallery Gallery { get; set; }
    }
}