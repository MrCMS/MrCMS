using System.Web.Mvc;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Core.Widgets
{
    public class TextWidget : Widget
    {
        [AllowHtml]
        public virtual string Text { get; set; }
    }
}
