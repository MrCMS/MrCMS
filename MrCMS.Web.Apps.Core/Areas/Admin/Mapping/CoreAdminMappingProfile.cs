using AutoMapper;
using MrCMS.Web.Apps.Core.Areas.Admin.Models;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Mapping
{
    public class CoreAdminMappingProfile : Profile
    {
        public CoreAdminMappingProfile()
        {
            CreateMap<TextPageViewModel, TextPage>(MemberList.Source);
        }
    }
}