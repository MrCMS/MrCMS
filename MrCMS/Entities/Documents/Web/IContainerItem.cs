using System.Collections.Generic;

namespace MrCMS.Entities.Documents.Web
{
    public interface IContainerItem
    {
        string ContainerUrl { get; }
        IList<Tag> Tags { get; }
    }
}