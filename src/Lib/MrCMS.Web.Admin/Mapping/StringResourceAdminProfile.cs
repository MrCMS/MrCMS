using Mapster;
using MrCMS.Entities.Resources;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class StringResourceAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<StringResource, UpdateStringResourceModel>();
            config.NewConfig<UpdateStringResourceModel, StringResource>();
            config.NewConfig<StringResource, AddStringResourceModel>();
            config.NewConfig<AddStringResourceModel, StringResource>()
                .MapEntityLookup(x => x.SiteId, x => x.Site);
        }
    }
}