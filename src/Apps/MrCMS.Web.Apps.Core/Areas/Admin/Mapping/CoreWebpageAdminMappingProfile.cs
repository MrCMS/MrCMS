using Mapster;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Webpages;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Mapping
{
    public class CoreWebpageAdminMappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<TextPage, TextPageViewModel>();
            config.NewConfig<TextPageViewModel, TextPage>();
        }
    }
}