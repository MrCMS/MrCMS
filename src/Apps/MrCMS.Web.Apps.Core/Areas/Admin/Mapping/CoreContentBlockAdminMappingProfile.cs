using AutoMapper;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.ContentBlocks;
using MrCMS.Web.Apps.Core.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Mapping
{
    public class CoreContentBlockAdminMappingProfile : Profile
    {
        public CoreContentBlockAdminMappingProfile()
        {
            CreateMap<FourColumnBlock, FourColumnBlockViewModel>().ReverseMap();
            CreateMap<OneColumnBlock, OneColumnBlockViewModel>().ReverseMap();
            CreateMap<OneColumnTextImageBlock, OneColumnTextImageBlockViewModel>().ReverseMap();
            CreateMap<PlainBlock, PlainBlockViewModel>().ReverseMap();
            CreateMap<SliderBlock, SliderBlockViewModel>().ReverseMap();
            CreateMap<ThreeColumnBlock, ThreeColumnBlockViewModel>().ReverseMap();
            CreateMap<TwoColumnBlock, TwoColumnBlockViewModel>().ReverseMap();
            CreateMap<TwoColumnTextImageBlock, TwoColumnTextImageBlockViewModel>().ReverseMap();
            CreateMap<VideoBlock, VideoBlockViewModel>().ReverseMap();
        }
    }
}