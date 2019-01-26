using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Documents;

namespace MrCMS.Web.Apps.Core.ContentBlocks
{
    public class VideoBlock : ContentBlock
    {
        [Required]
        public virtual string Url { get; set; }
        public virtual string Image { get; set; } // Placeholder
    }
}