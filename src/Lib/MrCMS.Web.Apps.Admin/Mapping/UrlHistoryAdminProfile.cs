using AutoMapper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class UrlHistoryAdminProfile : Profile
    {
        public UrlHistoryAdminProfile()
        {
            CreateMap<UrlHistory, AddUrlHistoryModel>().ReverseMap();
        }
    }
}