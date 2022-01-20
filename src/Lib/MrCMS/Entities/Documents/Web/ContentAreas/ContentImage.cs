using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Documents.Web.ContentAreas;

[Display(Name = "Content Image")]
public class ContentImage : BlockItem
{
    public string Url { get; set; }
}