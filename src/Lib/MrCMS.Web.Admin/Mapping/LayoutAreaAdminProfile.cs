using Mapster;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class LayoutAreaAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<LayoutArea, AddLayoutAreaModel>();
            config.NewConfig<AddLayoutAreaModel, LayoutArea>()
                .MapEntityLookup(x => x.LayoutId, x => x.Layout);
            config.NewConfig<LayoutArea, UpdateLayoutAreaModel>();
            config.NewConfig<UpdateLayoutAreaModel, LayoutArea>();
        }
    }
}