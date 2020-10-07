using X.PagedList.Mvc.Core.Common;

namespace MrCMS.Helpers
{
    public static class MrCMSPagedListRenderOptions
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