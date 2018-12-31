using System.ComponentModel.DataAnnotations;
using MrCMS.Helpers.Validation;

namespace MrCMS.Models
{
    public class ExportDocumentsModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}