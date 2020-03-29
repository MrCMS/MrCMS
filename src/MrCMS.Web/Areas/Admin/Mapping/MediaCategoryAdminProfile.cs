using AutoMapper;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Mapping
{
    public class MediaCategoryAdminProfile : Profile
    {
        public MediaCategoryAdminProfile()
        {
            CreateMap<MediaCategory, UpdateMediaCategoryModel>().ReverseMap();
            CreateMap<MediaCategory, AddMediaCategoryModel>().ReverseMap()
                ;
        }
    }
}