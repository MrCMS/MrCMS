using AutoMapper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class UrlHistoryAdminProfile : Profile
    {
        public UrlHistoryAdminProfile()
        {
            CreateMap<UrlHistory, AddUrlHistoryModel>().ReverseMap()
                .MapEntityLookup(x => x.WebpageId, x => x.Webpage);
        }
    }
}