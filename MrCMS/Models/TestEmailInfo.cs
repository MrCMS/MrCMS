using System.ComponentModel.DataAnnotations;
using MrCMS.Helpers.Validation;

namespace MrCMS.Models
{
    public class TestEmailInfo
    {
        [EmailValidator]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Content { get; set; }
    }
}