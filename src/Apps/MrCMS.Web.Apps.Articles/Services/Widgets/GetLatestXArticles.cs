using MrCMS.Web.Apps.Articles.Pages;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Articles.Services.Widgets
{
    public class GetLatestXArticles : IGetLatestXArticles
    {
        private readonly ISession _session;

        public GetLatestXArticles(ISession session)
        {
            _session = session;
        }

        public async Task<IList<Article>> GetArticlesAsync(int? relatedNewsId, int numberOfArticles)
        {
            if (!relatedNewsId.HasValue)
                return null;

            return await _session.QueryOver<Article>()
                    .Where(article => article.Parent.Id == relatedNewsId
                                      && article.PublishOn != null && article.PublishOn <= DateTime.UtcNow)
                    .OrderBy(x => x.PublishOn).Desc
                    .Take(numberOfArticles)
                    .Cacheable()
                    .ListAsync();
        }
    }
}
