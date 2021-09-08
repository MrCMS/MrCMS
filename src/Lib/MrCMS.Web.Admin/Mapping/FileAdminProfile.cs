using AutoMapper;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class FileAdminProfile : Profile
    {
        public FileAdminProfile()
        {
            CreateMap<MediaFile, UpdateFileSEOModel>().ReverseMap();
        }
    }
}