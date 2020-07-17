using Microsoft.Extensions.FileProviders;
using MrCMS.Services.Resources;
using System.Collections.Generic;

namespace MrCMS.Web.Admin.Services
{
    public class LocalisedScripts : ILocalisedScripts
    {
        private readonly IFileProvider _fileProvider;
        private readonly IGetCurrentUserCultureInfo _getCurrentUserCultureInfo;

        public LocalisedScripts(IFileProvider fileProvider, IGetCurrentUserCultureInfo getCurrentUserCultureInfo)
        {
            _fileProvider = fileProvider;
            _getCurrentUserCultureInfo = getCurrentUserCultureInfo;
        }

        public IEnumerable<string> Files
        {
            get
            {
                var files = new List<string>();

                var cultureInfo = _getCurrentUserCultureInfo.Get();

                var datePicker =
                    $"~/Areas/Admin/Content/Scripts/lib/jquery/ui/i18n/jquery.ui.datepicker-{cultureInfo.Name}.js";
                if (Exists(datePicker))
                {
                    files.Add(datePicker);
                }

                var validation =
                    $"~/Areas/Admin/Content/Scripts/lib/jquery/validate/localization/messages_{cultureInfo.Name.Replace("-", "_")}.js";
                if (Exists(validation))
                {
                    files.Add(validation);
                }

                return files;
            }
        }

        private bool Exists(string virtualPath)
        {
            return _fileProvider.GetFileInfo(virtualPath).Exists;
        }
    }
}