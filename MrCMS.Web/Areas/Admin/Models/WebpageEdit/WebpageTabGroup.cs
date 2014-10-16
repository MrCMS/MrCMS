using System.Collections.Generic;

namespace MrCMS.Web.Areas.Admin.Models.WebpageEdit
{
    public abstract class WebpageTabGroup : WebpageTabBase
    {
        protected WebpageTabGroup()
        {
            Children = new List<WebpageTabBase>();
        }

        public List<WebpageTabBase> Children { get; private set; }

        public void SetChildren(List<WebpageTabBase> children)
        {
            Children = children;
        }
    }
}