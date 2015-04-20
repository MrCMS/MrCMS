using System.Collections.Generic;
using System.Linq;
using MrCMS.Services;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Areas.Admin.Bundles
{
    public class AdminAppStylesheetBundle : IStylesheetBundle
    {
        private readonly IEnumerable<IAppStylesheetList> _stylesheetLists;

        public AdminAppStylesheetBundle(IEnumerable<IAppStylesheetList> stylesheetLists)
        {
            _stylesheetLists = stylesheetLists;
        }

        public string Url { get { return "~/admin/stylesheets/apps"; } }

        public IEnumerable<string> Files
        {
            get { return _stylesheetLists.SelectMany(list => list.AdminStylesheets); }
        }
    }
}