using AutoMapper;
using MrCMS.Web.Admin.Mapping;
using MrCMS.Web.Apps.Articles.Areas.Admin.Models;
using MrCMS.Web.Apps.Articles.Widgets;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Mapping
{
    public class ArticlesWidgetAdminMappingProfile : Profile
    {
        public ArticlesWidgetAdminMappingProfile()
        {
            CreateMap<ArticleArchive, UpdateArticleArchiveModel>().ReverseMap()
                .MapEntityLookup(x => x.ArticleListId, x => x.ArticleList);
            CreateMap<ArticleCategories, UpdateArticleCategoriesModel>().ReverseMap()
                .MapEntityLookup(x => x.ArticleListId, x => x.ArticleList);
            CreateMap<LatestXArticles, UpdateLatestXArticlesModel>().ReverseMap()
                .MapEntityLookup(x => x.RelatedNewsListId, x => x.RelatedNewsList);
        }
    }
}
