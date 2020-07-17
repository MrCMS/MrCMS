using AutoMapper;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Admin.Mapping;
using MrCMS.Web.Apps.Articles.Areas.Admin.Models;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Mapping
{
    public class ArticlesWebpageAdminMappingProfile : Profile
    {
        public ArticlesWebpageAdminMappingProfile()
        {
            CreateMap<ArticleList, ArticleListViewModel>()
                .ReverseMap()
                .ForMember(document => document.Tags, expression => expression.MapFrom<DocumentTagsMapper>())
                ;
            CreateMap<Article, ArticleViewModel>()
                .ReverseMap()
                .MapEntityLookup(x => x.UserId, x => x.User)
                .ForMember(document => document.Tags,
                    expression => expression.MapFrom<DocumentTagsMapper>());
        }
    }
}
