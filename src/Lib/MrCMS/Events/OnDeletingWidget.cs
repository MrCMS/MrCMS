using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;

namespace MrCMS.Events
{
    public class OnDeletingWidget : OnDataDeleting<Widget>
    {
        private readonly IRepository<Widget> _widgetRepository;

        public OnDeletingWidget(IRepository<Widget> widgetRepository)
        {
            _widgetRepository = widgetRepository;
        }
        //public void Execute(OnDeletingArgs<Widget> args)
        //{
        //    Widget widget = args.Item;
        //}

        public override Task<IResult> OnDeleting(Widget entity, DbContext dbContext)
        {
            entity.ShownOn.ForEach(webpage => webpage.ShownWidgets.Remove(entity));
            entity.HiddenOn.ForEach(webpage => webpage.HiddenWidgets.Remove(entity));
            entity.LayoutArea?.Widgets.Remove(entity); //required to clear cache
            entity.Webpage?.Widgets.Remove(entity);

            return Success;
        }
    }
}