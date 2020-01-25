using AutoMapper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class PageTemplateAdminProfile : Profile
    {
        public PageTemplateAdminProfile()
        {
            CreateMap<PageTemplate, AddPageTemplateModel>().ReverseMap()
                ;
            CreateMap<PageTemplate, UpdatePageTemplateModel>().ReverseMap()
                ;
        }
    }
}