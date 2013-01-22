using System.Collections.Generic;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.People;

namespace MrCMS.Web.Areas.Admin.Models
{
    [DoNotMap]
    public class AddUserModel : User
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}