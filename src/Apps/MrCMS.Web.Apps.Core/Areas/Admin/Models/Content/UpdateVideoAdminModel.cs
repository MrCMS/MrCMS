using System.ComponentModel;
using static MrCMS.Web.Apps.Core.Entities.ContentBlocks.Video;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Content
{
    public class UpdateVideoAdminModel
    {
        [DisplayName("Active Type")]
        public VideoType ActiveType { get; set; }
    }
}
