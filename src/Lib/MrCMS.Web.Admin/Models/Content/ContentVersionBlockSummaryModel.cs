using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Admin.Models.Content;

public class ContentVersionBlockSummaryModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public List<ContentVersionBlockItemSummaryModel> Items { get; set; }
    public bool ShowCaret => Items.Any();
}