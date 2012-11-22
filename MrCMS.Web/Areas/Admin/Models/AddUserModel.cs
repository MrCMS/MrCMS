using System.Collections.Generic;
using MrCMS.Entities.People;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class AddUserModel : User
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}