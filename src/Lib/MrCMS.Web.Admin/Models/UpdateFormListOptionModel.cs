using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models
{
    public class UpdateFormListOptionModel
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Value is required")]
        [Remote("CheckValueIsNotEnteredEdit", "FormListOption", AdditionalFields = "Id", ErrorMessage = "This value is already entered.")]
        public string Value { get; set; }
        public bool Selected { get; set; }
    }
}