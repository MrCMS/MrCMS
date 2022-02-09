using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Events
{
    public class AssignLayoutAreaToLayout : IOnAdding<LayoutArea>
    {
        public async Task Execute(OnAddingArgs<LayoutArea> args)
        {
            var layoutArea = args.Item;
            var layout = layoutArea.Layout;
            if (layout == null)
                return;
            
            layout.LayoutAreas.Add(layoutArea);
            await args.Session.SaveOrUpdateAsync(layout);
        }
    }
}