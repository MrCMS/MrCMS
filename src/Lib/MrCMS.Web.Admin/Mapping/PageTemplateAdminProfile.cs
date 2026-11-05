using Mapster;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class PageTemplateAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PageTemplate, AddPageTemplateModel>();
            config.NewConfig<AddPageTemplateModel, PageTemplate>()
                .MapEntityLookup(x => x.LayoutId, x => x.Layout);
            config.NewConfig<PageTemplate, UpdatePageTemplateModel>();
            config.NewConfig<UpdatePageTemplateModel, PageTemplate>()
                .MapEntityLookup(x => x.LayoutId, x => x.Layout);
        }
    }
}