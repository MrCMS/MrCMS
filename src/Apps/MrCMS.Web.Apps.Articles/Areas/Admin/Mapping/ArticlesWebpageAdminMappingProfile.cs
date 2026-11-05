using Mapster;
using MrCMS.Mapping;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Apps.Articles.Areas.Admin.Models;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Mapping
{
    public class ArticlesWebpageAdminMappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ArticleList, ArticleListViewModel>();
            config.NewConfig<ArticleListViewModel, ArticleList>()
                .Ignore(x => x.Tags)
                .AfterMapping((model, webpage) => webpage.Tags.ReplaceContents(
                    MapContext.Current.CreateWithServices<WebpageTagsMapper>().Resolve(model)));
            config.NewConfig<Article, ArticleViewModel>();
            config.NewConfig<ArticleViewModel, Article>()
                .MapEntityLookup(x => x.UserId, x => x.User)
                .Ignore(x => x.Tags)
                .AfterMapping((model, webpage) => webpage.Tags.ReplaceContents(
                    MapContext.Current.CreateWithServices<WebpageTagsMapper>().Resolve(model)));
        }
    }
}
