using AutoMapper;
using MrCMS.Entities.Documents;
using MrCMS.Web.Admin.Models.ContentBlocks;

namespace MrCMS.Web.Admin.Mapping
{
    public class ContentBlockAdminProfile : Profile
    {
        public ContentBlockAdminProfile()
        {
            CreateMap<ContentBlock, AddContentBlockViewModel>().ReverseMap()
                .MapEntityLookup(model => model.WebpageId, block => block.Webpage);
            CreateMap<ContentBlock, UpdateContentBlockViewModel>().ReverseMap();
        }
    }
}