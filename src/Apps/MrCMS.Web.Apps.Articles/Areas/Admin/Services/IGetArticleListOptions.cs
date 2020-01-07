using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Data;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Articles.Pages;


namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public interface IGetArticleListOptions
    {
        IList<SelectListItem> GetOptions();
    }

    public class GetArticleListOptions : IGetArticleListOptions
    {
        private readonly IRepository<ArticleList> _repository;
        private readonly IStringResourceProvider _stringResourceProvider;

        public GetArticleListOptions(IRepository<ArticleList> repository, IStringResourceProvider stringResourceProvider)
        {
            _repository = repository;
            _stringResourceProvider = stringResourceProvider;
        }

        public IList<SelectListItem> GetOptions()
        {
            return _repository.Readonly()
                .OrderBy(list => list.Name)
                .BuildSelectItemList(category => category.Name,
                    category => category.Id.ToString(),
                    emptyItemText: _stringResourceProvider.GetValue("Select an article list..."));
        }
    }
}
