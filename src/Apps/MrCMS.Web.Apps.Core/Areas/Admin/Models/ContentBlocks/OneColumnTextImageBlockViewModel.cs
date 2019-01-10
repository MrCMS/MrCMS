using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Core.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.ContentBlocks
{
    public class OneColumnTextImageBlockViewModel : IAddPropertiesViewModel<OneColumnTextImageBlock>, IUpdatePropertiesViewModel<OneColumnTextImageBlock>
    {
        public string Text { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
    }
}