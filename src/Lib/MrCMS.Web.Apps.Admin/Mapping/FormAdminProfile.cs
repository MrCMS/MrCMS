using AutoMapper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Models.Forms;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class FormAdminProfile : Profile
    {
        public FormAdminProfile()
        {
            CreateMap<Form, AddFormModel>().ReverseMap();
            CreateMap<Form, UpdateFormModel>().ReverseMap();
            CreateMap<Form, FormMessageTabViewModel>().ReverseMap();
            CreateMap<Form, FormGDPRTabViewModel>().ReverseMap();
            CreateMap<Form, FormDesignTabViewModel>().ReverseMap();
        }
    }
}