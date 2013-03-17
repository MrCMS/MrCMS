using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Galleries.Pages
{
    public class GalleryList : TextPage, IDocumentContainer<Gallery>
    {
        [DisplayName("Page Size")]
        public virtual int PageSize { get; set; }
        [DisplayName("Allow Paging")]
        public virtual bool AllowPaging { get; set; }
        
        public virtual IEnumerable<Gallery> ChildItems { get { return PublishedChildren.OfType<Gallery>().OrderByDescending(page => page.DisplayOrder); } }
    }
}