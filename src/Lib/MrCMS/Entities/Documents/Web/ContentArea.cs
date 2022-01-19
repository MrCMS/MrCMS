using System;

namespace MrCMS.Entities.Documents.Web;

public abstract class ContentArea
{
    public Guid Id { get; set; } = Guid.NewGuid();
}