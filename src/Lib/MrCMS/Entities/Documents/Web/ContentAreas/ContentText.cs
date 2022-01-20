using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Documents.Web.ContentAreas;

[Display(Name = "Content Text")]
public class ContentText : BlockItem
{
    public string Text { get; set; }
}