using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Settings;

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


        // public static bool CanAddChildren(this Webpage webpage)
        // {
        //     return webpage.GetMetadata().ValidChildrenTypes.Any();
        // }
    }
}