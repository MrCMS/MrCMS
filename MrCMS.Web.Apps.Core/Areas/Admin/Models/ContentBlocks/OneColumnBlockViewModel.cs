using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Core.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.ContentBlocks
{
    public class OneColumnBlockViewModel : IAddPropertiesViewModel<OneColumnBlock>, IUpdatePropertiesViewModel<OneColumnBlock>
    {
        public string Text { get; set; }
    }
}