using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Areas.Admin.Services;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public class GetFeatureAdminViewDataUsers : BaseAssignWebpageAdminViewData<Feature>
    {
        private readonly ISession _session;

        public GetFeatureAdminViewDataUsers(ISession session)
        {
            _session = session;
        }

        public override void AssignViewData(Feature webpage, ViewDataDictionary viewData)
        {
            viewData["users"] = _session.QueryOver<User>()
                .List()
                .BuildSelectItemList(user => user.Name, user => user.Id.ToString(),
                    user => user == webpage.User, new SelectListItem { Text = "Please select...", Value = "0" });
        }
    }
}