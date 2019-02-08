using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Articles.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public interface IGetArticleListOptions
    {
        IList<SelectListItem> GetOptions();
    }

    public class GetArticleListOptions : IGetArticleListOptions
    {
        private readonly ISession _session;
        private readonly IStringResourceProvider _stringResourceProvider;

        public GetArticleListOptions(ISession session, IStringResourceProvider stringResourceProvider)
        {
            _session = session;
            _stringResourceProvider = stringResourceProvider;
        }

        public IList<SelectListItem> GetOptions()
        {
            return _session.QueryOver<ArticleList>()
                .OrderBy(list => list.Name)
                .Asc.Cacheable().List()
                .BuildSelectItemList(category => category.Name,
                    category => category.Id.ToString(),
                    emptyItemText: _stringResourceProvider.GetValue("Select an article list..."));
        }
    }
}
