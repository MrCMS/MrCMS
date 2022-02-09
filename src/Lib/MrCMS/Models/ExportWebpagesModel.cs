using System.ComponentModel.DataAnnotations;

namespace MrCMS.Models
{
    public class ExportWebpagesModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}