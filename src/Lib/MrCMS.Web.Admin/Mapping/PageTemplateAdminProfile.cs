using AutoMapper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class PageTemplateAdminProfile : Profile
    {
        public PageTemplateAdminProfile()
        {
            CreateMap<PageTemplate, AddPageTemplateModel>().ReverseMap()
                .MapEntityLookup(x => x.LayoutId, x => x.Layout);
            CreateMap<PageTemplate, UpdatePageTemplateModel>().ReverseMap()
                .MapEntityLookup(x => x.LayoutId, x => x.Layout);
        }
    }
}