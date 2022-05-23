using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Entities.Documents.Web;

public class ContentVersion : SiteEntity
{
    public virtual Webpage Webpage { get; set; }
    public virtual ContentVersionStatus Status { get; set; }

    public virtual IList<ContentBlock> Blocks { get; set; } = new List<ContentBlock>();
    public virtual bool IsDraft => Status == ContentVersionStatus.Draft;
    public virtual bool IsLive => Status == ContentVersionStatus.Live;

    public virtual ContentVersion CreateDraft()
    {
        var version = new ContentVersion
        {
            Status = ContentVersionStatus.Draft,
            Webpage = Webpage,
            Site = Site,
        };
        version.Blocks = Blocks.Select(x => x.Clone(version)).ToList();
        return version;
    }
}