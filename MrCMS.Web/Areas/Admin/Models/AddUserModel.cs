using System.Collections.Generic;
using System.ComponentModel;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.People;

namespace MrCMS.Web.Areas.Admin.Models
{
    [DoNotMap]
    public class AddUserModel : User
    {
        public string Password { get; set; }
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}