using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Documents.Web.BlockItems;

[Display(Name = "Slide")]
public class Slide : BlockItem
{
    public string Url { get; set; }
    public string Caption { get; set; }
}