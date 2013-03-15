using System.Web.Mvc;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Core.Widgets
{
    [MrCMSMapClass]
    public class TextWidget : Widget
    {
        [AllowHtml]
        public virtual string Text { get; set; }
    }
}
