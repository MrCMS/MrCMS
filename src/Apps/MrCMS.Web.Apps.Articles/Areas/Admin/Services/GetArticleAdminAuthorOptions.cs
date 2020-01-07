using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services.Resources;


namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public class GetArticleAdminAuthorOptions : IGetArticleAdminAuthorOptions
    {
        private readonly IGlobalRepository<User> _repository;
        private readonly IStringResourceProvider _stringResourceProvider;

        public GetArticleAdminAuthorOptions(IStringResourceProvider stringResourceProvider, IGlobalRepository<User> repository)
        {
            _stringResourceProvider = stringResourceProvider;
            _repository = repository;
        }

        public IList<SelectListItem> GetUsers()
        {
            return _repository.Readonly()
                .BuildSelectItemList(user => user.Name, user => user.Id.ToString(),
                    emptyItem: new SelectListItem
                    {
                        Text = _stringResourceProvider.GetValue("Please select..."),
                        Value = ""
                    });
        }
    }
}
