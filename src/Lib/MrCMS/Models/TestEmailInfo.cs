using System.ComponentModel.DataAnnotations;
using MrCMS.Helpers.Validation;

namespace MrCMS.Models
{
    public class TestEmailInfo
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}