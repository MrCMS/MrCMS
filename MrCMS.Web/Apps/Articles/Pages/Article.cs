using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Articles.Pages
{
    public class Article : TextPage, IContainerItem
    {
        [AllowHtml]
        public virtual string Abstract { get; set; }

        public virtual string ContainerUrl
        {
            get
            {
                var documentContainer = Parent as IDocumentContainer<Article>;
                return documentContainer == null ? null : documentContainer.LiveUrlSegment;
            }
        }
    }
}