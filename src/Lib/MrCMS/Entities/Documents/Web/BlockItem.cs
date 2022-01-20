using System;

namespace MrCMS.Entities.Documents.Web;

public abstract class BlockItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
}