using System.ComponentModel;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Core.Pages
{
    public class TextPage : Webpage
    {
        [DisplayName("Featured Image")]
        public virtual string FeatureImage { get; set; }
    }
}