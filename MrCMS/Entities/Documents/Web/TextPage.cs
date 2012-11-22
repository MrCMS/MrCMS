using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.DbConfiguration.Mapping;

namespace MrCMS.Entities.Documents.Web
{
    [DocumentTypeDefinition(ChildrenListType.BlackList, Name = "Text Page", IconClass = "icon-file", DisplayOrder = 1, Type = typeof(TextPage), WebGetAction = "View", WebGetController = "TextPage", DefaultLayoutName = "Three Column")]
    [MrCMSMapClass]
    public class TextPage : Webpage
    {
        [AllowHtml]
        [DisplayName("Body Content")]
        public virtual string BodyContent { get; set; }

        [DisplayName("Featured Image")]
        public virtual string FeatureImage { get; set; }
    }
}