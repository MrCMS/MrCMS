using Mapster;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Apps.Articles.Areas.Admin.Models;
using MrCMS.Web.Apps.Articles.Widgets;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Mapping
{
    public class ArticlesWidgetAdminMappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ArticleArchive, UpdateArticleArchiveModel>();
            config.NewConfig<UpdateArticleArchiveModel, ArticleArchive>()
                .MapEntityLookup(x => x.ArticleListId, x => x.ArticleList);
            config.NewConfig<ArticleCategories, UpdateArticleCategoriesModel>();
            config.NewConfig<UpdateArticleCategoriesModel, ArticleCategories>()
                .MapEntityLookup(x => x.ArticleListId, x => x.ArticleList);
            config.NewConfig<LatestXArticles, UpdateLatestXArticlesModel>();
            config.NewConfig<UpdateLatestXArticlesModel, LatestXArticles>()
                .MapEntityLookup(x => x.RelatedNewsListId, x => x.RelatedNewsList);
        }
    }
}
