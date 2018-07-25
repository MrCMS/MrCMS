using System.Collections.Generic;

namespace MrCMS.Website.Optimization
{
    public interface IScriptBundle
    {
        string Url { get; }
        IEnumerable<string> Files { get; }
    }
}