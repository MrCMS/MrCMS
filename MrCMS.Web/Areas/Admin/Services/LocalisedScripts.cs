using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class LocalisedScripts : ILocalisedScripts
    {
        public IEnumerable<string> Files
        {
            get
            {
                var files = new List<string>();

                var datePicker = $"~/Areas/Admin/Content/Scripts/lib/jquery/ui/i18n/jquery.ui.datepicker-{CurrentRequestData.CultureInfo.Name}.js";
                if (Exists(datePicker))
                    files.Add(datePicker);

                var validation = $"~/Areas/Admin/Content/Scripts/lib/jquery/validate/localization/messages_{CurrentRequestData.CultureInfo.Name.Replace("-", "_")}.js";
                if (Exists(validation))
                    files.Add(validation);

                return files;
            }
        }

        private static bool Exists(string virtualPath)
        {
            var mapPath = HostingEnvironment.MapPath(virtualPath);
            return File.Exists(mapPath);
        }
    }
}