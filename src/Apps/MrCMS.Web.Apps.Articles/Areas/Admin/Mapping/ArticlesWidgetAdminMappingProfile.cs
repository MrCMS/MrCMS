using AutoMapper;
using MrCMS.Web.Apps.Admin.Mapping;
using MrCMS.Web.Apps.Articles.Areas.Admin.Models;
using MrCMS.Web.Apps.Articles.Widgets;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Mapping
{
    public class ArticlesWidgetAdminMappingProfile : Profile
    {
        public ArticlesWidgetAdminMappingProfile()
        {
            CreateMap<ArticleArchive, UpdateArticleArchiveModel>().ReverseMap();
            CreateMap<ArticleCategories, UpdateArticleCategoriesModel>().ReverseMap();
            CreateMap<LatestXArticles, UpdateLatestXArticlesModel>().ReverseMap();
        }
    }
}
