using AutoMapper;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class LayoutAdminProfile : Profile
    {
        public LayoutAdminProfile()
        {
            CreateMap<Layout, UpdateLayoutModel>().ReverseMap();
            CreateMap<Layout, AddLayoutModel>().ReverseMap()
                .MapEntityLookup(x => x.ParentId, x => x.Parent)
                ;
        }
    }
}