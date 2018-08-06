using AutoMapper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.WebpageEdit;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class UpdateWebpageProfile : Profile
    {
        public UpdateWebpageProfile()
        {
            CreateMap<FormDesignTabViewModel, Webpage>(MemberList.Source);
            CreateMap<FormMessageTabViewModel, Webpage>(MemberList.Source);
            CreateMap<LayoutTabViewModel, Webpage>(MemberList.Source);
            CreateMap<PermissionsTabViewModel, Webpage>(MemberList.Source);
            CreateMap<WebpagePropertiesTabViewModel, Webpage>(MemberList.Source);
            CreateMap<SEOTabViewModel, Webpage>(MemberList.Source);
        }
    }
}