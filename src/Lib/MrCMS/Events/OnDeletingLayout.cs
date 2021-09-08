using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Events
{
    public class OnDeletingLayout : IOnDeleting<Layout>
    {
        public Task Execute(OnDeletingArgs<Layout> args)
        {
            Layout layout = args.Item;
            if (layout == null) return Task.CompletedTask;
            foreach (PageTemplate pageTemplate in layout.PageTemplates)
            {
                pageTemplate.Layout = null;
            }

            layout.PageTemplates.Clear();
            return Task.CompletedTask;
        }
    }
}