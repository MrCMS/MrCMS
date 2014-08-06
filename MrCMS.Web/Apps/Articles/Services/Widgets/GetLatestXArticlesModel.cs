using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Articles.Widgets;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Services.Widgets
{
    public class GetLatestXArticlesModel : GetWidgetModelBase<LatestXArticles>
    {
        private readonly ISession _session;

        public GetLatestXArticlesModel(ISession session)
        {
            _session = session;
        }

        public override object GetModel(LatestXArticles widget)
        {
            if (widget.RelatedNewsList == null)
                return null;


            return new LatestXArticlesViewModel
            {
                Articles = _session.QueryOver<Article>()
                    .Where(article => article.Parent.Id == widget.RelatedNewsList.Id && article.PublishOn != null && article.PublishOn <= CurrentRequestData.Now)
                    .OrderBy(x => x.PublishOn).Desc
                    .Take(widget.NumberOfArticles)
                    .Cacheable()
                    .List(),
                Title = widget.Name
            };
        }
    }
}