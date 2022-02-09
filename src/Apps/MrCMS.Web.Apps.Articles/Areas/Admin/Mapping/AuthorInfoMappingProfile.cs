using AutoMapper;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Apps.Articles.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Articles.Areas.Admin.Models;
using MrCMS.Web.Apps.Articles.Entities;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Mapping
{
    public class AuthorInfoMappingProfile : Profile
    {
        public AuthorInfoMappingProfile()
        {
            CreateMap<AuthorInfo, AddAuthorInfoModel>().ReverseMap()
                .MapEntityLookup(x => x.UserId, info => info.User);
            CreateMap<AuthorInfo, EditAuthorInfoModel>().ReverseMap();
        }
    }
}