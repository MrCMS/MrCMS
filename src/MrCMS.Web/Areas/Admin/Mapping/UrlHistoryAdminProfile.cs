using AutoMapper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Mapping
{
    public class UrlHistoryAdminProfile : Profile
    {
        public UrlHistoryAdminProfile()
        {
            CreateMap<UrlHistory, AddUrlHistoryModel>().ReverseMap();
        }
    }
}