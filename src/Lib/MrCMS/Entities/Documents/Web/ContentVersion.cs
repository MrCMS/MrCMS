using System.Collections.Generic;

namespace MrCMS.Entities.Documents.Web;

public class ContentVersion : SiteEntity
{
    public virtual Webpage Webpage { get; set; }
    public virtual ContentVersionStatus Status { get; set; }

    public virtual IList<ContentRow> Rows { get; set; } = new List<ContentRow>();
    public virtual bool IsDraft => Status == ContentVersionStatus.Draft;
}