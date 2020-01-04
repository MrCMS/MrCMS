using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services.Resources;


namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public class GetArticleAdminAuthorOptions : IGetArticleAdminAuthorOptions
    {
        private readonly ISession _session;
        private readonly IStringResourceProvider _stringResourceProvider;

        public GetArticleAdminAuthorOptions(ISession session, IStringResourceProvider stringResourceProvider)
        {
            _session = session;
            _stringResourceProvider = stringResourceProvider;
        }

        public IList<SelectListItem> GetUsers()
        {
            return _session.QueryOver<User>()
                .Cacheable()
                .List()
                .BuildSelectItemList(user => user.Name, user => user.Id.ToString(),
                    emptyItem: new SelectListItem
                    {
                        Text = _stringResourceProvider.GetValue("Please select..."),
                        Value = ""
                    });
        }
    }
}
