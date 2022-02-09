using AutoMapper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Models.Forms;

namespace MrCMS.Web.Admin.Mapping
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