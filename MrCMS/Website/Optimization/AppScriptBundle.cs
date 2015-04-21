using System.Collections.Generic;
using System.Linq;
using MrCMS.Services;

namespace MrCMS.Website.Optimization
{
    public class AppScriptBundle : IScriptBundle
    {
        private readonly IEnumerable<IAppScriptList> _scriptLists;

        public AppScriptBundle(IEnumerable<IAppScriptList> scriptLists)
        {
            _scriptLists = scriptLists;
        }

        public string Url
        {
            get { return "~/scripts/apps"; }
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