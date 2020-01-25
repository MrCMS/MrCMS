using AutoMapper;
using MrCMS.Web.Apps.Admin.Infrastructure.Mapping;
using MrCMS.Web.Apps.Admin.Mapping;
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
                .ForMember(document => document.DocumentTags, expression => expression.MapFrom<DocumentTagsMapper>())
                ;
            CreateMap<Article, ArticleViewModel>()
                .ReverseMap()
                .ForMember(document => document.DocumentTags,
                    expression => expression.MapFrom<DocumentTagsMapper>());
        }
    }
}
