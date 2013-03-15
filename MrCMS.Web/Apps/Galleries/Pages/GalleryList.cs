using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Galleries.Pages
{
    public class GalleryList : TextPage 
    {
        public virtual IEnumerable<Gallery> ChildItems { get { return PublishedChildren.OfType<Gallery>().OrderByDescending(page => page.DisplayOrder); } }
    }
}