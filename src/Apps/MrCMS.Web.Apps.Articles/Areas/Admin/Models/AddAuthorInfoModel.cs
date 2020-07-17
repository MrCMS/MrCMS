using MrCMS.Web.Admin.Infrastructure.Models;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Models
{
    public class AddAuthorInfoModel : IAddUserProfileDataModel
    {
        public int UserId { get; set; }
        public string Bio { get; set; }
    }
}
