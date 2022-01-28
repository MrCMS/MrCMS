using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Documents.Web.BlockItems;

[Display(Name = "Content Image")]
public class ContentImage : BlockItem
{
    public string Url { get; set; }
}