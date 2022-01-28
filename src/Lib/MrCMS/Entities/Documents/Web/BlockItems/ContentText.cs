using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Documents.Web.BlockItems;

[Display(Name = "Content Text")]
public class ContentText : BlockItem
{
    public string Text { get; set; }
}