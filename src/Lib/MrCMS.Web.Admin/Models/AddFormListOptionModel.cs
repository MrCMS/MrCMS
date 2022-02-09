using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models
{
    public class AddFormListOptionModel
    {
        [Required]
        public int FormPropertyId { get; set; }
        [Required(ErrorMessage = "Value is required")]
        [Remote("CheckValueIsNotEnteredAdd", "FormListOption", AdditionalFields = "FormPropertyId", ErrorMessage = "This value is already entered.")]
        public string Value { get; set; }
        public bool Selected { get; set; }
    }
}