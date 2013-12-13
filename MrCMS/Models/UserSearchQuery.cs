using System.ComponentModel;
using MrCMS.Entities.People;

namespace MrCMS.Models
{
    public class UserSearchQuery
    {
        public UserSearchQuery()
        {
            Page = 1;
        }
        [DisplayName("Email or Name")]
        public string Query { get; set; }
        public int? UserRoleId { get; set; }
        public int Page { get; set; }
    }
}