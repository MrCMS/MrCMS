using Mapster;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Apps.Articles.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Articles.Areas.Admin.Models;
using MrCMS.Web.Apps.Articles.Entities;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Mapping
{
    public class AuthorInfoMappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AuthorInfo, AddAuthorInfoModel>();
            config.NewConfig<AddAuthorInfoModel, AuthorInfo>()
                .MapEntityLookup(x => x.UserId, x => x.User);
            config.NewConfig<AuthorInfo, EditAuthorInfoModel>();
            config.NewConfig<EditAuthorInfoModel, AuthorInfo>();
        }
    }
}