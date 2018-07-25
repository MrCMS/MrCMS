using System.Collections.Generic;

namespace MrCMS.Services
{
    public interface IAppScriptList
    {
        IEnumerable<string> UIScripts { get; }
        IEnumerable<string> AdminScripts { get; }
    }
}