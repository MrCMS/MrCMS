using System.Web.Mvc;
using MrCMS.DbConfiguration.Mapping;

namespace MrCMS.Entities.Widget
{
    [MrCMSMapClass]
    public class TextWidget : Widget
    {
        [AllowHtml]
        public virtual string Text { get; set; }
    }
}
