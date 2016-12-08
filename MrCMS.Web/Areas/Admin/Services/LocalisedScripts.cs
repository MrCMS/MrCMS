using System.Collections.Generic;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class LocalisedScripts : ILocalisedScripts
    {
        public IEnumerable<string> Files
        {
            get
            {
                var files = new List<string>
                {
                    $"~/Areas/Admin/Content/Scripts/lib/jquery/ui/i18n/jquery.ui.datepicker-{CurrentRequestData.CultureInfo.Name}.js"
                };
                if (!CurrentRequestData.CultureInfo.Name.StartsWith("en"))
                    files.Add(
                        $"~/Areas/Admin/Content/Scripts/lib/jquery/validate/localization/messages_{CurrentRequestData.CultureInfo.Name.Replace("-", "_")}.js");

                return files;
            }
        }
    }
}