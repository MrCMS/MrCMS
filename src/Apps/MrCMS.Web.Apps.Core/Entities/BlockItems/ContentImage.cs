using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Core.Entities.BlockItems;

[Display(Name = "Content Image")]
public class ContentImage : BlockItem
{
    public string Url { get; set; }
}