using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Events
{
    public class OnDeletingLayout : IOnDeleting
    {
        public void Execute(OnDeletingArgs args)
        {
            var layout = args.Item as Layout;
            if (layout == null) return;
            foreach (var pageTemplate in layout.PageTemplates)
            {
                pageTemplate.Layout = null;
            }
            layout.PageTemplates.Clear();
        }
    }
}