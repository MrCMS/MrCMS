using System.Collections.Generic;
using System.ComponentModel;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using System.Linq;

namespace MrCMS.AddOns.Pages.Portfolio
{
    [MrCMSMapClass]
    [DocumentTypeDefinition(ChildrenListType.WhiteList, typeof(PortfolioItem), Name = "Portfolio Container", IconClass = "icon-book", DisplayOrder = 8, Type = typeof(PortfolioContainer), WebGetAction = "View", WebGetController = "PortfolioContainer", MaxChildNodes = 15)]
    public class PortfolioContainer : TextPage, IDocumentContainer<PortfolioItem>
    {
        [DisplayName("Page Size")]
        public virtual int PageSize { get; set; }
        [DisplayName("Allow Paging")]
        public virtual bool AllowPaging { get; set; }

        public virtual IEnumerable<PortfolioItem> ChildItems
        {
            get { return PublishedChildren.OfType<PortfolioItem>(); }
        }
    }
}