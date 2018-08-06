using System.ComponentModel;
using MrCMS.Web.Apps.Admin.Models.Tabs;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models
{
    public class TextPageViewModel : IImplementationPropertiesViewModel<TextPage>
    {
        public int Id { get; set; }
        [DisplayName("Featured Image")]
        public string FeatureImage { get; set; }
    }
}