using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models.Content;

public class AddContentBlockModel
{
    public int ContentVersionId { get; set; }
    [Required]
    public string BlockType { get; set; }
}