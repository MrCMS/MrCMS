using System.Collections.Generic;

namespace MrCMS.Web.Admin.Models.Content;

public class ContentVersionModel
{
    public int Id { get; set; }
    public int WebpageId { get; set; }
    public string PreviewUrl { get; set; }
    public List<ContentVersionBlockSummaryModel> Blocks { get; set; }
}