using System.Collections.Generic;
using System.Linq;
using MrCMS.Services;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Areas.Admin.Bundles
{
    public class AdminAppScriptBundle : IScriptBundle
    {
        private readonly IEnumerable<IAppScriptList> _scriptLists;

        public AdminAppScriptBundle(IEnumerable<IAppScriptList> scriptLists)
        {
            _scriptLists = scriptLists;
        }

        public string Url
            => "~/admin/scripts/apps";

        public IEnumerable<string> Files
            => _scriptLists.SelectMany(list => list.AdminScripts);
    }
}