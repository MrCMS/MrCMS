using System.Collections.Generic;
using System.Linq;
using MrCMS.Services;

namespace MrCMS.Website.Optimization
{
    public class AppStylesheetBundle : IStylesheetBundle
    {
        public const string VirtualUrl = "~/stylesheets/apps";
        private readonly IEnumerable<IAppStylesheetList> _stylesheetLists;

        public AppStylesheetBundle(IEnumerable<IAppStylesheetList> stylesheetLists)
        {
            _stylesheetLists = stylesheetLists;
        }

        public string Url { get { return VirtualUrl; } }

        public IEnumerable<string> Files
        {
            get { return _stylesheetLists.SelectMany(list => list.UIStylesheets); }
        }
    }
}