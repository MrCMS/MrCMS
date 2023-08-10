using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models;

public class StringResourceInlineUpdateModel
{
    [Required]
    public string Value { get; set; }
    public string Key { get; set; }
}