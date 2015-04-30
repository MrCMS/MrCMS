using System.Collections.Generic;

namespace MrCMS.Website.Optimization
{
    public interface IStylesheetBundle
    {
        string Url { get; }
        IEnumerable<string> Files { get; }
    }
}