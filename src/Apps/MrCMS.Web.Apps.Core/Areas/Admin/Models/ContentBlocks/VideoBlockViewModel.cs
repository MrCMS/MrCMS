using System.ComponentModel.DataAnnotations;
using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Core.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.ContentBlocks
{
    public class VideoBlockViewModel : IAddPropertiesViewModel<VideoBlock>,
        IUpdatePropertiesViewModel<VideoBlock>
    {
        [Required]
        public string Url { get; set; }
        public string Image { get; set; } // Placeholder
    }
}