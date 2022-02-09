using System.ComponentModel.DataAnnotations;

namespace MrCMS.Models
{
    public class TestEmailInfo
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}