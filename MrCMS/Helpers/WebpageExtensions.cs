using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Paging;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Helpers
{
    public static class WebpageExtensions
    {
        public static bool IsHidden(this Webpage webpage, Widget widget)
        {
            if (widget.Webpage == webpage)
                return false;

            foreach (Webpage item in webpage.ActivePages)
            {
                if (item.HiddenWidgets.Contains(widget))
                    return true;
                if (item.ShownWidgets.Contains(widget))
                    return false;

                // if it's not overidden somehow and it is from the item we're looking at, use the recursive flag from the widget
                if (widget.Webpage.Unproxy() == item)
                    return !widget.IsRecursive;
            }
            return false;
        }

        public static IPagedList<T> PagedChildren<T>(this Webpage webpage, QueryOver<T> query = null, int pageNum = 1,
            int pageSize = 10) where T : Webpage
        {
            query = query ??
                    QueryOver.Of<T>()
                        .Where(a => a.Parent == webpage && a.PublishOn != null && a.PublishOn <= CurrentRequestData.Now)
                        .ThenBy(arg => arg.PublishOn)
                        .Desc;

            return MrCMSApplication.Get<ISession>().Paged(query, pageNum, pageSize);
        }

        public static bool CanAddChildren(this Webpage webpage)
        {
            return webpage.GetMetadata().ValidChildrenTypes.Any();
        }
    }
}