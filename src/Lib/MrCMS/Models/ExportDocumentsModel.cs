using System.ComponentModel.DataAnnotations;

namespace MrCMS.Models
{
    public class ExportDocumentsModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}