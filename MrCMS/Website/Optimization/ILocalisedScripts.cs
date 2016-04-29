using System.Collections.Generic;

namespace MrCMS.Website.Optimization
{
    public interface ILocalisedScripts
    {
        IEnumerable<string> Files { get; }
    }
}