using System.Collections.Generic;

namespace MrCMS.Models
{
    public interface IAdminMenuItem : IMenuItem
    {
        IDictionary<string, List<IMenuItem>> Children { get; }
        int DisplayOrder { get; }
    }
}