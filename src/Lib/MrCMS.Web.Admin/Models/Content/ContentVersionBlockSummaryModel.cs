using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Admin.Models.Content;

public class ContentVersionBlockSummaryModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public Type Type { get; set; }
    public List<ContentVersionBlockItemSummaryModel> Items { get; set; }
    public bool ShowCaret => Items.Any() || CanAddChildren;
    public string TypeName { get; set; }
    public bool CanAddChildren { get; set; }
}