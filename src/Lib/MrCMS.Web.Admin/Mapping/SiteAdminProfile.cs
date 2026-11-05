using Mapster;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class SiteAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Site, AddSiteModel>();
            config.NewConfig<AddSiteModel, Site>();
            config.NewConfig<Site, UpdateSiteModel>();
            config.NewConfig<UpdateSiteModel, Site>();
        }
    }
}