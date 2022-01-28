using X.PagedList.Web.Common;

namespace MrCMS.Helpers
{
    public static class MrCMSPagedListRenderOptions
    {
        public static PagedListRenderOptions Bootstrap4 =>
            new()
            {
                LiElementClasses = new string[] {"page-item"},
                PageClasses = new string[] {"page-link"},
                DisplayEllipsesWhenNotShowingAllPageNumbers = false
            };
    }
}