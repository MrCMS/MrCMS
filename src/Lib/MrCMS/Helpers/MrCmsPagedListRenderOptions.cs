using X.PagedList.Mvc.Common;

namespace MrCMS.Helpers
{
    public static class MrCmsPagedListRenderOptions
    {
        public static PagedListRenderOptions Bootstrap4 =>
            new PagedListRenderOptions
            {
                LiElementClasses = new string[] {"page-item"},
                PageClasses = new string[] {"page-link"},
                DisplayEllipsesWhenNotShowingAllPageNumbers = false
            };
    }
}