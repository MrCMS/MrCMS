using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Articles.Pages;
using NHibernate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public class GetArticleListOptions : IGetArticleListOptions
    {
        private readonly ISession _session;
        private readonly IStringResourceProvider _stringResourceProvider;

        public GetArticleListOptions(ISession session, IStringResourceProvider stringResourceProvider)
        {
            _session = session;
            _stringResourceProvider = stringResourceProvider;
        }

        public async Task<IList<SelectListItem>> GetOptions()
        {
            var articleLists = await _session.QueryOver<ArticleList>()
                .OrderBy(list => list.Name)
                .Asc.Cacheable().ListAsync();
            return articleLists
                .BuildSelectItemList(category => category.Name,
                    category => category.Id.ToString(),
                    emptyItemText: await _stringResourceProvider.GetValue("Select an article list..."));
        }
    }
}
