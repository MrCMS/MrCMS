using System.Collections.Generic;
using System.Linq;
using MrCMS.Services;

namespace MrCMS.Website.Optimization
{
    public class AppScriptBundle : IScriptBundle
    {
        public const string VirtualUrl = "~/scripts/apps";

        private readonly IEnumerable<IAppScriptList> _scriptLists;

        public AppScriptBundle(IEnumerable<IAppScriptList> scriptLists)
        {
            _scriptLists = scriptLists;
        }

        public string Url 
            => VirtualUrl;

        public IEnumerable<string> Files
            => _scriptLists.SelectMany(list => list.UIScripts);
    }
}