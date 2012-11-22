using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.AddOns.Pages.Blogs
{
    [DocumentTypeDefinition(ChildrenListType.WhiteList, typeof(BlogPage), Name = "Blog Container", IconClass = "icon-book", DisplayOrder = 1, Type = typeof(BlogContainer), WebGetAction = "View", WebGetController = "BlogContainer", SortBy = "CreatedOn", SortByDesc = true, DefaultLayoutName = "Two Column", MaxChildNodes = 15)]
    [MrCMSMapClass]
    public class BlogContainer : TextPage, IDocumentContainer<BlogPage>
    {
        [DisplayName("Page Size")]
        public virtual int PageSize { get; set; }
        [DisplayName("Allow Paging")]
        public virtual bool AllowPaging { get; set; }

        public virtual IEnumerable<BlogPage> ChildItems { get { return PublishedChildren.OfType<BlogPage>().OrderByDescending(page => page.CreatedOn); } }
    }
}
