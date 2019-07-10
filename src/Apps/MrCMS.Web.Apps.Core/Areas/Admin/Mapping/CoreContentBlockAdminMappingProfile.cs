using AutoMapper;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.ContentBlocks;
using MrCMS.Web.Apps.Core.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Mapping
{
    public class CoreContentBlockAdminMappingProfile : Profile
    {
        public CoreContentBlockAdminMappingProfile()
        {
            CreateMap<PlainBlock, PlainBlockViewModel>().ReverseMap();
        }
    }
}