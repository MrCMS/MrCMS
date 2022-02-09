using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Website.Optimization
{
    public interface IUIScriptBundle
    {
        int Priority { get; }
        Task<bool> ShouldShow(string theme);
        string Url { get; }
        IEnumerable<string> VendorFiles { get; }
    }
}