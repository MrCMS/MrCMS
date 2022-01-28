using System;

namespace MrCMS.Entities.Documents.Web;

public abstract class BlockItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public virtual string GetDisplayName(IContentBlock block) => Name;
    public string Name { get; set; }
}