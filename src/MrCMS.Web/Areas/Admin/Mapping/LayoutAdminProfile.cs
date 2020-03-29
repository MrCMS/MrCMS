using AutoMapper;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Mapping
{
    public class LayoutAdminProfile : Profile
    {
        public LayoutAdminProfile()
        {
            CreateMap<Layout, UpdateLayoutModel>().ReverseMap();
            CreateMap<Layout, AddLayoutModel>().ReverseMap()
                ;
        }
    }
}