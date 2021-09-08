namespace MrCMS.Web.Apps.Articles.Services.Widgets
{
    // todo - refactor to load in view
    // public class GetLatestXArticlesModel : GetWidgetModelBase<LatestXArticles>
    // {
    //     private readonly ISession _session;
    //
    //     public GetLatestXArticlesModel(ISession session)
    //     {
    //         _session = session;
    //     }
    //
    //     public override object GetModel(LatestXArticles widget)
    //     {
    //         if (widget.RelatedNewsList == null)
    //             return null;
    //
    //         return new LatestXArticlesViewModel
    //         {
    //             Articles = _session.QueryOver<Article>()
    //                 .Where(article => article.Parent.Id == widget.RelatedNewsList.Id
    //                                   && article.PublishOn != null && article.PublishOn <= DateTime.UtcNow)
    //                 .OrderBy(x => x.PublishOn).Desc
    //                 .Take(widget.NumberOfArticles)
    //                 .Cacheable()
    //                 .List(),
    //             Title = widget.Name
    //         };
    //     }
    // }
}
