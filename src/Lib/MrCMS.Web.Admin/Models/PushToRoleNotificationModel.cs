using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models
{
    public class PushToRoleNotificationModel : PushNotificationModel
    {
        [Required, Display(Name = "Role")] public int? RoleId { get; set; }
    }
}