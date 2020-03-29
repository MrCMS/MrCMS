using AutoMapper;
using MrCMS.Entities.Documents;
using MrCMS.Web.Areas.Admin.Models.ContentBlocks;

namespace MrCMS.Web.Areas.Admin.Mapping
{
    public class ContentBlockAdminProfile : Profile
    {
        public ContentBlockAdminProfile()
        {
            CreateMap<ContentBlock, AddContentBlockViewModel>().ReverseMap();
            CreateMap<ContentBlock, UpdateContentBlockViewModel>().ReverseMap();
        }
    }
}