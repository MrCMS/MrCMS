using System.Collections.Generic;

namespace MrCMS.Models
{
    public interface IAdminMenuItem : IMenuItem
    {
        List<IMenuItem> Children { get; }
        int DisplayOrder { get; }
    }
}