using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Events
{
    public class OnDeletingLayout : IOnDeleting<Layout>
    {
        public void Execute(OnDeletingArgs<Layout> args)
        {
            var layout = args.Item;
            if (layout == null) return;
            foreach (var pageTemplate in layout.PageTemplates)
            {
                pageTemplate.Layout = null;
            }
            layout.PageTemplates.Clear();
        }
    }
}