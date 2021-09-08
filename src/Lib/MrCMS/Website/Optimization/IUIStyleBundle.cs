using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Website.Optimization
{
    public interface IUIStyleBundle
    {
        int Priority { get; }
        Task<bool> ShouldShow(string theme);
        string Url { get; }
        string MinifiedUrl { get; }
        IEnumerable<string> VendorFiles { get; }
    }
}