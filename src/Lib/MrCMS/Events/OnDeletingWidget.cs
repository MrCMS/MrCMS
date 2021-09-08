using System.Threading.Tasks;
using MrCMS.Entities.Widget;

namespace MrCMS.Events
{
    public class OnDeletingWidget : IOnDeleting<Widget>
    {
        public Task Execute(OnDeletingArgs<Widget> args)
        {
            args.Item?.LayoutArea?.Widgets.Remove(args.Item); //required to clear cache

            return Task.CompletedTask;
        }
    }
}