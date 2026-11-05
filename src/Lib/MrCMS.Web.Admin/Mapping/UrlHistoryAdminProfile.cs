using Mapster;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class UrlHistoryAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UrlHistory, AddUrlHistoryModel>();
            config.NewConfig<AddUrlHistoryModel, UrlHistory>()
                .MapEntityLookup(x => x.WebpageId, x => x.Webpage);
        }
    }
}