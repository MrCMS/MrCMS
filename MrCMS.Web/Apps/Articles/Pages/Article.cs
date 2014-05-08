using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Articles.Pages
{
    public class Article : TextPage, IBelongToUser
    {
        [AllowHtml]
        [StringLength(500, ErrorMessage = "Abstract cannot be longer than 500 characters.")]
        public virtual string Abstract { get; set; }

        [DisplayName("Author")]
        public virtual User User { get; set; }

        public override void AdminViewData(ViewDataDictionary viewData, NHibernate.ISession session)
        {
            base.AdminViewData(viewData, session);
            viewData["users"] = session.QueryOver<User>()
                                       .List()
                                       .BuildSelectItemList(user => user.Name, user => user.Id.ToString(),
                                                            user => user == User, new SelectListItem{Text = "Please select...", Value = "0"});
        }
    }
}