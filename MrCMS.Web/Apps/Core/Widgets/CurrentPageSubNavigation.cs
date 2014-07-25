using System.ComponentModel;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Core.Widgets
{
    public class CurrentPageSubNavigation : Widget
    {
        [DisplayName("Show Name As Title")]
        public virtual bool ShowNameAsTitle { get; set; }
    }
}