using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using NHibernate;

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

        public async Task<IList<SelectListItem>> GetUsers()
        {
            var users = await _session.QueryOver<User>()
                .Cacheable()
                .ListAsync();
            return users
                .BuildSelectItemList(user => user.Name, user => user.Id.ToString(),
                    emptyItem: new SelectListItem
                    {
                        Text = await _stringResourceProvider.GetValue("Please select..."),
                        Value = ""
                    });
        }
    }
}
