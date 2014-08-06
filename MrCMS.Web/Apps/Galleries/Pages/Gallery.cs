using System.ComponentModel;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Galleries.Pages
{
    public class Gallery : TextPage
    {
        public virtual MediaCategory MediaGallery { get; set; }

        [DisplayName("Gallery Thumbnail Image")]
        public virtual string ThumbnailImage { get; set; }
    }
}