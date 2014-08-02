using MrCMS.DbConfiguration;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;

namespace MrCMS.Events
{
    public class OnDeletingWidget : IOnDeleting
    {
        public void Execute(OnDeletingArgs args)
        {
            var widget = args.Item as Widget;
            if (widget == null)
                return;

            widget.ShownOn.ForEach(webpage => webpage.ShownWidgets.Remove(widget));
            widget.HiddenOn.ForEach(webpage => webpage.HiddenWidgets.Remove(widget));
            if (widget.LayoutArea != null)
            {
                widget.LayoutArea.Widgets.Remove(widget); //required to clear cache
            }
            if (widget.Webpage != null)
            {
                widget.Webpage.Widgets.Remove(widget);
            }

        }
    }
}