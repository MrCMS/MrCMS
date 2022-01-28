using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Core.Entities.BlockItems;

[Display(Name = "Content Text")]
public class ContentText : BlockItem
{
    public string Text { get; set; }
}