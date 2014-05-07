using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Commenting.Widgets;

namespace MrCMS.Web.Apps.Commenting.Extensions
{
    public static class CommentingExtensions
    {
        public static LayoutArea GetCommentsLayoutArea(this Webpage webpage)
        {
            if (webpage == null || webpage.CurrentLayout == null)
                return null;
            IEnumerable<LayoutArea> layoutAreas = webpage.CurrentLayout.GetLayoutAreas();
            return layoutAreas.FirstOrDefault(area => area.AreaName == CommentingApp.LayoutAreaName);
        }

        public static CommentingWidget GetCommentingWidget(this Webpage webpage)
        {
            return webpage != null
                       ? webpage.Widgets.OfType<CommentingWidget>().FirstOrDefault()
                       : null;
        }

        public static bool CommentingDisabled(this Webpage webpage)
        {
            CommentingWidget commentingWidget = GetCommentingWidget(webpage);
            return commentingWidget == null || commentingWidget.CommentingDisabled;
        }
    }
}