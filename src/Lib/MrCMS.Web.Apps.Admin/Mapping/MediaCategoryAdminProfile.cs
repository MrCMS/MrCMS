using AutoMapper;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Mapping
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