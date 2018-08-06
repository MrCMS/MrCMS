using System.ComponentModel;
using MrCMS.Web.Apps.Admin.Models.Tabs;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models
{
    public class TextPageViewModel : IUpdatePropertiesViewModel<TextPage>, IAddPropertiesViewModel<TextPage>
    {
        [DisplayName("Featured Image")]
        public string FeatureImage { get; set; }
    }
}