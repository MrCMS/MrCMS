using System.Collections.Generic;
using System.Linq;
using MrCMS.Services;

namespace MrCMS.Website.Optimization
{
    public class AppScriptBundle : IScriptBundle
    {
        public const string VirtualUrl = "~/scripts/apps.js";
        private readonly IEnumerable<IAppScriptList> _scriptLists;

        public AppScriptBundle(IEnumerable<IAppScriptList> scriptLists)
        {
            _scriptLists = scriptLists;
        }

        public string Url
        {
            get { return VirtualUrl; }
        }

        public IEnumerable<string> Files
        {
            get
            {
                return _scriptLists.SelectMany(list => list.UIScripts);
            }
        }
    }
}