using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Events
{
    public class OnDeletingPageWidgetSort : IOnDeleting
    {
        public void Execute(OnDeletingArgs args)
        {
            var sort = args.Item as PageWidgetSort;
            if (sort == null) return;

            if (sort.LayoutArea.PageWidgetSorts.Contains(sort))
                sort.LayoutArea.PageWidgetSorts.Remove(sort);
            if (sort.Webpage.PageWidgetSorts.Contains(sort))
                sort.Webpage.PageWidgetSorts.Remove(sort);
            if (sort.Widget.PageWidgetSorts.Contains(sort))
                sort.Widget.PageWidgetSorts.Remove(sort);
        }
    }
}