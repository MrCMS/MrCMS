using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class UpdateUserModel 
    {
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UICulture { get; set; }
        public bool IsActive { get; set; }
        public bool DisableNotifications { get; set; }

        public string Name =>
            (string.IsNullOrEmpty(FirstName) || string.IsNullOrWhiteSpace(LastName)
                ? this.Email
                : this.FirstName + " " + this.LastName);
    }
}