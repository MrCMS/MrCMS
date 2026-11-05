using Mapster;
using MrCMS.Entities.Widget;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class WidgetAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Widget, AddWidgetModel>();
            config.NewConfig<AddWidgetModel, Widget>()
                .MapEntityLookup(x => x.LayoutAreaId, x => x.LayoutArea);
            config.NewConfig<Widget, UpdateWidgetModel>();
            config.NewConfig<UpdateWidgetModel, Widget>();
        }
    }
}