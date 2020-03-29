using AutoMapper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Mapping
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