using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class LocalisedScripts : ILocalisedScripts
    {
        private readonly IFileProvider _fileProvider;

        public LocalisedScripts(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }
        public IEnumerable<string> Files
        {
            get
            {
                var files = new List<string>();

                var cultureInfo = CultureInfo.CurrentCulture; // TODO: wire up from settings
                    //CurrentRequestData.CultureInfo;
                var datePicker = $"~/Areas/Admin/Content/Scripts/lib/jquery/ui/i18n/jquery.ui.datepicker-{cultureInfo.Name}.js";
                if (Exists(datePicker))
                    files.Add(datePicker);

                var validation = $"~/Areas/Admin/Content/Scripts/lib/jquery/validate/localization/messages_{cultureInfo.Name.Replace("-", "_")}.js";
                if (Exists(validation))
                    files.Add(validation);

                return files;
            }
        }

        private bool Exists(string virtualPath)
        {
            //var mapPath = Path.Combine(_hostingEnvironment.WebRootPath, virtualPath);
            //return File.Exists(mapPath);
            return _fileProvider.GetFileInfo(virtualPath).Exists;
        }
    }
}