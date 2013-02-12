using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Blog.Pages
{
    public class Blog : Webpage, IDocumentContainer<Post>
    {
        public virtual int PageSize { get; set; }
        public virtual bool AllowPaging { get; set; }

        public virtual IEnumerable<Post> ChildItems
        {
            get { return Children.OfType<Post>(); }
        }
    };
}