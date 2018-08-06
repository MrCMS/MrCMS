using AutoMapper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class AddWebpageProfile : Profile
    {
        public AddWebpageProfile()
        {
            CreateMap<Webpage, AddWebpageModel>().ReverseMap();
        }
    }
}