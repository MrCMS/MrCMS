using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Core.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.ContentBlocks
{
    public class FourColumnBlockViewModel : IAddPropertiesViewModel<FourColumnBlock>, IUpdatePropertiesViewModel<FourColumnBlock>
    {
        public string Text { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }

        public string Image { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }

        public string Link { get; set; }
        public string Link2 { get; set; }
        public string Link3 { get; set; }
        public string Link4 { get; set; }
    }
}