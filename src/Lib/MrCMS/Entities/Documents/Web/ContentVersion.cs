using System.Collections.Generic;

namespace MrCMS.Entities.Documents.Web;

public class ContentVersion : SiteEntity
{
    public virtual Webpage Webpage { get; set; }
    public virtual ContentVersionStatus Status { get; set; }

    public virtual IList<ContentBlock> Blocks { get; set; } = new List<ContentBlock>();
    public virtual bool IsDraft => Status == ContentVersionStatus.Draft;
    public virtual bool IsLive => Status == ContentVersionStatus.Live;
}