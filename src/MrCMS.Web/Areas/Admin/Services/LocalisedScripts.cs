using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using MrCMS.Services.Resources;

namespace MrCMS.Web.Areas.Admin.Services
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

        public async Task<IEnumerable<string>> GetFiles()
        {
            var files = new List<string>();

            var cultureInfo = await _getCurrentUserCultureInfo.Get();

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

        private bool Exists(string virtualPath)
        {
            return _fileProvider.GetFileInfo(virtualPath).Exists;
        }
    }
}