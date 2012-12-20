using System.ComponentModel;
using System.Web.Mvc;

namespace MrCMS.Entities.Documents.Web
{
    public interface ITextPage
    {
        [AllowHtml]
        [DisplayName("Body Content")]
        string BodyContent { get; set; }

        [DisplayName("Featured Image")]
        string FeatureImage { get; set; }
    }
}