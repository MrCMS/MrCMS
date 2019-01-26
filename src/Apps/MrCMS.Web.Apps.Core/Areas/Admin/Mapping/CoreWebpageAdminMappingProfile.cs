using AutoMapper;
using MrCMS.Web.Apps.Core.Areas.Admin.Models;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Webpages;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Mapping
{
    public class CoreWebpageAdminMappingProfile : Profile
    {
        public CoreWebpageAdminMappingProfile()
        {
            CreateMap<TextPage, TextPageViewModel>().ReverseMap();
        }
    }
}